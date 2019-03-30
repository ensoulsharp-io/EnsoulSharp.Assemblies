// Copyright 2014 - 2014 Esk0r
// Evader.cs is part of Evade.
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

using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using SharpDX;

#endregion

namespace Evade
{
    public static class Evader
    {
        public static Vector2 GetClosestOutsidePoint(Vector2 from, List<Geometry.Polygon> polygons)
        {
            var result = new List<Vector2>();

            foreach (var poly in polygons)
            {
                for (var i = 0; i <= poly.Points.Count - 1; i++)
                {
                    var sideStart = poly.Points[i];
                    var sideEnd = poly.Points[i == poly.Points.Count - 1 ? 0 : i + 1];

                    result.Add(from.ProjectOn(sideStart, sideEnd).SegmentPoint);
                }
            }

            return result.MinOrDefault(v => v.Distance(from));
        }

        /// <summary>
        /// Returns the possible evade points.
        /// </summary>
        public static List<Vector2> GetEvadePoints(int speed = -1, int delay = 0, bool isBlink = false, bool onlyGood = false)
        {
            speed = speed == -1 ? (int)ObjectManager.Player.MoveSpeed : speed;

            var goodCandidates = new List<Vector2>();
            var badCandidates = new List<Vector2>();

            var polygonList = new List<Geometry.Polygon>();

            var takeClosestPath = false;

            foreach (var skillshot in Program.DetectedSkillshots)
            {
                if (skillshot.Evade())
                {
                    if (skillshot.SpellData.TakeClosestPath && skillshot.IsDanger(Program.PlayerPosition))
                    {
                        takeClosestPath = true;
                    }

                    polygonList.Add(skillshot.EvadePolygon);
                }
            }

            // Create the danger polygon.
            var dangerPolygons = Geometry.ClipPolygons(polygonList).ToPolygons();
            var myPosition = Program.PlayerPosition;

            // Scan the sides of each polygon to find the safe point.
            foreach (var poly in dangerPolygons)
            {
                for (var i = 0; i <= poly.Points.Count - 1; i++)
                {
                    var sideStart = poly.Points[i];
                    var sideEnd = poly.Points[i == poly.Points.Count - 1 ? 0 : i + 1];

                    var originalCandidate = myPosition.ProjectOn(sideStart, sideEnd).SegmentPoint;
                    var distanceToEvadePoint = Vector2.DistanceSquared(originalCandidate, myPosition);

                    if (distanceToEvadePoint < 600 * 600)
                    {
                        var sideDistance = Vector2.DistanceSquared(sideEnd, sideStart);
                        var direction = (sideEnd - sideStart).Normalized();

                        var s = (distanceToEvadePoint < 200 * 200 && sideDistance > 90 * 90)
                            ? Config.DiagonalEvadePointsCount
                            : 0;

                        for (var j = -s; j <= s; j++)
                        {
                            var candidate = originalCandidate + j * Config.DiagonalEvadePointsStep * direction;
                            var pathToPoint = ObjectManager.Player.GetPath(candidate.ToVector3()).To2DList();

                            if (!isBlink)
                            {
                                if (Program.IsSafePath(pathToPoint, Config.EvadingFirstTimeOffset, speed, delay).IsSafe)
                                {
                                    goodCandidates.Add(candidate);
                                }

                                if (Program.IsSafePath(pathToPoint, Config.EvadingSecondTimeOffset, speed, delay).IsSafe && j == 0)
                                {
                                    badCandidates.Add(candidate);
                                }
                            }
                            else
                            {
                                if (Program.IsSafeToBlink(pathToPoint[pathToPoint.Count - 1], Config.EvadingFirstTimeOffset, delay))
                                {
                                    goodCandidates.Add(candidate);
                                }

                                if (Program.IsSafeToBlink(pathToPoint[pathToPoint.Count - 1], Config.EvadingSecondTimeOffset, delay))
                                {
                                    badCandidates.Add(candidate);
                                }
                            }
                        }
                    }
                }
            }

            if (takeClosestPath)
            {
                if (goodCandidates.Count > 0)
                {
                    goodCandidates = new List<Vector2>
                    {
                        goodCandidates.MinOrDefault(v => ObjectManager.Player.DistanceSquared(v))
                    };
                }

                if (badCandidates.Count > 0)
                {
                    badCandidates = new List<Vector2>
                    {
                        badCandidates.MinOrDefault(v => ObjectManager.Player.DistanceSquared(v))
                    };
                }
            }

            return (goodCandidates.Count > 0) ? goodCandidates : (onlyGood ? new List<Vector2>() : badCandidates);
        }

