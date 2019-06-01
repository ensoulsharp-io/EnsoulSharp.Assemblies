/*
 Copyright 2015 - 2015 SPrediction
 Obj_AI_HeroExtensions.cs is part of SPrediction
 
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
using System.Runtime.CompilerServices;

using EnsoulSharp;

namespace SPrediction
{
    /// <summary>
    /// AIHeroClient extensions for SPrediction
    /// </summary>
    public static class AIHeroClientExtensions
    {
        /// <summary>
        /// Gets passed time without moving
        /// </summary>
        /// <param name="t">target</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MovImmobileTime(this AIHeroClient t)
        {
            Prediction.AssertInitializationMode();
            return PathTracker.EnemyInfo[t.NetworkId].IsStopped ? Environment.TickCount - PathTracker.EnemyInfo[t.NetworkId].StopTick : 0;
        }

        /// <summary>
        /// Gets passed time from last movement change
        /// </summary>
        /// <param name="t">target</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LastMovChangeTime(this AIHeroClient t)
        {
            Prediction.AssertInitializationMode();
            return Environment.TickCount - PathTracker.EnemyInfo[t.NetworkId].LastWaypointTick;
        }

        /// <summary>
        /// Gets average movement reaction time
        /// </summary>
        /// <param name="t">target</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AvgMovChangeTime(this AIHeroClient t)
        {
            Prediction.AssertInitializationMode();
            return PathTracker.EnemyInfo[t.NetworkId].AvgTick + ConfigMenu.IgnoreReactionDelay;
        }

        /// <summary>
        /// Gets average path lenght
        /// </summary>
        /// <param name="t">target</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AvgPathLenght(this AIHeroClient t)
        {
            Prediction.AssertInitializationMode();
            return PathTracker.EnemyInfo[t.NetworkId].AvgPathLenght;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t">target</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LastAngleDiff(this AIHeroClient t)
        {
            Prediction.AssertInitializationMode();
            return PathTracker.EnemyInfo[t.NetworkId].LastAngleDiff;
        }
    }
}
