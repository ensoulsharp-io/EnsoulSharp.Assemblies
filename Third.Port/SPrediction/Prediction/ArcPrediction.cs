/*
 Copyright 2015 - 2015 SPrediction
 ArcPrediction.cs is part of SPrediction
 
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

    using SharpDX;

    using EnsoulSharp;
    using EnsoulSharp.SDK;

    /// <summary>
    /// Arc Prediction class
    /// </summary>
    public static class ArcPrediction
    {
        /// <summary>
        /// Gets Prediction result
        /// </summary>
        /// <param name="input">Neccesary inputs for prediction calculations</param>
        /// <returns>Prediction result as <see cref="PredictionResult"/></returns>
        public static PredictionResult GetPrediction(PredictionInput input)
        {
            return GetPrediction(input.Target, input.SpellWidth, input.SpellDelay, input.SpellMissileSpeed, input.SpellRange, input.SpellCollisionable, input.Path, input.AvgReactionTime, input.LastMovChangeTime, input.AvgPathLenght, input.LastAngleDiff, input.From.ToVector2(), input.RangeCheckFrom.ToVector2());
        }

        /// <summary>
        /// Gets Prediction result
        /// </summary>
        /// <param name="target">Target for spell</param>
        /// <param name="width">Spell width</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="range">Spell range</param>
        /// <param name="collisionable">Spell collisionable</param>
        /// <returns>Prediction result as <see cref="PredictionResult"/></returns>
        public static PredictionResult GetPrediction(AIHeroClient target, float width, float delay, float missileSpeed, float range, bool collisionable)
        {
            return GetPrediction(target, width, delay, missileSpeed, range, collisionable, target.GetWaypoints(), target.AvgMovChangeTime(), target.LastMovChangeTime(), target.AvgPathLenght(), target.LastAngleDiff(), ObjectManager.Player.PreviousPosition.ToVector2(), ObjectManager.Player.PreviousPosition.ToVector2());
        }

        /// <summary>
        /// Gets Prediction result
        /// </summary>
        /// <param name="target">Target for spell</param>
        /// <param name="width">Spell width</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="range">Spell range</param>
        /// <param name="collisionable">Spell collisionable</param>
        /// <param name="path">Waypoints of target</param>
        /// <param name="avgt">Average reaction time (in ms)</param>
        /// <param name="movt">Passed time from last movement change (in ms)</param>
        /// <param name="avgp">Average Path Lenght</param>
        /// <param name="from">Spell casted position</param>
        /// <param name="rangeCheckFrom">Spell range check fropm</param>
        /// <param name="arconly">Is Arc</param>
        /// <returns>Prediction result as <see cref="PredictionResult"/></returns>
        public static PredictionResult GetPrediction(AIBaseClient target, float width, float delay, float missileSpeed, float range, bool collisionable, List<Vector2> path, float avgt, float movt, float avgp, float anglediff, Vector2 from, Vector2 rangeCheckFrom, bool arconly = true)
        {
            if (arconly)
            {
                if (target.Distance(from) < width || target.Distance(from) > range * 0.75f)
                {
                    return CirclePrediction.GetPrediction(target, width, delay, missileSpeed, range, collisionable, path, avgt, movt, avgp, anglediff, from, rangeCheckFrom);
                }

                var pred = LinePrediction.GetLinePrediction(target, 80f, delay, missileSpeed, range, collisionable, path, avgt, movt, avgp, anglediff, from, rangeCheckFrom);
                if (pred.HitChance >= HitChance.Low)
                {
                    pred.CastPosition = (from + (pred.CastPosition - from).Normalized() * range);
                    var cos = (float)Math.Cos((1 - pred.UnitPosition.Distance(from) / 820f) * Math.PI / 2);
                    var sin = (float)Math.Sin((1 - pred.UnitPosition.Distance(from) / 820f) * Math.PI / 2);
                    var x = cos * (pred.CastPosition.X - from.X) - sin * (pred.CastPosition.Y - from.Y) + from.X;
                    var y = sin * (pred.CastPosition.X - from.X) + cos * (pred.CastPosition.Y - from.Y) + from.Y;
                    pred.CastPosition = new Vector2(x, y);
                }

                return pred;
            }

            var result = new PredictionResult();

            if (path.Count <= 1) //if target is not moving, easy to hit
            {
                result.HitChance = HitChance.Immobile;
                result.CastPosition = target.PreviousPosition.ToVector2();
                result.UnitPosition = result.CastPosition;
                return result;
            }

            if (target is AIHeroClient aiHero && aiHero.IsCastingImporantSpell())
            {
                result.HitChance = HitChance.Immobile;
                result.CastPosition = aiHero.PreviousPosition.ToVector2();
                result.UnitPosition = result.CastPosition;
                return result;
            }

            if (target.IsImmobileTarget())
            {
                return PredictionExtensions.GetImmobilePrediction(target, width, delay, missileSpeed, range, collisionable, SpellType.Circle, from, rangeCheckFrom);
            }

            if (target.IsDashing())
            {
                return PredictionExtensions.GetDashingPrediction(target, width, delay, missileSpeed, range, collisionable, SpellType.Circle, from, rangeCheckFrom);
            }

            var targetDistance = rangeCheckFrom.Distance(target.PreviousPosition);
            var flyTime = 0f;

            if (missileSpeed != 0)
            {
                var Vt = (path[path.Count - 1] - path[0]).Normalized() * target.MoveSpeed;
                var Vs = (target.PreviousPosition.ToVector2() - rangeCheckFrom).Normalized() * missileSpeed;
                var Vr = Vs - Vt;

                flyTime = targetDistance / Vr.Length();

                if (path.Count > 5)
                {
                    flyTime = targetDistance / missileSpeed;
                }
            }

            var t = flyTime + delay + Game.Ping / 2000f + Program.SpellDelay / 1000f;

            result.HitChance = PredictionExtensions.GetHitChance(t * 1000f, avgt, movt, avgp, anglediff);

            //arc collision test
            if (result.HitChance > HitChance.Low)
            {
                for (var i = 1; i < path.Count; i++)
                {
                    var senderPos = rangeCheckFrom;
                    var testPos = path[i];

                    var multp = (testPos.Distance(senderPos) / 875.0f);

                    var dianaArc = new Geometry.Polygon(
                        ClipperWrapper.DefineArc(senderPos - new Vector2(875 / 2f, 20), testPos, (float)Math.PI * multp, 410, 200 * multp),
                        ClipperWrapper.DefineArc(senderPos - new Vector2(875 / 2f, 20), testPos, (float)Math.PI * multp, 410, 320 * multp));

                    if (!dianaArc.IsOutside(target.PreviousPosition.ToVector2()))
                    {
                        result.HitChance = HitChance.VeryHigh;
                        result.CastPosition = testPos;
                        result.UnitPosition = testPos;
                        return result;
                    }
                }
            }

            return CirclePrediction.GetPrediction(target, width, delay, missileSpeed, range, collisionable, path, avgt, movt, avgp, anglediff, @from, rangeCheckFrom);
        }

        /// <summary>
        /// Gets Aoe Prediction result
        /// </summary>
        /// <param name="width">Spell width</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="range">Spell range</param>
        /// <param name="from">Spell casted position</param>
        /// <param name="rangeCheckFrom"></param>
        /// <returns>Prediction result as <see cref="PredictionExtensions.AoeResult"/></returns>
        public static SpellAoeResult GetAoePrediction(float width, float delay, float missileSpeed, float range, Vector2 from, Vector2 rangeCheckFrom)
        {
            var result = new SpellAoeResult();
            var enemies = GameObjects.EnemyHeroes.Where(p => p.IsValidTarget() && PredictionExtensions.GetFastUnitPosition(p, delay, 0, from).Distance(rangeCheckFrom) < range);

            foreach (var enemy in enemies)
            {
                var prediction = GetPrediction(enemy, width, delay, missileSpeed, range, false, enemy.GetWaypoints(), enemy.AvgMovChangeTime(), enemy.LastMovChangeTime(), enemy.AvgPathLenght(), enemy.LastAngleDiff(), from, rangeCheckFrom);
                if (prediction.HitChance > HitChance.Medium)
                {
                    var multp = (result.CastPosition.Distance(from) / 875.0f);

                    var spellHitBox = new Geometry.Polygon(
                                            ClipperWrapper.DefineArc(from - new Vector2(875 / 2f, 20), result.CastPosition, (float)Math.PI * multp, 410, 200 * multp),
                                            ClipperWrapper.DefineArc(from - new Vector2(875 / 2f, 20), result.CastPosition, (float)Math.PI * multp, 410, 320 * multp));

                    var collidedEnemies = GameObjects.EnemyHeroes.AsParallel().Where(p => ClipperWrapper.IsIntersects(ClipperWrapper.MakePaths(ClipperWrapper.DefineCircle(PredictionExtensions.GetFastUnitPosition(p, delay, missileSpeed), p.BoundingRadius)), ClipperWrapper.MakePaths(spellHitBox))).ToList();
                    var collisionCount = collidedEnemies.Count;
                    if (collisionCount > result.HitCount)
                    {
                        var collisionResult = new CollisionResult(collidedEnemies.ToList<AIBaseClient>(), CollisionFlags.EnemyChampions);
                        return new SpellAoeResult
                        {
                            CastPosition = prediction.CastPosition,
                            HitCount = collisionCount,
                            CollisionResult = collisionResult
                        };
                    }
                }
            }

            return result;
        }
    }
}
