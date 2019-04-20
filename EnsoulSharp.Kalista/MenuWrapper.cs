namespace EnsoulSharp.Kalista
{
    using EnsoulSharp.SDK.MenuUI.Values;

    internal class MenuWrapper
    {
        public class Combat
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q");
            public static readonly MenuBool DisableQ = new MenuBool("noq", "^ Block on High Attack Speed");
            public static readonly MenuBool E = new MenuBool("e", "Use E Harass Target");
            public static readonly MenuBool DisableE = new MenuBool("noe", "^ Block on Debuff");
            public static readonly MenuBool DisableE2 = new MenuBool("noe2", "^ Limit usage");
            public static readonly MenuBool OrbwalkerMinion = new MenuBool("orb", "Auto Orbwalker Minion to Dash");
        }

        public class Harass
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q");
            public static readonly MenuBool QMinion = new MenuBool("qm", "^ Use On Minion");
            public static readonly MenuBool E = new MenuBool("e", "Use E Harass Target");
            public static readonly MenuBool DisableE = new MenuBool("noe", "^ Block on Debuff");
            public static readonly MenuBool DisableE2 = new MenuBool("noe2", "^ Limit usage");
            public static readonly MenuSlider Mana = new MenuSlider("minmana", "^ Min ManaPercent <= x%", 60);
        }

        public class LaneClear
        {
            public static readonly MenuSliderButton E = new MenuSliderButton("e", "Use E when KillAble Count >= x", 3, 1, 10);
            public static readonly MenuSlider Mana = new MenuSlider("minmana", "^ Min ManaPercent <= x%", 60);
        }

        public class JungleClear
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q");
            public static readonly MenuBool E = new MenuBool("e", "Use E");
            public static readonly MenuSlider Mana = new MenuSlider("minmana", "^ Min ManaPercent <= x%", 15);
        }

        public class KillAble
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q");
            public static readonly MenuBool E = new MenuBool("w", "Use E");
        }

        public class Draw
        {
            public static readonly MenuBool Q = new MenuBool("q", "Draw Q Range");
            public static readonly MenuBool E = new MenuBool("e", "Draw E Range");
            public static readonly MenuBool OnlyReady = new MenuBool("od", "Only Spell Ready");
            public static readonly MenuBool DMG = new MenuBool("dmg", "Draw E Damage");
        }
    }
}
