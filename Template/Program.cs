namespace Template
{
    // namespace
    using System;
    using System.Linq;

    using EnsoulSharp;
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;
    using EnsoulSharp.SDK.MenuUI.Values;
    using EnsoulSharp.SDK.Prediction;
    using EnsoulSharp.SDK.Utility;

    using Color = System.Drawing.Color;

    public class Program
    {
        private static Menu MainMenu;

        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;

        private static void Main(string[] args)
        {
            // start init script when game already load
            GameEvent.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad()
        {
            // judge champion Name
            if (ObjectManager.Player.CharacterName != "Template")
            {
                return;
            }

            // create spell
            Q = new Spell(SpellSlot.Q, 1000f); //skillshot
            Q.SetSkillshot(0.25f, 80f, float.MaxValue, false, SkillshotType.Line);

            W = new Spell(SpellSlot.W, 1000f); //charge spell
            W.SetCharged("spellName", "buffName", 100, 1000, 0.5f);
            W.SetSkillshot(0.25f, 80f, float.MaxValue, false, SkillshotType.Line);

            E = new Spell(SpellSlot.E, 1000f); //self cast

            R = new Spell(SpellSlot.R, 1000f); //target
            R.SetTargetted(0.25f, float.MaxValue); // this one you can ignore it if you dont need to pred something

            // create menu
            MainMenu = new Menu("Template", "Template Module", true);

            // combo menu
            var comboMenu = new Menu("Combo", "Combo Settings");
            comboMenu.Add(new MenuBool("comboQ", "Use Q", true));
            comboMenu.Add(new MenuBool("comboW", "Use W", true));
            comboMenu.Add(new MenuBool("comboE", "Use E", true));
            comboMenu.Add(new MenuBool("comboR", "Use R", true));
            MainMenu.Add(comboMenu);

            // draw menu 
            var drawMenu = new Menu("Draw", "Draw Settings");
            drawMenu.Add(new MenuBool("drawQ", "Draw Q Range", true));
            MainMenu.Add(comboMenu);

            //example boolean on MainMenu
            MainMenu.Add(new MenuBool("isDead", "if Player is Dead not Draw Range", true));

            // init MainMenu
            MainMenu.Attach();

            // events
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private static void Combo()
        {
            // cast q (is skillshot spell)
            // if menuitem enabled + Q ready
            if (MainMenu["Combo"]["comboQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                // get target
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                // or like this
                target = TargetSelector.GetTarget(Q.Range);
                // both work for get target

                // judge target is valid
                if (target != null && target.IsValidTarget(Q.Range))
                {
                    // get pred
                    var pred = Q.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.Medium)
                    {
                        // cast skillshot
                        Q.Cast(pred.CastPosition);
                    }
                }
            }

            // cast w (is charge spell)
            // if menuitem enabled + Q ready
            if (MainMenu["Combo"]["comboW"].GetValue<MenuBool>().Enabled && W.IsReady())
            {
                // get target
                var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                // or like this
                target = TargetSelector.GetTarget(W.Range);
                // both work for get target

                // judge target is valid
                if (target != null && target.IsValidTarget(W.ChargedMaxRange))
                {
                    // is charge
                    if (W.IsCharging)
                    {
                        // get pred
                        var pred = W.GetPrediction(target);
                        if (pred.Hitchance >= HitChance.Medium)
                        {
                            // cast charge spell
                            W.ShootChargedSpell(pred.CastPosition);
                        }
                    }
                    else // if not charge, start charge W
                    {
                        // you can make some logic in here
                        // for start charge judge
                        W.StartCharging();
                    }
                }
            }


            // cast e (is selfcast spell)
            // if menuitem enabled + E ready
            if (MainMenu["Combo"]["comboE"].GetValue<MenuBool>().Enabled && E.IsReady())
            {
                // this is enough if you only cast on self
                // like Ashe Q, Olaf W, ect
                // this all is self cast spell
                E.Cast();
            }

            // cast r (is target spell)
            // if menuitem enabled + Q ready
            if (MainMenu["Combo"]["comboR"].GetValue<MenuBool>().Enabled && R.IsReady())
            {
                // get target
                var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                // or like this
                target = TargetSelector.GetTarget(R.Range);
                // both work for get target

                // judge target is valid
                if (target != null && target.IsValidTarget(R.Range))
                {
                    // cast target spell
                    R.CastOnUnit(target);
                }
            }
        }

        private static void Clear()
        {
            // check out Ashe or Kalista
            // it already have example

            // get Minion
            var minions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Q.Range) && x.IsMinion());

            // get Mob
            var mobs = GameObjects.Jungle.Where(x => x.IsValidTarget(Q.Range));

            // get Legendary Mob (Dragon, Baron, ect)
            var lMobs = GameObjects.Jungle.Where(x => x.IsValidTarget(Q.Range) && x.GetJungleType() == JungleType.Legendary);

            // get Large Mob (Red Buff, Blue Buff, ect)
            var bMobs = GameObjects.Jungle.Where(x => x.IsValidTarget(Q.Range) && x.GetJungleType() == JungleType.Large);
        }

        private static void OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    break;
                case OrbwalkerMode.LaneClear:
                    Clear();
                    break;
            }
        }

        private static void OnDraw(EventArgs args)
        {
            // if Player is Dead not Draw Range
            if (MainMenu["isDead"].GetValue<MenuBool>().Enabled)
            {
                if (ObjectManager.Player.IsDead)
                {
                    return;
                }
            }

            // draw Q Range
            if (MainMenu["Draw"]["drawQ"].GetValue<MenuBool>().Enabled)
            {
                // Draw Circle
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Aqua);
            }
        }
    }
}
