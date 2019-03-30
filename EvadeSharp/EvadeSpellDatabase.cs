// Copyright 2014 - 2014 Esk0r
// EvadeSpellDatabase.cs is part of Evade.
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
using System.Collections.Generic;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Core.Utils;

#endregion

namespace Evade
{
    internal class EvadeSpellDatabase
    {
        public static List<EvadeSpellData> Spells = new List<EvadeSpellData>();

        static EvadeSpellDatabase()
        {
            // Add available evading spells to the database. SORTED BY PRIORITY.
            EvadeSpellData spell;

            #region Champion SpellShields

            if (ObjectManager.Player.CharacterName == "Nocturne")
            {
                spell = new ShieldData("Nocturne W", SpellSlot.W, 100, 1, true);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Sivir")
            {
                spell = new ShieldData("Sivir E", SpellSlot.E, 100, 1, true);
                Spells.Add(spell);
            }

            #endregion

            // Walking
            spell = new EvadeSpellData("Walking", 1);
            Spells.Add(spell);

            #region Champion MoveSpeed buffs

            if (ObjectManager.Player.CharacterName == "Blitzcrank")
            {
                spell = new MoveBuffData("Blitzcrank W", SpellSlot.W, 100, 3,
                    () =>
                        ObjectManager.Player.MoveSpeed *
                        (1 + 0.7f + 0.05f * Math.Max(0, ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level - 1)));
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Draven")
            {
                spell = new MoveBuffData("Draven W", SpellSlot.W, 100, 3,
                    () =>
                        ObjectManager.Player.MoveSpeed *
                        (1 + 0.4f + 0.05f * Math.Max(0, ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level - 1)));
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Garen")
            {
                spell = new MoveBuffData("Garen Q", SpellSlot.Q, 100, 3,
                    () => ObjectManager.Player.MoveSpeed * 1.3f);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Kaisa")
            {
                spell = new MoveBuffData("Kaisa E", SpellSlot.E, 100, 3,
                    () =>
                        ObjectManager.Player.MoveSpeed *
                        (1 + (0.55f + 0.05f * Math.Max(0, ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level - 1))
                            * (1 + ObjectManager.Player.PercentAttackSpeedMod)));
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Karma")
            {
                spell = new MoveBuffData("Karma E Accel", SpellSlot.E, 100, 3,
                    () =>
                        ObjectManager.Player.MoveSpeed *
                        (1 + 0.4f + 0.05f * Math.Max(0, ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level - 1)));
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Katarina")
            {
                spell = new MoveBuffData("Katarina W", SpellSlot.W, 100, 3,
                    () =>
                        ObjectManager.Player.MoveSpeed *
                        (1 + 0.5f + 0.1f * Math.Max(0, ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level - 1)));
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Kayle")
            {
                spell = new MoveBuffData("Kayle W", SpellSlot.W, 100, 3,
                    () =>
                        ObjectManager.Player.MoveSpeed *
                        (1 + 0.26f + 0.06f * Math.Max(0, ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level - 1)
                        + 0.08f * ObjectManager.Player.TotalMagicalDamage / 100));
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Kennen")
            {
                spell = new MoveBuffData("Kennen E", SpellSlot.E, 100, 3,
                    () => ObjectManager.Player.MoveSpeed + 200);
                // Actually it should be +335 but ingame you only gain +230, rito plz
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Khazix")
            {
                spell = new MoveBuffData("Khazix R", SpellSlot.R, 100, 5,
                    () => ObjectManager.Player.MoveSpeed * 1.4f);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Lulu")
            {
                spell = new MoveBuffData("Lulu W", SpellSlot.W, 100, 3,
                    () =>
                        ObjectManager.Player.MoveSpeed *
                        (1.3f + 0.05f * ObjectManager.Player.TotalMagicalDamage / 100));
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Poppy")
            {
                spell = new MoveBuffData("Poppy W", SpellSlot.W, 100, 3,
                    () => ObjectManager.Player.MoveSpeed * 1.3f);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Rumble")
            {
                spell = new MoveBuffData("Rumble W Accel", SpellSlot.W, 100, 3,
                    () =>
                        ObjectManager.Player.MoveSpeed *
                        (1 + 0.1f + 0.05f * Math.Max(0, ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level - 1)) *
                        (ObjectManager.Player.HasBuff("rumbledangerzonebuff") ? 1.5f : 1));
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Shyvana")
            {
                spell = new MoveBuffData("Shyvana W", SpellSlot.W, 100, 3,
                    () =>
                        ObjectManager.Player.MoveSpeed *
                        (1 + 0.3f + 0.05f * Math.Max(0, ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level - 1)));
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Sona")
            {
                spell = new MoveBuffData("Sona E", SpellSlot.E, 100, 3,
                    () =>
                        ObjectManager.Player.MoveSpeed *
                        (1 + 0.1f + 0.01f * Math.Max(0, ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level - 1)
                        + 0.06f * ObjectManager.Player.TotalMagicalDamage / 100));
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Udyr")
            {
                spell = new MoveBuffData("Udyr E", SpellSlot.E, 100, 3,
                    () =>
                        ObjectManager.Player.MoveSpeed *
                        (1 + 0.15f + 0.05f * Math.Max(0, ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level - 1)));
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Volibear")
            {
                spell = new MoveBuffData("Volibear Q", SpellSlot.Q, 100, 3,
                    () =>
                        ObjectManager.Player.MoveSpeed *
                        (1 + 0.15f + 0.025f * Math.Max(0, ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level - 1)));
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Zilean")
            {
                spell = new MoveBuffData("Zilean E", SpellSlot.E, 100, 3,
                    () =>
                        ObjectManager.Player.MoveSpeed *
                        (1 + 0.4f + 0.15f * Math.Max(0, ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level - 1)));
                Spells.Add(spell);
            }

            #endregion

            #region Champion Dashes

            if (ObjectManager.Player.CharacterName == "Aatrox")
            {
                spell = new DashData("Aatrox E", SpellSlot.E, 300, false, 100, 800, 3);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Akali")
            {
                spell = new DashData("Akali E", SpellSlot.E, 395, true, 250, 1400, 3);
                spell.Invert = true;
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Alistar")
            {
                spell = new DashData("Alistar W", SpellSlot.W, 650, false, 100, 1500, 3);
                spell.ValidTargets = new SpellValidTargets[] { SpellValidTargets.EnemyChampions, SpellValidTargets.EnemyMinions };
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Braum")
            {
                spell = new DashData("Braum W", SpellSlot.W, 650, false, 100, 1500, 3);
                spell.ValidTargets = new SpellValidTargets[] { SpellValidTargets.AllyChampions, SpellValidTargets.AllyMinions };
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Caitlyn")
            {
                spell = new DashData("Caitlyn E", SpellSlot.E, 390, true, 250, 1000, 3);
                spell.Invert = true;
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Corki")
            {
                spell = new DashData("Corki W", SpellSlot.W, 600, false, 100, 975, 3);
                spell.CheckSpellName = "CarpetBomb";
                spell.MinRange = 300;
                Spells.Add(spell);

                spell = new DashData("Corki W Mega", SpellSlot.W, 1900, true, 100, 1500, 3);
                spell.CheckSpellName = "CarpetBombMega";
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Fiora")
            {
                spell = new DashData("Fiora Q", SpellSlot.Q, 400, false, 100, 1100, 3);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Gnar")
            {
                spell = new DashData("Gnar E", SpellSlot.E, 475, false, 100, 900, 3);
                spell.CheckSpellName = "GnarE";
                Spells.Add(spell);

                spell = new DashData("Gnar Big E", SpellSlot.E, 600, false, 100, 900, 3);
                spell.CheckSpellName = "GnarBigE";
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Gragas")
            {
                spell = new DashData("Gragas E", SpellSlot.E, 600, true, 100, 910, 3);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Graves")
            {
                spell = new DashData("Graves E", SpellSlot.E, 425, false, 100, 1140, 3);
                spell.MinRange = 270;
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Irelia")
            {
                spell = new DashData("Irelia Q", SpellSlot.Q, 600, false, 100, 1835, 3);
                spell.ValidTargets = new[] { SpellValidTargets.EnemyChampions, SpellValidTargets.EnemyMinions };
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Jax")
            {
                spell = new DashData("Jax Q", SpellSlot.Q, 700, false, 100, 1400, 3);
                spell.ValidTargets = new[]
                {
                    SpellValidTargets.AllyChampions, SpellValidTargets.AllyMinions, SpellValidTargets.AllyWards,
                    SpellValidTargets.EnemyChampions, SpellValidTargets.EnemyMinions, SpellValidTargets.EnemyWards
                };
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Jayce")
            {
                spell = new DashData("Jayce Q", SpellSlot.Q, 600, false, 100, 800, 3);
                spell.CheckSpellName = "JayceToTheSkies";
                spell.ValidTargets = new[] { SpellValidTargets.EnemyChampions, SpellValidTargets.EnemyMinions };
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Khazix")
            {
                spell = new DashData("Khazix E", SpellSlot.E, 700, false, 100, 1260, 3);
                spell.CheckSpellName = "KhazixE";
                Spells.Add(spell);

                spell = new DashData("Khazix E Long", SpellSlot.E, 900, false, 100, 1210, 3);
                spell.CheckSpellName = "KhazixELong";
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Kindred")
            {
                spell = new DashData("Kindred Q", SpellSlot.Q, 325, false, 100, 830, 3);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Kled")
            {
                spell = new DashData("Kled E", SpellSlot.E, 600, true, 100, 945, 3);
                spell.CheckSpellName = "KledE";
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Leblanc")
            {
                spell = new DashData("Leblanc W", SpellSlot.W, 600, false, 100, 1450, 3);
                spell.CheckSpellName = "LeblancW";
                Spells.Add(spell);

                spell = new DashData("Leblanc RW", SpellSlot.R, 600, false, 100, 1450, 3);
                spell.CheckSpellName = "LeblancRW";
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "LeeSin")
            {
                spell = new DashData("LeeSin W", SpellSlot.W, 700, false, 100, 2000, 3);
                spell.CheckSpellName = "BlindMonkWOne";
                spell.ValidTargets = new[] { SpellValidTargets.AllyChampions, SpellValidTargets.AllyMinions, SpellValidTargets.AllyWards };
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Lucian")
            {
                spell = new DashData("Lucian E", SpellSlot.E, 445, false, 100, 1350, 3);
                spell.MinRange = 200;
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "MonkeyKing")
            {
                spell = new DashData("MonkeyKing E", SpellSlot.E, 650, false, 100, 1395, 3);
                spell.ValidTargets = new[] { SpellValidTargets.EnemyChampions, SpellValidTargets.EnemyMinions };
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Nidalee")
            {
                spell = new DashData("Nidalee W", SpellSlot.W, 400, false, 100, 940, 3);
                spell.CheckSpellName = "Pounce";
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Pantheon")
            {
                spell = new DashData("Pantheon W", SpellSlot.W, 600, false, 100, 1000, 3);
                spell.ValidTargets = new[] { SpellValidTargets.EnemyChampions, SpellValidTargets.EnemyMinions };
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Pyke")
            {
                spell = new DashData("Pyke E", SpellSlot.E, 550, true, 100, 2000, 3);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Rakan")
            {
                spell = new DashData("Rakan W", SpellSlot.W, 650, false, 100, 1500, 3);
                Spells.Add(spell);

                spell = new DashData("Rakan E", SpellSlot.E, 650, false, 100, 1800, 3);
                spell.ValidTargets = new[] { SpellValidTargets.AllyChampions };
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Riven")
            {
                spell = new DashData("Riven Q", SpellSlot.Q, 275, true, 100, 780, 3);
                spell.RequiresPreMove = true;
                Spells.Add(spell);

                spell = new DashData("Riven E", SpellSlot.E, 250, true, 100, 1190, 3);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Sejuani")
            {
                spell = new DashData("Sejuani Q", SpellSlot.Q, 650, false, 100, 1000, 3);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Shen")
            {
                spell = new DashData("Shen E", SpellSlot.E, 600, false, 100, 1155, 3);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Sylas")
            {
                spell = new DashData("Sylas E", SpellSlot.E, 400, false, 100, 1450, 3);
                spell.CheckSpellName = "SylasE";
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Tristana")
            {
                spell = new DashData("Tristana W", SpellSlot.W, 900, false, 250, 1110, 3);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Tryndamere")
            {
                spell = new DashData("Tryndamere E", SpellSlot.E, 650, false, 100, 900, 3);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Vayne")
            {
                spell = new DashData("Vayne Q", SpellSlot.Q, 300, true, 100, 830, 3);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "XinZhao")
            {
                spell = new DashData("XinZhao E", SpellSlot.E, 650, false, 100, 3000, 3);
                spell.ValidTargets = new[] { SpellValidTargets.EnemyChampions, SpellValidTargets.EnemyMinions };
                Spells.Add(spell);
            }

            #endregion

            #region Champion Blinks

            if (ObjectManager.Player.CharacterName == "Ezreal")
            {
                spell = new BlinkData("Ezreal E", SpellSlot.E, 475, 250, 3);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Kassadin")
            {
                spell = new BlinkData("Kassadin R", SpellSlot.R, 500, 250, 3);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Katarina")
            {
                spell = new BlinkData("Katarina E", SpellSlot.E, 725, 100, 3);
                spell.PositionOnly = true;
                spell.ValidTargets = new[]
                {
                    SpellValidTargets.AllyChampions, SpellValidTargets.AllyMinions,
                    SpellValidTargets.EnemyChampions, SpellValidTargets.EnemyMinions
                };
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Shaco")
            {
                spell = new BlinkData("Shaco Q", SpellSlot.Q, 400, 250, 3);
                Spells.Add(spell);
            }

            #endregion

            #region Champion Invulnerabilities

            if (ObjectManager.Player.CharacterName == "Elise")
            {
                spell = new InvulnerabilityData("Elise E", SpellSlot.E, 100, 3);
                spell.CheckSpellName = "EliseSpiderEInitial";
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Fiora")
            {
                spell = new InvulnerabilityData("Fiora W", SpellSlot.W, 100, 4);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Fizz")
            {
                spell = new InvulnerabilityData("Fizz E", SpellSlot.E, 100, 3);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Maokai")
            {
                spell = new InvulnerabilityData("Maokai W", SpellSlot.W, 100, 3);
                spell.MaxRange = 525;
                spell.ValidTargets = new[] { SpellValidTargets.EnemyChampions, SpellValidTargets.EnemyMinions };
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "MasterYi")
            {
                spell = new InvulnerabilityData("MasterYi Q", SpellSlot.Q, 100, 3);
                spell.MaxRange = 600;
                spell.ValidTargets = new[] { SpellValidTargets.EnemyChampions, SpellValidTargets.EnemyMinions };
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Vladimir")
            {
                spell = new InvulnerabilityData("Vladimir W", SpellSlot.W, 100, 3);
                spell.SelfCast = true;
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Xayah")
            {
                spell = new InvulnerabilityData("Xayah R", SpellSlot.R, 100, 5);
                Spells.Add(spell);
            }

            #endregion

            // Flash
            spell = new BlinkData("Flash", SpellSlot.Unknown, 425, 100, 5, true);
            spell.CheckSpellName = "SummonerFlash";
            Spells.Add(spell);

            // Zhonyas
            spell = new EvadeSpellData("Zhonyas", 5);
            Spells.Add(spell);

            // Stopwatch
            spell = new EvadeSpellData("Stopwatch", 5);
            Spells.Add(spell);

            #region Champion Shields

            if (ObjectManager.Player.CharacterName == "Diana")
            {
                spell = new ShieldData("Diana W", SpellSlot.W, 100, 2);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Ivern")
            {
                spell = new ShieldData("Ivern E", SpellSlot.E, 100, 2);
                spell.CanShieldAllies = true;
                spell.MaxRange = 750;
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Janna")
            {
                spell = new ShieldData("Janna E", SpellSlot.E, 100, 2);
                spell.CanShieldAllies = true;
                spell.MaxRange = 800;
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "JarvanIV")
            {
                spell = new ShieldData("JarvanIV W", SpellSlot.W, 100, 2);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Karma")
            {
                spell = new ShieldData("Karma E Shield", SpellSlot.E, 100, 2);
                spell.CanShieldAllies = true;
                spell.MaxRange = 800;
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Lulu")
            {
                spell = new ShieldData("Lulu E", SpellSlot.E, 100, 2);
                spell.CanShieldAllies = true;
                spell.MaxRange = 650;
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Morgana")
            {
                spell = new ShieldData("Morgana E", SpellSlot.E, 100, 3);
                spell.CanShieldAllies = true;
                spell.MaxRange = 800;
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Nautilus")
            {
                spell = new ShieldData("Nautilus W", SpellSlot.W, 100, 2);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Rumble")
            {
                spell = new ShieldData("Rumble W Shield", SpellSlot.W, 100, 2);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Sion")
            {
                spell = new ShieldData("Sion W", SpellSlot.W, 100, 2);
                spell.CheckSpellName = "SionW";
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Skarner")
            {
                spell = new ShieldData("Skarner W", SpellSlot.W, 100, 2);
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Taric")
            {
                spell = new ShieldData("Taric W", SpellSlot.W, 250, 2);
                spell.CanShieldAllies = true;
                spell.MaxRange = 800;
                Spells.Add(spell);
            }

            if (ObjectManager.Player.CharacterName == "Udyr")
            {
                spell = new ShieldData("Udyr W", SpellSlot.W, 100, 2);
                Spells.Add(spell);
            }

            #endregion

            if (Config.PrintSpellData)
            {
                Logging.Write(false, true, "EV")(LogLevel.Info, "[EvadeSharp] Added {0} spells into EvadeSpellDatabase.", Spells.Count);
            }
        }

        public static EvadeSpellData GetByName(string name)
        {
            name = name.ToLower();
            foreach (var evadeSpellData in Spells)
            {
                if (evadeSpellData.Name.ToLower() == name)
                {
                    return evadeSpellData;
                }
            }
            return null;
        }
    }
}