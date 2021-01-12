namespace SPrediction
{
    using System.Collections.Generic;

    using EnsoulSharp;

    /// <summary>
    /// Collision Result structure
    /// </summary>
    public struct CollisionResult
    {
        public List<AIBaseClient> Units;
        public CollisionFlags Objects;

        public CollisionResult(List<AIBaseClient> _units, CollisionFlags _objects)
        {
            Units = _units;
            Objects = _objects;
        }
    }
}
