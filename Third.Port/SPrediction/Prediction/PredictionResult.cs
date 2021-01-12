namespace SPrediction
{
    using SharpDX;

    using EnsoulSharp;
    using EnsoulSharp.SDK;

    /// <summary>
    /// structure for prediction results
    /// </summary>
    public struct PredictionResult
    {
        public PredictionInput Input;
        public AIBaseClient Unit;
        public Vector2 CastPosition;
        public Vector2 UnitPosition;
        public HitChance HitChance;
        public CollisionResult CollisionResult;

        public PredictionResult(PredictionInput inp, AIBaseClient unit, Vector2 castpos, Vector2 unitpos, HitChance hc, CollisionResult col)
        {
            Input = inp;
            Unit = unit;
            CastPosition = castpos;
            UnitPosition = unitpos;
            HitChance = hc;
            CollisionResult = col;
        }

        internal void Lock(bool checkDodge = true)
        {
            this.CollisionResult = Collision.GetCollisions(this.Input.From.ToVector2(), this.CastPosition, this.Input.SpellRange, this.Input.SpellWidth, this.Input.SpellDelay, this.Input.SpellMissileSpeed);
            this.CheckCollisions();
            this.CheckOutofRange(checkDodge);
        }

        private void CheckCollisions()
        {
            if (this.Input.SpellCollisionable && (this.CollisionResult.Objects.HasFlag(CollisionFlags.Minions) ||
                                                  this.CollisionResult.Objects.HasFlag(CollisionFlags.YasuoWall)))
            {
                this.HitChance = HitChance.Collision;
            }
        }

        private void CheckOutofRange(bool checkDodge)
        {
            if (this.Input.RangeCheckFrom.ToVector2().Distance(this.CastPosition) > this.Input.SpellRange - (checkDodge ? PredictionExtensions.GetArrivalTime(this.Input.From.ToVector2().Distance(this.CastPosition), this.Input.SpellDelay, this.Input.SpellMissileSpeed) * this.Unit.MoveSpeed * (100 - Program.MaxRangeIgnore) / 100f : 0))
            {
                this.HitChance = HitChance.OutOfRange;
            }
        }

        internal PredictionOutput ToSDKResult()
        {
            return new PredictionOutput
            {
                Hitchance = this.HitChance,
                UnitPosition = this.UnitPosition.ToVector3(),
                CastPosition = this.CastPosition.ToVector3(),
                CollisionObjects = this.CollisionResult.Units
            };
        }
    }
}
