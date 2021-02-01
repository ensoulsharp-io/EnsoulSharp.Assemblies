using System;
using System.Drawing;
using System.Linq;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    using EnsoulSharp;
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;
    using SebbyLib;

    class Graves : Base
    {
        private readonly MenuBool onlyRdy = new MenuBool("onlyRdy", "Draw only ready spells");
        private readonly MenuBool qRange = new MenuBool("qRange", "Q range", false);
        private readonly MenuBool wRange = new MenuBool("wRange", "W range", false);
        private readonly MenuBool eRange = new MenuBool("eRange", "E range", false);
        private readonly MenuBool rRange = new MenuBool("rRange", "R range", false);

        private readonly MenuBool autoQ = new MenuBool("autoQ", "Auto Q");
        private readonly MenuBool harassQ = new MenuBool("harassQ", "Harass Q");

        private readonly MenuBool autoW = new MenuBool("autoW", "AutoW");
        private readonly MenuBool agcW = new MenuBool("agcW", "AntiGapcloser W");

        private readonly MenuBool autoE = new MenuBool("autoE", "Auto E");

        private readonly MenuBool autoR = new MenuBool("autoR", "Auto R");
        private readonly MenuBool fastR = new MenuBool("fastR", "Fast R ks Combo");
        private readonly MenuBool overkillR = new MenuBool("overkillR", "Overkill protection", false);
        private readonly MenuKeyBind useR = new MenuKeyBind("useR", "Semi-manual cast R key", Keys.T, KeyBindType.Press);

        private readonly MenuBool logicQW = new MenuBool("logicQW", "Use Q and W only if don't have ammo", false);

        private readonly MenuBool farmQ = new MenuBool("farmQ", "Lane clear Q");

        private readonly MenuBool jungleQ = new MenuBool("jungleQ", "Jungle clear Q");
        private readonly MenuBool jungleW = new MenuBool("jungleW", "Jungle clear W");
        private readonly MenuBool jungleE = new MenuBool("jungleE", "Jungle clear E");

        public Core.OKTWdash Dash;

        public Graves()
        {
            Q = new Spell(SpellSlot.Q, 925f);
            W = new Spell(SpellSlot.W, 950f);
            E = new Spell(SpellSlot.E, 425f);
            R = new Spell(SpellSlot.R, 1000f);
            R1 = new Spell(SpellSlot.R, 1700f);

            Q.SetSkillshot(0.25f, 40f, 3000f, true, SpellType.Line);
            W.SetSkillshot(0.25f, 120f, 1500f, true, SpellType.Circle);
            R.SetSkillshot(0.25f, 100f, 2100f, true, SpellType.Line);
            R1.SetSkillshot(0.25f, 100f, 2100f, false, SpellType.Line);

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
                agcW
            });

            Local.Add(new Menu("eConfig", "E Config")
            {
                autoE
            });

            Local.Add(new Menu("rConfig", "R Config")
            {
                autoR,
                fastR,
                overkillR,
                useR
            });

            Local.Add(logicQW);

            FarmMenu.Add(farmQ);
            FarmMenu.Add(jungleQ);
            FarmMenu.Add(jungleW);
            FarmMenu.Add(jungleE);

            Dash = new Core.OKTWdash(E);

            AntiGapcloser.OnGapcloser += AntiGapcloser_OnGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnAfterAttack += Orbwalker_OnAfterAttack;
        }

        private void AntiGapcloser_OnGapcloser(AIHeroClient sender, AntiGapcloser.GapcloserArgs args)
        {
            if (!OktwCommon.CheckGapcloser(sender, args))
            {
                return;
            }

            if (Player.Mana > EMANA + RMANA)
            {
                if (sender.IsValidTarget(E.Range))
                {
                    if (agcW.Enabled && W.IsReady())
                    {
                        W.Cast(args.EndPosition);
                    }
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
                        Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Cyan, 1);
                    }
                }
                else
                {
                    Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Cyan, 1);
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
            if (useR.Active && R1.IsReady())
            {
                var t = TargetSelector.GetTarget(R1.Range, DamageType.Physical);

                if (t.IsValidTarget())
                {
                    R1.Cast(t);
                }
            }

            if (LagFree(0))
            {
                SetMana();
                LogicJungle();
            }

            if (!logicQW.Enabled || !Player.HasBuff("gravesbasicattackammo1"))
            {
                if (LagFree(2) && autoQ.Enabled && !Player.IsWindingUp && Q.IsReady())
                {
                    LogicQ();
                }

                if (LagFree(3) && autoW.Enabled && !Player.IsWindingUp && W.IsReady())
                {
                    LogicW();
                }
            }

            if (LagFree(4) && autoR.Enabled && R.IsReady())
            {
                LogicR();
            }
        }

        private void Orbwalker_OnAfterAttack(object sender, AfterAttackEventArgs args)
        {
            if (autoE.Enabled && E.IsReady())
            {
                LogicE();
            }

            if (jungleE.Enabled && LaneClear && E.IsReady())
            {
                if (GameObjects.Jungle.Any(e => e.IsValidTarget(700) && e.NetworkId == args.Target.NetworkId))
                {
                    E.Cast(Game.CursorPos);
                }
            }
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (t.IsValidTarget())
            {
                var step = t.Distance(Player) / 20;

                for (var i = 0; i < 20; i++)
                {
                    var p = Player.Position.Extend(t.Position, step * i);
                    if (p.IsWall())
                    {
                        return;
                    }
                }

                if (Combo && Player.Mana > QMANA + RMANA)
                {
                    CastSpell(Q, t);
                }
                else if (Harass && harassQ.Enabled && Player.Mana > QMANA + QMANA + WMANA + EMANA + RMANA && HarassList.Any(e => e.Enabled && e.Name == "harass" + t.CharacterName))
                {
                    CastSpell(Q, t);
                }
                else
                {
                    var qDmg = OktwCommon.GetKsDamage(t, Q);
                    var rDmg = R.GetDamage(t);

                    if (qDmg > t.Health)
                    {
                        Q.Cast(t);
                    }
                    else if (qDmg + rDmg > t.Health && R.IsReady() && Player.Mana > QMANA + RMANA)
                    {
                        CastSpell(Q, t);

                        if (fastR.Enabled && rDmg < t.Health)
                        {
                            CastSpell(R, t);
                        }
                    }
                }

                if (!None && Player.Mana > QMANA + EMANA + RMANA)
                {
                    foreach (var enemy in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(Q.Range) && !OktwCommon.CanMove(e)))
                    {
                        Q.Cast(enemy);
                    }
                }
            }
            else if (FarmSpells && farmQ.Enabled)
            {
                var allMinionsQ = GameObjects.EnemyMinions.Where(e => e.IsValidTarget(Q.Range)).ToList();
                var farmRsQ = Q.GetLineFarmLocation(allMinionsQ);
                if (farmRsQ.MinionsHit > 2)
                {
                    Q.Cast(farmRsQ.Position);
                }
            }
        }

        private void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, DamageType.Magical);

            if (t.IsValidTarget())
            {
                var wDmg = OktwCommon.GetKsDamage(t, W);

                if (wDmg > t.Health)
                {
                    W.Cast(t);
                }
                else if (wDmg + Q.GetDamage(t) > t.Health && Player.Mana > QMANA + WMANA + RMANA)
                {
                    W.Cast(t);
                }
                else if (Combo && Player.Mana > QMANA + WMANA + RMANA)
                {
                    if (!t.InAutoAttackRange() || Player.CountEnemyHeroesInRange(300) > 0 || t.CountEnemyHeroesInRange(250) > 1 || Player.HealthPercent < 50)
                    {
                        W.Cast(t);
                    }
                    else if (Player.Mana > QMANA + WMANA + EMANA + RMANA)
                    {
                        foreach (var enemy in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(W.Range) && !OktwCommon.CanMove(e)))
                        {
                            W.Cast(enemy);
                        }
                    }
                }
            }
        }

        private void LogicE()
        {
            if (GameObjects.EnemyHeroes.Any(e => e.IsValidTarget(270) && e.IsMelee))
            {
                var dashPos = Dash.CastDash(true);
                if (!dashPos.IsZero)
                {
                    E.Cast(dashPos);
                }
            }

            if (Combo && Player.Mana > EMANA + RMANA && !Player.HasBuff("gravesbasicattackammo2"))
            {
                var dashPos = Dash.CastDash(true);
                if (!dashPos.IsZero)
                {
                    E.Cast(dashPos);
                }
            }
        }

        private void LogicR()
        {
            foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(R1.Range) && OktwCommon.ValidUlt(e)))
            {
                var rDmg = OktwCommon.GetKsDamage(target, R);

                if (rDmg < target.Health)
                {
                    continue;
                }

                if (overkillR.Enabled && target.Health < Player.Health)
                {
                    if (target.InAutoAttackRange())
                    {
                        continue;
                    }

                    if (target.CountAllyHeroesInRange(400) > 0)
                    {
                        continue;
                    }
                }

                var rDmg2 = rDmg * 0.8;

                if (target.IsValidTarget(R.Range) && !OktwCommon.IsSpellHeroCollision(target, R) && rDmg > target.Health)
                {
                    CastSpell(R, target);
                }
                else if (rDmg2 > target.Health)
                {
                    if (!OktwCommon.IsSpellHeroCollision(target, R1))
                    {
                        CastSpell(R1, target);
                    }
                    else if (target.IsValidTarget(1200))
                    {
                        CastSpell(R1, target);
                    }
                }
            }
        }

        private void LogicJungle()
        {
            if (LaneClear)
            {
                var mobs = GameObjects.GetJungles(600);

                if (mobs.Count > 0)
                {
                    var mob = mobs.First();

                    if (jungleQ.Enabled && Q.IsReady())
                    {
                        Q.Cast(mob.Position);
                        return;
                    }

                    if (jungleW.Enabled && W.IsReady())
                    {
                        W.Cast(mob.Position);
                        return;
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
    }
}