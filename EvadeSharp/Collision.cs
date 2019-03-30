// Copyright 2014 - 2014 Esk0r
// Collision.cs is part of Evade.
// 
// Evade is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Evade is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Evade. If not, see <http://www.gnu.org/licenses/>.

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Core.UI.IMenu.Values;
using EnsoulSharp.SDK.Core.Utils;
using SharpDX;

#endregion

namespace Evade
{
    public enum CollisionObjectTypes
    {
        Minion,
        Champions,
        YasuoWall,
        Wall
    }

    internal class FastPredResult
    {
        public Vector2 CurrentPos;
        public bool IsMoving;
        public Vector2 PredictedPos;
    }

    internal class DetectedCollision
    {
        public float Diff;
        public float Distance;
        public Vector2 Position;
        public CollisionObjectTypes Type;
        public AIBaseClient Unit;
    }

    internal static class Collision
    {
        public static FastPredResult FastPrediction(Vector2 from, AIBaseClient unit, int delay, int speed)
        {
            var tDelay = delay / 1000f + from.Distance(unit.Position.ToVector2()) / speed;
            var d = tDelay * unit.MoveSpeed;
            var path = unit.GetWaypoints();

            if (path.PathLength() > d)
            {
                return new FastPredResult
                {
                    IsMoving = true,
                    CurrentPos = unit.Position.ToVector2(),
                    PredictedPos = path.CutPath(d)[0]
                };
            }

            return new FastPredResult
            {
                IsMoving = false,
                CurrentPos = path[path.Count - 1],
                PredictedPos = path[path.Count - 1]
            };
        }

