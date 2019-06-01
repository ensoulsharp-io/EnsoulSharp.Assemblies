/*
 Copyright 2015 - 2015 SPrediction
 SpellExtensions.cs is part of SPrediction
 
 SPrediction is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.
 
 SPrediction is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.
 
 You should have received a copy of the GNU General Public License
 along with SPrediction. If not, see <http://www.gnu.org/licenses/>.
*/

using System;

using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Prediction;

using SharpDX;

namespace SPrediction
{
    /// <summary>
    /// Spell extensions for SPrediction
    /// </summary>
    public static class SpellExtensions
    {
        #region Prediction methods
        /// <summary>
        /// Gets Prediction result
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns>Prediction result as <see cref="Prediction.Result"/></returns>
        public static Prediction.Result GetSPrediction(this Spell s, AIHeroClient target)
        {
            #region if common prediction selected
            if (ConfigMenu.SelectedPrediction.Index == 1)
            {
                var pred = s.GetPrediction(target);
                var result = new Prediction.Result(new Prediction.Input(target, s), target, pred.CastPosition.ToVector2(), pred.UnitPosition.ToVector2(), pred.Hitchance, default(Collision.Result));
                result.Lock(false);
                return result;
            }
            #endregion

            switch (s.Type)
            {
                case SkillshotType.Line:
                    return LinePrediction.GetPrediction(target, s.Width, s.Delay, s.Speed, s.Range, s.Collision, target.GetWaypoints(), target.AvgMovChangeTime(), target.LastMovChangeTime(), target.AvgPathLenght(), target.LastAngleDiff(), s.From.ToVector2(), s.RangeCheckFrom.ToVector2());
                case SkillshotType.Circle:
                    return CirclePrediction.GetPrediction(target, s.Width, s.Delay, s.Speed, s.Range, s.Collision, target.GetWaypoints(), target.AvgMovChangeTime(), target.LastMovChangeTime(), target.AvgPathLenght(), target.LastAngleDiff(), s.From.ToVector2(), s.RangeCheckFrom.ToVector2());
                case SkillshotType.Cone:
                    return ConePrediction.GetPrediction(target, s.Width, s.Delay, s.Speed, s.Range, s.Collision, target.GetWaypoints(), target.AvgMovChangeTime(), target.LastMovChangeTime(), target.AvgPathLenght(), target.LastAngleDiff(), s.From.ToVector2(), s.RangeCheckFrom.ToVector2());
            }

            throw new NotSupportedException("Unknown skill shot type");
        }

        /// <summary>
        /// Gets Prediction result
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns>Prediction result as <see cref="Prediction.Result"/></returns>
        public static Prediction.Result GetArcSPrediction(this Spell s, AIHeroClient target)
        {
            return ArcPrediction.GetPrediction(target, s.Width, s.Delay, s.Speed, s.Range, s.Collision, target.GetWaypoints(), target.AvgMovChangeTime(), target.LastMovChangeTime(), target.AvgPathLenght(), target.LastAngleDiff(), s.From.ToVector2(), s.RangeCheckFrom.ToVector2());
        }

        /// <summary>
        /// Gets Prediction result
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="vectorLenght">Vector Lenght</param>
        /// <returns>Prediction result as <see cref="Prediction.Vector.Result"/></returns>
        public static VectorPrediction.Result GetVectorSPrediction(this Spell s, AIHeroClient target, float vectorLenght)
        {
            return VectorPrediction.GetPrediction(target, s.Width, s.Delay, s.Speed, s.Range, vectorLenght, target.GetWaypoints(), target.AvgMovChangeTime(), target.LastMovChangeTime(), target.AvgPathLenght(), s.RangeCheckFrom.ToVector2());
        }

        /// <summary>
        /// Gets Prediction result
        /// </summary>
        /// <param name="s"></param>
        /// <param name="target">Target</param>
        /// <param name="ringRadius">Ring radius</param>
        /// <returns>Prediction result as <see cref="Prediction.Result"/></returns>
        public static Prediction.Result GetRingSPrediction(this Spell s, AIHeroClient target, float ringRadius)
        {
            return RingPrediction.GetPrediction(target, s.Width, ringRadius, s.Delay, s.Speed, s.Range, s.Collision);
        }

