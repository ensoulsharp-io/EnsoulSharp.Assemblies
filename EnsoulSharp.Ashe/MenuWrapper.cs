namespace EnsoulSharp.Ashe
{
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.Core.UI.IMenu.Values;

    using System.Windows.Forms;

    internal class MenuWrapper
    {
        public class Combat
        {
            public static readonly MenuBool W = new MenuBool("w", "Use W", true);
            public static readonly MenuBool WAfterAA = new MenuBool("wa", "^ On AfterAttack", true);
        }

        public class Harass
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q", true);
            public static readonly MenuBool W = new MenuBool("w", "Use W", true);
            public static readonly MenuSlider Mana = new MenuSlider("minmana", "^ Min ManaPercent <= x%", 60);
        }

        public class JungleClear
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q", true);
            public static readonly MenuSlider Mana = new MenuSlider("minmana", "^ Min ManaPercent <= x%", 60);
        }

        public class KillAble
        {
            public static readonly MenuBool W = new MenuBool("w", "Use W", true);
        }

        public class Misc
        {
            public static readonly MenuBool RAntiGapcloser = new MenuBool("rantigapcloser", "Use R AntiGapcloser", true);
            public static readonly MenuBool RInterrupt = new MenuBool("rinterrupt", "Use R Interrupt Spell", true);
        }

        public class Draw
        {
            public static readonly MenuBool W = new MenuBool("w", "Draw W Range", true);
            public static readonly MenuBool OnlyReady = new MenuBool("od", "Only Spell Ready", true);
        }

        public class SemiR
        {
            public static readonly MenuKeyBind Key = new MenuKeyBind("semir", "Semi R Key", Keys.T, KeyBindType.Press);
        }
    }
}
