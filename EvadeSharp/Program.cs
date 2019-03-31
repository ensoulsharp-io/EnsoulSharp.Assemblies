// Copyright 2014 - 2014 Esk0r
// Program.cs is part of Evade.
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
using System.Collections.Generic;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Core.UI.IMenu.Values;
using EnsoulSharp.SDK.Core.Utils;
using Evade.Benchmarking;
using Evade.Pathfinding;
using SharpDX;
using Color = System.Drawing.Color;
using GamePath = System.Collections.Generic.List<SharpDX.Vector2>;

#endregion

namespace Evade
{
    internal class Program
    {
        public static SpellList<Skillshot> DetectedSkillshots = new SpellList<Skillshot>();
        public static Vector2 EvadeToPoint = new Vector2();
        public static int LastWardJumpAttempt = 0;
        public static bool NoSolutionFound = false;
        public static string PlayerChampionName;
        public static Vector2 PlayerPosition = new Vector2();
        public static Vector2 PreviousTickPosition = new Vector2();

        private static Vector2 _evadePoint;
        private static bool _evading;

        private static bool ForcePathFollowing = false;
        private static int LastSentMovePacketT = 0;
        private static int LastSentMovePacketT2 = 0;
        private static readonly Random RandomN = new Random();

        public static bool Evading
        {
            get { return _evading; }
            set
            {
                if (value == true)
                {
                    ForcePathFollowing = true;
                    LastSentMovePacketT = 0;
                    ObjectManager.Player.SendMovePacket(EvadePoint);
                }
                _evading = value;
            }
        }

        public static Vector2 EvadePoint
        {
            get { return _evadePoint; }
            set { _evadePoint = value; }
        }

        public static void Main(string[] args)
        {
            Events.OnLoad += Game_OnGameStart;
        }

        public static void Game_OnGameStart(object sender, EventArgs args)
        {
            PlayerChampionName = ObjectManager.Player.CharacterName;

            // Create the menu to allow the user to change the config.
            Config.CreateMenu();

            // Add the game events.
            Game.OnUpdate += Game_OnOnGameUpdate;
            Player.OnIssueOrder += AIHeroClientOnOnIssueOrder;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            // Set up the OnDetectSkillshot Event.
            SkillshotDetector.OnDetectSkillshot += OnDetectSkillshot;
            SkillshotDetector.OnDeleteMissile += SkillshotDetectorOnOnDeleteMissile;

            // For skillshot drawing.
            Drawing.OnDraw += Drawing_OnDraw;

            // OnDash event.
            Events.OnDash += UnitOnOnDash;

            DetectedSkillshots.OnAdd += DetectedSkillshots_OnAdd;

            if (Config.TestOnAllies)
            {
                Benchmark.Initialize();
            }
        }

        private static void DetectedSkillshots_OnAdd(object sender, EventArgs e)
        {
            Evading = false;
        }

