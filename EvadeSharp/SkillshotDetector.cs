// Copyright 2014 - 2014 Esk0r
// SkillshotDetector.cs is part of Evade.
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
using System.Text.RegularExpressions;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Core.Utils;
using SharpDX;

#endregion

namespace Evade
{
    internal static class SkillshotDetector
    {
        public delegate void OnDeleteMissileH(Skillshot skillshot, MissileClient missile);
        public delegate void OnDetectSkillshotH(Skillshot skillshot);

        /// <summary>
        ///     This event is fired after a skillshot missile collides.
        /// </summary>
        public static event OnDeleteMissileH OnDeleteMissile;

        /// <summary>
        ///     This event is fired after a skillshot is detected.
        /// </summary>
        public static event OnDetectSkillshotH OnDetectSkillshot;

        static SkillshotDetector()
        {
            // Detect when the skillshots are created.
            AIBaseClient.OnDoCast += AIHeroClientOnOnDoCast;

            // Detect when projectiles collide.
            GameObject.OnAssign += ObjSpellMissileOnOnCreate;
            GameObject.OnDelete += ObjSpellMissileOnOnDelete;
            GameObject.OnAssign += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var spellData = SpellDatabase.GetBySourceObjectName(sender.Name);

            if (spellData == null)
            {
                return;
            }

            if (Config.skillShots?[spellData.MenuItemName]?["Enabled" + spellData.MenuItemName] == null)
            {
                return;
            }

            TriggerOnDetectSkillshot(DetectionType.ProcessSpell, spellData, Utils.TickCount - Game.Ping / 2, sender.Position.ToVector2(), sender.Position.ToVector2(),
                GameObjects.Heroes.MinOrDefault(h => (h.IsAlly ? 1 : 0) + (h.CharacterName == spellData.ChampionName ? 0 : 1)));
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid || !Config.TestOnAllies && sender.Team == ObjectManager.Player.Team)
            {
                return;
            }

            for (var i = Program.DetectedSkillshots.Count - 1; i >= 0; i--)
            {
                var skillshot = Program.DetectedSkillshots[i];
                if (skillshot.SpellData.ToggleParticleName != "" && new Regex(skillshot.SpellData.ToggleParticleName).IsMatch(sender.Name))
                {
                    Program.DetectedSkillshots.RemoveAt(i);
                }
            }
        }

        private static void ObjSpellMissileOnOnCreate(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;

            if (missile == null || !missile.IsValid)
            {
                return;
            }

            var unit = missile.SpellCaster as AIHeroClient;

            if (missile.SData.Name == "AzirSoldierMissile")
            {
                var azirSoldier = missile.SpellCaster as AIMinionClient;

                if (azirSoldier == null || !azirSoldier.IsValid || (azirSoldier.Team == ObjectManager.Player.Team && !Config.TestOnAllies))
                {
                    return;
                }
            }
            else
            {
                if (unit == null || !unit.IsValid || (unit.Team == ObjectManager.Player.Team && !Config.TestOnAllies))
                {
                    return;
                }
            }

            var spellData = SpellDatabase.GetByMissileName(missile.SData.Name);

            if (spellData == null)
            {
                return;
            }

            DelayAction.Add(0, delegate
            {
                ObjSpellMissileOnOnCreateDelayed(sender, args);
            });
        }

