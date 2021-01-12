namespace SPrediction
{
    using SharpDX;

    /// <summary>
    /// structure for aoe Vector prediction results
    /// </summary>
    public struct VectorAoeResult
    {
        public Vector2 CastTargetPosition;
        public Vector2 CastSourcePosition;
        public CollisionResult CollisionResult;
        public int HitCount;
    }
}
