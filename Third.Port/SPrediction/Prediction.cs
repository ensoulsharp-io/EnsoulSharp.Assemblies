namespace SPrediction
{
    using System.Collections.Generic;

    using SharpDX;

    using EnsoulSharp;
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;

    public class Prediction : IPrediction
    {
        public Prediction()
        {
            Program.Menu = new Menu("SPrediction", "SPrediction", true)
            {
                new MenuBool("SPREDWINDUP", "Check for target AA Windup", false),
                new MenuSlider("SPREDMAXRANGEIGNORE", "Max Range Dodge Ignore (%)", 50),
                new MenuSlider("SPREDREACTIONDELAY", "Ignore Rection Delay", 0, 0, 200),
                new MenuSlider("SPREDDELAY", "Spell Delay", 0, 0, 200),
                new MenuBool("SPREDDRAWINGS", "Enable Drawings", false)
            }; 
            Program.Menu.Attach();

            PathTracker.Initialize();
            StasisPrediction.Initialize();
        }

        public PredictionOutput GetPrediction(EnsoulSharp.SDK.PredictionInput input)
        {
            if (input.Aoe && !input.Collision)
            {
                switch (input.Type)
                {
                    case SpellType.Line:
                        return LinePrediction.GetLineAoePrediction(input.Range, input.Delay, input.Speed, input.Range, input.From.ToVector2(), input.RangeCheckFrom.ToVector2()).ToSDKResult();
                    case SpellType.Circle:
                        return CirclePrediction.GetAoePrediction(input.Radius, input.Delay, input.Speed, input.Range, input.From.ToVector2(), input.RangeCheckFrom.ToVector2()).ToSDKResult();
                    case SpellType.Cone:
                        return ConePrediction.GetAoePrediction(input.Radius, input.Delay, input.Speed, input.Range, input.From.ToVector2(), input.RangeCheckFrom.ToVector2()).ToSDKResult();
                }

                return new PredictionOutput();
            }

            var inp = new PredictionInput(input.Unit, input.Delay, input.Speed, input.Radius, input.Range, input.Collision, input.Type, input.From, input.RangeCheckFrom);
            return this.GetPrediction(inp).ToSDKResult();
        }

        public PredictionOutput GetPrediction(EnsoulSharp.SDK.PredictionInput input, bool ft, bool checkCollision)
        {
            var inp = new PredictionInput(input.Unit, input.Delay, input.Speed, input.Radius, input.Range, checkCollision, input.Type, input.From, input.RangeCheckFrom);
            return this.GetPrediction(inp).ToSDKResult();
        }

        /// <summary>
        /// Gets Prediction result
        /// </summary>
        /// <param name="input">Neccesary inputs for prediction calculations</param>
        /// <returns>Prediction result as <see cref="PredictionResult"/></returns>
        public PredictionResult GetPrediction(PredictionInput input)
        {
            return this.GetPrediction(input.Target, input.SpellWidth, input.SpellDelay, input.SpellMissileSpeed, input.SpellRange, input.SpellCollisionable, input.SpellSpellType, input.Path, input.AvgReactionTime, input.LastMovChangeTime, input.AvgPathLenght, input.LastAngleDiff, input.From.ToVector2(), input.RangeCheckFrom.ToVector2());
        }

        /// <summary>
        /// Gets Prediction result
        /// </summary>
        /// <param name="target">Target for spell</param>
        /// <param name="width">Spell width</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="range">Spell range</param>
        /// <param name="collisionable">Spell collisionable</param>
        /// <param name="type">Spell skillshot type</param>
        /// <returns>Prediction result as <see cref="PredictionResult"/></returns>
        public PredictionResult GetPrediction(AIHeroClient target, float width, float delay, float missileSpeed, float range, bool collisionable, SpellType type)
        {
            return this.GetPrediction(target, width, delay, missileSpeed, range, collisionable, type, target.GetWaypoints(), target.AvgMovChangeTime(), target.LastMovChangeTime(), target.AvgPathLenght(), target.LastAngleDiff(), ObjectManager.Player.PreviousPosition.ToVector2(), ObjectManager.Player.PreviousPosition.ToVector2());
        }

        /// <summary>
        /// Gets Prediction result
        /// </summary>
        /// <param name="target">Target for spell</param>
        /// <param name="width">Spell width</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="range">Spell range</param>
        /// <param name="collisionable">Spell collisionable</param>
        /// <param name="type">Spell skillshot type</param>
        /// <param name="path">Waypoints of target</param>
        /// <param name="avgt">Average reaction time (in ms)</param>
        /// <param name="movt">Passed time from last movement change (in ms)</param>
        /// <param name="avgp">Average Path Lenght</param>
        /// <param name="from">Spell casted position</param>
        /// <param name="rangeCheckFrom"></param>
        /// <returns>Prediction result as <see cref="PredictionResult"/></returns>
        public PredictionResult GetPrediction(AIBaseClient target, float width, float delay, float missileSpeed, float range, bool collisionable, SpellType type, List<Vector2> path, float avgt, float movt, float avgp, float anglediff, Vector2 from, Vector2 rangeCheckFrom)
        {
            return PredictionExtensions.GetPrediction(target, width, delay, missileSpeed, range, collisionable, type, path, avgt, movt, avgp, anglediff, from, rangeCheckFrom);
        }

        /// <summary>
        /// Gets Prediction result while unit is dashing
        /// </summary>
        /// <param name="target">Target for spell</param>
        /// <param name="width">Spell width</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="range">Spell range</param>
        /// <param name="collisionable">Spell collisionable</param>
        /// <param name="type">Spell skillshot type</param>
        /// <param name="from">Spell casted position</param>
        /// <param name="rangeCheckFrom">Spell range check from position</param>
        /// <returns></returns>
        public PredictionResult GetDashingPrediction(AIBaseClient target, float width, float delay, float missileSpeed, float range, bool collisionable, SpellType type, Vector2 from, Vector2 rangeCheckFrom)
        {
            return PredictionExtensions.GetDashingPrediction(target, width, delay, missileSpeed, range, collisionable, type, from, rangeCheckFrom);
        }

        /// <summary>
        /// Gets Prediction result while unit is immobile
        /// </summary>
        /// <param name="target">Target for spell</param>
        /// <param name="width">Spell width</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="range">Spell range</param>
        /// <param name="collisionable">Spell collisionable</param>
        /// <param name="type">Spell skillshot type</param>
        /// <param name="from">Spell casted position</param>
        /// <returns></returns>
        public PredictionResult GetImmobilePrediction(AIBaseClient target, float width, float delay, float missileSpeed, float range, bool collisionable, SpellType type, Vector2 from, Vector2 rangeCheckFrom)
        {
            return PredictionExtensions.GetImmobilePrediction(target, width, delay, missileSpeed, range, collisionable, type, from, rangeCheckFrom);
        }

        /// <summary>
        /// Gets fast-predicted unit position
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="from">Spell casted position</param>
        /// <returns></returns>
        public Vector2 GetFastUnitPosition(AIBaseClient target, float delay, float missileSpeed = 0, Vector2? from = null, float distanceSet = 0)
        {
            return PredictionExtensions.GetFastUnitPosition(target, delay, missileSpeed, from, distanceSet);
        }

        /// <summary>
        /// Gets fast-predicted unit position
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="path">Path</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="from">Spell casted position</param>
        /// <param name="moveSpeed">Move speed</param>
        /// <param name="distanceSet"></param>
        /// <returns></returns>
        public Vector2 GetFastUnitPosition(AIBaseClient target, List<Vector2> path, float delay, float missileSpeed = 0, Vector2? from = null, float moveSpeed = 0, float distanceSet = 0)
        {
            return PredictionExtensions.GetFastUnitPosition(target, path, delay, missileSpeed, from, moveSpeed, distanceSet);
        }
    }
}