        private static void SkillshotDetectorOnOnDeleteMissile(Skillshot skillshot, MissileClient missile)
        {
            if (skillshot.SpellData.MissileSpellName == "DianaArcThrow")
            {
                DetectedSkillshots.RemoveAll(
                    item =>
                        item.SpellData.SpellName == "DianaArcArc" &&
                        item.Unit.NetworkId == skillshot.Unit.NetworkId &&
                        item.StartTick == skillshot.StartTick &&
                        item.Start == skillshot.Start &&
                        item.End == skillshot.End);
            }

            if (skillshot.SpellData.MissileSpellName == "GravesChargeShotShot")
            {
                DetectedSkillshots.RemoveAll(
                    item =>
                        item.SpellData.SpellName == "GravesRRange" &&
                        item.Unit.NetworkId == skillshot.Unit.NetworkId &&
                        item.StartTick == skillshot.StartTick &&
                        item.Direction.AngleBetween(skillshot.Direction) < 10 &&
                        item.Start == skillshot.End);
            }

            if (skillshot.SpellData.SpellName == "VelkozQ")
            {
                var spellData = SpellDatabase.GetByName("VelkozQSplit");
                var direction = skillshot.Direction.Perpendicular();
                if (DetectedSkillshots.Count(s => s.SpellData.SpellName == "VelkozQSplit" && s.Unit.NetworkId == skillshot.Unit.NetworkId) == 0)
                {
                    for (var i = -1; i <= 1; i = i + 2)
                    {
                        var skillshotToAdd = new Skillshot(
                            DetectionType.ProcessSpell, spellData, Utils.TickCount, missile.Position.ToVector2(),
                            missile.Position.ToVector2() + i * direction * spellData.Range, skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                    }
                }
            }
        }

        private static void OnDetectSkillshot(Skillshot skillshot)
        {
            // Check if the skillshot is already added.
            var alreadyAdded = false;

            if (Config.misc.GetValue<MenuBool>("DisableFow").Value && !skillshot.Unit.IsVisible)
            {
                return;
            }

            foreach (var item in DetectedSkillshots)
            {
                if (item.SpellData.SpellName == skillshot.SpellData.SpellName &&
                    item.Unit.NetworkId == skillshot.Unit.NetworkId &&
                    skillshot.Direction.AngleBetween(item.Direction) < 5 &&
                    (skillshot.Start.Distance(item.Start) < 100 || skillshot.SpellData.FromObjects.Length == 0))
                {
                    alreadyAdded = true;
                }
            }

            // Check if the skillshot is from an ally.
            if (skillshot.Unit.Team == ObjectManager.Player.Team && !Config.TestOnAllies)
            {
                return;
            }

            // Check if the skillshot is too far away.
            if (skillshot.Start.Distance(PlayerPosition) > (skillshot.SpellData.Range + skillshot.SpellData.Radius + 1000) * 1.5)
            {
                return;
            }

            // Add the skillshot to the detected skillshot list.
            if (!alreadyAdded || skillshot.SpellData.DontCheckForDuplicates)
            {
                if (skillshot.DetectionType == DetectionType.ProcessSpell)
                {
                    // Multiple skillshots like twisted fate Q.
                    if (skillshot.SpellData.MultipleNumber != -1)
                    {
                        var originalDirection = skillshot.Direction;
                        for (var i = -(skillshot.SpellData.MultipleNumber - 1) / 2; i <= (skillshot.SpellData.MultipleNumber - 1) / 2; i++)
                        {
                            var end = skillshot.Start +
                                      skillshot.SpellData.Range *
                                      originalDirection.Rotated(skillshot.SpellData.MultipleAngle * i);
                            var skillshotToAdd = new Skillshot(
                                skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, end, skillshot.Unit);
                            DetectedSkillshots.Add(skillshotToAdd);
                        }
                        return;
                    }

                    if (skillshot.SpellData.Invert)
                    {
                        var newDirection = -(skillshot.End - skillshot.Start).Normalized();
                        var end = skillshot.Start + newDirection * skillshot.Start.Distance(skillshot.End);
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, end, skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.Centered)
                    {
                        var start = skillshot.Start - skillshot.Direction * skillshot.SpellData.Range / 2;
                        var end = skillshot.Start + skillshot.Direction * skillshot.SpellData.Range / 2;
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end, skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "AatroxQ3")
                    {
                        var end = skillshot.Start + skillshot.Direction * 3 * skillshot.Unit.BoundingRadius;
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, end, skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "DianaArc")
                    {
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, SpellDatabase.GetByName("DianaArcArc"), skillshot.StartTick, skillshot.Start, skillshot.End, skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                    }

                    if (skillshot.SpellData.SpellName == "EkkoR")
                    {
                        skillshot.Circle.Center = skillshot.Start;
                        skillshot.UpdatePolygon();
                    }

                    if (skillshot.SpellData.SpellName == "HeimerdingerEUlt")
                    {
                        var delay1 = skillshot.SpellData.Delay + skillshot.Start.Distance(skillshot.End) * 1000 / skillshot.SpellData.MissileSpeed;
                        var sdata1 = SpellDatabase.GetByName("HeimerdingerESpell_ult2");
                        var bounce1 = new Skillshot(
                            skillshot.DetectionType,
                            sdata1,
                            skillshot.StartTick + (int)delay1,
                            skillshot.End,
                            skillshot.End + sdata1.Radius * skillshot.Direction,
                            skillshot.Unit);

                        var delay2 = delay1 + bounce1.SpellData.Radius * 1000 / bounce1.SpellData.MissileSpeed;
                        var sdata2 = SpellDatabase.GetByName("HeimerdingerESpell_ult3");
                        var bounce2 = new Skillshot(
                            skillshot.DetectionType,
                            sdata2,
                            skillshot.StartTick + (int)delay2,
                            bounce1.End,
                            bounce1.End + sdata2.Radius * skillshot.Direction,
                            skillshot.Unit);

                        DetectedSkillshots.Add(bounce1);
                        DetectedSkillshots.Add(bounce2);
                    }

                    if (skillshot.SpellData.SpellName == "LucianQ")
                    {
                        skillshot.SpellData.Delay = 400;
                        var sender = skillshot.Unit as AIHeroClient;
                        if (sender != null)
                        {
                            skillshot.SpellData.Delay = Math.Max(250, 400 - (sender.Level - 1) * 10);
                        }
                    }

                    if (skillshot.SpellData.SpellName == "MalzaharQ")
                    {
                        var start = skillshot.End - skillshot.Direction.Perpendicular() * 400;
                        var end = skillshot.End + skillshot.Direction.Perpendicular() * 400;
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end, skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "PykeR")
                    {
                        var h = (new Vector2(skillshot.End.X + 100, skillshot.End.Y) - skillshot.End).Normalized();
                        var v = (new Vector2(skillshot.End.X, skillshot.End.Y + 100) - skillshot.End).Normalized();
                        var line1 = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.End - h * 250 + v * 250, skillshot.End + h * 250 - v * 250, skillshot.Unit);
                        var line2 = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.End - h * 250 - v * 250, skillshot.End + h * 250 + v * 250, skillshot.Unit);
                        DetectedSkillshots.Add(line1);
                        DetectedSkillshots.Add(line2);
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "SylasQ")
                    {
                        var start1 = skillshot.Start + 100 * skillshot.Direction.Perpendicular();
                        var end1 = start1 + (skillshot.End - start1).Normalized() * skillshot.SpellData.Range;
                        var chain1 = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start1, end1, skillshot.Unit);

                        var start2 = skillshot.Start - 100 * skillshot.Direction.Perpendicular();
                        var end2 = start2 + (skillshot.End - start2).Normalized() * skillshot.SpellData.Range;
                        var chain2 = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start2, end2, skillshot.Unit);

                        DetectedSkillshots.Add(chain1);
                        DetectedSkillshots.Add(chain2);

                        return;
                    }

