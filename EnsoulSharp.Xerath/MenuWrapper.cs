namespace EnsoulSharp.Xerath
{
    using System.Windows.Forms;

    using EnsoulSharp.SDK.MenuUI.Values;

    internal class MenuWrapper
    {
        public class Ult
        {
            public static readonly MenuKeyBind Key = new MenuKeyBind("semir", "Semi R Key", Keys.T, KeyBindType.Press);
            public static readonly MenuBool Auto = new MenuBool("auto", "Auto R");
            public static readonly MenuBool NearMouse = new MenuBool("nearmouse", "Only R on Near Mouse Target");
            public static readonly MenuSlider MouseZone = new MenuSlider("mousezone", "Mouse Zone Range", 600, 0, 1200);
            public static readonly MenuBool DelayR = new MenuBool("delayr", "Delay R Cast", false);
            public static readonly MenuSlider DelayMs = new MenuSlider("delayms", "Cast Delay(ms)", 1500, 0, 3500);
        }

        public class Combat
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q");
            public static readonly MenuBool W = new MenuBool("w", "Use W");
            public static readonly MenuBool E = new MenuBool("e", "Use E");
            public static readonly MenuBool R = new MenuBool("r", "Use R");
        }

        public class Harass
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q");
            public static readonly MenuBool W = new MenuBool("w", "Use W");
            public static readonly MenuBool E = new MenuBool("e", "Use E", false);
            public static readonly MenuSlider Mana = new MenuSlider("minmana", "^ Min ManaPercent <= x%", 60);
        }

        public class LaneClear
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q");
            public static readonly MenuSlider QH = new MenuSlider("qh", "^ Min Hit Count >= x", 2, 1, 5);
            public static readonly MenuBool W = new MenuBool("w", "Use W");
            public static readonly MenuSlider WH = new MenuSlider("wh", "^ Min Hit Count >= x", 2, 1, 5);
            public static readonly MenuSlider Mana = new MenuSlider("minmana", "^ Min ManaPercent <= x%", 60);
        }

        public class JungleClear
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q");
            public static readonly MenuBool W = new MenuBool("w", "Use W");
            public static readonly MenuBool E = new MenuBool("e", "Use E");
            public static readonly MenuSlider Mana = new MenuSlider("minmana", "^ Min ManaPercent <= x%", 20);
        }

        public class KillAble
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q");
            public static readonly MenuBool W = new MenuBool("w", "Use W");
            public static readonly MenuBool E = new MenuBool("e", "Use E");
        }

        public class Misc
        {
            public static readonly MenuBool QSlow = new MenuBool("qslowcast", "Slow Q Cast(high hitchance)", false);
            public static readonly MenuBool RSlow = new MenuBool("rslowcast", "Slow R Cast(high hitchance)");
            public static readonly MenuBool EAntiGapcloser = new MenuBool("eantigapcloser", "Use E AntiGapcloser");
            public static readonly MenuBool EInterrupt = new MenuBool("einterrupt", "Use E Interrupt Spell");
        }

        public class Draw
        {
            public static readonly MenuBool Q = new MenuBool("q", "Draw Q Range");
            public static readonly MenuBool W = new MenuBool("w", "Draw W Range");
            public static readonly MenuBool E = new MenuBool("e", "Draw E Range");
            public static readonly MenuBool R = new MenuBool("r", "Draw R Range");
            public static readonly MenuBool RMouse = new MenuBool("rnm", "Draw R Near Mouse Range");
            public static readonly MenuBool OnlyReady = new MenuBool("od", "Only Spell Ready");
        }

        public class SemiKey
        {
            public static readonly MenuKeyBind W = new MenuKeyBind("semiw", "Semi W Key", Keys.W, KeyBindType.Press);
            public static readonly MenuKeyBind E = new MenuKeyBind("semie", "Semi E Key", Keys.E, KeyBindType.Press);
        }
    }
}
