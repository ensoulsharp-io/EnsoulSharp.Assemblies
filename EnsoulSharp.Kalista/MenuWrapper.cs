namespace EnsoulSharp.Kalista
{
    using EnsoulSharp.SDK.Core.UI.IMenu.Values;

    internal class MenuWrapper
    {
        public class Combat
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q", true);
            public static readonly MenuBool DisableQ = new MenuBool("noq", "^ Block on High Attack Speed", true);
            public static readonly MenuBool E = new MenuBool("e", "Use E Harass Target", true);
            public static readonly MenuBool DisableE = new MenuBool("noe", "^ Block on Debuff", true);
            public static readonly MenuBool DisableE2 = new MenuBool("noe2", "^ Limit usage", true);
            public static readonly MenuBool OrbwalkerMinion = new MenuBool("orb", "Auto Orbwalker Minion to Dash", true);
        }

        public class Harass
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q", true);
            public static readonly MenuBool QMinion = new MenuBool("qm", "^ Use On Minion", true);
            public static readonly MenuBool E = new MenuBool("e", "Use E Harass Target", true);
            public static readonly MenuBool DisableE = new MenuBool("noe", "^ Block on Debuff", true);
            public static readonly MenuBool DisableE2 = new MenuBool("noe2", "^ Limit usage", true);
            public static readonly MenuSlider Mana = new MenuSlider("minmana", "^ Min ManaPercent <= x%", 60);
        }

        public class LaneClear
        {
            public static readonly MenuSliderButton E = new MenuSliderButton("e", "Use E when KillAble Count >= x", 3, 1, 10, true);
            public static readonly MenuSlider Mana = new MenuSlider("minmana", "^ Min ManaPercent <= x%", 60);
        }

        public class JungleClear
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q", true);
            public static readonly MenuBool E = new MenuBool("e", "Use E", true);
            public static readonly MenuSlider Mana = new MenuSlider("minmana", "^ Min ManaPercent <= x%", 15);
        }

        public class KillAble
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q", true);
            public static readonly MenuBool E = new MenuBool("w", "Use E", true);
        }

        public class Misc
        {
            public static readonly MenuBool R = new MenuBool("r", "Auto Save your SweetHeart", true);
            public static readonly MenuSlider HP = new MenuSlider("minhp", "^ Min HealthPercent <= x%", 60);
        }

        public class Draw
        {
            public static readonly MenuBool Q = new MenuBool("q", "Draw Q Range", true);
            public static readonly MenuBool E = new MenuBool("e", "Draw E Range", true);
            public static readonly MenuBool OnlyReady = new MenuBool("od", "Only Spell Ready", true);
            public static readonly MenuBool DMG = new MenuBool("dmg", "Draw E Damage", true);
        }
    }
}
