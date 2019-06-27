namespace EnsoulSharp.Ashe
{
    using System;
    using System.Linq;

    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;
    using EnsoulSharp.SDK.Prediction;
    using EnsoulSharp.SDK.Utility;

    using Color = System.Drawing.Color;

    internal class Ashe
    {
        private static Menu MyMenu;
        private static Spell Q, W, R;

        public static void OnLoad()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1250f);
            R = new Spell(SpellSlot.R, 4500f);

            W.SetSkillshot(0.25f, 20, 1500, true, SkillshotType.Line);
            R.SetSkillshot(0.25f, 130, 1600, true, SkillshotType.Line);

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
            MyMenu.Add(jungle);

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
            Orbwalker.OnAction += OnOrbwalkerAction;
            Gapcloser.OnGapcloser += OnGapcloser;
            Interrupter.OnInterrupterSpell += OnInterrupterSpell;
            Drawing.OnDraw += OnDraw;
        }

        private static void SemiR()
        {
            var target = TargetSelector.GetTarget(R.Range);
            if (target != null && target.IsValidTarget())
            {
                var rPred = R.GetPrediction(target, false, 0, CollisionObjects.Heroes);
                if (rPred.Hitchance >= HitChance.High)
                {
                    R.Cast(rPred.CastPosition);
                }
            }
        }

        private static void KillAble()
        {
            if (MenuWrapper.KillAble.W.Enabled && W.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(W.Range - 100) && !x.IsInvulnerable))
                {
                    if (target.IsValidTarget() && target.Health < W.GetDamage(target))
                    {
                        var wPred = W.GetPrediction(target, false, 0, CollisionObjects.Minions);
                        if (wPred.Hitchance >= HitChance.High)
                        {
                            W.Cast(wPred.CastPosition);
                        }
                    }
                }
            }
        }

        private static void Combat()
        {
            if (MenuWrapper.Combat.W.Enabled && W.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range - 100);
                if (target != null && target.IsValidTarget(W.Range - 100) &&
                    target.DistanceToPlayer() > ObjectManager.Player.GetRealAutoAttackRange(target))
                {
                    var wPred = W.GetPrediction(target, false, 0, CollisionObjects.Minions);
                    if (wPred.Hitchance >= HitChance.High)
                    {
                        W.Cast(wPred.CastPosition);
                    }
                }
            }
        }

        private static void Harass()
        {
            if (MenuWrapper.Harass.W.Enabled && W.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range - 100);
                if (target != null && target.IsValidTarget(W.Range - 100))
                {
                    var wPred = W.GetPrediction(target, false, 0, CollisionObjects.Minions);
                    if (wPred.Hitchance >= HitChance.High)
                    {
                        W.Cast(wPred.CastPosition);
                    }
                }
            }
        }

        private static void OnTick(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.IsRecalling() || MenuGUI.IsChatOpen || ObjectManager.Player.IsWindingUp)
            {
                return;
            }

            if (MenuWrapper.SemiR.Key.Active && R.IsReady())
            {
                SemiR();
            }

            KillAble();

            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combat();
                    break;
                case OrbwalkerMode.Harass:
                    Harass();
                    break;
            }
        }

        private static void OnOrbwalkerAction(object obj, OrbwalkerActionArgs args)
        {
            if (args.Type == OrbwalkerType.BeforeAttack)
            {
                if (Q.IsReady() && args.Target != null && args.Target.Type == GameObjectType.AIHeroClient)
                {
                    if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                    {
                        Q.Cast();
                    }
                    else if (Orbwalker.ActiveMode == OrbwalkerMode.Harass)
                    {
                        if (MenuWrapper.Harass.Q.Enabled && ObjectManager.Player.ManaPercent >= MenuWrapper.Harass.Mana.Value)
                        {
                            Q.Cast();
                        }
                    }
                }
            }
            else if (args.Type == OrbwalkerType.AfterAttack)
            {
                if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                {
                    if (MenuWrapper.Combat.WAfterAA.Enabled && W.IsReady() && args.Target != null && args.Target.Type == GameObjectType.AIHeroClient)
                    {
                        var target = args.Target as AIHeroClient;
                        var wPred = W.GetPrediction(target);
                        if (wPred.Hitchance >= HitChance.High)
                        {
                            W.Cast(wPred.UnitPosition);
                        }
                    }
                }
                else if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
                {
                    if (ObjectManager.Player.ManaPercent >= MenuWrapper.JungleClear.Mana.Value && MenuWrapper.JungleClear.Q.Enabled && Q.IsReady())
                    {
                        if (args.Target != null && args.Target.Type == GameObjectType.AIMinionClient)
                        {
                            var mob = args.Target as AIMinionClient;
                            if (mob != null && mob.InAutoAttackRange() &&
                                (mob.GetJungleType() == JungleType.Large ||
                                 mob.GetJungleType() == JungleType.Legendary) &&
                                mob.Health > ObjectManager.Player.GetAutoAttackDamage(mob) * 4)
                            {
                                Q.Cast();
                            }
                        }
                    }
                }
            }
        }

        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {
            if (MenuWrapper.Misc.RAntiGapcloser.Enabled && R.IsReady() && args.EndPosition.DistanceToPlayer() < 250)
            {
                R.Cast(sender.Position);
            }
        }

        private static void OnInterrupterSpell(AIHeroClient target, Interrupter.InterruptSpellArgs args)
        {
            if (MenuWrapper.Misc.RInterrupt.Enabled && R.IsReady() && args.DangerLevel >= Interrupter.DangerLevel.Medium && target.DistanceToPlayer() < 1200)
            {
                R.Cast(target.Position);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || MenuGUI.IsChatOpen)
            {
                return;
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
        }
    }
}