                    if (skillshot.SpellData.SpellName == "ZiggsQ")
                    {
                        var d1 = skillshot.Start.Distance(skillshot.End);
                        var d2 = d1 * 0.4f;
                        var d3 = d2 * 0.69f;

                        var bounce1SpellData = SpellDatabase.GetByName("ZiggsQSpell2");
                        var bounce2SpellData = SpellDatabase.GetByName("ZiggsQSpell3");

                        var bounce1Pos = skillshot.End + skillshot.Direction * d2;
                        var bounce2Pos = bounce1Pos + skillshot.Direction * d3;

                        bounce1SpellData.MissileSpeed = (int)(d2 * 1000 / 480f);
                        bounce2SpellData.MissileSpeed = (int)(d3 * 1000 / 430f);

                        var bounce1Delay = skillshot.SpellData.Delay + d1 * 1000 / skillshot.SpellData.MissileSpeed;
                        var bounce2Delay = bounce1Delay + d2 * 1000 / bounce1SpellData.MissileSpeed;

                        var bounce1 = new Skillshot(
                            skillshot.DetectionType, bounce1SpellData, skillshot.StartTick + (int)bounce1Delay, skillshot.End, bounce1Pos, skillshot.Unit);
                        var bounce2 = new Skillshot(
                            skillshot.DetectionType, bounce2SpellData, skillshot.StartTick + (int)bounce2Delay, bounce1Pos, bounce2Pos, skillshot.Unit);

                        DetectedSkillshots.Add(bounce1);
                        DetectedSkillshots.Add(bounce2);
                    }

                    if (skillshot.SpellData.SpellName == "ZileanQ")
                    {
                        skillshot.SpellData.MissileSpeed = (int)(skillshot.Start.Distance(skillshot.End) * 1000 / 450f);
                    }

                    if (skillshot.SpellData.SpellName == "ZyraQ")
                    {
                        var start = skillshot.End - skillshot.Direction.Perpendicular() * 425;
                        var end = skillshot.End + skillshot.Direction.Perpendicular() * 425;
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end, skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }
                }

                if (skillshot.SpellData.SpellName == "BraumRWrapper")
                {
                    var skillshotToAdd = new Skillshot(
                        skillshot.DetectionType, SpellDatabase.GetByName("BraumRRange"), skillshot.StartTick, skillshot.Start, skillshot.Start, skillshot.Unit);
                    DetectedSkillshots.Add(skillshotToAdd);
                }

                if (skillshot.SpellData.SpellName == "GravesChargeShot")
                {
                    var skillshotToAdd = new Skillshot(
                        skillshot.DetectionType, SpellDatabase.GetByName("GravesRRange"), skillshot.StartTick, skillshot.End, skillshot.End + 800 * skillshot.Direction, skillshot.Unit);
                    DetectedSkillshots.Add(skillshotToAdd);
                }

                if (skillshot.SpellData.SpellName == "HowlingGaleSpell")
                {
                    skillshot.SpellData.MissileSpeed = (int)Math.Max(0f, Math.Min(1166f, skillshot.Start.Distance(skillshot.End) / 1.5f));
                }

                if (skillshot.SpellData.SpellName == "JinxW")
                {
                    skillshot.SpellData.Delay = (int)(600 - skillshot.Unit.PercentAttackSpeedMod / 2.5f * 200);
                    if (skillshot.DetectionType == DetectionType.RecvPacket)
                    {
                        skillshot.StartTick -= skillshot.SpellData.MissileDelayed ? 0 : skillshot.SpellData.Delay;
                    }
                }

                if (skillshot.SpellData.SpellName == "KarmaQMissileMantra")
                {
                    var skillshotToAdd = new Skillshot(
                        skillshot.DetectionType, SpellDatabase.GetByName("KarmaQMantraRange"), skillshot.StartTick, skillshot.Start, skillshot.End, skillshot.Unit);
                    DetectedSkillshots.Add(skillshotToAdd);
                }

                if (skillshot.SpellData.SpellName == "MaokaiQ")
                {
                    var skillshotToAdd = new Skillshot(
                        skillshot.DetectionType, SpellDatabase.GetByName("MaokaiQRange"), skillshot.StartTick, skillshot.Start, skillshot.Start, skillshot.Unit);
                    DetectedSkillshots.Add(skillshotToAdd);
                }

                if (skillshot.SpellData.SpellName == "OrianaIzunaCommand")
                {
                    var skillshotToAdd = new Skillshot(
                        skillshot.DetectionType, SpellDatabase.GetByName("OriannaQRange"), skillshot.StartTick, skillshot.Start, skillshot.End, skillshot.Unit);
                    DetectedSkillshots.Add(skillshotToAdd);
                }

                // Dont allow fow detection.
                if (skillshot.SpellData.DisableFowDetection && skillshot.DetectionType == DetectionType.RecvPacket)
                {
                    return;
                }