        /// <summary>
        /// Gets aoe prediction result
        /// </summary>
        /// <returns>Prediction result as <see cref="Prediction.Result"/></returns>
        public static Prediction.AoeResult GetAoeSPrediction(this Spell s)
        {
            if (s.Collision)
                throw new InvalidOperationException("Collisionable spell");

            switch (s.Type)
            {
                case SkillshotType.Line:
                    return LinePrediction.GetAoePrediction(s.Width, s.Delay, s.Speed, s.Range, s.From.ToVector2(), s.RangeCheckFrom.ToVector2());
                case SkillshotType.Circle:
                    return CirclePrediction.GetAoePrediction(s.Width, s.Delay, s.Speed, s.Range, s.From.ToVector2(), s.RangeCheckFrom.ToVector2());
                case SkillshotType.Cone:
                    return ConePrediction.GetAoePrediction(s.Width, s.Delay, s.Speed, s.Range, s.From.ToVector2(), s.RangeCheckFrom.ToVector2());
            }

            throw new NotSupportedException("Unknown skill shot type");
        }

        /// <summary>
        /// Gets aoe arc prediction
        /// </summary>
        /// <returns>Prediction result as <see cref="Prediction.Result"/></returns>
        public static Prediction.AoeResult GetAoeArcSPrediction(this Spell s)
        {
            if (s.Collision)
                throw new InvalidOperationException("Collisionable spell");

            return ArcPrediction.GetAoePrediction(s.Width, s.Delay, s.Speed, s.Range, s.From.ToVector2(), s.RangeCheckFrom.ToVector2());
        }

        /// <summary>
        /// Gets aoe vector prediction
        /// </summary>
        /// <param name="vectorLenght">Vector lenght</param>
        /// <returns>Prediction result as <see cref="VectorPrediction.AoeResult"/></returns>
        public static VectorPrediction.AoeResult GetAoeVectorSPrediction(this Spell s, float vectorLenght)
        {
            if (s.Collision)
                throw new InvalidOperationException("Collisionable spell");

            return VectorPrediction.GetAoePrediction(s.Width, s.Delay, s.Speed, s.Range, vectorLenght, s.RangeCheckFrom.ToVector2());
        }
        #endregion

        #region Collision methods
        /// <summary>
        /// Checks collisions
        /// </summary>
        /// <param name="to">End position</param>
        /// <param name="checkMinion">Check minion collisions</param>
        /// <param name="checkEnemyHero">Check Enemy collisions</param>
        /// <param name="checkYasuoWall">Check Yasuo wall collisions</param>
        /// <param name="checkAllyHero">Check Ally collisions</param>
        /// <param name="checkWall">Check wall collisions</param>
        /// <param name="isArc">Checks collision for arc spell</param>
        /// <returns>true if collision found</returns>
        public static bool CheckCollision(this Spell s, Vector2 to, bool checkMinion = true, bool checkEnemyHero = false, bool checkYasuoWall = true, bool checkAllyHero = false, bool checkWall = false, bool isArc = false)
        {
            return Collision.CheckCollision(s.From.ToVector2(), to, s.Width, s.Delay, s.Speed, checkMinion, checkEnemyHero, checkYasuoWall, checkAllyHero, checkWall, isArc);
        }

        /// <summary>
        /// Checks minion collisions
        /// </summary>
        /// <param name="to">End position</param>
        /// <param name="isArc">Checks collision for arc spell</param>
        /// <returns>true if collision found</returns>
        public static bool CheckMinionCollision(this Spell s, Vector2 to, bool isArc = false)
        {
            return Collision.CheckMinionCollision(s.From.ToVector2(), to, s.Width, s.Delay, s.Speed, isArc);
        }

        /// <summary>
        /// Checks enemy hero collisions
        /// </summary>
        /// <param name="to">End position</param>
        /// <param name="isArc">Checks collision for arc spell</param>
        /// <returns>true if collision found</returns>
        public static bool CheckEnemyHeroCollision(this Spell s, Vector2 to, bool isArc = false)
        {
            return Collision.CheckEnemyHeroCollision(s.From.ToVector2(), to, s.Width, s.Delay, s.Speed, isArc);
        }

