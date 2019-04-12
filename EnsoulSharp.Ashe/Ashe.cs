namespace EnsoulSharp.Ashe
{
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.Core.UI.IMenu;
    using EnsoulSharp.SDK.Core.Utils;
    using EnsoulSharp.SDK.Core.Wrappers.Damages;

    using System;
    using System.Linq;

    using Color = System.Drawing.Color;

    internal class Ashe
    {
        private static Menu MyMenu;
        private static Spell Q, W, R;

        public static void OnLoad()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1250f).SetSkillshot(0.25f, 20, 1500, true, SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R, 4500f).SetSkillshot(0.25f, 130, 1600, true, SkillshotType.SkillshotLine);

            MyMenu = new Menu("ensoulsharp.ashe", "EnsoulSharp.Ashe", true);

            var combat = new Menu("combat", "Combo Settings");
            combat.Add(MenuWrapper.Combat.W);
            combat.Add(MenuWrapper.Combat.WAfterAA);
            MyMenu.Add(combat);

            var harass = new Menu("harass", "Harass Settings");
            harass.Add(MenuWrapper.Harass.Q);
            harass.Add(MenuWrapper.Harass.W);
            harass.Add(MenuWrapper.Harass.Mana);
            MyMenu.Add(harass);

            var jungle = new Menu("jungle", "JungleClear Settings");
            jungle.Add(MenuWrapper.JungleClear.Q);
            jungle.Add(MenuWrapper.JungleClear.Mana);

            var killable = new Menu("killable", "KillSteal Settings");
            killable.Add(MenuWrapper.KillAble.W);
            MyMenu.Add(killable);

            var misc = new Menu("misc", "Misc Settings");
            misc.Add(MenuWrapper.Misc.RAntiGapcloser);
            misc.Add(MenuWrapper.Misc.RInterrupt);
            MyMenu.Add(misc);

            var draw = new Menu("draw", "Draw Settings");
            draw.Add(MenuWrapper.Draw.W);
            draw.Add(MenuWrapper.Draw.OnlyReady);
            MyMenu.Add(draw);

            MyMenu.Add(MenuWrapper.SemiR.Key);

            MyMenu.Attach();

            Game.OnUpdate += OnTick;
            Variables.Orbwalker.OnAction += OnOrbwalkerAction;
            Events.OnGapCloser += OnGapCloser;
            Events.OnInterruptableTarget += OnInterruptableTarget;
            Drawing.OnDraw += OnDraw;
        }

        private static void SemiR()
        {
            var target = Variables.TargetSelector.GetTarget(R.Range, DamageType.Magical, false);
            if (target != null && target.IsValidTarget())
            {
                var rPred = R.GetPrediction(target, false, 0, CollisionableObjects.Heroes);
                if (rPred.Hitchance >= HitChance.High)
                {
                    R.Cast(rPred.UnitPosition);
                }
            }
        }

        private static void KillAble()
        {
            if (MenuWrapper.KillAble.W.Value && W.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(W.Range - 100) && !x.IsInvulnerable))
                {
                    if (target.IsValidTarget() && target.Health < W.GetDamage(target))
                    {
                        var wPred = W.GetPrediction(target, false, 0, CollisionableObjects.Minions);
                        if (wPred.Hitchance >= HitChance.High)
                        {
                            W.Cast(wPred.UnitPosition);
                        }
                    }
                }
            }
        }

        private static void Combat()
        {
            if (MenuWrapper.Combat.W.Value && W.IsReady())
            {
                var target = Variables.TargetSelector.GetTarget(W.Range - 100);
                if (target != null && target.IsValidTarget(W.Range - 100) &&
                    target.DistanceToPlayer() > Player.Instance.GetRealAutoAttackRange(target))
                {
                    var wPred = W.GetPrediction(target, false, 0, CollisionableObjects.Minions);
                    if (wPred.Hitchance >= HitChance.High)
                    {
                        W.Cast(wPred.UnitPosition);
                    }
                }
            }
        }

        private static void Harass()
        {
            if (MenuWrapper.Harass.W.Value && W.IsReady())
            {
                var target = Variables.TargetSelector.GetTarget(W.Range - 100);
                if (target != null && target.IsValidTarget(W.Range - 100))
                {
                    var wPred = W.GetPrediction(target, false, 0, CollisionableObjects.Minions);
                    if (wPred.Hitchance >= HitChance.High)
                    {
                        W.Cast(wPred.UnitPosition);
                    }
                }
            }
        }

        private static void OnTick(EventArgs args)
        {
            if (Player.Instance.IsDead || Player.Instance.IsRecalling() || MenuGUI.IsChatOpen || Player.Instance.IsWindingUp)
            {
                return;
            }

            if (MenuWrapper.SemiR.Key.Active && R.IsReady())
            {
                SemiR();
            }

            KillAble();

            switch (Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    Combat();
                    break;
                case OrbwalkingMode.Hybrid:
                    Harass();
                    break;
            }
        }

        private static void OnOrbwalkerAction(object obj, OrbwalkingActionArgs args)
        {
            if (args.Type == OrbwalkingType.BeforeAttack)
            {
                if (Q.IsReady() && args.Target != null && args.Target.Type == GameObjectType.AIHeroClient)
                {
                    if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
                    {
                        Q.Cast();
                    }
                    else if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Hybrid)
                    {
                        if (MenuWrapper.Harass.Q.Value && Player.Instance.ManaPercent >= MenuWrapper.Harass.Mana.Value)
                        {
                            Q.Cast();
                        }
                    }
                }
            }
            else if (args.Type == OrbwalkingType.AfterAttack)
            {
                if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
                {
                    if (MenuWrapper.Combat.WAfterAA.Value && W.IsReady() && args.Target != null && args.Target.Type == GameObjectType.AIHeroClient)
                    {
                        var target = args.Target as AIHeroClient;
                        var wPred = W.GetPrediction(target);
                        if (wPred.Hitchance >= HitChance.High)
                        {
                            W.Cast(wPred.UnitPosition);
                        }
                    }
                }
                else if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.LaneClear)
                {
                    if (Player.Instance.ManaPercent >= MenuWrapper.JungleClear.Mana.Value && MenuWrapper.JungleClear.Q.Value && Q.IsReady())
                    {
                        if (args.Target != null && args.Target.Type == GameObjectType.AIMinionClient)
                        {
                            var mob = args.Target as AIMinionClient;
                            if (mob != null && mob.InAutoAttackRange() &&
                                (mob.GetJungleType() == JungleType.Large ||
                                 mob.GetJungleType() == JungleType.Legendary) &&
                                mob.Health > Player.Instance.GetAutoAttackDamage(mob) * 4)
                            {
                                Q.Cast();
                            }
                        }
                    }
                }
            }
        }

        private static void OnGapCloser(object obj, Events.GapCloserEventArgs args)
        {
            if (MenuWrapper.Misc.RAntiGapcloser.Value && R.IsReady() && args.IsDirectedToPlayer && args.Sender.DistanceToPlayer() < 250)
            {
                R.Cast(args.Sender.Position);
            }
        }

        private static void OnInterruptableTarget(object obj, Events.InterruptableTargetEventArgs args)
        {
            if (MenuWrapper.Misc.RInterrupt.Value && R.IsReady() && args.DangerLevel >= DangerLevel.Medium && args.Sender.DistanceToPlayer() < 1200)
            {
                R.Cast(args.Sender.Position);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Player.Instance.IsDead || MenuGUI.IsChatOpen)
            {
                return;
            }

            if (MenuWrapper.Draw.W.Value)
            {
                if (MenuWrapper.Draw.OnlyReady.Value && W.IsReady())
                {
                    Render.Circle.DrawCircle(GameObjects.Player.Position, W.Range, Color.FromArgb(255, 159, 0), 1);
                }
                else if (!MenuWrapper.Draw.OnlyReady.Value)
                {
                    Render.Circle.DrawCircle(GameObjects.Player.Position, W.Range, Color.FromArgb(255, 159, 0), 1);
                }
            }
        }
    }
}