        public static Vector2 GetCollisionPoint(Skillshot skillshot)
        {
            var collisions = new List<DetectedCollision>();
            var from = skillshot.GetMissilePosition(0);

            skillshot.ForceDisabled = false;

            foreach (var cObject in skillshot.SpellData.CollisionObjects)
            {
                switch (cObject)
                {
                    case CollisionObjectTypes.Minion:
                        if (!Config.collision["MinionCollision"].GetValue<MenuBool>().Value)
                        {
                            break;
                        }

                        var minionList =
                            GameObjects.Jungle
                                .Where(
                                    m =>
                                        m.IsValidTarget(1200, false, from.ToVector3())
                                        && (!skillshot.SpellData.CollisionExceptMini || m.GetJungleType() != JungleType.Small)).ToList();

                        if (!skillshot.SpellData.CollisionExceptMini)
                        {
                            minionList.AddRange(
                                GameObjects.Minions
                                    .Where(
                                        m =>
                                            m.IsValidTarget(1200, false, from.ToVector3())));
                        }

                        foreach (var minion in minionList)
                        {
                            var pred = FastPrediction(
                                from, minion,
                                Math.Max(0, skillshot.SpellData.Delay - (Utils.TickCount - skillshot.StartTick)),
                                skillshot.SpellData.MissileSpeed);
                            var pos = pred.PredictedPos;
                            var w = skillshot.SpellData.RawRadius + (!pred.IsMoving ? (minion.BoundingRadius - 15) : 0) - pos.Distance(from, skillshot.End, true);

                            if (w > 0)
                            {
                                collisions.Add(
                                    new DetectedCollision
                                    {
                                        Position = pos.ProjectOn(skillshot.End, skillshot.Start).LinePoint + skillshot.Direction * 30,
                                        Unit = minion,
                                        Type = CollisionObjectTypes.Minion,
                                        Distance = pos.Distance(from),
                                        Diff = w
                                    });
                            }
                        }

                        break;
                    case CollisionObjectTypes.Champions:
                        if (!Config.collision["HeroCollision"].GetValue<MenuBool>().Value)
                        {
                            break;
                        }

                        var heroTeam = skillshot.Unit == null
                            ? ObjectManager.Player.Team
                            : (skillshot.Unit.Team == GameObjectTeam.Order
                                ? GameObjectTeam.Chaos
                                : GameObjectTeam.Order);

                        foreach (var hero in
                            GameObjects.Heroes
                                .Where(
                                    h =>
                                        h.IsValidTarget(1200, false)
                                        && h.Team == heroTeam
                                        && !h.IsMe))
                        {
                            var pred = FastPrediction(
                                from, hero,
                                Math.Max(0, skillshot.SpellData.Delay - (Utils.TickCount - skillshot.StartTick)),
                                skillshot.SpellData.MissileSpeed);
                            var pos = pred.PredictedPos;
                            var w = skillshot.SpellData.RawRadius + 30 - pos.Distance(from, skillshot.End, true);

                            if (w > 0)
                            {
                                collisions.Add(
                                    new DetectedCollision
                                    {
                                        Position = pos.ProjectOn(skillshot.End, skillshot.Start).LinePoint + skillshot.Direction * 30,
                                        Unit = hero,
                                        Type = CollisionObjectTypes.Minion,
                                        Distance = pos.Distance(from),
                                        Diff = w
                                    });
                            }
                        }

                        break;
                    case CollisionObjectTypes.YasuoWall:
                        if (!Config.collision["YasuoCollision"].GetValue<MenuBool>().Value)
                        {
                            break;
                        }

                        var wallTeam = skillshot.Unit == null
                            ? ObjectManager.Player.Team
                            : (skillshot.Unit.Team == GameObjectTeam.Order
                                ? GameObjectTeam.Chaos
                                : GameObjectTeam.Order);

                        if (!GameObjects.Heroes
                            .Any(
                                hero =>
                                    hero.IsValidTarget(float.MaxValue, false)
                                    && hero.Team == wallTeam
                                    && hero.CharacterName == "Yasuo"))
                        {
                            break;
                        }

                        var intersections = new List<Tuple<Vector2, float>>();

                        foreach (var effectEmitter in GameObjects.ParticleEmitters)
                        {
                            if (effectEmitter.IsValid &&
                                Regex.IsMatch(effectEmitter.Name,
                                    wallTeam == ObjectManager.Player.Team
                                        ? @"Yasuo_.+_W_windwall\d"
                                        : @"Yasuo_.+_w_windwall_enemy_\d", RegexOptions.IgnoreCase))
                            {
                                var wall = effectEmitter;
                                var lvlLen = wallTeam == ObjectManager.Player.Team ? 1 : 2;
                                var level = wall.Name.Substring(wall.Name.Length - lvlLen, lvlLen);
                                var wallWidth = 250 + 50 * Convert.ToInt32(level);
                                var wallDirection = wall.Perpendicular.ToVector2();
                                var wallStart = wall.Position.ToVector2() + wallWidth / 2 * wallDirection;
                                var wallEnd = wallStart - wallWidth * wallDirection;
                                var wallPolygon = new Geometry.Rectangle(wallStart, wallEnd, 75).ToPolygon();

                                for (var i = 0; i < wallPolygon.Points.Count; i++)
                                {
                                    var inter = wallPolygon.Points[i].Intersection(
                                        wallPolygon.Points[i != wallPolygon.Points.Count - 1 ? i + 1 : 0], from, skillshot.End);
                                    if (inter.Intersects)
                                    {
                                        intersections.Add(new Tuple<Vector2, float>(inter.Point, wall.RestartTime));
                                    }
                                }
                            }
                        }

                        if (intersections.Count > 0)
                        {
                            var sortedIntersections = intersections.OrderBy(item => item.Item1.Distance(from)).ToList();
                            foreach (var inter in sortedIntersections)
                            {
                                var collisionT = Utils.TickCount +
                                                 Math.Max(
                                                     0,
                                                     skillshot.SpellData.Delay -
                                                     (Utils.TickCount - skillshot.StartTick)) + 100 +
                                                 (1000 * inter.Item1.Distance(from)) / skillshot.SpellData.MissileSpeed;
                                if (collisionT - inter.Item2 < 4000)
                                {
                                    if (skillshot.SpellData.Type != SkillShotType.SkillshotMissileLine)
                                    {
                                        skillshot.ForceDisabled = true;
                                    }
                                    return inter.Item1;
                                }
                            }
                        }

                        break;
                    case CollisionObjectTypes.Wall:
                        break;
                }
            }

            var result = Vector2.Zero;
            if (collisions.Count > 0)
            {
                result = collisions.OrderBy(c => c.Distance).ToList()[0].Position;
            }

            return result;
        }
    }
}