                DetectedSkillshots.Add(skillshot);
            }
        }

        private static void Game_OnOnGameUpdate(EventArgs args)
        {
            PlayerPosition = ObjectManager.Player.Position.ToVector2();

            // Set evading to false after blinking.
            if (PreviousTickPosition.IsValid() && PlayerPosition.Distance(PreviousTickPosition) > 200)
            {
                Evading = false;
                EvadeToPoint = Vector2.Zero;
            }

            PreviousTickPosition = PlayerPosition;

            // Remove the detected skillshots that have expired.
            DetectedSkillshots.RemoveAll(skillshot => !skillshot.IsActive());

            // Trigger OnGameUpdate on each skillshot.
            foreach (var skillshot in DetectedSkillshots)
            {
                skillshot.Game_OnGameUpdate();
            }

            // Evading disabled
            if (!Config.Menu.GetValue<MenuKeyBind>("Enabled").Active)
            {
                Evading = false;
                return;
            }

            if (PlayerChampionName == "Olaf" && Config.misc.GetValue<MenuBool>("DisableEvadeForOlafR").Value && ObjectManager.Player.HasBuff("OlafRagnarok"))
            {
                Evading = false;
                return;
            }

            // Avoid sending move/cast packets while dead.
            if (ObjectManager.Player.IsDead)
            {
                Evading = false;
                EvadeToPoint = Vector2.Zero;
                return;
            }

            // Avoid sending move/cast packets while channeling interruptable spells that cause hero not being able to move.
            if (ObjectManager.Player.IsCastingInterruptableSpell(true))
            {
                Evading = false;
                EvadeToPoint = Vector2.Zero;
                return;
            }

            if (ObjectManager.Player.Spellbook.IsAutoAttack && !AutoAttack.IsAutoAttack(ObjectManager.Player.GetLastCastedSpell().Name))
            {
                Evading = false;
                return;
            }

            // Avoid evading while stunned or immobile.
            if (Utils.ImmobileTime(ObjectManager.Player) - Utils.TickCount > Game.Ping / 2 + 70)
            {
                Evading = false;
                return;
            }

            // Avoid evading while dashing.
            if (ObjectManager.Player.IsDashing())
            {
                Evading = false;
                return;
            }

            // Don't evade while casting R as sion.
            if (PlayerChampionName == "Sion" && ObjectManager.Player.HasBuff("SionR"))
            {
                return;
            }

            // Shield allies.
            foreach (var ally in GameObjects.AllyHeroes)
            {
                if (ally.IsValidTarget(1000, false) && !ally.IsMe)
                {
                    var shieldAlly = Config.shielding.GetValue<MenuBool>("shield" + ally.CharacterName);
                    if (shieldAlly != null && shieldAlly.Value)
                    {
                        var allySafeResult = IsSafe(ally.Position.ToVector2());
                        if (!allySafeResult.IsSafe)
                        {
                            var dangerLevel = 0;

                            foreach (var skillshot in allySafeResult.SkillshotList)
                            {
                                dangerLevel = Math.Max(dangerLevel, skillshot.GetValue<MenuSlider>("DangerLevel").Value);
                            }

                            foreach (var evadeSpell in EvadeSpellDatabase.Spells)
                            {
                                if (evadeSpell.IsShield && evadeSpell.CanShieldAllies &&
                                    ally.Distance(ObjectManager.Player.Position) < evadeSpell.MaxRange &&
                                    dangerLevel >= evadeSpell.DangerLevel &&
                                    evadeSpell.IsReady() &&
                                    IsAboutToHit(ally, evadeSpell.Delay))
                                {
                                    ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, ally);
                                }
                            }
                        }
                    }
                }
            }

            // Spell Shielded
            if (ObjectManager.Player.IsSpellShield())
            {
                return;
            }

            var currentPath = ObjectManager.Player.Path.ToList().ToVector2();
            var safeResult = IsSafe(PlayerPosition);
            var safePath = IsSafePath(currentPath, 100);

            NoSolutionFound = false;

            // Continue evading
            if (Evading && IsSafe(EvadePoint).IsSafe)
            {
                if (safeResult.IsSafe)
                {
                    // We are safe, stop evading.
                    Evading = false;
                }
                else
                {
                    if (Utils.TickCount - LastSentMovePacketT > 1000 / 15)
                    {
                        LastSentMovePacketT = Utils.TickCount;
                        ObjectManager.Player.SendMovePacket(EvadePoint);
                    }
                    return;
                }
            }
            // Stop evading if the point is not safe.
            else if (Evading)
            {
                Evading = false;
            }

            // The path is not safe.
            if (!safePath.IsSafe)
            {
                // Inside the danger polygon.
                if (!safeResult.IsSafe)
                {
                    // Search for an evade point.
                    TryToEvade(safeResult.SkillshotList, EvadeToPoint.IsValid() ? EvadeToPoint : Game.CursorPosCenter.ToVector2());
                }
            }

            // FollowPath
            if (!NoSolutionFound && !Evading && EvadeToPoint.IsValid() && safeResult.IsSafe)
            {
                if (EvadeSpellDatabase.Spells.Any(evadeSpell => evadeSpell.Name == "Walking" && evadeSpell.Enabled))
                {
                    if (safePath.IsSafe && !ForcePathFollowing)
                    {
                        return;
                    }

                    if (Utils.TickCount - LastSentMovePacketT2 > 1000 / 15 || !PathFollower.IsFollowing)
                    {
                        LastSentMovePacketT2 = Utils.TickCount;

                        if (DetectedSkillshots.Count == 0)
                        {
                            if (ObjectManager.Player.Distance(EvadeToPoint) > 75)
                            {
                                ObjectManager.Player.SendMovePacket(EvadeToPoint);
                            }
                            return;
                        }

                        var path2 = ObjectManager.Player.GetPath(EvadeToPoint.ToVector3()).To2DList();
                        var safePath2 = IsSafePath(path2, 100);

                        if (safePath2.IsSafe)
                        {
                            if (ObjectManager.Player.Distance(EvadeToPoint) > 75)
                            {
                                ObjectManager.Player.SendMovePacket(EvadeToPoint);
                            }
                            return;
                        }

                        var candidate = Pathfinding.Pathfinding.PathFind(PlayerPosition, EvadeToPoint);

                        if (candidate.Count == 0)
                        {
                            if (!safePath.Intersection.Valid && currentPath.Count <= 1)
                            {
                                safePath = IsSafePath(path2, 100);
                            }

                            if (safePath.Intersection.Valid)
                            {
                                if (ObjectManager.Player.Distance(safePath.Intersection.Point) > 75)
                                {
                                    ObjectManager.Player.SendMovePacket(safePath.Intersection.Point);
                                    return;
                                }
                            }
                        }

                        PathFollower.Follow(candidate);

                        PathFollower.KeepFollowingPath(new EventArgs());
                    }
                }
            }
        }

        /// <summary>
        /// Used to block the movement to avoid entering in dangerous areas.
        /// </summary>
        private static void AIHeroClientOnOnIssueOrder(AIBaseClient sender, PlayerIssueOrderEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (args.Order == GameObjectOrder.MoveTo)
            {
                EvadeToPoint.X = args.TargetPosition.X;
                EvadeToPoint.Y = args.TargetPosition.Y;
            }
            else
            {
                EvadeToPoint = Vector2.Zero;
            }

            if (DetectedSkillshots.Count == 0)
            {
                ForcePathFollowing = false;
            }

            // Don't block the movement packets if cant find an evade point.
            if (NoSolutionFound)
            {
                return;
            }

            // Evading disabled
            if (!Config.Menu.GetValue<MenuKeyBind>("Enabled").Active)
            {
                return;
            }

            if (EvadeSpellDatabase.Spells.Any(evadeSpell => evadeSpell.Name == "Walking" && !evadeSpell.Enabled))
            {
                return;
            }

            // Spell Shielded
            if (ObjectManager.Player.IsSpellShield())
            {
                return;
            }

            if (PlayerChampionName == "Olaf" && Config.misc.GetValue<MenuBool>("DisableEvadeForOlafR").Value && ObjectManager.Player.HasBuff("OlafRagnarok"))
            {
                return;
            }

            var myPath = ObjectManager.Player.GetPath(new Vector3(args.TargetPosition.X, args.TargetPosition.Y, ObjectManager.Player.Position.Z)).To2DList();
            var safeResult = IsSafe(PlayerPosition);

            // If we are evading
            if (Evading || !safeResult.IsSafe)
            {
                var rcSafePath = IsSafePath(myPath, Config.EvadingRouteChangeTimeOffset);
                if (args.Order == GameObjectOrder.MoveTo)
                {
                    var willMove = false;
                    if (Evading && Utils.TickCount - Config.LastEvadePointChangeT > Config.EvadePointChangeInterval)
                    {
                        // Update the evade point to the closest one
                        var points = Evader.GetEvadePoints(-1, 0, false, true);
                        if (points.Count > 0)
                        {
                            var to = new Vector2(args.TargetPosition.X, args.TargetPosition.Y);
                            EvadePoint = to.Closest(points);
                            Evading = true;
                            Config.LastEvadePointChangeT = Utils.TickCount;
                            willMove = true;
                        }
                    }

                    // If the path is safe  let the user follow it.
                    if (rcSafePath.IsSafe && IsSafe(myPath[myPath.Count - 1]).IsSafe)
                    {
                        EvadePoint = myPath[myPath.Count - 1];
                        Evading = true;
                        willMove = true;
                    }

                    if (!willMove)
                    {
                        ForcePathFollowing = true;
                    }
                }
                // Block the packets if we are evading or not safe.
                args.Process = false;
                return;
            }

            var safePath = IsSafePath(myPath, Config.CrossingTimeOffset);

            // Not evading, outside the skillshots.
            // The path is not safe, stop in the intersection point.
            if (!safePath.IsSafe && args.Order != GameObjectOrder.AttackUnit)
            {
                if (safePath.Intersection.Valid)
                {
                    if (ObjectManager.Player.Distance(safePath.Intersection.Point) > 75)
                    {
                        ForcePathFollowing = true;
                    }
                }
                ForcePathFollowing = true;
                args.Process = false;
            }

            // AutoAttack
            if (!safePath.IsSafe && args.Order == GameObjectOrder.AttackUnit)
            {
                var target = args.Target;
                if (target != null && target.IsValid && target.IsVisible)
                {
                    // Out of attack range.
                    if (PlayerPosition.Distance(target.Position) >
                        ObjectManager.Player.AttackRange + ObjectManager.Player.BoundingRadius + target.BoundingRadius)
                    {
                        if (safePath.Intersection.Valid)
                        {
                            ObjectManager.Player.SendMovePacket(safePath.Intersection.Point);
                        }
                        args.Process = false;
                    }
                }
            }
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsValid && sender.Owner.IsMe)
            {
                if (args.Slot == SpellSlot.Recall)
                {
                    EvadeToPoint = new Vector2();
                }

                if (Evading)
                {
                    var blockLevel = Config.misc.GetValue<MenuList<string>>("BlockSpells").Index;

                    if (blockLevel == 0)
                    {
                        return;
                    }

                    var isDangerous = DetectedSkillshots
                        .Any(
                            s =>
                                s.Evade() &&
                                s.IsDanger(PlayerPosition) &&
                                s.GetValue<MenuBool>("IsDangerous").Value);

                    if (blockLevel == 1 && !isDangerous)
                    {
                        return;
                    }

                    args.Process = !SpellBlocker.ShouldBlock(args.Slot);
                }
            }
        }

        private static void UnitOnOnDash(object sender, Events.DashArgs args)
        {
            if (args.Unit.IsMe)
            {
                if (Config.PrintSpellData)
                {
                    Console.WriteLine(Utils.TickCount + "DASH: Speed: " + args.Speed + " Width: " + args.EndPos.Distance(args.StartPos));
                }

                EvadeToPoint = args.EndPos;
            }
        }

        /// <summary>
        /// Returns true if the point is not inside the detected skillshots.
        /// </summary>
        public static IsSafeResult IsSafe(Vector2 point)
        {
            var result = new IsSafeResult();
            result.SkillshotList = new List<Skillshot>();

            foreach (var skillshot in DetectedSkillshots)
            {
                if (skillshot.Evade() && skillshot.IsDanger(point))
                {
                    result.SkillshotList.Add(skillshot);
                }
            }

            result.IsSafe = result.SkillshotList.Count == 0;
            return result;
        }

        /// <summary>
        /// Returns if the unit will get hit by skillshots taking the path.
        /// </summary>
        public static SafePathResult IsSafePath(GamePath path, int timeOffset, int speed = -1, int delay = 0, AIBaseClient unit = null)
        {
            var IsSafe = true;
            var intersections = new List<FoundIntersection>();

            foreach (var skillshot in DetectedSkillshots)
            {
                if (skillshot.Evade())
                {
                    var sResult = skillshot.IsSafePath(path, timeOffset, speed, delay, unit);
                    IsSafe = IsSafe ? sResult.IsSafe : false;

                    if (sResult.Intersection.Valid)
                    {
                        intersections.Add(sResult.Intersection);
                    }
                }
            }

            // Return the first intersection
            if (!IsSafe)
            {
                var intersection = intersections.MinOrDefault(o => o.Distance);
                return new SafePathResult(false, intersection.Valid ? intersection : new FoundIntersection());
            }

            return new SafePathResult(true, new FoundIntersection());
        }

        /// <summary>
        /// Returns if you can blink to the point without being hit.
        /// </summary>
        public static bool IsSafeToBlink(Vector2 point, int timeOffset, int delay)
        {
            foreach (var skillshot in DetectedSkillshots)
            {
                if (skillshot.Evade())
                {
                    if (!skillshot.IsSafeToBlink(point, timeOffset, delay))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if some detected skillshot is about to hit the unit.
        /// </summary>
        public static bool IsAboutToHit(AIBaseClient unit, int time)
        {
            time += 150;
            foreach (var skillshot in DetectedSkillshots)
            {
                if (skillshot.Evade())
                {
                    if (skillshot.IsAboutToHit(time, unit))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static void TryToEvade(List<Skillshot> HitBy, Vector2 to)
        {
            var dangerLevel = 0;

            foreach (var skillshot in HitBy)
            {
                dangerLevel = Math.Max(dangerLevel, skillshot.GetValue<MenuSlider>("DangerLevel").Value);
            }

            foreach (var evadeSpell in EvadeSpellDatabase.Spells)
            {
                if (evadeSpell.Enabled && evadeSpell.DangerLevel <= dangerLevel)
                {
                    // SpellShields
                    if (evadeSpell.IsSpellShield && evadeSpell.IsReady())
                    {
                        if (IsAboutToHit(ObjectManager.Player, evadeSpell.Delay))
                        {
                            ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, ObjectManager.Player);
                        }
                        // Let the user move freely inside the skillshot.
                        NoSolutionFound = true;
                        return;
                    }

                    // Walking
                    if (evadeSpell.Name == "Walking")
                    {
                        var points = Evader.GetEvadePoints();
                        if (points.Count > 0)
                        {
                            EvadePoint = to.Closest(points);
                            var nEvadePoint = EvadePoint.Extend(PlayerPosition, -100);
                            if (IsSafePath(ObjectManager.Player.GetPath(nEvadePoint.ToVector3()).To2DList(),
                                Config.EvadingSecondTimeOffset,
                                (int)ObjectManager.Player.MoveSpeed, 100).IsSafe)
                            {
                                EvadePoint = nEvadePoint;
                            }
                            Evading = true;
                            return;
                        }
                    }

                    // Support Unsealed Spellbook
                    if (evadeSpell.IsSummonerSpell && evadeSpell.IsBlink)
                    {
                        evadeSpell.Slot = ObjectManager.Player.GetSpellSlot(evadeSpell.CheckSpellName);
                    }

                    if (evadeSpell.IsReady())
                    {
                        // MovementSpeed Buff
                        if (evadeSpell.IsMovementSpeedBuff)
                        {
                            var points = Evader.GetEvadePoints((int)evadeSpell.MoveSpeedTotalAmount());
                            if (points.Count > 0)
                            {
                                EvadePoint = to.Closest(points);
                                Evading = true;
                                ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, ObjectManager.Player);
                                return;
                            }
                        }

                        // Dashes
                        if (evadeSpell.IsDash)
                        {
                            // Targetted dashes
                            if (evadeSpell.IsTargetted) // LeeSin W.
                            {
                                var targets = Evader.GetEvadeTargets(
                                    evadeSpell.ValidTargets, evadeSpell.Speed, evadeSpell.Delay, evadeSpell.MaxRange);
                                if (targets.Count > 0)
                                {
                                    var closestTarget = Utils.Closest(targets, to);
                                    EvadePoint = closestTarget.Position.ToVector2();
                                    Evading = true;
                                    ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, closestTarget);
                                    return;
                                }
                                if (Utils.TickCount - LastWardJumpAttempt < 250)
                                {
                                    // Let the user move freely inside the skillshot.
                                    NoSolutionFound = true;
                                    return;
                                }
                                if (evadeSpell.IsTargetted &&
                                    evadeSpell.ValidTargets.Contains(SpellValidTargets.AllyWards) &&
                                    Config.evadeSpells[evadeSpell.Name].GetValue<MenuBool>("WardJump" + evadeSpell.Name).Value)
                                {
                                    var wardSlot = Items.GetWardSlot();
                                    if (wardSlot != null)
                                    {
                                        var points = Evader.GetEvadePoints(evadeSpell.Speed, evadeSpell.Delay);
                                        // Remove the points out of range
                                        points.RemoveAll(item => item.Distance(ObjectManager.Player.Position) > 600);
                                        if (points.Count > 0)
                                        {
                                            // Dont dash just to the edge
                                            for (var i = 0; i < points.Count; i++)
                                            {
                                                var k = (int)(600 - PlayerPosition.Distance(points[i]));
                                                k = k - new Random(Utils.TickCount).Next(k);
                                                var extended = points[i] + k * (points[i] - PlayerPosition).Normalized();
                                                if (IsSafe(extended).IsSafe)
                                                {
                                                    points[i] = extended;
                                                }
                                            }

                                            var ePoint = to.Closest(points);
                                            ObjectManager.Player.Spellbook.CastSpell(wardSlot.SpellSlot, ePoint.ToVector3());
                                            LastWardJumpAttempt = Utils.TickCount;
                                            // Let the user move freely inside the skillshot.
                                            NoSolutionFound = true;
                                            return;
                                        }
                                    }
                                }
                            }
                            // Skillshot type dashes.
                            else
                            {
                                var points = Evader.GetEvadePoints(evadeSpell.Speed, evadeSpell.Delay);
                                // Remove the points out of range
                                points.RemoveAll(item => item.Distance(ObjectManager.Player.Position) > evadeSpell.MaxRange);
                                // If the spell has a fixed range (Vaynes Q), calculate the real dashing location. TODO: take into account walls in the future.
                                if (evadeSpell.FixedRange)
                                {
                                    for (var i = 0; i < points.Count; i++)
                                    {
                                        points[i] = PlayerPosition.Extend(points[i], evadeSpell.MaxRange);
                                    }

                                    for (var i = points.Count - 1; i > 0; i--)
                                    {
                                        if (!IsSafe(points[i]).IsSafe)
                                        {
                                            points.RemoveAt(i);
                                        }
                                    }
                                }
                                else
                                {
                                    for (var i = 0; i < points.Count; i++)
                                    {
                                        var k = (int)(evadeSpell.MaxRange - PlayerPosition.Distance(points[i]));
                                        k -= Math.Max(RandomN.Next(k) - 100, 0);
                                        k = Math.Max(k, (int)evadeSpell.MinRange);
                                        var extended = points[i] + k * (points[i] - PlayerPosition).Normalized();
                                        if (IsSafe(extended).IsSafe)
                                        {
                                            points[i] = extended;
                                        }
                                    }
                                }

                                if (points.Count > 0)
                                {
                                    EvadePoint = to.Closest(points);
                                    Evading = true;
                                    if (!evadeSpell.Invert)
                                    {
                                        if (evadeSpell.RequiresPreMove)
                                        {
                                            ObjectManager.Player.SendMovePacket(EvadePoint);
                                            var theSpell = evadeSpell;
                                            DelayAction.Add(Game.Ping / 2 + 100,
                                                delegate
                                                {
                                                    ObjectManager.Player.Spellbook.CastSpell(theSpell.Slot, EvadePoint.ToVector3());
                                                });
                                        }
                                        else
                                        {
                                            ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, EvadePoint.ToVector3());
                                        }
                                    }
                                    else
                                    {
                                        var castPoint = PlayerPosition - (EvadePoint - PlayerPosition);
                                        ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, castPoint.ToVector3());
                                    }
                                    return;
                                }
                            }
                        }

                        // Blinks
                        if (evadeSpell.IsBlink)
                        {
                            // Targetted blinks
                            if (evadeSpell.IsTargetted)
                            {
                                var targets = Evader.GetEvadeTargets(
                                    evadeSpell.ValidTargets, int.MaxValue, evadeSpell.Delay, evadeSpell.MaxRange, true);
                                if (targets.Count > 0)
                                {
                                    if (IsAboutToHit(ObjectManager.Player, evadeSpell.Delay))
                                    {
                                        var closestTarget = Utils.Closest(targets, to);
                                        EvadePoint = closestTarget.Position.ToVector2();
                                        Evading = true;
                                        if (evadeSpell.PositionOnly)
                                        {
                                            ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, closestTarget.Position);
                                        }
                                        else
                                        {
                                            ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, closestTarget);
                                        }
                                    }
                                    // Let the user move freely inside the skillshot.
                                    NoSolutionFound = true;
                                    return;
                                }
                                if (Utils.TickCount - LastWardJumpAttempt < 250)
                                {
                                    // Let the user move freely inside the skillshot.
                                    NoSolutionFound = true;
                                    return;
                                }
                                if (evadeSpell.IsTargetted &&
                                    evadeSpell.ValidTargets.Contains(SpellValidTargets.AllyWards) &&
                                    Config.evadeSpells[evadeSpell.Name].GetValue<MenuBool>("WardJump" + evadeSpell.Name).Value)
                                {
                                    var wardSlot = Items.GetWardSlot();
                                    if (wardSlot != null)
                                    {
                                        var points = Evader.GetEvadePoints(int.MaxValue, evadeSpell.Delay, true);
                                        // Remove the points out of range
                                        points.RemoveAll(item => item.Distance(ObjectManager.Player.Position) > 600);
                                        if (points.Count > 0)
                                        {
                                            // Dont blink just to the edge
                                            for (var i = 0; i < points.Count; i++)
                                            {
                                                var k = (int)(600 - PlayerPosition.Distance(points[i]));
                                                k = k - new Random(Utils.TickCount).Next(k);
                                                var extended = points[i] + k * (points[i] - PlayerPosition).Normalized();
                                                if (IsSafe(extended).IsSafe)
                                                {
                                                    points[i] = extended;
                                                }
                                            }

                                            var ePoint = to.Closest(points);
                                            ObjectManager.Player.Spellbook.CastSpell(wardSlot.SpellSlot, ePoint.ToVector3());
                                            LastWardJumpAttempt = Utils.TickCount;
                                            // Let the user move freely inside the skillshot.
                                            NoSolutionFound = true;
                                            return;
                                        }
                                    }
                                }
                            }
                            // Skillshot type blinks.
                            else
                            {
                                var points = Evader.GetEvadePoints(int.MaxValue, evadeSpell.Delay, true);
                                // Remove the points out of range
                                points.RemoveAll(item => item.Distance(ObjectManager.Player.Position) > evadeSpell.MaxRange);
                                // Dont blink just to the edge
                                for (var i = 0; i < points.Count; i++)
                                {
                                    var k = (int)(evadeSpell.MaxRange - PlayerPosition.Distance(points[i]));
                                    k = k - new Random(Utils.TickCount).Next(k);
                                    var extended = points[i] + k * (points[i] - PlayerPosition).Normalized();
                                    if (IsSafe(extended).IsSafe)
                                    {
                                        points[i] = extended;
                                    }
                                }

                                if (points.Count > 0)
                                {
                                    if (IsAboutToHit(ObjectManager.Player, evadeSpell.Delay))
                                    {
                                        EvadePoint = to.Closest(points);
                                        Evading = true;
                                        ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, EvadePoint.ToVector3());
                                    }
                                    // Let the user move freely inside the skillshot.
                                    NoSolutionFound = true;
                                    return;
                                }
                            }
                        }

                        // Invulnerabilities, like Fizz's E
                        if (evadeSpell.IsInvulnerability)
                        {
                            if (evadeSpell.IsTargetted)
                            {
                                var targets = Evader.GetEvadeTargets(evadeSpell.ValidTargets, int.MaxValue, evadeSpell.Delay, evadeSpell.MaxRange, true, false, true);
                                if (targets.Count > 0)
                                {
                                    if (IsAboutToHit(ObjectManager.Player, evadeSpell.Delay))
                                    {
                                        var closestTarget = Utils.Closest(targets, to);
                                        EvadePoint = closestTarget.Position.ToVector2();
                                        Evading = true;
                                        ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, closestTarget);
                                    }
                                    // Let the user move freely inside the skillshot.
                                    NoSolutionFound = true;
                                    return;
                                }
                            }
                            else
                            {
                                if (IsAboutToHit(ObjectManager.Player, evadeSpell.Delay))
                                {
                                    if (evadeSpell.SelfCast)
                                    {
                                        ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot);
                                    }
                                    else
                                    {
                                        ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, ObjectManager.Player.Position);
                                    }
                                }
                            }
                            // Let the user move freely inside the skillshot.
                            NoSolutionFound = true;
                            return;
                        }
                    }

                    // Zhonyas
                    if (evadeSpell.Name == "Zhonyas" && Items.CanUseItem((int)ItemId.Zhonyas_Hourglass))
                    {
                        if (IsAboutToHit(ObjectManager.Player, 100))
                        {
                            Items.UseItem((int)ItemId.Zhonyas_Hourglass);
                        }
                        // Let the user move freely inside the skillshot.
                        NoSolutionFound = true;
                        return;
                    }

                    // Stopwatch
                    var replica_stopwatch = Items.CanUseItem((int)ItemId.Replica_Stopwatch);
                    var stopwatch = Items.CanUseItem((int)ItemId.Stopwatch);
                    if (evadeSpell.Name == "Stopwatch" && (replica_stopwatch || stopwatch))
                    {
                        if (IsAboutToHit(ObjectManager.Player, 100))
                        {
                            if (replica_stopwatch)
                            {
                                Items.UseItem((int)ItemId.Replica_Stopwatch);
                            }
                            else if (stopwatch)
                            {
                                Items.UseItem((int)ItemId.Stopwatch);
                            }
                        }
                        // Let the user move freely inside the skillshot.
                        NoSolutionFound = true;
                        return;
                    }

                    // Shields
                    if (evadeSpell.IsShield && evadeSpell.IsReady())
                    {
                        if (IsAboutToHit(ObjectManager.Player, evadeSpell.Delay))
                        {
                            ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, ObjectManager.Player);
                        }
                        // Let the user move freely inside the skillshot.
                        NoSolutionFound = true;
                        return;
                    }
                }
            }

            NoSolutionFound = true;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Config.drawings.GetValue<MenuBool>("EnableDrawings").Value)
            {
                return;
            }

            if (Config.misc.GetValue<MenuBool>("ShowEvadeStatus").Value)
            {
                var heropos = Drawing.WorldToScreen(ObjectManager.Player.Position);
                Drawing.DrawText(
                    heropos.X,
                    heropos.Y,
                    Color.White,
                    Config.Menu.GetValue<MenuKeyBind>("Enabled").Active
                        ? "Evade: ON"
                        : "Evade: OFF");
            }

            var Border = Config.drawings.GetValue<MenuSlider>("Border").Value;
            var missileColor = Config.drawings.GetValue<MenuColor>("MissileColor").Color.ToSystemColor();

            // Draw the polygon for each skillshot.
            foreach (var skillshot in DetectedSkillshots)
            {
                skillshot.Draw(
                    skillshot.Evade() && Config.Menu.GetValue<MenuKeyBind>("Enabled").Active
                        ? Config.drawings.GetValue<MenuColor>("EnabledColor").Color.ToSystemColor()
                        : Config.drawings.GetValue<MenuColor>("DisabledColor").Color.ToSystemColor(), missileColor, Border);
            }

            if (Config.TestOnAllies)
            {
                var myPath = ObjectManager.Player.Path.ToList().ToVector2();

                for (var i = 0; i < myPath.Count - 1; i++)
                {
                    var A = myPath[i];
                    var B = myPath[i + 1];
                    var SA = Drawing.WorldToScreen(A.ToVector3());
                    var SB = Drawing.WorldToScreen(B.ToVector3());
                    Drawing.DrawLine(SA.X, SA.Y, SB.X, SB.Y, 1, Color.White);
                }

                var evadePath = Pathfinding.Pathfinding.PathFind(PlayerPosition, Game.CursorPosCenter.ToVector2());

                for (var i = 0; i < evadePath.Count - 1; i++)
                {
                    var A = evadePath[i];
                    var B = evadePath[i + 1];
                    var SA = Drawing.WorldToScreen(A.ToVector3());
                    var SB = Drawing.WorldToScreen(B.ToVector3());
                    Drawing.DrawLine(SA.X, SA.Y, SB.X, SB.Y, 1, Color.Red);
                }

                Drawing.DrawCircle(EvadePoint.ToVector3(), 300, Color.White);
                Drawing.DrawCircle(EvadeToPoint.ToVector3(), 300, Color.Red);
            }
        }

        public struct IsSafeResult
        {
            public bool IsSafe;
            public List<Skillshot> SkillshotList;
        }
    }
}