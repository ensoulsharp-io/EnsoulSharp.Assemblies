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

namespace SPrediction
{
    using System.Linq;

    using EnsoulSharp;

    /// <summary>
    /// AIBaseClient extensions for SPrediction
    /// </summary>
    public static class AIBaseClientExtensions
    {
        /// <summary>
        /// Checks if the given target is immobile
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns></returns>
        internal static bool IsImmobileTarget(this AIBaseClient target)
        {
            return target.Buffs.Any(p => p.Type.IsImmobilizeBuff());
        }

        /// <summary>
        /// Gets left immobile time of given target
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns></returns>
        internal static float LeftImmobileTime(this AIBaseClient target)
        {
            return target.Buffs.Where(p => p.IsActive && p.Type.IsImmobilizeBuff() && p.EndTime > Game.Time).Max(q => q.EndTime - Game.Time);
        }
    }
}
