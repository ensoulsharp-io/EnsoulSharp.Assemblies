using System;
using System.Linq;
using System.Windows.Forms;

using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;

using SebbyLib;

using Menu = EnsoulSharp.SDK.MenuUI.Menu;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Graves : Program
    {
        public static Core.OKTWdash Dash;
        public float OverKill = 0;

        public Graves()
        {
            Q = new Spell(SpellSlot.Q, 800);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 425);
            R = new Spell(SpellSlot.R, 1100);
            R1 = new Spell(SpellSlot.R, 1900);

            Q.SetSkillshot(0.25f, 40, 3000, false, SkillshotType.Line);
            W.SetSkillshot(0.25f, 225, 1500, false, SkillshotType.Circle);
            R.SetSkillshot(0.25f, 100, 2100, false, SkillshotType.Line);
            R1.SetSkillshot(0.25f, 100, 2100, false, SkillshotType.Line);

            var wrapper = new Menu(Player.CharacterName, Player.CharacterName);

            var draw = new Menu("draw", "Draw");
            draw.Add(new MenuBool("qRange", "Q range", false, Player.CharacterName));
            draw.Add(new MenuBool("wRange", "W range", false, Player.CharacterName));
            draw.Add(new MenuBool("eRange", "E range", false, Player.CharacterName));
            draw.Add(new MenuBool("rRange", "R range", false, Player.CharacterName));
            draw.Add(new MenuBool("onlyRdy", "Draw only ready spells", true, Player.CharacterName));
            wrapper.Add(draw);

            var q = new Menu("QConfig", "Q Config");
            q.Add(new MenuBool("autoQ", "Auto Q", true, Player.CharacterName));
            q.Add(new MenuBool("harassQ", "Harass Q", true, Player.CharacterName));
            wrapper.Add(q);

            var w = new Menu("WConfig", "W Config");
            w.Add(new MenuBool("autoW", "Auto W", true, Player.CharacterName));
            w.Add(new MenuBool("AGCW", "AntiGapcloser W", true, Player.CharacterName));
            wrapper.Add(w);

            var e = new Menu("EConfig", "E Config");
            e.Add(new MenuBool("autoE", "Auto E", true, Player.CharacterName));
            wrapper.Add(e);

            var r = new Menu("RConfig", "R Config");
            r.Add(new MenuBool("autoR", "Auto R", true, Player.CharacterName));
            r.Add(new MenuBool("fastR", "Fast R ks Combo", true, Player.CharacterName));
            r.Add(new MenuBool("overkillR", "Overkill protection", false, Player.CharacterName));
            r.Add(new MenuKeyBind("useR", "Semi-manual cast R key", Keys.T, KeyBindType.Press, Player.CharacterName));
            wrapper.Add(r);

            var farm = new Menu("farm", "Farm");
            farm.Add(new MenuBool("farmQ", "Lane clear Q", true, Player.CharacterName));
            farm.Add(new MenuSlider("LCMana", "Lane clear minimum Mana", 80, 30, 100, Player.CharacterName));
            farm.Add(new MenuSlider("LCMinions", "Lane clear minimum Minions", 2, 0, 10, Player.CharacterName));
            farm.Add(new MenuBool("jungleQ", "Jungle clear Q", true, Player.CharacterName));
            farm.Add(new MenuBool("jungleW", "Jungle clear W", true, Player.CharacterName));
            farm.Add(new MenuBool("jungleE", "Jungle clear E", true, Player.CharacterName));
            farm.Add(new MenuSlider("JCMana", "Jungle clear minimum Mana", 80, 30, 100, Player.CharacterName));
            wrapper.Add(farm);

            wrapper.Add(new MenuBool("QWlogic", "Use Q and W only if don't have ammo", false, Player.CharacterName));

            Config.Add(wrapper);

            Dash = new Core.OKTWdash(E);

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Orbwalker.OnAction += Orbwalker_OnAction;
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range);
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

                if (Combo && Player.Mana > RMANA + QMANA)
                    CastSpell(Q, t);
                else if (Harass && Player.Mana > RMANA + EMANA + QMANA + QMANA && Config[Player.CharacterName]["QConfig"].GetValue<MenuBool>("harassQ").Enabled && Config["harass"].GetValue<MenuBool>("harass" + t.CharacterName).Enabled)
                    CastSpell(Q, t);
                else
                {
                    var qDmg = OktwCommon.GetKsDamage(t, Q);
                    var rDmg = R.GetDamage(t);
                    if (qDmg > t.Health)
                    {
                        Q.Cast(t, true);
                        OverKill = Game.Time;
                    }
                    else if (qDmg + rDmg > t.Health && Player.Mana > RMANA + QMANA && R.IsReady())
                    {
                        CastSpell(Q, t);
                        if (Config[Player.CharacterName]["RConfig"].GetValue<MenuBool>("fastR").Enabled && rDmg < t.Health)
                            CastSpell(R, t);
                    }
                }

                if (!None && Player.Mana > RMANA + QMANA + EMANA)
                {
                    foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy);
                }
            }
            else if (LaneClear)
            {
                var farm = Config[Player.CharacterName]["farm"];
                if (farm.GetValue<MenuBool>("farmQ").Enabled && Player.ManaPercent > farm.GetValue<MenuSlider>("LCMana").Value)
                {
                    var minionList = Cache.GetMinions(Player.PreviousPosition, Q.Range);
                    var farmLocation = Q.GetLineFarmLocation(minionList, Q.Width);
                    if (farmLocation.MinionsHit > farm.GetValue<MenuSlider>("LCMinions").Value)
                        Q.Cast(farmLocation.Position);
                }
            }
        }

        private void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range);
            if (t.IsValidTarget())
            {
                var wDmg = OktwCommon.GetKsDamage(t, W);
                if (wDmg > t.Health)
                {
                    W.Cast(t, true, false, true);
                }
                else if (wDmg + Q.GetDamage(t) > t.Health && Player.Mana > QMANA + WMANA + RMANA)
                {
                    W.Cast(t, true, false, true);
                }
                else if (Combo && Player.Mana > RMANA + WMANA + QMANA)
                {
                    if (!t.InAutoAttackRange() || Player.CountEnemyHeroesInRange(300) > 0 || t.CountEnemyHeroesInRange(250) > 1 || Player.HealthPercent < 50)
                        W.Cast(t, true, false, true);
                    else if (Player.Mana > RMANA + WMANA + QMANA + EMANA)
                    {
                        foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                            W.Cast(enemy, true, false, true);
                    }
                }
            }
        }

        private void LogicE()
        {
            if (GameObjects.EnemyHeroes.Any(target => target.IsValidTarget(270) && target.IsMelee))
            {
                var dashPos = Dash.CastDash(true);
                if (!dashPos.IsZero)
                {
                    E.Cast(dashPos);
                }
            }

            if (Combo && Player.Mana > RMANA + EMANA && !Player.HasBuff("gravesbasicattackammo2"))
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
            foreach (var target in GameObjects.EnemyHeroes.Where(target => target.IsValidTarget(R1.Range) && OktwCommon.ValidUlt(target)))
            {
                var rDmg = OktwCommon.GetKsDamage(target, R);

                if (rDmg < target.Health)
                    continue;

                if (Config[Player.CharacterName]["RConfig"].GetValue<MenuBool>("overkillR").Enabled && target.Health < Player.Health)
                {
                    if (target.InAutoAttackRange())
                        continue;
                    if (target.CountAllyHeroesInRange(400) > 0)
                        continue;
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

        private void Jungle()
        {
            var farm = Config[Player.CharacterName]["farm"];
            if (LaneClear && Player.ManaPercent > farm.GetValue<MenuSlider>("JCMana").Value)
            {
                var mobs = Cache.GetMinions(Player.PreviousPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (Q.IsReady() && farm.GetValue<MenuBool>("jungleQ").Enabled)
                    {
                        Q.Cast(mob.Position);
                        return;
                    }
                    if (W.IsReady() && farm.GetValue<MenuBool>("jungleW").Enabled)
                    {
                        W.Cast(mob.Position);
                        return;
                    }
                }
            }
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

            QMANA = Q.Instance.SData.ManaArray[Math.Max(0, Q.Level - 1)];
            WMANA = W.Instance.SData.ManaArray[Math.Max(0, W.Level - 1)];
            EMANA = E.Instance.SData.ManaArray[Math.Max(0, E.Level - 1)];

            if (!R.IsReady())
                RMANA = QMANA - Player.PARRegenRate * Q.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.ManaArray[Math.Max(0, R.Level - 1)];
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
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (Config[Player.CharacterName]["RConfig"].GetValue<MenuKeyBind>("useR").Active && R.IsReady())
            {
                var t = TargetSelector.GetTarget(R1.Range);
                if (t.IsValidTarget())
                    R1.Cast(t, true);
            }

            if (LagFree(0))
            {
                SetMana();
                Jungle();
            }

            if (!Config[Player.CharacterName].GetValue<MenuBool>("QWlogic").Enabled || !Player.HasBuff("gravesbasicattackammo1"))
            {
                if (LagFree(2) && Q.IsReady() && !Player.IsWindingUp && Config[Player.CharacterName]["QConfig"].GetValue<MenuBool>("autoQ").Enabled)
                    LogicQ();
                if (LagFree(3) && W.IsReady() && !Player.IsWindingUp && Config[Player.CharacterName]["WConfig"].GetValue<MenuBool>("autoW").Enabled)
                    LogicW();
            }
            if (LagFree(4) && R.IsReady() && Config[Player.CharacterName]["RConfig"].GetValue<MenuBool>("autoR").Enabled)
                LogicR();
        }

        private void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {
            if (Player.Mana > RMANA + EMANA)
            {
                if (sender.IsValidTarget(E.Range))
                {
                    if (W.IsReady() && Config[Player.CharacterName]["WConfig"].GetValue<MenuBool>("AGCW").Enabled)
                    {
                        W.Cast(args.EndPosition);
                    }
                }
            }
        }

        private void Orbwalker_OnAction(object sender, OrbwalkerActionArgs args)
        {
            if (args.Type == OrbwalkerType.AfterAttack)
            {
                if (E.IsReady() && Config[Player.CharacterName]["EConfig"].GetValue<MenuBool>("autoE").Enabled)
                    LogicE();
                if (LaneClear && args.Target != null && Config[Player.CharacterName]["farm"].GetValue<MenuBool>("jungleE").Enabled && Player.ManaPercent > Config[Player.CharacterName]["farm"].GetValue<MenuSlider>("JCMana").Value)
                {
                    if (E.IsReady() && Cache.GetMinions(Player.PreviousPosition, 700, MinionTeam.Neutral).Any(x => x.NetworkId == args.Target.NetworkId))
                        E.Cast(Game.CursorPosRaw);
                }
            }
        }
    }
}