        private static void ObjSpellMissileOnOnCreateDelayed(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;

            if (missile == null || !missile.IsValid)
            {
                return;
            }

            var unit = missile.SpellCaster as AIHeroClient;

            if (missile.SData.Name == "AzirSoldierMissile")
            {
                var azirSoldier = missile.SpellCaster as AIMinionClient;

                if (azirSoldier == null || !azirSoldier.IsValid || (azirSoldier.Team == ObjectManager.Player.Team && !Config.TestOnAllies))
                {
                    return;
                }
            }
            else
            {
                if (unit == null || !unit.IsValid || (unit.Team == ObjectManager.Player.Team && !Config.TestOnAllies))
                {
                    return;
                }
            }

            var spellData = SpellDatabase.GetByMissileName(missile.SData.Name);

            if (spellData == null)
            {
                return;
            }

            var missilePosition = missile.Position.ToVector2();
            var unitPosition = missile.StartPosition.ToVector2();
            var endPos = missile.EndPosition.ToVector2();

            if (spellData.MissileSpellName == "DianaArcThrow")
            {
                endPos = missile.TargetEndPos.ToVector2();
            }

            // Calculate the real end point.
            var direction = (endPos - unitPosition).Normalized();
            if (unitPosition.Distance(endPos) > spellData.Range || spellData.FixedRange)
            {
                endPos = unitPosition + direction * spellData.Range;
            }

            if (spellData.ExtraRange != -1)
            {
                endPos = endPos + Math.Min(spellData.ExtraRange, spellData.Range - endPos.Distance(unitPosition)) * direction;
            }

            if (spellData.SpellName == "ZiggsQSpell2")
            {
                spellData.MissileSpeed = (int)(unitPosition.Distance(endPos) * 1000 / 480f);
            }

            if (spellData.SpellName == "ZiggsQSpell3")
            {
                spellData.MissileSpeed = (int)(unitPosition.Distance(endPos) * 1000 / 430f);
            }

            if (spellData.SpellName == "ZileanQ")
            {
                spellData.MissileSpeed = (int)(unitPosition.Distance(endPos) * 1000 / 450f);
            }

            var castTime = Utils.TickCount - Game.Ping / 2 - (spellData.MissileDelayed ? 0 : spellData.Delay) -
                           (int)(1000f * missilePosition.Distance(unitPosition) / spellData.MissileSpeed);

            // Trigger the skillshot detection callbacks.
            TriggerOnDetectSkillshot(DetectionType.RecvPacket, spellData, castTime, unitPosition, endPos, missile.SpellCaster);
        }

        private static void ObjSpellMissileOnOnDelete(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;

            if (missile == null || !missile.IsValid)
            {
                return;
            }

            var caster = missile.SpellCaster as AIHeroClient;

            if (missile.SData.Name == "AzirSoldierMissile")
            {
                var azirSoldier = missile.SpellCaster as AIMinionClient;

                if (azirSoldier == null || !azirSoldier.IsValid || (azirSoldier.Team == ObjectManager.Player.Team && !Config.TestOnAllies))
                {
                    return;
                }
            }
            else
            {
                if (caster == null || !caster.IsValid || (caster.Team == ObjectManager.Player.Team && !Config.TestOnAllies))
                {
                    return;
                }
            }

            var spellName = missile.SData.Name;
            var startPos = missile.StartPosition.ToVector2();
            var endPos = missile.EndPosition.ToVector2();

            if (spellName == "DianaArcThrow")
            {
                endPos = missile.TargetEndPos.ToVector2();
            }

            if (OnDeleteMissile != null)
            {
                for (var i = Program.DetectedSkillshots.Count - 1; i >= 0; i--)
                {
                    var skillshot = Program.DetectedSkillshots[i];
                    if ((skillshot.SpellData.MissileSpellName.Equals(spellName, StringComparison.InvariantCultureIgnoreCase) ||
                         skillshot.SpellData.ExtraMissileSpellNames.Contains(spellName, StringComparer.InvariantCultureIgnoreCase)) &&
                        skillshot.Unit.NetworkId == missile.SpellCaster.NetworkId &&
                        (endPos - startPos).AngleBetween(skillshot.Direction) < 10 &&
                        skillshot.SpellData.CanBeRemoved)
                    {
                        OnDeleteMissile(skillshot, missile);
                    }
                }
            }

            Program.DetectedSkillshots.RemoveAll(
                skillshot =>
                    (skillshot.SpellData.MissileSpellName.Equals(spellName, StringComparison.InvariantCultureIgnoreCase) ||
                     skillshot.SpellData.ExtraMissileSpellNames.Contains(spellName, StringComparer.InvariantCultureIgnoreCase)) &&
                    skillshot.Unit.NetworkId == missile.SpellCaster.NetworkId &&
                    (endPos - startPos).AngleBetween(skillshot.Direction) < 10 &&
                    skillshot.SpellData.CanBeRemoved);
        }

