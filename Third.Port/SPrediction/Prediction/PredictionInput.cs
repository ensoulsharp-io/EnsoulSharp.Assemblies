namespace SPrediction
{
    using System.Collections.Generic;

    using SharpDX;

    using EnsoulSharp;
    using EnsoulSharp.SDK;

    /// <summary>
    /// Neccesary input structure for prediction calculations
    /// </summary>
    public struct PredictionInput
    {
        public AIBaseClient Target;
        public float SpellDelay;
        public float SpellMissileSpeed;
        public float SpellWidth;
        public float SpellRange;
        public bool SpellCollisionable;
        public SpellType SpellSpellType;
        public List<Vector2> Path;
        public float AvgReactionTime;
        public float LastMovChangeTime;
        public float AvgPathLenght;
        public float LastAngleDiff;
        public Vector3 From;
        public Vector3 RangeCheckFrom;

        public PredictionInput(AIBaseClient _target, Spell s)
        {
            Target = _target;
            SpellDelay = s.Delay;
            SpellMissileSpeed = s.Speed;
            SpellWidth = s.Width;
            SpellRange = s.Range;
            SpellCollisionable = s.Collision;
            SpellSpellType = s.Type;
            Path = Target.GetWaypoints();
            if (Target is AIHeroClient t)
            {
                AvgReactionTime = t.AvgMovChangeTime();
                LastMovChangeTime = t.LastMovChangeTime();
                AvgPathLenght = t.AvgPathLenght();
                LastAngleDiff = t.LastAngleDiff();
            }
            else
            {
                AvgReactionTime = 0;
                LastMovChangeTime = 0;
                AvgPathLenght = 0;
                LastAngleDiff = 0;
            }
            From = s.From;
            RangeCheckFrom = s.RangeCheckFrom;
        }

        public PredictionInput(AIBaseClient _target, float delay, float speed, float radius, float range, bool collision, SpellType type, Vector3 _from, Vector3 _rangeCheckFrom)
        {
            Target = _target;
            SpellDelay = delay;
            SpellMissileSpeed = speed;
            SpellWidth = radius;
            SpellRange = range;
            SpellCollisionable = collision;
            SpellSpellType = type;
            Path = Target.GetWaypoints();
            if (Target is AIHeroClient t)
            {
                AvgReactionTime = t.AvgMovChangeTime();
                LastMovChangeTime = t.LastMovChangeTime();
                AvgPathLenght = t.AvgPathLenght();
                LastAngleDiff = t.LastAngleDiff();
            }
            else
            {
                AvgReactionTime = 0;
                LastMovChangeTime = 0;
                AvgPathLenght = 0;
                LastAngleDiff = 0;
            }
            From = _from;
            RangeCheckFrom = _rangeCheckFrom;
        }
    }
}
