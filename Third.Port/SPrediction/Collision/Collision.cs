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

namespace SPrediction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using SharpDX;

    using EnsoulSharp;
    using EnsoulSharp.SDK;

    /// <summary>
    /// SPrediction Collision class
    /// </summary>
    public static class Collision
    {
        /// <summary>
        /// Checks wall collisions
        /// </summary>
        /// <param name="from">Start position</param>
        /// <param name="to">End position</param>
        /// <returns>true if collision found</returns>
        public static bool CheckWallCollision(Vector2 from, Vector2 to)
        {
            var step = from.Distance(to) / 20;
            for (var i = 0; i < 20; i++)
            {
                var p = from.Extend(to, step * i);
                if (NavMesh.GetCollisionFlags(p.X, p.Y).HasFlag(EnsoulSharp.CollisionFlags.Wall))
                {
                    return true;
                }
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
                spellHitBox = new Geometry.Polygon(
                                ClipperWrapper.DefineArc(from - new Vector2(900 / 2f, 20), to, (float)Math.PI * (to.Distance(from) / 900), 410, 200 * (to.Distance(from) / 900)),
                                ClipperWrapper.DefineArc(from - new Vector2(900 / 2f, 20), to, (float)Math.PI * (to.Distance(from) / 900), 410, 320 * (to.Distance(from) / 900)));
            }

            foreach (var effectEmitter in ObjectManager.Get<EffectEmitter>())
            {
                if (effectEmitter != null && effectEmitter.IsValid && Regex.IsMatch(effectEmitter.Name, @"Yasuo_.+_w_windwallenemy\d", RegexOptions.IgnoreCase))
                {
                    var wall = effectEmitter;
                    var level = wall.Name.Substring(wall.Name.Length - 2, 2);
                    var wallWidth = 250 + 50 * Convert.ToInt32(level);
                    var wallDirection = wall.Position.Perpendicular().ToVector2();
                    var wallStart = wall.Position.ToVector2() + wallWidth / 2f * wallDirection;
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
        /// <param name="range"></param>
        /// <param name="width">Rectangle scale</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="isArc"></param>
        /// <returns>Collision result as <see cref="CollisionResult"/></returns>
        public static CollisionResult GetCollisions(Vector2 from, Vector2 to, float range, float width, float delay, float missileSpeed = 0, bool isArc = false)
        {
            var collidedUnits = new List<AIBaseClient>();
            var spellHitBox = ClipperWrapper.MakePaths(ClipperWrapper.DefineRectangle(from, to.Extend(from, -width), width));
            if (isArc)
            {
                spellHitBox = ClipperWrapper.MakePaths(new Geometry.Polygon(
                                ClipperWrapper.DefineArc(from - new Vector2(900 / 2f, 20), to, (float)Math.PI * (to.Distance(from) / 900), 410, 200 * (to.Distance(from) / 900)),
                                ClipperWrapper.DefineArc(from - new Vector2(900 / 2f, 20), to, (float)Math.PI * (to.Distance(from) / 900), 410, 320 * (to.Distance(from) / 900))));
            }
            var _colFlags = CollisionFlags.None;
            var collidedMinions = GameObjects.GetMinions(range + 100).AsParallel().Where(p => ClipperWrapper.IsIntersects(ClipperWrapper.MakePaths(ClipperWrapper.DefineCircle(PredictionExtensions.GetFastUnitPosition(p, delay, missileSpeed), p.BoundingRadius + 15)), spellHitBox)).ToList();
            var collidedJungles = GameObjects.GetJungles(range + 100).AsParallel().Where(p => ClipperWrapper.IsIntersects(ClipperWrapper.MakePaths(ClipperWrapper.DefineCircle(PredictionExtensions.GetFastUnitPosition(p, delay, missileSpeed), p.BoundingRadius + 15)), spellHitBox)).ToList();
            var collidedEnemies = GameObjects.EnemyHeroes.AsParallel().Where(p => ClipperWrapper.IsIntersects(ClipperWrapper.MakePaths(ClipperWrapper.DefineCircle(PredictionExtensions.GetFastUnitPosition(p, delay, missileSpeed), p.BoundingRadius)), spellHitBox)).ToList();
            var collidedAllies = GameObjects.AllyHeroes.AsParallel().Where(p => ClipperWrapper.IsIntersects(ClipperWrapper.MakePaths(ClipperWrapper.DefineCircle(PredictionExtensions.GetFastUnitPosition(p, delay, missileSpeed), p.BoundingRadius)), spellHitBox)).ToList();

            if (collidedMinions.Count != 0)
            {
                collidedUnits.AddRange(collidedMinions);
                _colFlags |= CollisionFlags.Minions;
            }

            if (collidedJungles.Count != 0)
            {
                collidedUnits.AddRange(collidedJungles);
                _colFlags |= CollisionFlags.Minions;
            }

            if (collidedEnemies.Count != 0)
            {
                collidedUnits.AddRange(collidedEnemies);
                _colFlags |= CollisionFlags.EnemyChampions;
            }

            if (collidedAllies.Count != 0)
            {
                collidedUnits.AddRange(collidedAllies);
                _colFlags |= CollisionFlags.AllyChampions;
            }

            if (CheckWallCollision(from, to))
            {
                _colFlags |= CollisionFlags.Wall;
            }

            if (CheckYasuoWallCollision(from, to, width))
            {
                _colFlags |= CollisionFlags.YasuoWall;
            }

            return new CollisionResult(collidedUnits, _colFlags);
        }
    }
}
