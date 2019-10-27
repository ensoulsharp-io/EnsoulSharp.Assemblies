using System;
using EnsoulSharp;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI.Values;

namespace BadaoActionsLimiter
{
    public static class Program
    {
        public static Menu Config;
        public static void Main(string[] args)
        {
            GameEvent.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad()
        {
            Config = new Menu("BadaoActionsLimiter", "BadaoActionsLimiter", true);
            Config.Attach();
            Config.Add(new MenuBool("DrawSpell", "Draw Spell Block"));
            Config.Add(new MenuBool("DrawAttack", "Draw Attack Block"));
            Config.Add(new MenuBool("DrawMove", "Draw Movement Block"));
            Config.Add(new MenuBool("CameraControl", "Camera To Out-Screen Cast Position"));
            SpellBlock.BadaoActivate();
            AttackBlock.BadaoActivate();
            MovementBlock.BadaoActivate();
            CameraControling.BadaoActivate();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.Print("Badao Actions Limiter Loaded !");
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Config["DrawSpell"].GetValue<MenuBool>().Enabled)
            {
                Drawing.DrawText(Drawing.Width - 180, 100, System.Drawing.Color.Lime, "Blocked " + SpellBlock.SpellBlockCount + " Spells");
            }
            if (Config["DrawAttack"].GetValue<MenuBool>().Enabled)
            {
                Drawing.DrawText(Drawing.Width - 180, 115, System.Drawing.Color.Lime, "Blocked " + AttackBlock.AttackBlockCount + " Attacks");
            }
            if (Config["DrawMove"].GetValue<MenuBool>().Enabled)
            {
                Drawing.DrawText(Drawing.Width - 180, 130, System.Drawing.Color.Lime, "Blocked " + MovementBlock.MovementBlockCount + " Movements");
            }
        }
    }
}
