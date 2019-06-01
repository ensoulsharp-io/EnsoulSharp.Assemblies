using System.Collections.Generic;
using System.Linq;

using EnsoulSharp;
using EnsoulSharp.SDK;

using SharpDX;

namespace SPrediction
{
    public class MinionManager
    {
        /// <summary>
        ///     An enum representing the order the minions should be listed.
        /// </summary>
        public enum MinionOrderTypes
        {
            /// <summary>
            ///     No order.
            /// </summary>
            None,

            /// <summary>
            ///     Ordered by the current health of the minion. (Least to greatest)
            /// </summary>
            Health,

            /// <summary>
            ///     Ordered by the maximum health of the minions. (Greatest to least)
            /// </summary>
            MaxHealth
        }

        /// <summary>
        ///     The team of the minion.
        /// </summary>
        public enum MinionTeam
        {
            /// <summary>
            ///     The minion is not on either team.
            /// </summary>
            Neutral,

            /// <summary>
            ///     The minions is an ally
            /// </summary>
            Ally,

            /// <summary>
            ///     The minions is an enemy
            /// </summary>
            Enemy,

            /// <summary>
            ///     The minion is not an ally
            /// </summary>
            NotAlly,

            /// <summary>
            ///     The minions is not an ally for the enemy
            /// </summary>
            NotAllyForEnemy,

            /// <summary>
            ///     Any minion.
            /// </summary>
            All
        }

        /// <summary>
        ///     The type of minion.
        /// </summary>
        public enum MinionTypes
        {
            /// <summary>
            ///     Ranged minions.
            /// </summary>
            Ranged,

            /// <summary>
            ///     Melee minions.
            /// </summary>
            Melee,

            /// <summary>
            ///     Any minion
            /// </summary>
            All
        }

        /// <summary>
        /// Determines whether the specified object is a minion.
        /// </summary>
        /// <param name="minion">The minion.</param>
        /// <returns><c>true</c> if the specified minion is minion; otherwise, <c>false</c>.</returns>
        public static bool IsMinion(AIMinionClient minion)
        {
            return minion.CharacterName.Contains("Minion");
        }

        /// <summary>
        /// Gets the minions.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="type">The type.</param>
        /// <param name="team">The team.</param>
        /// <param name="order">The order.</param>
        /// <returns>List&lt;AIBaseClient&gt;.</returns>
        public static List<AIBaseClient> GetMinions(
            float range,
            MinionTypes type = MinionTypes.All,
            MinionTeam team = MinionTeam.Enemy,
            MinionOrderTypes order = MinionOrderTypes.Health)
        {
            return GetMinions(ObjectManager.Player.PreviousPosition, range, type, team, order);
        }

        /// <summary>
        /// Gets minions based on range, type, team and then orders them.
        /// </summary>
        /// <param name="from">The point to get the minions from.</param>
        /// <param name="range">The range.</param>
        /// <param name="type">The type.</param>
        /// <param name="team">The team.</param>
        /// <param name="order">The order.</param>
        /// <returns>List&lt;AIBaseClient&gt;.</returns>
        public static List<AIBaseClient> GetMinions(
            Vector3 from,
            float range,
            MinionTypes type = MinionTypes.All,
            MinionTeam team = MinionTeam.Enemy,
            MinionOrderTypes order = MinionOrderTypes.Health)
        {
            var result = (from minion in ObjectManager.Get<AIMinionClient>()
                          where minion.IsValidTarget(range, false, @from)
                          let minionTeam = minion.Team
                          where
                              team == MinionTeam.Neutral && minionTeam == GameObjectTeam.Neutral
                              || team == MinionTeam.Ally
                              && minionTeam
                              == (ObjectManager.Player.Team == GameObjectTeam.Chaos
                                      ? GameObjectTeam.Chaos
                                      : GameObjectTeam.Order)
                              || team == MinionTeam.Enemy
                              && minionTeam
                              == (ObjectManager.Player.Team == GameObjectTeam.Chaos
                                      ? GameObjectTeam.Order
                                      : GameObjectTeam.Chaos)
                              || team == MinionTeam.NotAlly && minionTeam != ObjectManager.Player.Team
                              || team == MinionTeam.NotAllyForEnemy
                              && (minionTeam == ObjectManager.Player.Team || minionTeam == GameObjectTeam.Neutral)
                              || team == MinionTeam.All
                          where
                              minion.IsMelee() && type == MinionTypes.Melee
                              || !minion.IsMelee() && type == MinionTypes.Ranged || type == MinionTypes.All
                          where
                              IsMinion(minion)
                              || minionTeam == GameObjectTeam.Neutral && minion.MaxHealth > 5 && minion.IsHPBarRendered
                          select minion).Cast<AIBaseClient>().ToList();

            switch (order)
            {
                case MinionOrderTypes.Health:
                    result = result.OrderBy(o => o.Health).ToList();
                    break;
                case MinionOrderTypes.MaxHealth:
                    result = result.OrderByDescending(o => o.MaxHealth).ToList();
                    break;
            }

            return result;
        }
    }
}
