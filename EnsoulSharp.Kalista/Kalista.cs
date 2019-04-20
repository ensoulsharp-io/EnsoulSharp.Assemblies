namespace EnsoulSharp.Kalista
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;
    using EnsoulSharp.SDK.Prediction;
    using EnsoulSharp.SDK.Utility;

    using Color = System.Drawing.Color;

    internal class Kalista
    {
        private static Menu MyMenu;
        private static Spell Q, E;

        public static void OnLoad()
        {
            Q = new Spell(SpellSlot.Q, 1150f);
            E = new Spell(SpellSlot.E, 1000f);

            Q.SetSkillshot(0.35f, 40, 2400, true, SkillshotType.Line);

            MyMenu = new Menu("ensoulsharp.kalista", "EnsoulSharp.Kalista", true);

            var combat = new Menu("combat", "Combo Settings");
            combat.Add(MenuWrapper.Combat.Q);
            combat.Add(MenuWrapper.Combat.DisableQ);
            combat.Add(MenuWrapper.Combat.E);
            combat.Add(MenuWrapper.Combat.DisableE);
            combat.Add(MenuWrapper.Combat.DisableE2);
            combat.Add(MenuWrapper.Combat.OrbwalkerMinion);
            MyMenu.Add(combat);

            var harass = new Menu("harass", "Harass Settings");
            harass.Add(MenuWrapper.Harass.Q);
            harass.Add(MenuWrapper.Harass.QMinion);
            harass.Add(MenuWrapper.Harass.E);
            harass.Add(MenuWrapper.Harass.DisableE);
            harass.Add(MenuWrapper.Harass.DisableE2);
            harass.Add(MenuWrapper.Harass.Mana);
            MyMenu.Add(harass);

            var lane = new Menu("lane", "LaneClear Settings");
            lane.Add(MenuWrapper.LaneClear.E);
            lane.Add(MenuWrapper.LaneClear.Mana);
            MyMenu.Add(lane);

            var jungle = new Menu("jungle", "JungleClear Settings");
            jungle.Add(MenuWrapper.JungleClear.Q);
            jungle.Add(MenuWrapper.JungleClear.E);
            jungle.Add(MenuWrapper.JungleClear.Mana);

            var killable = new Menu("killable", "KillSteal Settings");
            killable.Add(MenuWrapper.KillAble.Q);
            killable.Add(MenuWrapper.KillAble.E);
            MyMenu.Add(killable);

            var draw = new Menu("draw", "Draw Settings");
            draw.Add(MenuWrapper.Draw.Q);
            draw.Add(MenuWrapper.Draw.E);
            draw.Add(MenuWrapper.Draw.OnlyReady);
            draw.Add(MenuWrapper.Draw.DMG);
            MyMenu.Add(draw);

            MyMenu.Attach();

            Game.OnUpdate += OnTick;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
        }

        private static float GetEDamage(AIBaseClient target)
        {
            if (target == null || !target.IsValidTarget())
            {
                return 0;
            }

            return E.GetDamage(target) + E.GetDamage(target, DamageStage.Buff);
        }

        private static void KillAble()
        {
            if (MenuWrapper.KillAble.Q.Enabled && Q.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Q.Range) && !x.IsInvulnerable))
                {
                    if (target.IsValidTarget(Q.Range) && target.Health < Q.GetDamage(target))
                    {
                        var qPred = Q.GetPrediction(target, false, 0, CollisionObjects.Minions | CollisionObjects.YasuoWall);
                        if (qPred.Hitchance >= HitChance.High)
                        {
                            Q.Cast(qPred.UnitPosition);
                        }
                    }
                }
            }

            if (MenuWrapper.KillAble.E.Enabled && E.IsReady())
            {
                if (GameObjects.EnemyHeroes.Any(x =>
                    x.IsValidTarget(E.Range) && x.HasBuff("kalistaexpungemarker") && x.Health < GetEDamage(x) && !x.IsInvulnerable))
                {
                    E.Cast();
                }
            }
        }

        private static void Combat()
        {
            var target = TargetSelector.GetTarget(Q.Range);
            if (target == null || !target.IsValidTarget(Q.Range))
            {
                return;
            }

            if (MenuWrapper.Combat.Q.Enabled && Q.IsReady())
            {
                if (MenuWrapper.Combat.DisableQ.Enabled)
                {
                    if (Variables.Orbwalker.AttackSpeed < 1.98)
                    {
                        var qPred = Q.GetPrediction(target, false, 0, CollisionObjects.Minions | CollisionObjects.YasuoWall);
                        if (qPred.Hitchance >= HitChance.High)
                        {
                            Q.Cast(qPred.UnitPosition);
                        }
                    }
                }
                else
                {
                    var qPred = Q.GetPrediction(target, false, 0, CollisionObjects.Minions | CollisionObjects.YasuoWall);
                    if (qPred.Hitchance >= HitChance.High)
                    {
                        Q.Cast(qPred.UnitPosition);
                    }
                }
            }

            if (MenuWrapper.Combat.E.Enabled && E.IsReady())
            {
                foreach (var t in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(E.Range) && x.HasBuff("kalistaexpungemarker")))
                {
                    if (MenuWrapper.Combat.DisableE.Enabled)
                    {
                        if (t.HasBuffOfType(BuffType.Asleep) || t.HasBuffOfType(BuffType.Charm) ||
                            t.HasBuffOfType(BuffType.Fear) || t.HasBuffOfType(BuffType.Knockup) ||
                            t.HasBuffOfType(BuffType.Sleep) || t.HasBuffOfType(BuffType.Slow) ||
                            t.HasBuffOfType(BuffType.Stun))
                        {
                            continue;
                        }

                        if (GameObjects.EnemyMinions.Any(m => m.IsValidTarget(E.Range) && m.HasBuff("kalistaexpungemarker") && m.Health <= GetEDamage(m)))
                        {
                            if (MenuWrapper.Combat.DisableE2.Enabled)
                            {
                                if (Variables.TickCount - E.LastCastAttemptT > 2500)
                                {
                                    E.Cast();
                                }
                            }
                            else
                            {
                                E.Cast();
                            }
                        }
                    }
                }
            }

            if (MenuWrapper.Combat.OrbwalkerMinion.Enabled)
            {
                if (GameObjects.EnemyHeroes.All(x => !x.IsValidTarget(Player.Instance.AttackRange + Player.Instance.BoundingRadius + x.BoundingRadius)) &&
                    GameObjects.EnemyHeroes.Any(x => x.IsValidTarget(1000)))
                {
                    var AttackUnit =
                        GameObjects.EnemyMinions.Where(x => x.InAutoAttackRange())
                            .OrderBy(x => x.Distance(Game.CursorPosRaw))
                            .FirstOrDefault();

                    if (AttackUnit != null && !AttackUnit.IsDead && AttackUnit.InAutoAttackRange())
                    {
                        Variables.Orbwalker.ForceTarget = AttackUnit;
                    }
                }
                else
                {
                    Variables.Orbwalker.ForceTarget = null;
                }
            }
        }

        private static void Harass()
        {
            if (Player.Instance.ManaPercent < MenuWrapper.Harass.Mana.Value)
            {
                return;
            }

            var target = TargetSelector.GetTarget(Q.Range);
            if (target == null || !target.IsValidTarget(Q.Range))
            {
                return;
            }

            if (MenuWrapper.Harass.Q.Enabled && Q.IsReady())
            {
                var qPred = Q.GetPrediction(target, false, 0, CollisionObjects.Minions | CollisionObjects.YasuoWall);
                if (qPred.Hitchance >= HitChance.High)
                {
                    Q.Cast(qPred.UnitPosition);
                }
                else if (MenuWrapper.Harass.QMinion.Enabled)
                {
                    var c = qPred.CollisionObjects;
                    if (c.Count > 0 && !c.All(x =>
                            GameObjects.EnemyMinions.Any(m => m.NetworkId == x.NetworkId) ||
                            GameObjects.Heroes.Any(h => h.NetworkId == x.NetworkId))) // no hit on windwall
                    {
                        if (c.Any(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)))
                        {
                            Q.Cast(qPred.UnitPosition);
                        }
                    }
                }
            }

            if (MenuWrapper.Harass.E.Enabled && E.IsReady())
            {
                foreach (var t in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(E.Range) && x.HasBuff("kalistaexpungemarker")))
                {
                    if (MenuWrapper.Harass.DisableE.Enabled)
                    {
                        if (t.HasBuffOfType(BuffType.Asleep) || t.HasBuffOfType(BuffType.Charm) ||
                            t.HasBuffOfType(BuffType.Fear) || t.HasBuffOfType(BuffType.Knockup) ||
                            t.HasBuffOfType(BuffType.Sleep) || t.HasBuffOfType(BuffType.Slow) ||
                            t.HasBuffOfType(BuffType.Stun))
                        {
                            continue;
                        }

                        if (GameObjects.EnemyMinions.Any(m => m.IsValidTarget(E.Range) && m.HasBuff("kalistaexpungemarker") && m.Health <= GetEDamage(m)))
                        {
                            if (MenuWrapper.Harass.DisableE2.Enabled)
                            {
                                if (Variables.TickCount - E.LastCastAttemptT > 2500)
                                {
                                    E.Cast();
                                }
                            }
                            else
                            {
                                E.Cast();
                            }
                        }
                    }
                }
            }
        }

        private static void LaneClear()
        {
            if (Player.Instance.ManaPercent < MenuWrapper.LaneClear.Mana.Value)
            {
                return;
            }

            if (MenuWrapper.LaneClear.E.Enabled && E.IsReady())
            {
                if (GameObjects.EnemyMinions.Count(x =>
                        x.IsValidTarget(E.Range) && x.HasBuff("kalistaexpungemarker") && x.Health < GetEDamage(x)) >=
                    MenuWrapper.LaneClear.E.Value)
                {
                    E.Cast();
                }
            }
        }

        private static void JungleClear()
        {
            if (Player.Instance.ManaPercent < MenuWrapper.JungleClear.Mana.Value)
            {
                return;
            }

            if (MenuWrapper.JungleClear.Q.Enabled && Q.IsReady())
            {
                foreach (var mob in GameObjects.Jungle.Where(x => x.IsValidTarget(Q.Range) && (x.GetJungleType() == JungleType.Large || x.GetJungleType() == JungleType.Legendary)))
                {
                    if (mob.IsValidTarget(Q.Range))
                    {
                        var qPred = Q.GetPrediction(mob);
                        if (qPred.Hitchance >= HitChance.Medium)
                        {
                            Q.Cast(qPred.UnitPosition);
                        }
                    }
                }
            }

            if (MenuWrapper.JungleClear.E.Enabled && E.IsReady())
            {
                if (GameObjects.JungleLarge.Any(x => x.IsValidTarget(E.Range) && x.HasBuff("kalistaexpungemarker") && x.Health < GetEDamage(x) * 0.5))
                {
                    E.Cast();
                }

                if (GameObjects.JungleLegendary.Any(x => x.IsValidTarget(E.Range) && x.HasBuff("kalistaexpungemarker") && x.Health < GetEDamage(x) * 0.5))
                {
                    E.Cast();
                }

                if (GameObjects.JungleSmall.Any(x => x.IsValidTarget(E.Range) && x.HasBuff("kalistaexpungemarker") && x.Health < GetEDamage(x)))
                {
                    E.Cast();
                }
            }
        }

        private static void OnTick(EventArgs args)
        {
            if (Player.Instance.IsDead || Player.Instance.IsRecalling() || MenuGUI.IsChatOpen)
            {
                return;
            }

            KillAble();

            switch (Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combat();
                    break;
                case OrbwalkerMode.Harass:
                    Harass();
                    break;
                case OrbwalkerMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Player.Instance.IsDead || MenuGUI.IsChatOpen)
            {
                return;
            }

            if (MenuWrapper.Draw.Q.Enabled)
            {
                if (MenuWrapper.Draw.OnlyReady.Enabled && Q.IsReady())
                {
                    Render.Circle.DrawCircle(GameObjects.Player.Position, Q.Range, Color.FromArgb(48, 120, 252), 1);
                }
                else if (!MenuWrapper.Draw.OnlyReady.Enabled)
                {
                    Render.Circle.DrawCircle(GameObjects.Player.Position, Q.Range, Color.FromArgb(48, 120, 252), 1);
                }
            }

            if (MenuWrapper.Draw.E.Enabled)
            {
                if (MenuWrapper.Draw.OnlyReady.Enabled && E.IsReady())
                {
                    Render.Circle.DrawCircle(GameObjects.Player.Position, E.Range, Color.FromArgb(255, 65, 65), 1);
                }
                else if (!MenuWrapper.Draw.OnlyReady.Enabled)
                {
                    Render.Circle.DrawCircle(GameObjects.Player.Position, E.Range, Color.FromArgb(255, 65, 65), 1);
                }
            }
        }

        private static void OnEndScene(EventArgs args)
        {
            if (Player.Instance.IsDead || MenuGUI.IsChatOpen)
            {
                return;
            }

            if (MenuWrapper.Draw.DMG.Enabled && E.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x =>
                    x.IsValidTarget() && x.IsVisibleOnScreen && x.HasBuff("kalistaexpungemarker")))
                {
                    var dmg = GetEDamage(target);
                    if (dmg > 0)
                    {
                        var barPos = target.HPBarPosition;
                        var xPos = barPos.X - 45;
                        var yPos = barPos.Y - 19;
                        if (target.CharacterName == "Annie")
                        {
                            yPos += 2;
                        }

                        var remainHealth = target.Health - dmg;
                        var x1 = xPos + (target.Health / target.MaxHealth * 104);
                        var x2 = (float) (xPos + ((remainHealth > 0 ? remainHealth : 0) / target.MaxHealth * 103.4));
                        Drawing.DrawLine(x1, yPos, x2, yPos, 11, Color.FromArgb(255, 147, 0));
                    }
                }
            }
        }
    }
}
