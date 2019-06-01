/*
 Copyright 2015 - 2015 SPrediction
 Drawings.cs is part of SPrediction
 
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

using EnsoulSharp;
using EnsoulSharp.SDK;

using SharpDX;

namespace SPrediction
{
    /// <summary>
    /// SPrediction Drawings class
    /// </summary>
    public static class Drawings
    {
        #region Internal Properties

        internal static string s_DrawHitChance;
        internal static Vector2 s_DrawPos;
        internal static Vector2 s_DrawDirection;
        internal static int s_DrawTick;
        internal static int s_DrawWidth;

        #endregion

        #region Initializer Method

        public static void Initialize()
        {
            s_DrawTick = 0;

            Drawing.OnDraw += Drawing_OnDraw;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// OnDraw event for prediction drawings
        /// </summary>
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (ConfigMenu.SelectedPrediction.Index == 0 && ConfigMenu.EnableDrawings)
            {
                foreach (var enemy in GameObjects.EnemyHeroes)
                {
                    var waypoints = enemy.GetWaypoints();
                    if (waypoints != null && waypoints.Count > 1)
                    {
                        for (int i = 0; i < waypoints.Count - 1; i++)
                        {
                            Vector2 posFrom = Drawing.WorldToScreen(waypoints[i].ToVector3());
                            Vector2 posTo = Drawing.WorldToScreen(waypoints[i + 1].ToVector3());
                            Drawing.DrawLine(posFrom, posTo, 2, System.Drawing.Color.Aqua);
                        }

                        Vector2 pos = Drawing.WorldToScreen(waypoints[waypoints.Count - 1].ToVector3());
                        Drawing.DrawText(pos.X, pos.Y, System.Drawing.Color.Black, (waypoints.PathLength() / enemy.MoveSpeed).ToString("0.00")); //arrival time
                    }
                }

                if (Variables.TickCount - s_DrawTick <= 2000)
                {
                    Vector2 centerPos = Drawing.WorldToScreen((s_DrawPos - s_DrawDirection * 5).ToVector3());
                    Vector2 startPos = Drawing.WorldToScreen((s_DrawPos - s_DrawDirection * s_DrawWidth).ToVector3());
                    Vector2 endPos = Drawing.WorldToScreen((s_DrawPos + s_DrawDirection * s_DrawWidth).ToVector3());
                    Drawing.DrawLine(startPos, endPos, 3, System.Drawing.Color.Gold);
                    Drawing.DrawText(centerPos.X, centerPos.Y, System.Drawing.Color.Red, s_DrawHitChance);
                }
            }
        }

        #endregion
    }
}
