using EnsoulSharp;
using EnsoulSharp.SDK;

namespace Evade
{
    static class Helpers
    {
        public static bool IsSpellShield(this AIHeroClient unit)
        {
            if (ObjectManager.Player.HasBuffOfType(BuffType.SpellShield))
            {
                return true;
            }

            if (ObjectManager.Player.HasBuffOfType(BuffType.SpellImmunity))
            {
                return true;
            }

            var lastCasted = unit.GetLastCastedSpell();

            if (lastCasted.IsValid)
            {
                // Sivir E
                if (lastCasted.Name == "SivirE" && lastCasted.Target?.NetworkId == unit.NetworkId && (Variables.TickCount - lastCasted.StartTime) < 300)
                {
                    return true;
                }

                // Morgana E
                if (lastCasted.Name == "MorganaE" && lastCasted.Target?.NetworkId == unit.NetworkId && (Variables.TickCount - lastCasted.StartTime) < 300)
                {
                    return true;
                }

                // Nocturne W
                if (lastCasted.Name == "NocturneShroudofDarkness" && lastCasted.Target?.NetworkId == unit.NetworkId && (Variables.TickCount - lastCasted.StartTime) < 300)
                {
                    return true;
                }
            }

            return false;
        }
    }
}