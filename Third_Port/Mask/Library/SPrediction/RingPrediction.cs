/*
 Copyright 2015 - 2015 SPrediction
 RingPrediction.cs is part of SPrediction
 
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

using System.Collections.Generic;

using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Prediction;

using SharpDX;

namespace SPrediction
{
    /// <summary>
    /// Ring prediction class 
    /// </summary>
    public class RingPrediction
    {
        /// <summary>
        /// Gets Prediction result
        /// </summary>
        /// <param name="input">Neccesary inputs for prediction calculations</param>
        /// <param name="ringRadius">Ring radius</param>
        /// <returns>Prediction result as <see cref="Prediction.Result"/></returns>
        public static Prediction.Result GetPrediction(Prediction.Input input, float ringRadius)
        {
            return GetPrediction(input.Target, input.SpellWidth, ringRadius, input.SpellDelay, input.SpellMissileSpeed, input.SpellRange, input.SpellCollisionable, input.Path, input.AvgReactionTime, input.LastMovChangeTime, input.AvgPathLenght, input.From.ToVector2(), input.RangeCheckFrom.ToVector2());
        }

        /// <summary>
        /// Gets Prediction result
        /// </summary>
        /// <param name="target">Target for spell</param>
        /// <param name="radius">Spell radius</param>
        /// <param name="ringRadius">Ring radius</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="range">Spell range</param>
        /// <param name="collisionable">Spell collisionable</param>
        /// <returns>Prediction result as <see cref="Prediction.Result"/></returns>
        public static Prediction.Result GetPrediction(AIHeroClient target, float radius, float ringRadius, float delay, float missileSpeed, float range, bool collisionable)
        {
            return GetPrediction(target, radius, ringRadius, delay, missileSpeed, range, collisionable, target.GetWaypoints(), target.AvgMovChangeTime(), target.LastMovChangeTime(), target.AvgPathLenght(), ObjectManager.Player.PreviousPosition.ToVector2(), ObjectManager.Player.PreviousPosition.ToVector2());
        }

        /// <summary>
        /// Gets Prediction result
        /// </summary>
        /// <param name="target">Target for spell</param>
        /// <param name="radius">Spell radius</param>
        /// <param name="ringRadius">Ring radius</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="range">Spell range</param>
        /// <param name="collisionable">Spell collisionable</param>
        /// <param name="type">Spell skillshot type</param>
        /// <param name="path">Waypoints of target</param>
        /// <param name="avgt">Average reaction time (in ms)</param>
        /// <param name="movt">Passed time from last movement change (in ms)</param>
        /// <param name="avgp">Average Path Lenght</param>
        /// <param name="from">Spell casted position</param>
        /// <param name="rangeCheckFrom"></param>
        /// <returns>Prediction result as <see cref="Prediction.Result"/></returns>
        public static Prediction.Result GetPrediction(AIBaseClient target, float radius, float ringRadius, float delay, float missileSpeed, float range, bool collisionable, List<Vector2> path, float avgt, float movt, float avgp, Vector2 from, Vector2 rangeCheckFrom)
        {
            //if you are copying it negro; dont forget sprediction credits, ty.
            Prediction.Result result = CirclePrediction.GetPrediction(target, ringRadius, delay, missileSpeed, range + radius, collisionable, path, avgt, movt, avgp, 360, from, rangeCheckFrom);
            if (result.HitChance > HitChance.Low)
            {
                Vector2 direction = (result.CastPosition - from + target.Direction.ToVector2()).Normalized();
                result.CastPosition -= direction * (radius - ringRadius / 2f);

                if (result.CastPosition.Distance(from) > range)
                    result.HitChance = HitChance.OutOfRange;
            }

            return result;
        }
    }
}
