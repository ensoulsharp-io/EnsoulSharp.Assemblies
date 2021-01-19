/*
 Copyright 2015 - 2015 SPrediction
 ConfigMenu.cs is part of SPrediction
 
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
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;

    /// <summary>
    /// SPrediction Config Menu class
    /// </summary>
    public static class Program
    {
        public static Menu Menu;

        private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += () =>
            {
                var sPred = new Prediction();
                EnsoulSharp.SDK.Prediction.AddPrediction("SPrediction", sPred);
                EnsoulSharp.SDK.Prediction.SetPrediction("SPrediction");
            };
        }

        /// <summary>
        /// Gets or sets Check AA WindUp value
        /// </summary>
        public static bool CheckAAWindUp
        {
            get => Menu["SPREDWINDUP"].GetValue<MenuBool>().Enabled;
            set => Menu["SPREDWINDUP"].GetValue<MenuBool>().Enabled = value;
        }

        /// <summary>
        /// Gets or sets max range ignore value
        /// </summary>
        public static int MaxRangeIgnore
        {
            get => Menu["SPREDMAXRANGEIGNORE"].GetValue<MenuSlider>().Value;
            set => Menu["SPREDMAXRANGEIGNORE"].GetValue<MenuSlider>().Value = value;
        }

        /// <summary>
        /// Gets or sets ignore reaction delay value
        /// </summary>
        public static int IgnoreReactionDelay
        {
            get => Menu["SPREDREACTIONDELAY"].GetValue<MenuSlider>().Value;
            set => Menu["SPREDREACTIONDELAY"].GetValue<MenuSlider>().Value = value;
        }

        /// <summary>
        /// Gets or sets spell delay value
        /// </summary>
        public static int SpellDelay
        {
            get => Menu["SPREDDELAY"].GetValue<MenuSlider>().Value;
            set => Menu["SPREDDELAY"].GetValue<MenuSlider>().Value = value;
        }
    }
}