        private static void AIHeroClientOnOnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender == null || !sender.IsValid)
            {
                return;
            }

            if (Config.PrintSpellData && sender is AIHeroClient)
            {
                Chat.Print(Utils.TickCount + " ProcessSpellCast: " + args.SData.Name);
                Console.WriteLine(Utils.TickCount + " ProcessSpellCast: " + args.SData.Name);
            }

            if (sender.Team == ObjectManager.Player.Team && !Config.TestOnAllies)
            {
                return;
            }

            if (args.SData.Name == "DravenRDoublecast")
            {
                Program.DetectedSkillshots.RemoveAll(
                    s => s.Unit.NetworkId == sender.NetworkId && s.SpellData.SpellName == "DravenRCast");
            }

            // Get the skillshot data.
            var spellData = SpellDatabase.GetByName(args.SData.Name);

            // Skillshot not added in the database.
            if (spellData == null)
            {
                return;
            }

            if (spellData.SpellName == "FlashFrost" && args.Target != null && args.Target.NetworkId == sender.NetworkId)
            {
                return;
            }

            if (spellData.SpellName == "WarwickR")
            {
                spellData.Range = (int)Math.Ceiling(2.5 * sender.MoveSpeed);
            }

            var startPos = new Vector2();

            if (spellData.FromObject != "")
            {
                foreach (var o in ObjectManager.Get<GameObject>())
                {
                    if (o.Name.Contains(spellData.FromObject) || new Regex(spellData.FromObject).IsMatch(o.Name))
                    {
                        startPos = o.Position.ToVector2();
                    }
                }
            }
            else
            {
                startPos = sender.Position.ToVector2();
            }

            if (spellData.FromObjects != null && spellData.FromObjects.Length > 0)
            {
                foreach (var obj in ObjectManager.Get<GameObject>())
                {
                    if ((obj.IsEnemy || Config.TestOnAllies) && spellData.FromObjects.Any(f => new Regex(f).IsMatch(obj.Name)))
                    {
                        var start = obj.Position.ToVector2();
                        var end = start + spellData.Range * (args.To.ToVector2() - obj.Position.ToVector2()).Normalized();
                        TriggerOnDetectSkillshot(
                            DetectionType.ProcessSpell, spellData, Utils.TickCount - Game.Ping / 2, start, end, sender);
                    }
                }
            }

            if (!startPos.IsValid())
            {
                return;
            }

            var endPos = args.To.ToVector2();

            // Calculate the real end point.
            var direction = (endPos - startPos).Normalized();
            if (startPos.Distance(endPos) > spellData.Range || spellData.FixedRange)
            {
                endPos = startPos + direction * spellData.Range;
            }

            if (spellData.ExtraRange != -1)
            {
                endPos = endPos + Math.Min(spellData.ExtraRange, spellData.Range - endPos.Distance(startPos)) * direction;
            }

            // Trigger the skillshot detection callbacks.
            TriggerOnDetectSkillshot(DetectionType.ProcessSpell, spellData, Utils.TickCount - Game.Ping / 2, startPos, endPos, sender);
        }

        internal static void TriggerOnDetectSkillshot(DetectionType detectionType,
            SpellData spellData,
            int startT,
            Vector2 start,
            Vector2 end,
            AIBaseClient unit)
        {
            var skillshot = new Skillshot(detectionType, spellData, startT, start, end, unit);

            if (OnDetectSkillshot != null)
            {
                OnDetectSkillshot(skillshot);
            }
        }
    }
}