        /// <summary>
        /// Returns the safe targets to cast escape spells.
        /// </summary>
        public static List<AIBaseClient> GetEvadeTargets(SpellValidTargets[] validTargets,
            int speed,
            int delay,
            float range,
            bool isBlink = false,
            bool onlyGood = false,
            bool DontCheckForSafety = false)
        {
            var badTargets = new List<AIBaseClient>();
            var goodTargets = new List<AIBaseClient>();
            var allTargets = new List<AIBaseClient>();

            foreach (var targetType in validTargets)
            {
                switch (targetType)
                {
                    case SpellValidTargets.AllyMinions:
                        allTargets.AddRange(GameObjects.AllyMinions.Where(m => m.IsValidTarget(range, false)));
                        break;
                    case SpellValidTargets.EnemyMinions:
                        allTargets.AddRange(GameObjects.EnemyMinions.Where(m => m.IsValidTarget(range)));
                        break;
                    case SpellValidTargets.AllyWards:
                        allTargets.AddRange(GameObjects.AllyWards.Where(w => w.IsValidTarget(range, false)));
                        break;
                    case SpellValidTargets.EnemyWards:
                        allTargets.AddRange(GameObjects.EnemyWards.Where(w => w.IsValidTarget(range)));
                        break;
                    case SpellValidTargets.AllyChampions:
                        allTargets.AddRange(GameObjects.AllyHeroes.Where(h => h.IsValidTarget(range, false) && !h.IsMe));
                        break;
                    case SpellValidTargets.EnemyChampions:
                        allTargets.AddRange(GameObjects.EnemyHeroes.Where(h => h.IsValidTarget(range)));
                        break;
                }
            }

            foreach (var target in allTargets)
            {
                if (DontCheckForSafety || Program.IsSafe(target.Position.ToVector2()).IsSafe)
                {
                    if (isBlink)
                    {
                        if (Utils.TickCount - Program.LastWardJumpAttempt < 250 ||
                            Program.IsSafeToBlink(target.Position.ToVector2(), Config.EvadingFirstTimeOffset, delay))
                        {
                            goodTargets.Add(target);
                        }

                        if (Utils.TickCount - Program.LastWardJumpAttempt < 250 ||
                            Program.IsSafeToBlink(target.Position.ToVector2(), Config.EvadingSecondTimeOffset, delay))
                        {
                            badTargets.Add(target);
                        }
                    }
                    else
                    {
                        var pathToTarget = new List<Vector2>();
                        pathToTarget.Add(Program.PlayerPosition);
                        pathToTarget.Add(target.Position.ToVector2());

                        if (Utils.TickCount - Program.LastWardJumpAttempt < 250 ||
                            Program.IsSafePath(pathToTarget, Config.EvadingFirstTimeOffset, speed, delay).IsSafe)
                        {
                            goodTargets.Add(target);
                        }

                        if (Utils.TickCount - Program.LastWardJumpAttempt < 250 ||
                            Program.IsSafePath(pathToTarget, Config.EvadingSecondTimeOffset, speed, delay).IsSafe)
                        {
                            badTargets.Add(target);
                        }
                    }
                }
            }

            return (goodTargets.Count > 0) ? goodTargets : (onlyGood ? new List<AIBaseClient>() : badTargets);
        }
    }
}