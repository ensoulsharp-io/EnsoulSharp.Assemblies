using System.Collections.Generic;
using System.Linq;

using EnsoulSharp;
using EnsoulSharp.SDK;

using SharpDX;

namespace SebbyLib
{
    public class Cache
    {
        public static List<AIBaseClient> GetMinions(Vector3 from, float range = float.MaxValue, MinionTeam team = MinionTeam.Enemy)
        {
            if (team == MinionTeam.Enemy)
            {
                return GameObjects.EnemyMinions.Where(minion => CanReturn(minion, from, range)).Select(minion => minion as AIBaseClient).ToList();
            }
            else if (team == MinionTeam.Ally)
            {
                return GameObjects.AllyMinions.Where(minion => CanReturn(minion, from, range)).Select(minion => minion as AIBaseClient).ToList();
            }
            else if (team == MinionTeam.Neutral)
            {
                return GameObjects.Jungle.Where(minion => CanReturn(minion, from, range)).OrderByDescending(minion => minion.MaxHealth).Select(minion => minion as AIBaseClient).ToList();
            }
            else
            {
                return GameObjects.Minions.Where(minion => CanReturn(minion, from, range)).Select(minion => minion as AIBaseClient).ToList();
            }
        }

        private static bool CanReturn(AIBaseClient minion, Vector3 from, float range)
        {
            return minion.IsValidTarget(range == 0 ? ObjectManager.Player.GetRealAutoAttackRange(minion) : range, false, from);
        }
    }

    public enum MinionTeam
    {
        Neutral,
        Ally,
        Enemy,
        NotAlly,
        NotAllyForEnemy,
        All
    }
}
