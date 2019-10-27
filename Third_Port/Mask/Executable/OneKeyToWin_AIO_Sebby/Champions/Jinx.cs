using System;
using System.Linq;
using System.Windows.Forms;

using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;

using SebbyLib;

using SharpDX;

using Menu = EnsoulSharp.SDK.MenuUI.Menu;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Jinx : Program
    {
        private float WCastTime = 0, grabTime = 0, dragonTime = 0, baronTime = 0;
        private float dragonDmg = 0, baronDmg = 0;

        public Jinx()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1500);
            E = new Spell(SpellSlot.E, 900);
            R = new Spell(SpellSlot.R, 3000);

            W.SetSkillshot(0.6f, 60, 3300, true, false, SkillshotType.Line);
            E.SetSkillshot(1f, 60, 1750, false, false, SkillshotType.Circle);
            R.SetSkillshot(0.6f, 140, 1700, false, false, SkillshotType.Line);

            var wrapper = new Menu(Player.CharacterName, Player.CharacterName);

            var draw = new Menu("draw", "Draw");
            draw.Add(new MenuBool("noti", "Show notification", false, Player.CharacterName));
            draw.Add(new MenuBool("semi", "Semi-manual R target", false, Player.CharacterName));
            draw.Add(new MenuBool("qRange", "Q range", false, Player.CharacterName));
            draw.Add(new MenuBool("wRange", "W range", false, Player.CharacterName));
            draw.Add(new MenuBool("eRange", "E range", false, Player.CharacterName));
            draw.Add(new MenuBool("rRange", "R range", false, Player.CharacterName));
            draw.Add(new MenuBool("onlyRdy", "Draw only ready spells", true, Player.CharacterName));
            wrapper.Add(draw);

            var q = new Menu("QConfig", "Q Config");
            q.Add(new MenuBool("autoQ", "Auto Q", true, Player.CharacterName));
            q.Add(new MenuBool("Qharass", "Harass Q", true, Player.CharacterName));
            wrapper.Add(q);

            var w = new Menu("WConfig", "W Config");
            w.Add(new MenuBool("autoW", "Auto W", true, Player.CharacterName));
            w.Add(new MenuBool("Wharass", "Harass W", true, Player.CharacterName));
            wrapper.Add(w);

            var e = new Menu("EConfig", "E Config");
            e.Add(new MenuBool("autoE", "Auto E on CC", true, Player.CharacterName));
            e.Add(new MenuBool("comboE", "Auto E in Combo", true, Player.CharacterName));
            e.Add(new MenuBool("AGC", "Anti Gapcloser E", true, Player.CharacterName));
            e.Add(new MenuBool("opsE", "Detect special spells E", true, Player.CharacterName));
            e.Add(new MenuBool("telE", "Auto E teleport", true, Player.CharacterName));
            wrapper.Add(e);

            var r = new Menu("RConfig", "R Config");
            var rjs = new Menu("rjs", "R Jungle stealer");
            rjs.Add(new MenuBool("Rjungle", "R Jungle stealer", true, Player.CharacterName));
            rjs.Add(new MenuBool("Rdragon", "Dragon", true, Player.CharacterName));
            rjs.Add(new MenuBool("Rbaron", "Baron", true, Player.CharacterName));
            r.Add(new MenuBool("autoR", "Auto R", true, Player.CharacterName));
            r.Add(rjs);
            r.Add(new MenuKeyBind("useR", "OneKeyToCast R", Keys.T, KeyBindType.Press, Player.CharacterName));
            r.Add(new MenuBool("Rturret", "Don't R under enemy turret", true, Player.CharacterName));
            wrapper.Add(r);

            var farm = new Menu("farm", "Farm");
            farm.Add(new MenuBool("farmQout", "Q farm out range AA", true, Player.CharacterName));
            farm.Add(new MenuBool("farmQ", "Lane clear Q", true, Player.CharacterName));
            farm.Add(new MenuSlider("LCMana", "Lane clear minimum Mana", 80, 30, 100, Player.CharacterName));
            wrapper.Add(farm);

            Config.Add(wrapper);

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            AIBaseClient.OnDoCast += AIBaseClient_OnDoCast;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Orbwalker.OnAction += Orbwalker_OnAction;
        }

        private bool FishBoneActive
        {
            get
            {
                return Player.HasBuff("JinxQ");
            }
        }

        private float bonusRange()
        {
            return 525f + 75f + 25f * (Player.Spellbook.GetSpell(SpellSlot.Q).Level - 1);
        }

        private float GetRealPowPowRange(GameObject target)
        {
            return 525f + Player.BoundingRadius + target.BoundingRadius;
        }

        private float GetRealDistance(AIBaseClient target)
        {
            return Player.PreviousPosition.Distance(SpellPrediction.GetPrediction(target, 0.05f).CastPosition) + Player.BoundingRadius + target.BoundingRadius;
        }

        private float GetUltTravelTime(AIHeroClient source, float speed, float delay, Vector3 targetPos)
        {
            var distance = Vector3.Distance(source.PreviousPosition, targetPos);
            var time = delay;
            if (distance > 765)
            {
                time += 0.45f;
                time += (distance - 765) / (speed + 500);
            }
            else
                time += distance / speed;
            return time;
        }

        private void SetWDelay()
        {
            W.Delay = Math.Max(0.4f, (600 - Player.PercentAttackSpeedMod / 2.5f * 200) / 1000f);
        }

        private void SetMana()
        {
            if ((Config["extraSet"].GetValue<MenuBool>("manaDisable").Enabled && Combo) || Player.HealthPercent < 20)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
                return;
            }

            QMANA = 20;
            WMANA = W.Instance.SData.ManaArray[Math.Max(0, W.Level - 1)];
            EMANA = E.Instance.SData.ManaArray[Math.Max(0, E.Level - 1)];

            if (!R.IsReady())
                RMANA = WMANA - Player.PARRegenRate * W.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.ManaArray[Math.Max(0, R.Level - 1)];
        }

        private void LogicQ()
        {
            if (LaneClear && !FishBoneActive && !Player.Spellbook.IsAutoAttack && Orbwalker.GetTarget() == null && Orbwalker.CanAttack() && Config[Player.CharacterName]["farm"].GetValue<MenuBool>("farmQout").Enabled && Player.Mana > RMANA + WMANA + EMANA + QMANA)
            {
                foreach (var minion in Cache.GetMinions(Player.PreviousPosition, bonusRange() + 30).Where(
                    minion => !minion.InAutoAttackRange() && GetRealPowPowRange(minion) < GetRealDistance(minion) && bonusRange() < GetRealDistance(minion)))
                {
                    var hpPred = HealthPrediction.GetPrediction(minion, 400, 70);
                    if (hpPred < Player.GetAutoAttackDamage(minion) * 1.1 && hpPred > 5)
                    {
                        Orbwalker.ForceTarget = minion;
                        Q.Cast();
                        return;
                    }
                }
            }

            var t = TargetSelector.GetTarget(bonusRange() + 60);
            if (t.IsValidTarget())
            {
                if (!FishBoneActive && (!t.InAutoAttackRange() || t.CountEnemyHeroesInRange(250) > 2) && Orbwalker.GetTarget() == null)
                {
                    var distance = GetRealDistance(t);
                    if (Combo && (Player.Mana > RMANA + WMANA + QMANA || Player.GetAutoAttackDamage(t) * 3 > t.Health))
                        Q.Cast();
                    else if (Harass && !Player.Spellbook.IsAutoAttack && Orbwalker.CanAttack() && Config[Player.CharacterName]["QConfig"].GetValue<MenuBool>("Qharass").Enabled && !Player.IsUnderEnemyTurret() && Player.Mana > RMANA + WMANA + EMANA + QMANA + QMANA && distance < bonusRange() + t.BoundingRadius + Player.BoundingRadius)
                        Q.Cast();
                }
            }
            else if (!FishBoneActive && Combo && Player.Mana > RMANA + WMANA + QMANA + QMANA && Player.CountEnemyHeroesInRange(2000) > 0)
                Q.Cast();
            else if (FishBoneActive && Combo && Player.Mana < RMANA + WMANA + QMANA + QMANA)
                Q.Cast();
            else if (FishBoneActive && Combo && Player.CountEnemyHeroesInRange(2000) == 0)
                Q.Cast();
            else if (FishBoneActive && LaneClear)
                Q.Cast();
        }

        private void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range);
            if (t.IsValidTarget())
            {
                foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsValidTarget(W.Range) && enemy.DistanceToPlayer() > bonusRange()))
                {
                    var comboDmg = OktwCommon.GetKsDamage(enemy, W);
                    if (R.IsReady() && Player.Mana > RMANA + WMANA + QMANA + QMANA)
                    {
                        comboDmg += R.GetDamage(enemy);
                    }
                    if (comboDmg > enemy.Health && OktwCommon.ValidUlt(enemy))
                    {
                        CastSpell(W, enemy);
                        return;
                    }
                }

                if (Player.CountEnemyHeroesInRange(bonusRange()) == 0)
                {
                    if (Combo && Player.Mana > RMANA + WMANA + QMANA)
                    {
                        foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsValidTarget(W.Range) && GetRealDistance(enemy) > bonusRange()).OrderBy(enemy => enemy.Health))
                            CastSpell(W, enemy);
                    }
                    else if (Harass && Player.Mana > RMANA + EMANA + WMANA + WMANA + QMANA * 4 && OktwCommon.CanHarass() && Config[Player.CharacterName]["WConfig"].GetValue<MenuBool>("Wharass").Enabled)
                    {
                        foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsValidTarget(W.Range) && Config["harass"].GetValue<MenuBool>("harass" + enemy.CharacterName).Enabled))
                            CastSpell(W, enemy);
                    }
                }

                if (!None && Player.Mana > RMANA + WMANA && Player.CountEnemyHeroesInRange(GetRealPowPowRange(t)) == 0)
                {
                    foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                        W.Cast(enemy, true);
                }
            }
        }

        private void LogicE()
        {
            var e = Config[Player.CharacterName]["EConfig"];
            if (Player.Mana > RMANA + EMANA && e.GetValue<MenuBool>("autoE").Enabled && Game.Time - grabTime > 1)
            {
                foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsValidTarget(E.Range + 50) && !OktwCommon.CanMove(enemy)))
                {
                    E.Cast(enemy);
                    return;
                }

                if (!LagFree(1))
                    return;

                if (e.GetValue<MenuBool>("telE").Enabled)
                {
                    var trapPos = OktwCommon.GetTrapPos(E.Range);
                    if (!trapPos.IsZero)
                        E.Cast(trapPos);
                }

                if (Combo && Player.IsMoving && e.GetValue<MenuBool>("comboE").Enabled && Player.Mana > RMANA + EMANA + WMANA)
                {
                    var t = TargetSelector.GetTarget(E.Range);
                    if (t.IsValidTarget(E.Range) && E.GetPrediction(t).CastPosition.Distance(t.Position) > 250)
                    {
                        E.CastIfWillHit(t, 2);
                        if (t.HasBuffOfType(BuffType.Slow))
                        {
                            CastSpell(E, t);
                        }
                        if (OktwCommon.IsMovingInSameDirection(Player, t))
                        {
                            CastSpell(E, t);
                        }
                    }
                }
            }
        }

        private void LogicR()
        {
            var r = Config[Player.CharacterName]["RConfig"];
            if (Player.IsUnderEnemyTurret() && r.GetValue<MenuBool>("Rturret").Enabled)
                return;
            if (Game.Time - WCastTime > 0.9 && r.GetValue<MenuBool>("autoR").Enabled)
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(target => target.IsValidTarget(R.Range) && OktwCommon.ValidUlt(target)))
                {
                    var predictedHealth = target.Health - OktwCommon.GetIncomingDamage(target);
                    var Rdmg = R.GetDamage(target);

                    if (Rdmg > predictedHealth && !OktwCommon.IsSpellHeroCollision(target, R) && GetRealDistance(target) > bonusRange() + 200)
                    {
                        if (GetRealDistance(target) > bonusRange() + 300 + target.BoundingRadius && target.CountAllyHeroesInRange(500) == 0 && Player.CountEnemyHeroesInRange(400) == 0)
                        {
                            CastSpell(R, target);
                        }
                        else if (target.CountEnemyHeroesInRange(200) > 2)
                        {
                            R.Cast(target, true, false, true);
                        }
                    }
                }
            }
        }

        private void KsJungle()
        {
            var rjs = Config[Player.CharacterName]["RConfig"]["rjs"];
            if (rjs.GetValue<MenuBool>("Rjungle").Enabled)
            {
                var mobs = Cache.GetMinions(Player.PreviousPosition, float.MaxValue, SebbyLib.MinionTeam.Neutral);
                foreach (var mob in mobs)
                {
                    if (mob.Health < mob.MaxHealth && mob.CountAllyHeroesInRange(1000) == 0 && mob.DistanceToPlayer() > 1500)
                    {
                        if (mob.CharacterName.ToLower().Contains("dragon") && rjs.GetValue<MenuBool>("Rdragon").Enabled)
                        {
                            if (dragonDmg == 0)
                                dragonDmg = mob.Health;

                            if (Game.Time - dragonTime > 4)
                            {
                                if (dragonDmg - mob.Health > 0)
                                {
                                    dragonDmg = mob.Health;
                                }
                                dragonTime = Game.Time;
                            }
                            else
                            {
                                var dmgSec = (dragonDmg - mob.Health) * (Math.Abs(dragonTime - Game.Time) / 4);
                                if (dragonDmg - mob.Health > 0)
                                {
                                    var timeTravel = GetUltTravelTime(Player, R.Speed, R.Delay, mob.Position);
                                    var timeR = (mob.Health - Player.CalculateDamage(mob, DamageType.Physical, 250 + 100 * (R.Level - 1) + 0.15 * Player.GetBonusPhysicalDamage()) / (dmgSec / 4));
                                    if (timeTravel > timeR)
                                        R.Cast(mob.Position);
                                }
                                else
                                    dragonDmg = mob.Health;
                            }
                        }
                        else if (mob.CharacterName == "SRU_Baron" && rjs.GetValue<MenuBool>("Rbaron").Enabled)
                        {
                            if (baronDmg == 0)
                                baronDmg = mob.Health;

                            if (Game.Time - baronTime > 4)
                            {
                                if (baronDmg - mob.Health > 0)
                                {
                                    baronDmg = mob.Health;
                                }
                                baronTime = Game.Time;
                            }
                            else
                            {
                                var dmgSec = (baronDmg - mob.Health) * (Math.Abs(baronTime - Game.Time) / 4);
                                if (baronDmg - mob.Health > 0)
                                {
                                    var timeTravel = GetUltTravelTime(Player, R.Speed, R.Delay, mob.Position);
                                    var timeR = (mob.Health - Player.CalculateDamage(mob, DamageType.Physical, 250 + 100 * (R.Level - 1) + 0.15 * Player.GetBonusPhysicalDamage()) / (dmgSec / 4));
                                    if (timeTravel > timeR)
                                        R.Cast(mob.Position);
                                }
                                else
                                    baronDmg = mob.Health;
                            }
                        }
                    }
                }
            }
        }

        private void Orbwalker_OnAction(object sender, OrbwalkerActionArgs args)
        {
            if (args.Type == OrbwalkerType.BeforeAttack)
            {
                var q = Config[Player.CharacterName]["QConfig"];
                if (!Q.IsReady() || !q.GetValue<MenuBool>("autoQ").Enabled || !FishBoneActive)
                    return;

                var t = args.Target as AIHeroClient;

                if (t != null)
                {
                    var realDistance = GetRealDistance(t) - 50;
                    if (Combo && (realDistance < GetRealPowPowRange(t) || (Player.Mana < RMANA + QMANA + QMANA && Player.GetAutoAttackDamage(t) * 3 < t.Health)))
                        Q.Cast();
                    else if (Harass && q.GetValue<MenuBool>("Qharass").Enabled && (realDistance > bonusRange() || realDistance < GetRealPowPowRange(t) || Player.Mana < RMANA + EMANA + WMANA + WMANA))
                        Q.Cast();
                }

                var minion = args.Target as AIMinionClient;

                if (LaneClear && minion != null)
                {
                    var realDistance = GetRealDistance(minion);
                    if (realDistance < GetRealPowPowRange(minion) || Player.ManaPercent < Config[Player.CharacterName]["farm"].GetValue<MenuSlider>("LCMana").Value)
                        Q.Cast();
                    else if (GameObjects.EnemyHeroes.Any(tar => tar.IsValidTarget(1000) && args.Target.Distance(SpellPrediction.GetPrediction(tar, 0.25f).CastPosition) < 200))
                        Q.Cast();
                }
            }
        }

        private void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {
            if (Config[Player.CharacterName]["EConfig"].GetValue<MenuBool>("AGC").Enabled && E.IsReady() && Player.Mana > RMANA + EMANA)
            {
                if (sender.IsValidTarget(E.Range))
                    E.Cast(args.EndPosition);
            }
        }

        private void AIBaseClient_OnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            var unit = sender as AIHeroClient;
            if (unit == null)
                return;

            if (unit.IsMe && args.SData.Name == "JinxWMissile")
            {
                WCastTime = Game.Time;
            }

            if (E.IsReady())
            {
                if (unit.IsEnemy && Config[Player.CharacterName]["EConfig"].GetValue<MenuBool>("opsE").Enabled && unit.IsValidTarget(E.Range) && unit.IsCastingImporantSpell())
                {
                    E.Cast(unit.PreviousPosition, true);
                }
                if (unit.IsAlly && args.SData.Name == "RocketGrab" && unit.DistanceToPlayer() < E.Range)
                {
                    grabTime = Game.Time;
                }
            }
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (R.IsReady())
            {
                if (Config[Player.CharacterName]["RConfig"].GetValue<MenuKeyBind>("useR").Active)
                {
                    var t = TargetSelector.GetTarget(R.Range);
                    if (t.IsValidTarget())
                        R.Cast(t);
                }
                KsJungle();
            }

            if (LagFree(0))
            {
                SetMana();
                SetWDelay();
            }

            if (E.IsReady())
                LogicE();
            if (LagFree(2) && Q.IsReady() && Config[Player.CharacterName]["QConfig"].GetValue<MenuBool>("autoQ").Enabled)
                LogicQ();
            if (LagFree(3) && W.IsReady() && !Player.Spellbook.IsAutoAttack && Config[Player.CharacterName]["WConfig"].GetValue<MenuBool>("autoW").Enabled)
                LogicW();
            if (LagFree(4) && R.IsReady())
                LogicR();
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            var onlyRdy = Config[Player.CharacterName]["draw"].GetValue<MenuBool>("onlyRdy");

            if (Config[Player.CharacterName]["draw"].GetValue<MenuBool>("qRange").Enabled)
            {
                if (onlyRdy)
                {
                    if (Q.IsReady())
                        Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1);
                }
                else
                    Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1);
            }

            if (Config[Player.CharacterName]["draw"].GetValue<MenuBool>("wRange").Enabled)
            {
                if (onlyRdy)
                {
                    if (W.IsReady())
                        Render.Circle.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Orange, 1);
                }
                else
                    Render.Circle.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Orange, 1);
            }

            if (Config[Player.CharacterName]["draw"].GetValue<MenuBool>("eRange").Enabled)
            {
                if (onlyRdy)
                {
                    if (E.IsReady())
                        Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Yellow, 1);
                }
                else
                    Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Yellow, 1);
            }

            if (Config[Player.CharacterName]["draw"].GetValue<MenuBool>("rRange").Enabled)
            {
                if (onlyRdy)
                {
                    if (R.IsReady())
                        Render.Circle.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Gray, 1);
                }
                else
                    Render.Circle.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Gray, 1);
            }

            if (Config[Player.CharacterName]["draw"].GetValue<MenuBool>("noti").Enabled)
            {
                var t = TargetSelector.GetTarget(R.Range);

                if (t.IsValidTarget() && R.IsReady() && R.GetDamage(t) > t.Health)
                {
                    Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "Ult can kill: " + t.CharacterName + " have: " + t.Health + " hp");
                    drawLine(t.Position, Player.Position, 5, System.Drawing.Color.Red);
                }
                else if (t.IsValidTarget(2000) && W.GetDamage(t) > t.Health)
                {
                    Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.4f, System.Drawing.Color.Red, "W can kill: " + t.CharacterName + " have: " + t.Health + " hp");
                    drawLine(t.Position, Player.Position, 3, System.Drawing.Color.Yellow);
                }
            }
        }
    }
}
