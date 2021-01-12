/*
 Copyright 2015 - 2015 SPrediction
 CirclePrediction.cs is part of SPrediction
 
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
    /// Circle prediction class
    /// </summary>
    public static class CirclePrediction
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
        /// <param name="type">Spell skillshot type</param>
        /// <param name="path">Waypoints of target</param>
        /// <param name="avgt">Average reaction time (in ms)</param>
        /// <param name="movt">Passed time from last movement change (in ms)</param>
        /// <param name="avgp">Average Path Lenght</param>
        /// <param name="anglediff"></param>
        /// <param name="from">Spell casted position</param>
        /// <param name="rangeCheckFrom"></param>
        /// <returns>Prediction result as <see cref="PredictionResult"/></returns>
        public static PredictionResult GetPrediction(AIBaseClient target, float width, float delay, float missileSpeed, float range, bool collisionable, List<Vector2> path, float avgt, float movt, float avgp, float anglediff, Vector2 from, Vector2 rangeCheckFrom)
        {
            return PredictionExtensions.GetPrediction(target, width, delay, missileSpeed, range, collisionable, SpellType.Circle, path, avgt, movt, avgp, anglediff, from, rangeCheckFrom);
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
        /// <returns>Prediction result as <see cref="SpellAoeResult"/></returns>
        public static SpellAoeResult GetAoePrediction(float width, float delay, float missileSpeed, float range, Vector2 from, Vector2 rangeCheckFrom)
        {
            var result = new SpellAoeResult();
            var enemies = GameObjects.EnemyHeroes.Where(p => p.IsValidTarget() && PredictionExtensions.GetFastUnitPosition(p, delay, 0, from).Distance(rangeCheckFrom) < range).ToList();

            if (enemies.Count > 0)
            {
                var posSummary = Vector2.Zero;

                // ReSharper disable once AccessToModifiedClosure
                enemies.AsParallel().ForAll(p => posSummary += PredictionExtensions.GetFastUnitPosition(p, delay, missileSpeed, from));

                var center = posSummary / enemies.Count;
                float flyTime = 0;
                if (missileSpeed != 0)
                {
                    flyTime = from.Distance(center) / missileSpeed;
                }

                posSummary = Vector2.Zero;
                var predictionResults = new List<Tuple<PredictionResult, float>>();

                foreach (var enemy in enemies)
                {
                    var prediction = GetPrediction(enemy, width, delay + flyTime, 0, range, false, enemy.GetWaypoints(), enemy.AvgMovChangeTime(), enemy.LastMovChangeTime(), enemy.AvgPathLenght(), enemy.LastAngleDiff(), from, rangeCheckFrom);
                    if (prediction.HitChance > HitChance.Medium)
                    {
                        posSummary += prediction.UnitPosition;
                        predictionResults.Add(new Tuple<PredictionResult, float>(prediction, enemy.BoundingRadius));
                    }
                }

                if (predictionResults.Count > 0)
                {
                    center = posSummary / predictionResults.Count;
                    result.CastPosition = center;
                    foreach (var unused in predictionResults.Where(res => center.CircleCircleIntersection(res.Item1.UnitPosition, width, res.Item2).Length > 1))
                    {
                        result.HitCount++;
                    }
                }

                predictionResults.Clear();
                GC.Collect(GC.GetGeneration(predictionResults));
            }

            return result;
        }
    }
}
