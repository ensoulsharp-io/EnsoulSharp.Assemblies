namespace SPrediction
{
    using System.Linq;

    using SharpDX;

    using EnsoulSharp;
    using EnsoulSharp.SDK;

    /// <summary>
    /// structure for aoe prediction result
    /// </summary>
    public struct SpellAoeResult
    {
        public Vector2 CastPosition;
        public CollisionResult CollisionResult;
        public int HitCount;

        public SpellAoeResult(Vector2 castpos, CollisionResult col, int hc)
        {
            CastPosition = castpos;
            CollisionResult = col;
            HitCount = hc;
        }

        internal PredictionOutput ToSDKResult()
        {
            return new PredictionOutput
            {
                Hitchance = HitChance.VeryHigh,
                UnitPosition = this.CastPosition.ToVector3(),
                CastPosition = this.CastPosition.ToVector3(),
                AoeTargetsHitCount = this.HitCount,
                AoeTargetsHit = this.CollisionResult.Units.Where(x => x is AIHeroClient).Cast<AIHeroClient>().ToList()
            };
        }
    }
}
