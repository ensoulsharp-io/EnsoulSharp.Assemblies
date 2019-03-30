using System.Collections.Generic;
using EnsoulSharp;

namespace Evade
{
    public static class SpellBlocker
    {
        public static List<SpellSlot> Whitelisted_SpellSlots;

        static SpellBlocker()
        {
            switch (ObjectManager.Player.CharacterName.ToLower())
            {
                case "aatrox":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.E, SpellSlot.R };
                    break;

                case "ahri":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.R };
                    break;

                case "akali":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "alistar":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "amumu":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.R };
                    break;

                case "anivia":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.R };
                    break;

                case "annie":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E };
                    break;

                case "aurelionsol":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.E };
                    break;

                case "azir":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.E };
                    break;

                case "blitzcrank":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E };
                    break;

                case "braum":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E };
                    break;

                case "caitlyn":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.E };
                    break;

                case "camille":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "chogath":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.E };
                    break;

                case "corki":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E };
                    break;

                case "darius":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.R };
                    break;

                case "diana":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.R };
                    break;

                case "draven":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W };
                    break;

                case "drmundo":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "ekko":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.E, SpellSlot.R };
                    break;

                case "elise":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.E, SpellSlot.R };
                    break;

                case "ezreal":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.E };
                    break;

                case "fiora":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "fizz":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.E };
                    break;

                case "galio":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E };
                    break;

                case "garen":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.E };
                    break;

                case "gnar":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.E };
                    break;

                case "gragas":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E };
                    break;

                case "graves":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.E };
                    break;

                case "hecarim":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "heimerdinger":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.R };
                    break;

                case "illaoi":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W };
                    break;

                case "irelia":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.E };
                    break;

                case "ivern":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.E };
                    break;

                case "janna":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.E };
                    break;

                case "jarvaniv":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "jax":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "jayce":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "jinx":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.E };
                    break;

                case "kaisa":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.E, SpellSlot.R };
                    break;

                case "kalista":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.R };
                    break;

                case "karma":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.E, SpellSlot.R };
                    break;

                case "karthus":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.E };
                    break;

                case "kassadin":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.R };
                    break;

                case "katarina":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E };
                    break;

                case "kayle":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "kayn":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.E, SpellSlot.R };
                    break;

                case "kennen":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.E };
                    break;

                case "khazix":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.E, SpellSlot.R };
                    break;

                case "kindred":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.R };
                    break;

                case "kled":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.E, SpellSlot.R };
                    break;

                case "leblanc":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.R };
                    break;

                case "leesin":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W };
                    break;

                case "leona":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W };
                    break;

                case "lissandra":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "lucian":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.E, SpellSlot.R };
                    break;

                case "lulu":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "malphite":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.R };
                    break;

                case "malzahar":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W };
                    break;

                case "maokai":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W };
                    break;

                case "masteryi":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.E, SpellSlot.R };
                    break;

                case "missfortune":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W };
                    break;

                case "monkeyking":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "mordekaiser":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W };
                    break;

                case "morgana":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.E, SpellSlot.R };
                    break;

                case "nami":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E };
                    break;

                case "nasus":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.R };
                    break;

                case "nautilus":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W };
                    break;

                case "neeko":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.R };
                    break;

                case "nidalee":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.R };
                    break;

                case "nocturne":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "nunu":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E };
                    break;

                case "olaf":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.R };
                    break;

                case "orianna":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.E };
                    break;

                case "ornn":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.R };
                    break;

                case "pantheon":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W };
                    break;

                case "poppy":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E };
                    break;

                case "pyke":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "quinn":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W };
                    break;

                case "rakan":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "rammus":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.R };
                    break;

                case "reksai":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.R };
                    break;

                case "renekton":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "rengar":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.R };
                    break;

                case "riven":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.E };
                    break;

                case "rumble":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W };
                    break;

                case "ryze":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.R };
                    break;

                case "sejuani":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W };
                    break;

                case "shaco":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.R };
                    break;

                case "shen":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "shyvana":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.R };
                    break;

                case "singed":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.R };
                    break;

                case "sion":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.R };
                    break;

                case "sivir":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.E, SpellSlot.R };
                    break;

                case "skarner":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W };
                    break;

                case "sona":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "soraka":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.R };
                    break;

                case "swain":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.R };
                    break;

                case "sylas":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E };
                    break;

                case "syndra":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.R };
                    break;

                case "talon":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.E, SpellSlot.R };
                    break;

                case "taric":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "teemo":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W };
                    break;

                case "thresh":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W };
                    break;

                case "tristana":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W };
                    break;

                case "trundle":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W };
                    break;

                case "tryndamere":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.E, SpellSlot.R };
                    break;

                case "twistedfate":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W };
                    break;

                case "twitch":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q };
                    break;

                case "udyr":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "urgot":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E };
                    break;

                case "varus":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W };
                    break;

                case "vayne":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.R };
                    break;

                case "velkoz":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W };
                    break;

                case "vi":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.E, SpellSlot.R };
                    break;

                case "viktor":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.E };
                    break;

                case "vladimir":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "volibear":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.E };
                    break;

                case "warwick":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.E, SpellSlot.R };
                    break;

                case "xayah":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.E, SpellSlot.R };
                    break;

                case "xerath":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q };
                    break;

                case "xinzhao":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.E, SpellSlot.R };
                    break;

                case "yasuo":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "yorick":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W };
                    break;

                case "zac":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.R };
                    break;

                case "zed":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "zilean":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;

                case "zyra":
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.W };
                    break;

                default:
                    Whitelisted_SpellSlots = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
                    break;
            }
        }

        public static bool ShouldBlock(SpellSlot spellToCast)
        {
            if (spellToCast == SpellSlot.Summoner1 || spellToCast == SpellSlot.Summoner2)
            {
                return false;
            }

            if (Whitelisted_SpellSlots.Contains(spellToCast))
            {
                return false;
            }

            if (spellToCast == SpellSlot.Q || spellToCast == SpellSlot.W || spellToCast == SpellSlot.E || spellToCast == SpellSlot.R)
            {
                return true;
            }

            return false;
        }
    }
}