        /// <summary>
        /// Checks ally hero collisions
        /// </summary>
        /// <param name="to">End position</param>
        /// <param name="isArc">Checks collision for arc spell</param>
        /// <returns>true if collision found</returns>
        public static bool CheckAllyHeroCollision(this Spell s, Vector2 to, bool isArc = false)
        {
            return Collision.CheckAllyHeroCollision(s.From.ToVector2(), to, s.Width, s.Delay, s.Speed, isArc);
        }

        /// <summary>
        /// Checks wall collisions
        /// </summary>
        /// <param name="to">End position</param>
        /// <returns>true if collision found</returns>
        public static bool CheckWallCollision(this Spell s, Vector2 to)
        {
            return Collision.CheckWallCollision(s.From.ToVector2(), to);
        }

        /// <summary>
        /// Check Yasuo wall collisions
        /// </summary>
        /// <param name="to">End position</param>
        /// <param name="isArc">Check collision for arc spell</param>
        /// <returns>true if collision found</returns>
        public static bool CheckYasuoWallCollision(this Spell s, Vector2 to, bool isArc = false)
        {
            return Collision.CheckYasuoWallCollision(s.From.ToVector2(), to, s.Width, isArc);
        }

        /// <summary>
        /// Gets collision flags
        /// </summary>
        /// <param name="from">Start position</param>
        /// <param name="to">End position</param>
        /// <param name="isArc">Check collision for arc spell</param>
        /// <returns>true if collision found</returns>
        public static Collision.Flags GetCollisionFlags(this Spell s, Vector2 from, Vector2 to, bool isArc = false)
        {
            Collision.Flags colFlags = Collision.Flags.None;
            if (s.CheckMinionCollision(to))
                colFlags |= Collision.Flags.Minions;

            if (s.CheckEnemyHeroCollision(to))
                colFlags |= Collision.Flags.EnemyChampions;

            if (s.CheckAllyHeroCollision(to))
                colFlags |= Collision.Flags.AllyChampions;

            if (s.CheckWallCollision(to))
                colFlags |= Collision.Flags.Wall;

            if (s.CheckYasuoWallCollision(to))
                colFlags |= Collision.Flags.YasuoWall;

            return colFlags;
        }

        /// <summary>
        /// Gets collided units & flags
        /// </summary>
        /// <param name="to">End position</param>
        /// <param name="isArc">Checks collision for arc spell</param>
        /// <returns>Collision result as <see cref="Collision.Result"/></returns>
        public static Collision.Result GetCollisions(this Spell s, Vector2 to, bool isArc = false)
        {
            return Collision.GetCollisions(s.From.ToVector2(), to, s.Range, s.Width, s.Delay, s.Speed, isArc);
        }
        #endregion

