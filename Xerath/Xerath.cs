namespace EnsoulSharp.Xerath
{
    using System;
    using System.Linq;

    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.Events;
    using EnsoulSharp.SDK.MenuUI;
    using EnsoulSharp.SDK.Prediction;
    using EnsoulSharp.SDK.Utility;

    using Color = System.Drawing.Color;

    internal class Xerath
    {
        private static Spell Q, W, E, R;

        public static void OnLoad()
        {
            Q = new Spell(SpellSlot.Q, 750f);
            Q.SetSkillshot(0.60f, 95f, float.MaxValue, false, false, SkillshotType.Line);
            Q.SetCharged("XerathArcanopulseChargeUp", "XerathArcanopulseChargeUp", 750, 1550, 1.5f);

            W = new Spell(SpellSlot.W, 950f);
            W.SetSkillshot(0.70f, 125f, float.MaxValue, false, true, SkillshotType.Circle);

            E = new Spell(SpellSlot.E, 1050f);
            E.SetSkillshot(0.25f, 60f, 1400f, true, false, SkillshotType.Line);

            R = new Spell(SpellSlot.R, 3200f);
            R.SetSkillshot(0.70f, 125f, float.MaxValue, false, false, SkillshotType.Circle);

            var MyMenu = new Menu(ObjectManager.Player.CharacterName, "EnsoulSharp.Xerath", true);

            var ult = new Menu("Ult", "Ult Settings")
            {
                MenuWrapper.Ult.Key,
                MenuWrapper.Ult.Auto,
                MenuWrapper.Ult.NearMouse,
                MenuWrapper.Ult.MouseZone,
                MenuWrapper.Ult.DelayR,
                MenuWrapper.Ult.DelayMs
            };
            MyMenu.Add(ult);

            var combat = new Menu("Combo", "Combo Settings")
            {
                MenuWrapper.Combat.Q,
                MenuWrapper.Combat.W,
                MenuWrapper.Combat.E,
                MenuWrapper.Combat.R
            };
            MyMenu.Add(combat);

            var harass = new Menu("Harass", "Harass Settings")
            {
                MenuWrapper.Harass.Q,
                MenuWrapper.Harass.W,
                MenuWrapper.Harass.E,
                MenuWrapper.Harass.Mana
            };
            MyMenu.Add(harass);

            var lane = new Menu("LaneClear", "LaneClear Settings")
            {
                MenuWrapper.LaneClear.Q,
                MenuWrapper.LaneClear.QH,
                MenuWrapper.LaneClear.W,
                MenuWrapper.LaneClear.WH,
                MenuWrapper.LaneClear.Mana
            };
            MyMenu.Add(lane);

            var jungle = new Menu("JungleClear", "JungleClear Settings")
            {
                MenuWrapper.JungleClear.Q,
                MenuWrapper.JungleClear.W,
                MenuWrapper.JungleClear.E,
                MenuWrapper.JungleClear.Mana
            };
            MyMenu.Add(jungle);

            var killable = new Menu("KillSteal", "KillSteal Settings")
            {
                MenuWrapper.KillAble.Q,
                MenuWrapper.KillAble.W,
                MenuWrapper.KillAble.E
            };
            MyMenu.Add(killable);

            var misc = new Menu("Misc", "Misc Settings")
            {
                MenuWrapper.Misc.QSlow,
                MenuWrapper.Misc.EAntiGapcloser,
                MenuWrapper.Misc.EInterrupt,
                MenuWrapper.Misc.RSlow
            };
            MyMenu.Add(misc);

            var draw = new Menu("Draw", "Draw Settings")
            {
                MenuWrapper.Draw.Q,
                MenuWrapper.Draw.W,
                MenuWrapper.Draw.E,
                MenuWrapper.Draw.R,
                MenuWrapper.Draw.RMouse,
                MenuWrapper.Draw.OnlyReady
            };
            MyMenu.Add(draw);

            MyMenu.Add(MenuWrapper.SemiKey.W);
            MyMenu.Add(MenuWrapper.SemiKey.E);

            MyMenu.Attach();

            Tick.OnTick += OnUpdate;
            Gapcloser.OnGapcloser += OnGapcloser;
            Interrupter.OnInterrupterSpell += OnInterrupterSpell;
            Drawing.OnDraw += OnDraw;
        }

        private static HitChance QHitchance => MenuWrapper.Misc.QSlow.Enabled ? HitChance.VeryHigh : HitChance.High;

        private static HitChance RHitchance => MenuWrapper.Misc.RSlow.Enabled ? HitChance.VeryHigh : HitChance.High;

        private static void Ultimate()
        {
            if (!ObjectManager.Player.HasBuff("XerathLocusOfPower2") || Q.IsCharging)
            {
                return;
            }

            if (!MenuWrapper.Ult.Auto.Enabled &&
                !(MenuWrapper.Combat.R.Enabled && Orbwalker.ActiveMode == OrbwalkerMode.Combo) && 
                !MenuWrapper.Ult.Key.Active)
            {
                return;
            }

            if (!MenuWrapper.Ult.Key.Active && MenuWrapper.Ult.DelayR.Enabled &&
                Variables.GameTimeTickCount - R.LastCastAttemptT < MenuWrapper.Ult.DelayMs.Value)
            {
                return;
            }

            var target = TargetSelector.GetTarget(R.Range);
            if (MenuWrapper.Ult.NearMouse.Enabled && MenuWrapper.Ult.MouseZone.Value > 0)
            {
                target = TargetSelector.GetTargets(R.Range).FirstOrDefault(x =>
                    x.Position.Distance(Game.CursorPos) <= MenuWrapper.Ult.MouseZone.Value);
            }

            if (target != null && target.IsValidTarget(R.Range))
            {
                var pred = R.GetPrediction(target);
                if (pred.Hitchance >= RHitchance)
                {
                    R.Cast(pred.CastPosition);
                }
            }
        }

        private static void SemiAutomatic()
        {
            if (MenuWrapper.SemiKey.W.Active && W.IsReady() && !ObjectManager.Player.HasBuff("XerathLocusOfPower2") && 
                !Q.IsCharging)
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

                var target = TargetSelector.GetTarget(W.Range);
                if (target != null && target.IsValidTarget(W.Range))
                {
                    var pred = W.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.VeryHigh)
                    {
                        W.Cast(pred.CastPosition);
                    }
                }
            }

            if (MenuWrapper.SemiKey.E.Active && E.IsReady() && !ObjectManager.Player.HasBuff("XerathLocusOfPower2") && 
                !Q.IsCharging)
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

                var target = TargetSelector.GetTarget(E.Range);
                if (target != null && target.IsValidTarget(E.Range))
                {
                    var pred = E.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.VeryHigh)
                    {
                        E.Cast(pred.CastPosition);
                    }
                }
            }

            if (MenuWrapper.Ult.Key.Active && R.IsReady() && !ObjectManager.Player.HasBuff("XerathLocusOfPower2") &&
                !Q.IsCharging)
            {
                var target = TargetSelector.GetTarget(R.Range);
                if (target != null && target.IsValidTarget(R.Range))
                {
                    R.Cast(target.Position);
                }
            }
        }

        private static void Killable()
        {
            if (MenuWrapper.KillAble.Q.Enabled && Q.IsReady() && Q.IsCharging)
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x =>
                    x.IsValidTarget(Q.ChargedMaxRange) && !x.IsInvulnerable && x.Health < Q.GetDamage(x)))
                {
                    if (target.IsValidTarget(Q.Range))
                    {
                        var pred = Q.GetPrediction(target);
                        if (pred.Hitchance >= QHitchance)
                        {
                            Q.ShootChargedSpell(pred.CastPosition, true);
                            return;
                        }
                    }
                }
            }

            // make sure dont cast spell on the same time
            if (Q.IsCharging || Variables.GameTimeTickCount - Q.LastCastAttemptT < 1500)
            {
                return;
            }

            if (MenuWrapper.KillAble.W.Enabled && W.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x =>
                    x.IsValidTarget(W.Range) && !x.IsInvulnerable && x.Health < W.GetDamage(x)))
                {
                    if (target.IsValidTarget(W.Range))
                    {
                        var pred = W.GetPrediction(target);
                        if (pred.Hitchance >= HitChance.VeryHigh)
                        {
                            W.Cast(pred.CastPosition);
                            return;
                        }
                    }
                }
            }

            if (MenuWrapper.KillAble.E.Enabled && E.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x =>
                    x.IsValidTarget(E.Range) && !x.IsInvulnerable && x.Health < E.GetDamage(x)))
                {
                    if (target.IsValidTarget(E.Range))
                    {
                        var pred = E.GetPrediction(target);
                        if (pred.Hitchance >= HitChance.VeryHigh)
                        {
                            E.Cast(pred.CastPosition);
                            return;
                        }
                    }
                }
            }
        }

        private static void Combo()
        {
            if (!Q.IsCharging)
            {
                // ult check
                if (!ObjectManager.Player.HasBuff("XerathLocusOfPower2"))
                {
                    if (MenuWrapper.Combat.W.Enabled && W.IsReady())
                    {
                        var target = TargetSelector.GetTarget(W.Range);
                        if (target != null && target.IsValidTarget(W.Range))
                        {
                            var pred = W.GetPrediction(target);
                            if (pred.Hitchance >= HitChance.VeryHigh)
                            {
                                W.Cast(pred.CastPosition);
                            }
                        }
                    }

                    if (MenuWrapper.Combat.E.Enabled && E.IsReady())
                    {
                        var target = TargetSelector.GetTarget(E.Range);
                        if (target != null && target.IsValidTarget(E.Range))
                        {
                            var pred = E.GetPrediction(target);
                            if (pred.Hitchance >= HitChance.VeryHigh)
                            {
                                E.Cast(pred.CastPosition);
                            }
                        }
                    }

                    if (MenuWrapper.Combat.Q.Enabled && Q.IsReady())
                    {
                        var target = TargetSelector.GetTarget(Q.ChargedMaxRange);
                        if (target != null && target.IsValidTarget(Q.ChargedMaxRange))
                        {
                            // slow buff = more hitchance || target too far and cant be w hit
                            if (!W.IsReady() || target.DistanceToPlayer() > 850)
                            {
                                var pred = Q.GetPrediction(target);
                                if (pred.Hitchance >= HitChance.High)
                                {
                                    Q.StartCharging();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (MenuWrapper.Combat.Q.Enabled && Q.IsReady() && Q.IsCharging)
                {
                    var target = TargetSelector.GetTarget(Q.Range);
                    if (target != null && target.IsValidTarget(Q.Range))
                    {
                        var pred = Q.GetPrediction(target);
                        if (pred.Hitchance >= QHitchance)
                        {
                            Q.ShootChargedSpell(pred.CastPosition, true);
                        }
                    }
                }
            }
        }

        private static void Harass()
        {
            if (!Q.IsCharging)
            {
                // ult + mana check
                if (!ObjectManager.Player.HasBuff("XerathLocusOfPower2") &&
                    ObjectManager.Player.ManaPercent >= MenuWrapper.Harass.Mana.Value)
                {
                    if (MenuWrapper.Harass.W.Enabled && W.IsReady())
                    {
                        var target = TargetSelector.GetTarget(W.Range);
                        if (target != null && target.IsValidTarget(W.Range))
                        {
                            var pred = W.GetPrediction(target);
                            if (pred.Hitchance >= HitChance.VeryHigh)
                            {
                                W.Cast(pred.CastPosition);
                                return;
                            }
                        }
                    }

                    if (MenuWrapper.Harass.E.Enabled && E.IsReady())
                    {
                        var target = TargetSelector.GetTarget(E.Range);
                        if (target != null && target.IsValidTarget(E.Range))
                        {
                            var pred = E.GetPrediction(target);
                            if (pred.Hitchance >= HitChance.VeryHigh)
                            {
                                E.Cast(pred.CastPosition);
                                return;
                            }
                        }
                    }

                    if (MenuWrapper.Harass.Q.Enabled && Q.IsReady())
                    {
                        var target = TargetSelector.GetTarget(Q.ChargedMaxRange);
                        if (target != null && target.IsValidTarget(Q.ChargedMaxRange))
                        {
                            // slow buff = more hitchance || target too far and cant be w hit
                            if (!W.IsReady() || target.DistanceToPlayer() > 850)
                            {
                                var pred = Q.GetPrediction(target);
                                if (pred.Hitchance >= HitChance.High)
                                {
                                    Q.StartCharging(true);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // ignore mana check when q charge
                if (MenuWrapper.Harass.Q.Enabled && Q.IsReady() && Q.IsCharging)
                {
                    var target = TargetSelector.GetTarget(Q.Range);
                    if (target != null && target.IsValidTarget(Q.Range))
                    {
                        var pred = Q.GetPrediction(target);
                        if (pred.Hitchance >= QHitchance)
                        {
                            Q.ShootChargedSpell(pred.CastPosition, true);
                        }
                    }
                }
            }
        }

        private static void LaneClear()
        {
            if (!Q.IsCharging)
            {
                // ult + mana check
                if (!ObjectManager.Player.HasBuff("XerathLocusOfPower2") &&
                    ObjectManager.Player.ManaPercent >= MenuWrapper.Harass.Mana.Value)
                {
                    if (MenuWrapper.LaneClear.W.Enabled && W.IsReady())
                    {
                        var minions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(W.Range) && x.IsMinion())
                            .Cast<AIBaseClient>().ToList();
                        if (minions.Any())
                        {
                            var wFarmLocation = W.GetCircularFarmLocation(minions);
                            if (wFarmLocation.Position.IsValid() &&
                                wFarmLocation.MinionsHit >= MenuWrapper.LaneClear.WH.Value)
                            {
                                W.Cast(wFarmLocation.Position);
                                return;
                            }
                        }
                    }

                    if (MenuWrapper.LaneClear.Q.Enabled && Q.IsReady())
                    {
                        var minions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Q.ChargedMaxRange) && x.IsMinion())
                            .Cast<AIBaseClient>().ToList();
                        if (minions.Any())
                        {
                            var qFarmLocation = Q.GetLineFarmLocation(minions);
                            if (qFarmLocation.Position.IsValid() &&
                                qFarmLocation.MinionsHit >= MenuWrapper.LaneClear.QH.Value)
                            {
                                Q.StartCharging();
                            }
                        }
                    }
                }
            }
            else
            {
                // ignore mana check when q charge
                if (MenuWrapper.LaneClear.Q.Enabled && Q.IsReady() && Q.IsCharging)
                {
                    var minions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Q.Range) && x.IsMinion())
                        .Cast<AIBaseClient>().ToList();
                    if (minions.Any())
                    {
                        var qFarmLocation = Q.GetLineFarmLocation(minions);
                        if (qFarmLocation.Position.IsValid() &&
                            qFarmLocation.MinionsHit >= MenuWrapper.LaneClear.QH.Value)
                        {
                            Q.ShootChargedSpell(qFarmLocation.Position.ToVector3());
                        }
                    }
                }
            }
        }

        private static void JungleClear()
        {
            if (!Q.IsCharging)
            {
                // ult + mana check
                if (!ObjectManager.Player.HasBuff("XerathLocusOfPower2") &&
                    ObjectManager.Player.ManaPercent >= MenuWrapper.LaneClear.Mana.Value)
                {
                    if (MenuWrapper.JungleClear.W.Enabled && W.IsReady())
                    {
                        var mob = GameObjects.Jungle
                            .Where(x => x.IsValidTarget(W.Range) && x.GetJungleType() != JungleType.Unknown)
                            .OrderByDescending(x => x.MaxHealth).FirstOrDefault();

                        if (mob != null && mob.IsValidTarget(W.Range))
                        {
                            var pred = W.GetPrediction(mob);
                            if (pred.Hitchance >= HitChance.High)
                            {
                                W.Cast(pred.CastPosition);
                                return;
                            }
                        }
                    }

                    if (MenuWrapper.JungleClear.E.Enabled && E.IsReady())
                    {
                        var mob = GameObjects.Jungle
                            .Where(x => x.IsValidTarget(E.Range) && x.GetJungleType() != JungleType.Unknown)
                            .OrderByDescending(x => x.MaxHealth).FirstOrDefault();

                        if (mob != null && mob.IsValidTarget(E.Range))
                        {
                            var pred = E.GetPrediction(mob);
                            if (pred.Hitchance >= HitChance.High)
                            {
                                E.Cast(pred.CastPosition);
                                return;
                            }
                        }
                    }

                    if (MenuWrapper.JungleClear.Q.Enabled && Q.IsReady())
                    {
                        var mob = GameObjects.Jungle
                            .Where(x => x.IsValidTarget(Q.ChargedMaxRange) && x.GetJungleType() != JungleType.Unknown)
                            .OrderByDescending(x => x.MaxHealth).FirstOrDefault();

                        if (mob != null && mob.IsValidTarget(Q.ChargedMaxRange))
                        {
                            Q.StartCharging();
                        }
                    }
                }
            }
            else
            {
                // ignore mana check when q charge
                if (MenuWrapper.JungleClear.Q.Enabled && Q.IsReady() && Q.IsCharging)
                {
                    var mob = GameObjects.Jungle
                        .Where(x => x.IsValidTarget(Q.ChargedMaxRange) && x.GetJungleType() != JungleType.Unknown)
                        .OrderByDescending(x => x.MaxHealth).FirstOrDefault();

                    if (mob != null && mob.IsValidTarget(Q.Range))
                    {
                        var pred = Q.GetPrediction(mob);
                        if (pred.Hitchance >= HitChance.High)
                        {
                            Q.ShootChargedSpell(pred.CastPosition);
                        }
                    }
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.IsRecalling())
            {
                return;
            }

            if (MenuGUI.IsChatOpen || MenuGUI.IsShopOpen)
            {
                return;
            }

            // r range fix
            if (R.Level > 0)
            {
                R.Range = 2000 + 1200 * R.Level;
            }

            // do not move while casting ult
            if (ObjectManager.Player.HasBuff("XerathLocusOfPower2"))
            {
                Orbwalker.AttackState = false;
                Orbwalker.MovementState = false;
                Ultimate();
                return;
            }

            // when q charge, automatic movement
            if (Q.IsCharging && Orbwalker.ActiveMode != OrbwalkerMode.None)
            {
                Orbwalker.AttackState = false;
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            // fix orbwalker status
            Orbwalker.AttackState = true;
            Orbwalker.MovementState = true;

            SemiAutomatic();
            Killable();

            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
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

        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.IsRecalling())
            {
                return;
            }

            if (MenuGUI.IsChatOpen || MenuGUI.IsShopOpen)
            {
                return;
            }

            if (Q.IsCharging || ObjectManager.Player.HasBuff("XerathLocusOfPower2"))
            {
                return;
            }

            if (MenuWrapper.Misc.EAntiGapcloser.Enabled && E.IsReady() && args.EndPosition.DistanceToPlayer() < 250)
            {
                var pred = E.GetPrediction(sender);
                if (pred.Hitchance >= HitChance.High)
                {
                    E.Cast(pred.CastPosition);
                }
            }
        }

        private static void OnInterrupterSpell(AIHeroClient sender, Interrupter.InterruptSpellArgs args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.IsRecalling())
            {
                return;
            }

            if (MenuGUI.IsChatOpen || MenuGUI.IsShopOpen)
            {
                return;
            }

            if (Q.IsCharging || ObjectManager.Player.HasBuff("XerathLocusOfPower2"))
            {
                return;
            }

            if (MenuWrapper.Misc.EAntiGapcloser.Enabled && E.IsReady() && args.DangerLevel >= Interrupter.DangerLevel.Medium && sender.DistanceToPlayer() < E.Range)
            {
                var pred = E.GetPrediction(sender);
                if (pred.Hitchance >= HitChance.High)
                {
                    E.Cast(pred.CastPosition);
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.IsRecalling())
            {
                return;
            }

            if (MenuGUI.IsChatOpen || MenuGUI.IsShopOpen)
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

            if (MenuWrapper.Draw.W.Enabled)
            {
                if (MenuWrapper.Draw.OnlyReady.Enabled && W.IsReady())
                {
                    Render.Circle.DrawCircle(GameObjects.Player.Position, W.Range, Color.FromArgb(255, 159, 0), 1);
                }
                else if (!MenuWrapper.Draw.OnlyReady.Enabled)
                {
                    Render.Circle.DrawCircle(GameObjects.Player.Position, W.Range, Color.FromArgb(255, 159, 0), 1);
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

            if (MenuWrapper.Draw.R.Enabled)
            {
                if (MenuWrapper.Draw.OnlyReady.Enabled && R.IsReady())
                {
                    Render.Circle.DrawCircle(GameObjects.Player.Position, R.Range, Color.White, 1);
                }
                else if (!MenuWrapper.Draw.OnlyReady.Enabled)
                {
                    Render.Circle.DrawCircle(GameObjects.Player.Position, R.Range, Color.White, 1);
                }
            }

            if (MenuWrapper.Draw.RMouse.Enabled && MenuWrapper.Ult.NearMouse.Enabled && MenuWrapper.Ult.MouseZone.Value > 0)
            {
                if (MenuWrapper.Draw.OnlyReady.Enabled && R.IsReady())
                {
                    Render.Circle.DrawCircle(Game.CursorPos, MenuWrapper.Ult.MouseZone.Value, Color.White, 1);
                }
                else if (!MenuWrapper.Draw.OnlyReady.Enabled)
                {
                    Render.Circle.DrawCircle(Game.CursorPos, MenuWrapper.Ult.MouseZone.Value, Color.White, 1);
                }
            }
        }
    }
}
