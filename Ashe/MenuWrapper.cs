namespace EnsoulSharp.Ashe
{
    using System.Windows.Forms;

    using EnsoulSharp.SDK.MenuUI.Values;

    internal class MenuWrapper
    {
        public class Combat
        {
            public static readonly MenuBool W = new MenuBool("w", "Use W");
            public static readonly MenuBool WAfterAA = new MenuBool("wa", "^ On AfterAttack");
        }

        public class Harass
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q");
            public static readonly MenuBool W = new MenuBool("w", "Use W");
            public static readonly MenuSlider Mana = new MenuSlider("minmana", "^ Min ManaPercent <= x%", 60);
        }

        public class JungleClear
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q");
            public static readonly MenuSlider Mana = new MenuSlider("minmana", "^ Min ManaPercent <= x%", 60);
        }

        public class KillAble
        {
            public static readonly MenuBool W = new MenuBool("w", "Use W");
        }

        public class Misc
        {
            public static readonly MenuBool RAntiGapcloser = new MenuBool("rantigapcloser", "Use R AntiGapcloser");
            public static readonly MenuBool RInterrupt = new MenuBool("rinterrupt", "Use R Interrupt Spell");
        }

        public class Draw
        {
            public static readonly MenuBool W = new MenuBool("w", "Draw W Range");
            public static readonly MenuBool OnlyReady = new MenuBool("od", "Only Spell Ready");
        }

        public class SemiR
        {
            public static readonly MenuKeyBind Key = new MenuKeyBind("semir", "Semi R Key", Keys.T, KeyBindType.Press);
        }
    }
}
