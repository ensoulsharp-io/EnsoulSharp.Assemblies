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

using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;

namespace SPrediction
{
    /// <summary>
    /// SPrediction Config Menu class
    /// </summary>
    public static class ConfigMenu
    {
        #region Private Properties

        private static Menu s_Menu = null;

        #endregion

        #region Initalizer Methods

        /// <summary>
        /// Creates the sprediciton menu and attach to the given menu
        /// </summary>
        /// <param name="menuToAttach">The menu to attach.</param>
        public static void Initialize(Menu menuToAttach, string prefMenuName)
        {
            Initialize(prefMenuName);
            if (menuToAttach == null)
                return;
            menuToAttach.Add(s_Menu);
        }

        /// <summary>
        /// Creates the sprediciton menu
        /// </summary>
        public static Menu Initialize(string prefMenuName = "SPRED")
        {
            s_Menu = new Menu(prefMenuName, "SPrediction");
            s_Menu.Add(new MenuList("PREDICTONLIST", "Prediction Method", new[] { "SPrediction", "Common Prediction" }) { Index = 0 });
            s_Menu.Add(new MenuBool("SPREDWINDUP", "Check for target AA Windup", false));
            s_Menu.Add(new MenuSlider("SPREDMAXRANGEIGNORE", "Max Range Dodge Ignore (%)", 50));
            s_Menu.Add(new MenuSlider("SPREDREACTIONDELAY", "Ignore Rection Delay", 0, 0, 200));
            s_Menu.Add(new MenuSlider("SPREDDELAY", "Spell Delay", 0, 0, 200));
            s_Menu.Add(new MenuBool("SPREDDRAWINGS", "Enable Drawings", false));

            return s_Menu;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets selected prediction for spell extensions
        /// </summary>
        public static MenuList SelectedPrediction
        {
            get { return s_Menu.GetValue<MenuList>("PREDICTONLIST"); }
        }

        /// <summary>
        /// Gets or sets Check AA WindUp value
        /// </summary>
        public static bool CheckAAWindUp
        {
            get { return s_Menu.GetValue<MenuBool>("SPREDWINDUP").Enabled; }
            set { s_Menu.GetValue<MenuBool>("SPREDWINDUP").Enabled = value; }
        }

        /// <summary>
        /// Gets or sets max range ignore value
        /// </summary>
        public static int MaxRangeIgnore
        {
            get { return s_Menu.GetValue<MenuSlider>("SPREDMAXRANGEIGNORE").Value; }
            set { s_Menu.GetValue<MenuSlider>("SPREDMAXRANGEIGNORE").Value = value; }
        }

        /// <summary>
        /// Gets or sets ignore reaction delay value
        /// </summary>
        public static int IgnoreReactionDelay
        {
            get { return s_Menu.GetValue<MenuSlider>("SPREDREACTIONDELAY").Value; }
            set { s_Menu.GetValue<MenuSlider>("SPREDREACTIONDELAY").Value = value; }
        }

        /// <summary>
        /// Gets or sets spell delay value
        /// </summary>
        public static int SpellDelay
        {
            get { return s_Menu.GetValue<MenuSlider>("SPREDDELAY").Value; }
            set { s_Menu.GetValue<MenuSlider>("SPREDDELAY").Value = value; }
        }

        /// <summary>
        /// Gets or sets drawings are enabled
        /// </summary>
        public static bool EnableDrawings
        {
            get { return s_Menu.GetValue<MenuBool>("SPREDDRAWINGS").Enabled; }
            set { s_Menu.GetValue<MenuBool>("SPREDDRAWINGS").Enabled = value; }
        }

        #endregion
    }
}