        #region Cast methods
        /// <summary>
        /// Spell extension for cast spell with SPrediction
        /// </summary>
        /// <param name="s">Spell to cast</param>
        /// <param name="t">Target for spell</param>
        /// <param name="hc">Minimum HitChance to cast</param>
        /// <param name="reactionIgnoreDelay">Delay to ignore target's reaction time</param>
        /// <param name="minHit">Minimum Hit Count to cast</param>
        /// <param name="rangeCheckFrom">Position where spell will be casted from</param>
        /// <param name="filterHPPercent">Minimum HP Percent to cast (for target)</param>
        /// <returns>true if spell has casted</returns>
        public static bool SPredictionCast(this Spell s, AIHeroClient t, HitChance hc, int reactionIgnoreDelay = 0, byte minHit = 1, Vector3? rangeCheckFrom = null, float filterHPPercent = 100)
        {
            if (rangeCheckFrom == null)
                rangeCheckFrom = ObjectManager.Player.PreviousPosition;

            if (t == null)
                return s.Cast();

            if (!s.IsSkillShot)
                return s.Cast(t);

            #region if common prediction selected
            if (ConfigMenu.SelectedPrediction.Index == 1)
            {
                var pout = s.GetPrediction(t, minHit > 1);

                if (minHit > 1)
                    if (pout.AoeTargetsHitCount >= minHit)
                        return s.Cast(pout.CastPosition);
                    else return false;

                if (pout.Hitchance >= hc)
                    return s.Cast(pout.CastPosition);
                else
                    return false;
            }
            #endregion

            if (minHit > 1)
                return SPredictionCastAoe(s, minHit);

            if (t.HealthPercent > filterHPPercent)
                return false;

            float avgt = t.AvgMovChangeTime() + reactionIgnoreDelay;
            float movt = t.LastMovChangeTime();
            float avgp = t.AvgPathLenght();
            var waypoints = t.GetWaypoints();

            Prediction.Result result;

            switch (s.Type)
            {
                case SkillshotType.Line:
                    result = LinePrediction.GetPrediction(t, s.Width, s.Delay, s.Speed, s.Range, s.Collision, waypoints, avgt, movt, avgp, t.LastAngleDiff(), s.From.ToVector2(), s.RangeCheckFrom.ToVector2());
                    break;
                case SkillshotType.Circle:
                    result = CirclePrediction.GetPrediction(t, s.Width, s.Delay, s.Speed, s.Range, s.Collision, waypoints, avgt, movt, avgp, t.LastAngleDiff(), s.From.ToVector2(), s.RangeCheckFrom.ToVector2());
                    break;
                case SkillshotType.Cone:
                    result = ConePrediction.GetPrediction(t, s.Width, s.Delay, s.Speed, s.Range, s.Collision, waypoints, avgt, movt, avgp, t.LastAngleDiff(), s.From.ToVector2(), s.RangeCheckFrom.ToVector2());
                    break;
                default:
                    throw new InvalidOperationException("Unknown spell type");
            }

            Drawings.s_DrawTick = Variables.TickCount;
            Drawings.s_DrawPos = result.CastPosition;
            Drawings.s_DrawHitChance = result.HitChance.ToString();
            Drawings.s_DrawDirection = (result.CastPosition - s.From.ToVector2()).Normalized().Perpendicular();
            Drawings.s_DrawWidth = (int)s.Width;

            if (result.HitChance >= hc)
            {
                s.Cast(result.CastPosition);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Spell extension for cast arc spell with SPrediction
        /// </summary>
        /// <param name="s">Spell to cast</param>
        /// <param name="t">Target for spell</param>
        /// <param name="hc">Minimum HitChance to cast</param>
        /// <param name="reactionIgnoreDelay">Delay to ignore target's reaction time</param>
        /// <param name="minHit">Minimum Hit Count to cast</param>
        /// <param name="rangeCheckFrom">Position where spell will be casted from</param>
        /// <param name="filterHPPercent">Minimum HP Percent to cast (for target)</param>
        /// <returns>true if spell has casted</returns>
        public static bool SPredictionCastArc(this Spell s, AIHeroClient t, HitChance hc, bool arconly = true, int reactionIgnoreDelay = 0, byte minHit = 1, Vector3? rangeCheckFrom = null, float filterHPPercent = 100)
        {
            if (ConfigMenu.SelectedPrediction.Index == 1)
                throw new NotSupportedException("Arc Prediction not supported in Common prediction");

            if (minHit > 1)
                return SPredictionCastAoeArc(s, minHit);

            if (t.HealthPercent > filterHPPercent)
                return false;

            if (rangeCheckFrom == null)
                rangeCheckFrom = ObjectManager.Player.PreviousPosition;


            float avgt = t.AvgMovChangeTime() + reactionIgnoreDelay;
            float movt = t.LastMovChangeTime();
            float avgp = t.AvgPathLenght();
            var result = ArcPrediction.GetPrediction(t, s.Width, s.Delay, s.Speed, s.Range, s.Collision, t.GetWaypoints(), avgt, movt, avgp, t.LastAngleDiff(), s.From.ToVector2(), s.RangeCheckFrom.ToVector2(), arconly);

            if (result.HitChance >= hc)
            {
                s.Cast(result.CastPosition);
                return true;
            }

            return false;

        }

        /// <summary>
        /// Spell extension for cast vector spell with SPrediction
        /// </summary>
        /// <param name="s">Spell to cast</param>
        /// <param name="t">Target for spell</param>
        /// <param name="vectorLenght">Vector lenght</param>
        /// <param name="hc">Minimum HitChance to cast</param>
        /// <param name="reactionIgnoreDelay">Delay to ignore target's reaction time</param>
        /// <param name="minHit">Minimum Hit Count to cast</param>
        /// <param name="rangeCheckFrom">Position where spell will be casted from</param>
        /// <param name="filterHPPercent">Minimum HP Percent to cast (for target)</param>
        /// <returns>true if spell has casted</returns>
        public static bool SPredictionCastVector(this Spell s, AIHeroClient t, float vectorLenght, HitChance hc, int reactionIgnoreDelay = 0, byte minHit = 1, Vector3? rangeCheckFrom = null, float filterHPPercent = 100)
        {
            if (ConfigMenu.SelectedPrediction.Index == 1)
                throw new NotSupportedException("Vector Prediction not supported in Common prediction");

            if (minHit > 1)
                return SPredictionCastAoeVector(s, vectorLenght, minHit);

            if (t.HealthPercent > filterHPPercent)
                return false;

            if (rangeCheckFrom == null)
                rangeCheckFrom = ObjectManager.Player.PreviousPosition;


            float avgt = t.AvgMovChangeTime() + reactionIgnoreDelay;
            float movt = t.LastMovChangeTime();
            float avgp = t.AvgPathLenght();
            var result = VectorPrediction.GetPrediction(t, s.Width, s.Delay, s.Speed, s.Range, vectorLenght, t.GetWaypoints(), avgt, movt, avgp, s.RangeCheckFrom.ToVector2());

            if (result.HitChance >= hc)
            {
                s.Cast(result.CastSourcePosition.ToVector3(), result.CastTargetPosition.ToVector3());
                return true;
            }

            return false;
        }

        /// <summary>
        /// Spell extension for cast vector spell with SPrediction
        /// </summary>
        /// <param name="s">Spell to cast</param>
        /// <param name="t">Target for spell</param>
        /// <param name="ringRadius">Ring Radius</param>
        /// <param name="hc">Minimum HitChance to cast</param>
        /// <param name="reactionIgnoreDelay">Delay to ignore target's reaction time</param>
        /// <param name="minHit">Minimum Hit Count to cast</param>
        /// <param name="rangeCheckFrom">Position where spell will be casted from</param>
        /// <param name="filterHPPercent">Minimum HP Percent to cast (for target)</param>
        /// <returns>true if spell has casted</returns>
        public static bool SPredictionCastRing(this Spell s, AIHeroClient t, float ringRadius, HitChance hc, bool onlyEdge = true, int reactionIgnoreDelay = 0, byte minHit = 1, Vector3? rangeCheckFrom = null, float filterHPPercent = 100)
        {
            if (ConfigMenu.SelectedPrediction.Index == 1)
                throw new NotSupportedException("Vector Prediction not supported in Common prediction");

            if (minHit > 1)
                throw new NotSupportedException("Ring aoe prediction not supported yet");

            if (t.HealthPercent > filterHPPercent)
                return false;

            if (rangeCheckFrom == null)
                rangeCheckFrom = ObjectManager.Player.PreviousPosition;


            float avgt = t.AvgMovChangeTime() + reactionIgnoreDelay;
            float movt = t.LastMovChangeTime();
            float avgp = t.AvgPathLenght();
            Prediction.Result result;
            if (onlyEdge)
                result = RingPrediction.GetPrediction(t, s.Width, ringRadius, s.Delay, s.Speed, s.Range, s.Collision, t.GetWaypoints(), avgt, movt, avgp, s.From.ToVector2(), rangeCheckFrom.Value.ToVector2());
            else
                result = CirclePrediction.GetPrediction(t, s.Width, s.Delay, s.Speed, s.Range + ringRadius, s.Collision, t.GetWaypoints(), avgt, movt, avgp, 360, s.From.ToVector2(), rangeCheckFrom.Value.ToVector2());

            Drawings.s_DrawTick = Variables.TickCount;
            Drawings.s_DrawPos = result.CastPosition;
            Drawings.s_DrawHitChance = result.HitChance.ToString();
            Drawings.s_DrawDirection = (result.CastPosition - s.From.ToVector2()).Normalized().Perpendicular();
            Drawings.s_DrawWidth = (int)ringRadius;
            if (result.HitChance >= hc)
            {
                s.Cast(result.CastPosition);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Spell extension for cast aoe spell with SPrediction
        /// </summary>
        /// <param name="minHit">Minimum aoe hits to cast</param>
        /// <returns></returns>
        public static bool SPredictionCastAoe(this Spell s, int minHit)
        {
            if (minHit < 2)
                throw new InvalidOperationException("Minimum aoe hit count cannot be less than 2");

            if (s.Collision)
                throw new InvalidOperationException("Collisionable spell");

            Prediction.AoeResult result;

            switch (s.Type)
            {
                case SkillshotType.Line:
                    result = LinePrediction.GetAoePrediction(s.Width, s.Delay, s.Speed, s.Range, s.From.ToVector2(), s.RangeCheckFrom.ToVector2());
                    break;
                case SkillshotType.Circle:
                    result = CirclePrediction.GetAoePrediction(s.Width, s.Delay, s.Speed, s.Range, s.From.ToVector2(), s.RangeCheckFrom.ToVector2());
                    break;
                case SkillshotType.Cone:
                    result = ConePrediction.GetAoePrediction(s.Width, s.Delay, s.Speed, s.Range, s.From.ToVector2(), s.RangeCheckFrom.ToVector2());
                    break;
                default:
                    throw new InvalidOperationException("Unknown spell type");
            }

            Drawings.s_DrawTick = Variables.TickCount;
            Drawings.s_DrawPos = result.CastPosition;
            Drawings.s_DrawHitChance = String.Format("Aoe Cast (Hits: {0})", result.HitCount);
            Drawings.s_DrawDirection = (result.CastPosition - s.From.ToVector2()).Normalized().Perpendicular();
            Drawings.s_DrawWidth = (int)s.Width;

            if (result.HitCount >= minHit)
                return s.Cast(result.CastPosition);

            return false;
        }

        /// <summary>
        /// Spell extension for cast aoe arc spell with SPrediction
        /// </summary>
        /// <param name="minHit">Minimum aoe hits to cast</param>
        /// <returns></returns>
        public static bool SPredictionCastAoeArc(this Spell s, int minHit)
        {
            if (minHit < 2)
                throw new InvalidOperationException("Minimum aoe hit count cannot be less than 2");

            if (s.Collision)
                throw new InvalidOperationException("Collisionable spell");

            Prediction.AoeResult result = ArcPrediction.GetAoePrediction(s.Width, s.Delay, s.Speed, s.Range, s.From.ToVector2(), s.RangeCheckFrom.ToVector2());

            if (result.HitCount >= minHit)
                return s.Cast(result.CastPosition);

            return false;
        }

        /// <summary>
        /// Spell extension for cast aoe vector spell with SPrediction
        /// </summary>
        /// <param name="vectorLenght">Vector lenght</param>
        /// <param name="minHit">Minimum aoe hits to cast</param>
        /// <returns></returns>
        public static bool SPredictionCastAoeVector(this Spell s, float vectorLenght, int minHit)
        {
            if (minHit < 2)
                throw new InvalidOperationException("Minimum aoe hit count cannot be less than 2");

            if (s.Collision)
                throw new InvalidOperationException("Collisionable spell");

            VectorPrediction.AoeResult result = VectorPrediction.GetAoePrediction(s.Width, s.Delay, s.Speed, s.Range, vectorLenght, s.RangeCheckFrom.ToVector2());


            if (result.HitCount >= minHit)
                return s.Cast(result.CastSourcePosition.ToVector3(), result.CastTargetPosition.ToVector3());

            return false;
        }
        #endregion

        #region Stasis Prediction registers

        /// <summary>
        /// Registers spell callback to stasis prediction
        /// </summary>
        /// <param name="s">The spell.</param>
        /// <param name="fn">The eventhandler.</param>
        public static void RegisterStasisCallback(this Spell s, EventHandler<StasisPrediction.Result> fn)
        {
            StasisPrediction.RegisterSpell(s);
            StasisPrediction.OnGuaranteedHit += fn;
        }

        /// <summary>
        /// Unregisters spell callback from stasis prediction
        /// </summary>
        /// <param name="s">The spell.</param>
        /// <param name="fn">The eventhandler.</param>
        public static void UnregisterStasisCallback(this Spell s, EventHandler<StasisPrediction.Result> fn)
        {
            StasisPrediction.UnregisterSpell(s);
            StasisPrediction.OnGuaranteedHit -= fn;
        }

        #endregion
    }
}
