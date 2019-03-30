// Copyright 2014 - 2014 Esk0r
// Config.cs is part of Evade.
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

using System;
using System.Linq;
using System.Windows.Forms;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Core.UI;
using EnsoulSharp.SDK.Core.UI.IMenu.Values;
using SharpDX;
using Menu = EnsoulSharp.SDK.Core.UI.IMenu.Menu;

#endregion

namespace Evade
{
    internal static class Config
    {
        public const bool PrintSpellData = false;
        public const bool TestOnAllies = false;
        public const int SkillShotsExtraRadius = 9;
        public const int SkillShotsExtraRange = 20;
        public const int GridSize = 10;
        public const int ExtraEvadeDistance = 15;
        public const int PathFindingDistance = 60;
        public const int PathFindingDistance2 = 35;

        public const int DiagonalEvadePointsCount = 7;
        public const int DiagonalEvadePointsStep = 20;

        public const int CrossingTimeOffset = 250;

        public const int EvadingFirstTimeOffset = 250;
        public const int EvadingSecondTimeOffset = 80;

        public const int EvadingRouteChangeTimeOffset = 250;

        public const int EvadePointChangeInterval = 300;
        public static int LastEvadePointChangeT = 0;

        public static Menu Menu, evadeSpells, skillShots, shielding, collision, drawings, misc;

        public static void CreateMenu()
        {
            Menu = new Menu("Evade", "Evade#", true);

            // Create the evade spells submenus.
            evadeSpells = new Menu("evadeSpells", "Evade spells");
            foreach (var spell in EvadeSpellDatabase.Spells)
            {
                var subMenu = new Menu(spell.Name, spell.Name);

                subMenu.Add(new MenuSlider("DangerLevel" + spell.Name, "Danger level", spell.DangerLevel, 1, 5));

                if (spell.IsTargetted && spell.ValidTargets.Contains(SpellValidTargets.AllyWards))
                {
                    subMenu.Add(new MenuBool("WardJump" + spell.Name, "WardJump", true));
                }

                subMenu.Add(new MenuBool("Enabled" + spell.Name, "Enabled", true));

                evadeSpells.Add(subMenu);
            }
            Menu.Add(evadeSpells);

            // Create the skillshots submenus.
            skillShots = new Menu("Skillshots", "Skillshots");
            foreach (var hero in GameObjects.Heroes)
            {
                if (hero.Team != ObjectManager.Player.Team || TestOnAllies)
                {
                    foreach (var spell in SpellDatabase.Spells)
                    {
                        if (String.Equals(spell.ChampionName, hero.CharacterName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            var subMenu = new Menu(spell.MenuItemName, spell.MenuItemName);

                            subMenu.Add(new MenuSlider("DangerLevel" + spell.MenuItemName, "Danger level", spell.DangerValue, 1, 5));
                            subMenu.Add(new MenuBool("IsDangerous" + spell.MenuItemName, "Is Dangerous", spell.IsDangerous));
                            subMenu.Add(new MenuBool("Draw" + spell.MenuItemName, "Draw", true));
                            subMenu.Add(new MenuBool("Enabled" + spell.MenuItemName, "Enabled", !spell.DisabledByDefault));

                            skillShots.Add(subMenu);
                        }
                    }
                }
            }
            Menu.Add(skillShots);

            shielding = new Menu("Shielding", "Ally shielding");
            foreach (var ally in GameObjects.AllyHeroes)
            {
                if (!ally.IsMe)
                {
                    shielding.Add(new MenuBool("shield" + ally.CharacterName, "Shield " + ally.CharacterName, true));
                }
            }
            Menu.Add(shielding);

            collision = new Menu("Collision", "Collision");
            collision.Add(new MenuBool("MinionCollision", "Minion collision", false));
            collision.Add(new MenuBool("HeroCollision", "Hero collision", false));
            collision.Add(new MenuBool("YasuoCollision", "Yasuo wall collision", true));
            collision.Add(new MenuBool("EnableCollision", "Enabled", false));
            Menu.Add(collision);

            drawings = new Menu("Drawings", "Drawings");
            drawings.Add(new MenuColor("EnabledColor", "Enabled spell color", Color.White));
            drawings.Add(new MenuColor("DisabledColor", "Disabled spell color", Color.Red));
            drawings.Add(new MenuColor("MissileColor", "Missile color", Color.LimeGreen));
            drawings.Add(new MenuSlider("Border", "Border Width", 2, 1, 5));
            drawings.Add(new MenuBool("EnableDrawings", "Enabled", true));
            Menu.Add(drawings);

            misc = new Menu("Misc", "Misc");
            misc.Add(new MenuList<string>("BlockSpells", "Block spells while evading", new[] { "No", "Only dangerous", "Always" }) { Index = 1 });
            misc.Add(new MenuBool("DisableFow", "Disable fog of war dodging", false));
            misc.Add(new MenuBool("ShowEvadeStatus", "Show Evade Status", false));
            if (ObjectManager.Player.Hero == Champion.Olaf)
            {
                misc.Add(new MenuBool("DisableEvadeForOlafR", "Automatic disable Evade when Olaf's ulti is active!", true));
            }
            Menu.Add(misc);

            Menu.Add(new MenuKeyBind("Enabled", "Enabled", Keys.K, KeyBindType.Toggle) { Active = true }).Permashow(true, "Evade");
            Menu.Add(new MenuKeyBind("OnlyDangerous", "Dodge only dangerous", Keys.Space, KeyBindType.Press)).Permashow();

            Menu.Attach();
        }
    }
}