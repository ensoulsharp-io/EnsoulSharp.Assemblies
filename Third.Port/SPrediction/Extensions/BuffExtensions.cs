namespace SPrediction
{
    using EnsoulSharp;

    /// <summary>
    /// Buff extensions for SPrediction
    /// </summary>
    public static class BuffExtensions
    {       
        /// <summary>
        /// Checks if the given bufftype is immobilizer 
        /// </summary>
        /// <param name="type">Buff type</param>
        /// <returns></returns>
        internal static bool IsImmobilizeBuff(this BuffType type)
        {
            return type == BuffType.Snare || type == BuffType.Stun || type == BuffType.Asleep || type == BuffType.Knockup || type == BuffType.Suppression;
        }
    }
}
