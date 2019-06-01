/*
 Copyright 2015 - 2015 SPrediction
 Collision.cs is part of SPrediction
 
 SPrediction is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.
 
 SPrediction is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.
 
 You should have received a copy of the GNU General Public License
 along with SPrediction. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using EnsoulSharp;
using EnsoulSharp.SDK;

using SharpDX;

namespace SPrediction
{
    /// <summary>
    /// SPrediction Collision class
    /// </summary>
    public static class Collision
    {
        /// <summary>
        /// Enum for collision flags
        /// </summary>
        public enum Flags
        {
            None = 0,
            Minions = 1,
            AllyChampions = 2,
            EnemyChampions = 4,
            Wall = 8,
            YasuoWall = 16,
        }

        /// <summary>
        /// Collision Result structure
        /// </summary>
        public struct Result
        {
            public List<AIBaseClient> Units;
            public Flags Objects;
            public static readonly Result NoCollision;

            public Result(List<AIBaseClient> _units, Flags _objects)
            {
                Units = _units;
                Objects = _objects;
            }
        }

        /// <summary>
        /// Checks collisions
        /// </summary>
        /// <param name="from">Start position</param>
        /// <param name="to">End position</param>
        /// <param name="width">Rectangle scale</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="checkMinion">Check minion collisions</param>
        /// <param name="checkEnemyHero">Check Enemy collisions</param>
        /// <param name="checkYasuoWall">Check Yasuo wall collisions</param>
        /// <param name="checkAllyHero">Check Ally collisions</param>
        /// <param name="checkWall">Check wall collisions</param>
        /// <param name="isArc">Checks collision for arc spell</param>
        /// <returns>true if collision found</returns>
        public static bool CheckCollision(Vector2 from, Vector2 to, float width, float delay, float missileSpeed, bool checkMinion = true, bool checkEnemyHero = false, bool checkYasuoWall = true, bool checkAllyHero = false, bool checkWall = false, bool isArc = false)
        {
            return (checkMinion && CheckMinionCollision(from, to, width, delay, missileSpeed, isArc))
                   || (checkEnemyHero && CheckEnemyHeroCollision(from, to, width, delay, missileSpeed, isArc))
                   || (checkYasuoWall && CheckYasuoWallCollision(from, to, width, isArc))
                   || (checkAllyHero && CheckAllyHeroCollision(from, to, width, delay, missileSpeed, isArc))
                   || (checkWall && CheckWallCollision(from, to));
        }

        /// <summary>
        /// Checks minion collisions
        /// </summary>
        /// <param name="from">Start position</param>
        /// <param name="to">End position</param>
        /// <param name="width">Rectangle scale</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="isArc">Checks collision for arc spell</param>
        /// <returns>true if collision found</returns>
        public static bool CheckMinionCollision(Vector2 from, Vector2 to, float width, float delay, float missileSpeed = 0, bool isArc = false)
        {
            var spellHitBox = ClipperWrapper.MakePaths(ClipperWrapper.DefineRectangle(from, to, width));
            if (isArc)
            {
                spellHitBox = ClipperWrapper.MakePaths(new SPrediction.Geometry.Polygon(
                                ClipperWrapper.DefineArc(from - new Vector2(900 / 2f, 20), to, (float)Math.PI * (to.Distance(from) / 900), 410, 200 * (to.Distance(from) / 900)),
                                ClipperWrapper.DefineArc(from - new Vector2(900 / 2f, 20), to, (float)Math.PI * (to.Distance(from) / 900), 410, 320 * (to.Distance(from) / 900))));

            }
            return MinionManager.GetMinions(from.Distance(to) + 250, MinionManager.MinionTypes.All, MinionManager.MinionTeam.NotAlly, MinionManager.MinionOrderTypes.None).AsParallel().Any(p => ClipperWrapper.IsIntersects(ClipperWrapper.MakePaths(ClipperWrapper.DefineCircle(Prediction.GetFastUnitPosition(p, delay, missileSpeed), p.BoundingRadius * 2f + 10f)), spellHitBox));
        }

        /// <summary>
        /// Checks enemy hero collisions
        /// </summary>
        /// <param name="from">Start position</param>
        /// <param name="to">End position</param>
        /// <param name="width">Rectangle scale</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="isArc">Checks collision for arc spell</param>
        /// <returns>true if collision found</returns>
        public static bool CheckEnemyHeroCollision(Vector2 from, Vector2 to, float width, float delay, float missileSpeed = 0, bool isArc = false)
        {
            var spellHitBox = ClipperWrapper.MakePaths(ClipperWrapper.DefineRectangle(from, to, width));
            if (isArc)
            {
                spellHitBox = ClipperWrapper.MakePaths(new SPrediction.Geometry.Polygon(
                                ClipperWrapper.DefineArc(from - new Vector2(900 / 2f, 20), to, (float)Math.PI * (to.Distance(from) / 900), 410, 200 * (to.Distance(from) / 900)),
                                ClipperWrapper.DefineArc(from - new Vector2(900 / 2f, 20), to, (float)Math.PI * (to.Distance(from) / 900), 410, 320 * (to.Distance(from) / 900))));
            }
            return GameObjects.EnemyHeroes.AsParallel().Any(p => ClipperWrapper.IsIntersects(ClipperWrapper.MakePaths(ClipperWrapper.DefineCircle(Prediction.GetFastUnitPosition(p, delay, missileSpeed), p.BoundingRadius)), spellHitBox));
        }

        /// <summary>
        /// Checks enemy hero collisions
        /// </summary>
        /// <param name="from">Start position</param>
        /// <param name="to">End position</param>
        /// <param name="width">Rectangle scale</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="isArc">Checks collision for arc spell</param>
        /// <returns>true if collision found</returns>
        public static bool CheckAllyHeroCollision(Vector2 from, Vector2 to, float width, float delay, float missileSpeed = 0, bool isArc = false)
        {
            var spellHitBox = ClipperWrapper.MakePaths(ClipperWrapper.DefineRectangle(from, to, width));
            if (isArc)
            {
                spellHitBox = ClipperWrapper.MakePaths(new SPrediction.Geometry.Polygon(
                                ClipperWrapper.DefineArc(from - new Vector2(900 / 2f, 20), to, (float)Math.PI * (to.Distance(from) / 900), 410, 200 * (to.Distance(from) / 900)),
                                ClipperWrapper.DefineArc(from - new Vector2(900 / 2f, 20), to, (float)Math.PI * (to.Distance(from) / 900), 410, 320 * (to.Distance(from) / 900))));
            }
            return GameObjects.AllyHeroes.AsParallel().Any(p => ClipperWrapper.IsIntersects(ClipperWrapper.MakePaths(ClipperWrapper.DefineCircle(Prediction.GetFastUnitPosition(p, delay, missileSpeed), p.BoundingRadius)), spellHitBox));
        }

        /// <summary>
        /// Checks wall collisions
        /// </summary>
        /// <param name="from">Start position</param>
        /// <param name="to">End position</param>
        /// <returns>true if collision found</returns>
        public static bool CheckWallCollision(Vector2 from, Vector2 to)
        {
            float step = from.Distance(to) / 20;
            for (var i = 0; i < 20; i++)
            {
                var p = from.Extend(to, step * i);
                if (NavMesh.GetCollisionFlags(p.X, p.Y).HasFlag(CollisionFlags.Wall))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Check Yasuo wall collisions
        /// </summary>
        /// <param name="from">Start position</param>
        /// <param name="to">End position</param>
        /// <param name="width">Rectangle scale</param>
        /// <param name="isArc">Check collision for arc spell</param>
        /// <returns>true if collision found</returns>
        public static bool CheckYasuoWallCollision(Vector2 from, Vector2 to, float width, bool isArc = false)
        {
            if (GameObjects.EnemyHeroes.All(x => x.CharacterName != "Yasuo"))
            {
                return false;
            }

            var spellHitBox = ClipperWrapper.DefineRectangle(from, to, width);

            if (isArc)
            {
                spellHitBox = new SPrediction.Geometry.Polygon(
                                ClipperWrapper.DefineArc(from - new Vector2(900 / 2f, 20), to, (float)Math.PI * (to.Distance(from) / 900), 410, 200 * (to.Distance(from) / 900)),
                                ClipperWrapper.DefineArc(from - new Vector2(900 / 2f, 20), to, (float)Math.PI * (to.Distance(from) / 900), 410, 320 * (to.Distance(from) / 900)));
            }

            foreach (var effectEmitter in GameObjects.ParticleEmitters)
            {
                if (effectEmitter.IsValid &&
                    Regex.IsMatch(effectEmitter.Name, @"Yasuo_.+_w_windwallenemy\d", RegexOptions.IgnoreCase))
                {
                    var wall = effectEmitter;
                    var level = wall.Name.Substring(wall.Name.Length - 2, 2);
                    var wallWidth = 250 + 50 * Convert.ToInt32(level);
                    var wallDirection = wall.Perpendicular.ToVector2();
                    var wallStart = wall.Position.ToVector2() + wallWidth / 2 * wallDirection;
                    var wallEnd = wallStart - wallWidth * wallDirection;
                    var wallPoly = ClipperWrapper.DefineRectangle(wallStart, wallEnd, 5);

                    if (Variables.GameTimeTickCount < wall.RestartTime + 4000)
                    {
                        if (ClipperWrapper.IsIntersects(ClipperWrapper.MakePaths(wallPoly), ClipperWrapper.MakePaths(spellHitBox)))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks Yasuo wall collisions
        /// </summary>
        /// <param name="poly">Polygon to check collision</param>
        /// <returns>true if collision found</returns>
        public static bool CheckYasuoWallCollision(Geometry.Polygon spellHitBox)
        {
            if (GameObjects.EnemyHeroes.All(x => x.CharacterName != "Yasuo"))
            {
                return false;
            }

            foreach (var effectEmitter in GameObjects.ParticleEmitters)
            {
                if (effectEmitter.IsValid &&
                    Regex.IsMatch(effectEmitter.Name, @"Yasuo_.+_w_windwallenemy\d", RegexOptions.IgnoreCase))
                {
                    var wall = effectEmitter;
                    var level = wall.Name.Substring(wall.Name.Length - 2, 2);
                    var wallWidth = 250 + 50 * Convert.ToInt32(level);
                    var wallDirection = wall.Perpendicular.ToVector2();
                    var wallStart = wall.Position.ToVector2() + wallWidth / 2 * wallDirection;
                    var wallEnd = wallStart - wallWidth * wallDirection;
                    var wallPoly = ClipperWrapper.DefineRectangle(wallStart, wallEnd, 5);

                    if (Variables.GameTimeTickCount < wall.RestartTime + 4000)
                    {
                        if (ClipperWrapper.IsIntersects(ClipperWrapper.MakePaths(wallPoly), ClipperWrapper.MakePaths(spellHitBox)))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets collided units & flags
        /// </summary>
        /// <param name="from">Start position</param>
        /// <param name="to">End position</param>
        /// <param name="width">Rectangle scale</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <returns>Collision result as <see cref="Collision.Result"/></returns>
        public static Result GetCollisions(Vector2 from, Vector2 to, float range, float width, float delay, float missileSpeed = 0, bool isArc = false)
        {
            List<AIBaseClient> collidedUnits = new List<AIBaseClient>();
            var spellHitBox = ClipperWrapper.MakePaths(ClipperWrapper.DefineRectangle(from, to.Extend(from, -width), width));
            if (isArc)
            {
                spellHitBox = ClipperWrapper.MakePaths(new SPrediction.Geometry.Polygon(
                                ClipperWrapper.DefineArc(from - new Vector2(900 / 2f, 20), to, (float)Math.PI * (to.Distance(from) / 900), 410, 200 * (to.Distance(from) / 900)),
                                ClipperWrapper.DefineArc(from - new Vector2(900 / 2f, 20), to, (float)Math.PI * (to.Distance(from) / 900), 410, 320 * (to.Distance(from) / 900))));
            }
            Flags _colFlags = Flags.None;
            var collidedMinions = MinionManager.GetMinions(range + 100, MinionManager.MinionTypes.All, MinionManager.MinionTeam.NotAlly, MinionManager.MinionOrderTypes.None).AsParallel().Where(p => ClipperWrapper.IsIntersects(ClipperWrapper.MakePaths(ClipperWrapper.DefineCircle(Prediction.GetFastUnitPosition(p, delay, missileSpeed), p.BoundingRadius + 15)), spellHitBox));
            var collidedEnemies = GameObjects.EnemyHeroes.AsParallel().Where(p => ClipperWrapper.IsIntersects(ClipperWrapper.MakePaths(ClipperWrapper.DefineCircle(Prediction.GetFastUnitPosition(p, delay, missileSpeed), p.BoundingRadius)), spellHitBox));
            var collidedAllies = GameObjects.AllyHeroes.AsParallel().Where(p => ClipperWrapper.IsIntersects(ClipperWrapper.MakePaths(ClipperWrapper.DefineCircle(Prediction.GetFastUnitPosition(p, delay, missileSpeed), p.BoundingRadius)), spellHitBox));

            if (collidedMinions != null && collidedMinions.Count() != 0)
            {
                collidedUnits.AddRange(collidedMinions);
                _colFlags |= Flags.Minions;
            }

            if (collidedEnemies != null && collidedEnemies.Count() != 0)
            {
                collidedUnits.AddRange(collidedEnemies);
                _colFlags |= Flags.EnemyChampions;
            }

            if (collidedAllies != null && collidedAllies.Count() != 0)
            {
                collidedUnits.AddRange(collidedAllies);
                _colFlags |= Flags.AllyChampions;
            }

            if (CheckWallCollision(from, to))
                _colFlags |= Flags.Wall;

            if (CheckYasuoWallCollision(from, to, width))
                _colFlags |= Flags.YasuoWall;

            return new Result(collidedUnits, _colFlags);
        }
    }
}
