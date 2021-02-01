using System;
using System.Drawing;
using System.Linq;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    using EnsoulSharp;
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;
    using SebbyLib;

    class Lucian : Base
    {
        private readonly MenuBool onlyRdy = new MenuBool("onlyRdy", "Draw only ready spells");
        private readonly MenuBool qRange = new MenuBool("qRange", "Q range", false);
        private readonly MenuBool wRange = new MenuBool("wRange", "W range", false);
        private readonly MenuBool eRange = new MenuBool("eRange", "E range", false);
        private readonly MenuBool rRange = new MenuBool("rRange", "R range", false);

        private readonly MenuBool autoQ = new MenuBool("autoQ", "Auto Q");
        private readonly MenuBool harassQ = new MenuBool("harassQ", "Use Q on minion");

        private readonly MenuBool autoW = new MenuBool("autoW", "Auto W");
        private readonly MenuBool ignoreCol = new MenuBool("ignoreCol", "Ignore collision");
        private readonly MenuBool wInAaRange = new MenuBool("wInAaRange", "Cast only in AA range");

        private readonly MenuBool autoE = new MenuBool("autoE", "Auto E");

        private readonly MenuBool autoR = new MenuBool("autoR", "Auto R");
        private readonly MenuKeyBind useR = new MenuKeyBind("useR", "Semi-manual cast R key", Keys.T, KeyBindType.Press);

        private readonly MenuBool farmQ = new MenuBool("farmQ", "LaneClear Q");
        private readonly MenuBool farmW = new MenuBool("farmW", "LaneClear W");

        private bool passRdy = false;
        private float castR = Game.Time;

        public Core.OKTWdash Dash;

        private bool SpellLock
        {
            get
            {
                return Player.HasBuff("LucianPassiveBuff");
            }
        }

        public Lucian()
        {
            Q = new Spell(SpellSlot.Q, 500f);
            W = new Spell(SpellSlot.W, 900f);
            E = new Spell(SpellSlot.E, 425f);
            R = new Spell(SpellSlot.R, 1200f);
            Q1 = new Spell(SpellSlot.Q, 1000f);
            R1 = new Spell(SpellSlot.R, 1200f);

            Q.SetTargetted(0.4f, float.MaxValue);
            W.SetSkillshot(0.25f, 55f, 1600f, true, SpellType.Line);
            R.SetSkillshot(0.1f, 110f, 2800f, true, SpellType.Line);
            Q1.SetSkillshot(0.4f, 60f, float.MaxValue, true, SpellType.Line);
            R1.SetSkillshot(0.1f, 110f, 2800f, false, SpellType.Line);

            Local.Add(new Menu("draw", "Draw")
            {
                onlyRdy,
                qRange,
                wRange,
                eRange,
                rRange
            });

            Local.Add(new Menu("qConfig", "Q Config")
            {
                autoQ,
                harassQ
            });

            Local.Add(new Menu("wConfig", "W Config")
            {
                autoW,
                ignoreCol,
                wInAaRange
            });

            Local.Add(new Menu("eConfig", "E Config")
            {
                autoE
            });

            Local.Add(new Menu("rConfig", "R Config")
            {
                autoR,
                useR
            });

            FarmMenu.Add(farmQ);
            FarmMenu.Add(farmW);

            Dash = new Core.OKTWdash(E);

            AIBaseClient.OnDoCast += AIBaseClient_OnDoCast;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private void AIBaseClient_OnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "LucianQ" || args.SData.Name == "LucianW" || args.SData.Name == "LucianE")
                {
                    passRdy = true;
                }
                else
                {
                    passRdy = false;
                }

                if (args.SData.Name == "LucianR")
                {
                    castR = Game.Time;
                }
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (qRange.Enabled)
            {
                if (onlyRdy.Enabled)
                {
                    if (Q.IsReady())
                    {
                        Render.Circle.DrawCircle(Player.Position, Q1.Range, Color.Cyan, 1);
                    }
                }
                else
                {
                    Render.Circle.DrawCircle(Player.Position, Q1.Range, Color.Cyan, 1);
                }
            }

            if (wRange.Enabled)
            {
                if (onlyRdy.Enabled)
                {
                    if (W.IsReady())
                    {
                        Render.Circle.DrawCircle(Player.Position, W.Range, Color.Orange, 1);
                    }
                }
                else
                {
                    Render.Circle.DrawCircle(Player.Position, W.Range, Color.Orange, 1);
                }
            }

            if (eRange.Enabled)
            {
                if (onlyRdy.Enabled)
                {
                    if (E.IsReady())
                    {
                        Render.Circle.DrawCircle(Player.Position, E.Range, Color.Yellow, 1);
                    }
                }
                else
                {
                    Render.Circle.DrawCircle(Player.Position, E.Range, Color.Yellow, 1);
                }
            }

            if (rRange.Enabled)
            {
                if (onlyRdy.Enabled)
                {
                    if (R.IsReady())
                    {
                        Render.Circle.DrawCircle(Player.Position, R.Range, Color.Gray, 1);
                    }
                }
                else
                {
                    Render.Circle.DrawCircle(Player.Position, R.Range, Color.Gray, 1);
                }
            }
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (Player.HasBuff("LucianR") && (int)(Game.Time * 10) % 2 == 0)
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            if (R1.IsReady() && Game.Time - castR > 5 && useR.Active)
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);

                if (t.IsValidTarget())
                {
                    R1.Cast(t);
                    return;
                }
            }

            if (LagFree(0))
            {
                SetMana();
                SetDelay();
            }

            if (LagFree(1) && Q.IsReady() && !passRdy && !SpellLock && autoQ.Enabled)
            {
                LogicQ();
            }

            if (LagFree(2) && W.IsReady() && !passRdy && !SpellLock && autoW.Enabled)
            {
                LogicW();
            }

            if (LagFree(3) && E.IsReady() && autoE.Enabled)
            {
                LogicE();
            }

            if (LagFree(4))
            {
                if (R.IsReady() && Game.Time - castR > 5 && autoR.Enabled)
                {
                    LogicR();
                }

                if (!passRdy && !SpellLock)
                {
                    LogicFarm();
                }
            }
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E)
            {
                passRdy = true;
            }
        }

        private double AaDamage(AIHeroClient target)
        {
            if (Player.Level > 12)
            {
                return Player.GetAutoAttackDamage(target) + Player.TotalAttackDamage * 0.6;
            }
            else if (Player.Level > 7)
            {
                return Player.GetAutoAttackDamage(target) + Player.TotalAttackDamage * 0.55;
            }
            else if (Player.Level > 0)
            {
                return Player.GetAutoAttackDamage(target) + Player.TotalAttackDamage * 0.5;
            }

            return 0;
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var t1 = TargetSelector.GetTarget(Q1.Range, DamageType.Physical);

            if (t.IsValidTarget())
            {
                if (OktwCommon.GetKsDamage(t, Q) + AaDamage(t) > t.Health)
                {
                    Q.Cast(t);
                }
                else if (Combo && Player.Mana > QMANA + RMANA)
                {
                    Q.Cast(t);
                }
                else if (Harass && HarassList.Any(e => e.Enabled && e.Name == "harass" + t.CharacterName) && Player.Mana > QMANA + WMANA + EMANA + RMANA)
                {
                    Q.Cast(t);
                }
            }
            else if ((Combo || Harass) && harassQ.Enabled && t1.IsValidTarget() && HarassList.Any(e => e.Enabled && e.Name == "harass" + t1.CharacterName) && Player.Distance(t1.PreviousPosition) > Q.Range + 100)
            {
                if (Combo && Player.Mana < QMANA + RMANA)
                {
                    return;
                }

                if (Harass && Player.Mana < QMANA + WMANA + EMANA + RMANA)
                {
                    return;
                }

                if (!OktwCommon.CanHarass())
                {
                    return;
                }

                var prepos = Prediction.GetPrediction(t1, Q1.Delay);

                if (prepos.Hitchance < HitChance.VeryHigh)
                {
                    return;
                }

                var distance = Player.Distance(prepos.CastPosition);
                var minions = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(Q.Range));

                foreach (var minion in minions)
                {
                    if (prepos.CastPosition.Distance(Player.Position.Extend(minion.Position, distance)) < 25)
                    {
                        Q.Cast(minion);
                        return;
                    }
                }
            }
        }

        private void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, DamageType.Physical);

            if (t.IsValidTarget())
            {
                var inAARange = t.InAutoAttackRange();

                if (ignoreCol.Enabled && inAARange)
                {
                    W.Collision = false;
                }
                else
                {
                    W.Collision = true;
                }

                var qDmg = Q.GetDamage(t);
                var wDmg = OktwCommon.GetKsDamage(t, W);

                if (inAARange)
                {
                    qDmg += (float)AaDamage(t);
                    wDmg += (float)AaDamage(t);
                }

                if (wDmg > t.Health)
                {
                    CastSpell(W, t);
                }
                else if (qDmg + wDmg > t.Health && Q.IsReady() && Player.Mana > QMANA + WMANA + RMANA)
                {
                    CastSpell(W, t);
                }

                var orbT = Orbwalker.GetTarget() as AIHeroClient;

                if (orbT == null)
                {
                    if (wInAaRange.Enabled)
                    {
                        return;
                    }
                }
                else if (orbT.IsValidTarget())
                {
                    t = orbT;
                }

                if (Orbwalker.ActiveMode == OrbwalkerMode.Combo && Player.Mana > QMANA + WMANA + EMANA + RMANA)
                {
                    CastSpell(W, t);
                }
                else if (Harass && HarassList.Any(e => e.Enabled && e.Name == "harass" + t.CharacterName) && !Player.IsUnderEnemyTurret() && Player.ManaPercent > 80 && Player.Mana > QMANA + WMANA + WMANA + EMANA + RMANA)
                {
                    CastSpell(W, t);
                }
                else if ((Combo || Harass) && Player.Mana > WMANA + EMANA + RMANA)
                {
                    foreach (var enemy in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(W.Range) && !OktwCommon.CanMove(e)))
                    {
                        W.Cast(enemy);
                    }
                }
            }
        }

        private void LogicE()
        {
            if (Player.Mana < EMANA + RMANA)
            {
                return;
            }

            if (GameObjects.EnemyHeroes.Any(e => e.IsValidTarget(270) && e.IsMelee))
            {
                var dashPos = Dash.CastDash(true);

                if (!dashPos.IsZero)
                {
                    E.Cast(dashPos);
                }
            }
            else if (!Combo || passRdy || SpellLock)
            {
                return;
            }
            else
            {
                var dashPos = Dash.CastDash();

                if (!dashPos.IsZero)
                {
                    E.Cast(dashPos);
                }
            }
        }

        private void LogicR()
        {
            var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);

            if (t.IsValidTarget() && t.CountAllyHeroesInRange(500) == 0 && OktwCommon.ValidUlt(t) && !t.InAutoAttackRange())
            {
                var rDmg = R.GetDamage(t) * (16 + 6 * R.Level);
                var tDis = Player.Distance(t.PreviousPosition);

                if (rDmg * 0.8 > t.Health && tDis < 700 && !Q.IsReady())
                {
                    R.Cast(t);
                }
                else if (rDmg * 0.7 > t.Health && tDis < 800)
                {
                    R.Cast(t);
                }
                else if (rDmg * 0.6 > t.Health && tDis < 900)
                {
                    R.Cast(t);
                }
                else if (rDmg * 0.5 > t.Health && tDis < 1000)
                {
                    R.Cast(t);
                }
                else if (rDmg * 0.4 > t.Health && tDis < 1100)
                {
                    R.Cast(t);
                }
                else if (rDmg * 0.3 > t.Health && tDis < 1200)
                {
                    R.Cast(t);
                }
            }
        }

        private void LogicFarm()
        {
            if (LaneClear && Player.Mana > QMANA + RMANA)
            {
                var mobs = GameObjects.GetJungles(Q.Range);

                if (mobs.Count > 0)
                {
                    var mob = mobs.First();

                    if (Q.IsReady())
                    {
                        Q.Cast(mob);
                        return;
                    }

                    if (W.IsReady())
                    {
                        W.Cast(mob);
                        return;
                    }
                }

                if (FarmSpells)
                {
                    if (Q.IsReady() && farmQ.Enabled)
                    {
                        var minions = GameObjects.GetMinions(Q1.Range);

                        foreach (var minion in minions)
                        {
                            var poutput = Q1.GetPrediction(minion);
                            var col = poutput.CollisionObjects;

                            if (col.Count > 2)
                            {
                                var minionQ = col.First();

                                if (minionQ.IsValidTarget(Q.Range))
                                {
                                    Q.Cast(minionQ);
                                    return;
                                }
                            }
                        }
                    }

                    if (W.IsReady() && farmW.Enabled)
                    {
                        var minions = GameObjects.GetMinions(W.Range).ToList();
                        var wFarm = W.GetCircularFarmLocation(minions, 150);

                        if (wFarm.MinionsHit > 3)
                        {
                            W.Cast(wFarm.Position);
                        }
                    }
                }
            }
        }

        private void SetMana()
        {
            if ((manaDisable.Enabled && Combo) || Player.HealthPercent < 20)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
                return;
            }

            QMANA = Q.Instance.ManaCost;
            WMANA = W.Instance.ManaCost;
            EMANA = E.Instance.ManaCost;

            if (!R.IsReady())
            {
                RMANA = QMANA - Player.PARRegenRate * Q.Instance.Cooldown;
            }
            else
            {
                RMANA = R.Instance.ManaCost;
            }
        }

        private void SetDelay()
        {
            var qDelayArray = new[] { 0.4f, 0.39f, 0.38f, 0.37f, 0.36f, 0.36f, 0.35f, 0.34f, 0.33f, 0.32f, 0.31f, 0.3f, 0.29f, 0.29f, 0.28f, 0.27f, 0.26f, 0.25f };
            var qDelay = qDelayArray[Math.Max(0, Math.Min(17, Player.Level - 1))];

            Q.Delay = qDelay;
            Q1.Delay = qDelay;
        }
    }
}