/*
Copyright 2015 - 2015 SPrediction
Prediction.cs is part of SPrediction

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

namespace SPrediction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SharpDX;

    using EnsoulSharp;
    using EnsoulSharp.SDK;

    /// <summary>
    /// Spacebar Prediction class
    /// </summary>
    public static class PredictionExtensions
    {
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
        public static PredictionResult GetPrediction(AIBaseClient target, float width, float delay, float missileSpeed, float range, bool collisionable, SpellType type, List<Vector2> path, float avgt, float movt, float avgp, float anglediff, Vector2 from, Vector2 rangeCheckFrom)
        {
            var result = new PredictionResult();
            var input = new PredictionInput(target, delay, missileSpeed, width, range, collisionable, type, from.ToVector3World(), rangeCheckFrom.ToVector3World());
            result.Input = input;
            result.Unit = target;

            try
            {
                if (type == SpellType.Circle)
                {
                    range += width;
                }

                //to do: hook logic ? by storing average movement direction etc
                if (path.Count <= 1 && movt > 100 && (Environment.TickCount - PathTracker.EnemyInfo[target.NetworkId].LastAATick > 300 || !Program.CheckAAWindUp)) //if target is not moving, easy to hit (and not aaing)
                {
                    result.HitChance = HitChance.VeryHigh;
                    result.CastPosition = target.PreviousPosition.ToVector2();
                    result.UnitPosition = result.CastPosition;
                    result.Lock();

                    return result;
                }

                if (target is AIHeroClient hero)
                {
                    if (hero.IsCastingImporantSpell())
                    {
                        result.HitChance = HitChance.VeryHigh;
                        result.CastPosition = hero.PreviousPosition.ToVector2();
                        result.UnitPosition = result.CastPosition;
                        result.Lock();

                        return result;
                    }

                    if (Environment.TickCount - PathTracker.EnemyInfo[hero.NetworkId].LastAATick < 300 && Program.CheckAAWindUp)
                    {
                        if (hero.AttackCastDelay * 1000 + PathTracker.EnemyInfo[hero.NetworkId].AvgOrbwalkTime + avgt - width / 2f / hero.MoveSpeed >= PredictionExtensions.GetArrivalTime(hero.PreviousPosition.ToVector2().Distance(from), delay, missileSpeed))
                        {
                            result.HitChance = HitChance.High;
                            result.CastPosition = hero.PreviousPosition.ToVector2();
                            result.UnitPosition = result.CastPosition;
                            result.Lock();

                            return result;
                        }
                    }

                    //to do: find a fuking logic
                    if (avgp < 400 && movt < 100 && path.PathLength() <= avgp)
                    {
                        result.HitChance = HitChance.High;
                        result.CastPosition = path.Last();
                        result.UnitPosition = result.CastPosition;
                        result.Lock();

                        return result;
                    }
                }

                if (target.IsDashing()) //if unit is dashing
                {
                    return GetDashingPrediction(target, width, delay, missileSpeed, range, collisionable, type, from, rangeCheckFrom);
                }

                if (target.IsImmobileTarget()) //if unit is immobile
                {
                    return GetImmobilePrediction(target, width, delay, missileSpeed, range, collisionable, type, from, rangeCheckFrom);
                }

                result = WaypointAnlysis(target, width, delay, missileSpeed, range, collisionable, type, path, avgt, movt, avgp, anglediff, from);
                result.Input = input;

                var d = result.CastPosition.Distance(target.PreviousPosition.ToVector2());
                if (d >= (avgt - movt) * target.MoveSpeed && d >= avgp)
                {
                    result.HitChance = HitChance.Medium;
                }

                result.Lock();

                return result;
            }
            finally
            {
                //check if movement changed while prediction calculations
                if (!target.GetWaypoints().SequenceEqual(path))
                {
                    result.HitChance = HitChance.Medium;
                }
            }
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
        /// <returns></returns>
        public static PredictionResult GetDashingPrediction(AIBaseClient target, float width, float delay, float missileSpeed, float range, bool collisionable, SpellType type, Vector2 from, Vector2 rangeCheckFrom)
        {
            var result = new PredictionResult
            {
                Input = new PredictionInput(target, delay, missileSpeed, width, range, collisionable, type, from.ToVector3World(), rangeCheckFrom.ToVector3World()),
                Unit = target
            };

            if (target.IsDashing())
            {
                var dashInfo = target.GetDashInfo();
                result.CastPosition = GetFastUnitPosition(target, dashInfo.Path, delay, missileSpeed, from, dashInfo.Speed);
                result.HitChance = HitChance.Dash;

                result.Lock(false);
            }
            else
            {
                result = GetPrediction(target, width, delay, missileSpeed, range, collisionable, type, target.GetWaypoints(), 0, 0, 0, 0, from, rangeCheckFrom);
                result.Lock(false);
            }
            return result;
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
        public static PredictionResult GetImmobilePrediction(AIBaseClient target, float width, float delay, float missileSpeed, float range, bool collisionable, SpellType type, Vector2 from, Vector2 rangeCheckFrom)
        {
            var result = new PredictionResult
            {
                Input = new PredictionInput(target, delay, missileSpeed, width, range, collisionable, type, from.ToVector3World(), rangeCheckFrom.ToVector3World()),
                Unit = target,
                CastPosition = target.PreviousPosition.ToVector2()
            };
            result.UnitPosition = result.CastPosition;

            //calculate spell arrival time
            var t = delay + Game.Ping / 2000f;
            if (missileSpeed != 0)
            {
                t += from.Distance(target.PreviousPosition) / missileSpeed;
            }

            if (type == SpellType.Circle)
            {
                t += width / target.MoveSpeed / 2f;
            }

            if (t >= target.LeftImmobileTime())
            {
                result.HitChance = HitChance.Immobile;
                result.Lock();

                return result;
            }

            if (target is AIHeroClient hero)
            {
                result.HitChance = PredictionExtensions.GetHitChance(t - hero.LeftImmobileTime(), hero.AvgMovChangeTime(), 0, 0, 0);
            }
            else
            {
                result.HitChance = HitChance.High;
            }

            result.Lock();

            return result;
        }

        /// <summary>
        /// Gets fast-predicted unit position
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="from">Spell casted position</param>
        /// <returns></returns>
        public static Vector2 GetFastUnitPosition(AIBaseClient target, float delay, float missileSpeed = 0, Vector2? from = null, float distanceSet = 0)
        {
            var path = target.GetWaypoints();
            if (from == null)
            {
                from = ObjectManager.Player.PreviousPosition.ToVector2();
            }

            if (path.Count <= 1 || (target is AIHeroClient hero && hero.IsCastingImporantSpell()) || target.IsImmobileTarget())
            {
                return target.PreviousPosition.ToVector2();
            }

            if (target.IsDashing())
            {
                return target.GetDashInfo().Path.Last();
            }

            var distance = distanceSet;

            if (distance == 0)
            {
                var targetDistance = from.Value.Distance(target.PreviousPosition);
                var flyTime = targetDistance / missileSpeed;

                if (missileSpeed != 0 && path.Count == 2)
                {
                    var Vt = (path[1] - path[0]).Normalized() * target.MoveSpeed;
                    var Vs = (target.PreviousPosition.ToVector2() - from.Value).Normalized() * missileSpeed;
                    var Vr = Vt - Vs;

                    flyTime = targetDistance / Vr.Length();
                }

                var t = flyTime + delay + Game.Ping / 2000f;
                distance = t * target.MoveSpeed;
            }

            for (var i = 0; i < path.Count - 1; i++)
            {
                var d = path[i + 1].Distance(path[i]);
                if (distance == d)
                {
                    return path[i + 1];
                }

                if (distance < d)
                {
                    return path[i] + distance * (path[i + 1] - path[i]).Normalized();
                }

                distance -= d;
            }

            return path.Last();
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
        public static Vector2 GetFastUnitPosition(AIBaseClient target, List<Vector2> path, float delay, float missileSpeed = 0, Vector2? from = null, float moveSpeed = 0, float distanceSet = 0)
        {
            if (from == null)
            {
                from = target.PreviousPosition.ToVector2();
            }

            if (moveSpeed == 0)
            {
                moveSpeed = target.MoveSpeed;
            }

            if (path.Count <= 1 || (target is AIHeroClient hero && hero.IsCastingImporantSpell()) || target.IsImmobileTarget())
            {
                return target.PreviousPosition.ToVector2();
            }

            if (target.IsDashing())
            {
                return target.GetDashInfo().Path.Last();
            }

            var distance = distanceSet;

            if (distance == 0)
            {
                var targetDistance = from.Value.Distance(target.PreviousPosition);
                var flyTime = 0f;

                if (missileSpeed != 0) //skillshot with a missile
                {
                    var Vt = (path[path.Count - 1] - path[0]).Normalized() * moveSpeed;
                    var Vs = (target.PreviousPosition.ToVector2() - from.Value).Normalized() * missileSpeed;
                    var Vr = Vs - Vt;

                    flyTime = targetDistance / Vr.Length();

                    if (path.Count > 5) //complicated movement
                    {
                        flyTime = targetDistance / missileSpeed;
                    }
                }

                var t = flyTime + delay + Game.Ping / 2000f + Program.SpellDelay / 1000f;
                distance = t * moveSpeed;
            }

            for (var i = 0; i < path.Count - 1; i++)
            {
                var d = path[i + 1].Distance(path[i]);
                if (distance == d)
                {
                    return path[i + 1];
                }

                if (distance < d)
                {
                    return path[i] + distance * (path[i + 1] - path[i]).Normalized();
                }

                distance -= d;
            }

            return path.Last();
        }

        /// <summary>
        /// Calculates cast position with target's path
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
        /// <returns></returns>
        public static PredictionResult WaypointAnlysis(AIBaseClient target, float width, float delay, float missileSpeed, float range, bool collisionable, SpellType type, List<Vector2> path, float avgt, float movt, float avgp, float anglediff, Vector2 from, float moveSpeed = 0, bool isDash = false)
        {
            if (moveSpeed == 0)
            {
                moveSpeed = target.MoveSpeed;
            }

            var result = new PredictionResult { Unit = target };

            var flyTimeMax = 0f;

            if (missileSpeed != 0) //skillshot with a missile
            {
                flyTimeMax = range / missileSpeed;
            }

            var tMin = delay + Game.Ping / 2000f + Program.SpellDelay / 1000f;
            var tMax = flyTimeMax + delay + Game.Ping / 1000f + Program.SpellDelay / 1000f;
            var pathTime = 0f;
            var pathBounds = new[] { -1, -1 };

            //find bounds
            for (var i = 0; i < path.Count - 1; i++)
            {
                var t = path[i + 1].Distance(path[i]) / moveSpeed;

                if (pathTime <= tMin && pathTime + t >= tMin)
                {
                    pathBounds[0] = i;
                }

                if (pathTime <= tMax && pathTime + t >= tMax)
                {
                    pathBounds[1] = i;
                }

                if (pathBounds[0] != -1 && pathBounds[1] != -1)
                {
                    break;
                }

                pathTime += t;
            }

            //calculate cast & unit position
            if (pathBounds[0] != -1 && pathBounds[1] != -1)
            {
                for (var k = pathBounds[0]; k <= pathBounds[1]; k++)
                {
                    var direction = (path[k + 1] - path[k]).Normalized();
                    var distance = width;
                    var extender = target.BoundingRadius;

                    if (type == SpellType.Line)
                    {
                        extender = width;
                    }

                    var steps = (int)Math.Floor(path[k].Distance(path[k + 1]) / distance);
                    //split & anlyse current path
                    for (var i = 1; i < steps - 1; i++)
                    {
                        var pCenter = path[k] + (direction * distance * i);
                        var pA = pCenter - (direction * extender);
                        var pB = pCenter + (direction * extender);

                        var flytime = missileSpeed != 0 ? from.Distance(pCenter) / missileSpeed : 0f;
                        var t = flytime + delay + Game.Ping / 2000f + Program.SpellDelay / 1000f;

                        var currentPosition = target.PreviousPosition.ToVector2();

                        var arriveTimeA = currentPosition.Distance(pA) / moveSpeed;
                        var arriveTimeB = currentPosition.Distance(pB) / moveSpeed;

                        if (Math.Min(arriveTimeA, arriveTimeB) <= t && Math.Max(arriveTimeA, arriveTimeB) >= t)
                        {
                            result.HitChance = PredictionExtensions.GetHitChance(t, avgt, movt, avgp, anglediff);
                            result.CastPosition = pCenter;
                            result.UnitPosition = pCenter; //+ (direction * (t - Math.Min(arriveTimeA, arriveTimeB)) * moveSpeed);
                            return result;
                        }
                    }
                }
            }

            result.HitChance = HitChance.None;
            result.CastPosition = target.PreviousPosition.ToVector2();

            return result;
        }

        /// <summary>
        /// Get HitChance
        /// </summary>
        /// <param name="t">Arrive time to target (in ms)</param>
        /// <param name="avgt">Average reaction time (in ms)</param>
        /// <param name="movt">Passed time from last movement change (in ms)</param>
        /// <param name="avgp">Average Path Lenght</param>
        /// <returns>HitChance</returns>
        internal static HitChance GetHitChance(float t, float avgt, float movt, float avgp, float anglediff)
        {
            if (avgp > 400)
            {
                if (movt > 50)
                {
                    if (avgt >= t + Game.Ping)
                    {
                        return anglediff < 30 ? HitChance.VeryHigh : HitChance.High;
                    }

                    if (avgt - movt >= t)
                    {
                        return HitChance.Medium;
                    }

                    return HitChance.Low;
                }

                return HitChance.VeryHigh;
            }

            return HitChance.High;
        }

        /// <summary>
        /// Gets spell arrival time to cast position
        /// </summary>
        /// <param name="distance">Distance from to to</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <returns></returns>
        internal static float GetArrivalTime(float distance, float delay, float missileSpeed = 0)
        {
            if (missileSpeed != 0)
            {
                return distance / missileSpeed + delay;
            }

            return delay;
        }
    }
}
