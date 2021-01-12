namespace SPrediction
{
    using SharpDX;

    using EnsoulSharp.SDK;

    /// <summary>
    /// structure for Vector prediction results
    /// </summary>
    public struct VectorResult
    {
        public Vector2 CastTargetPosition;
        public Vector2 CastSourcePosition;
        public Vector2 UnitPosition;
        public HitChance HitChance;
        public CollisionResult CollisionResult;
    }
}
