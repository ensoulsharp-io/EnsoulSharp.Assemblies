/*
 Copyright 2015 - 2015 SPrediction
 Prediction.PathTracker.cs is part of SPrediction
 
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
using System.Collections.Generic;
using System.Linq;

using EnsoulSharp;
using EnsoulSharp.SDK;

using SharpDX;

namespace SPrediction
{
    /// <summary>
    /// Path Tracker class for SPrediction
    /// </summary>
    internal class PathTracker
    {
        /// <summary>
        /// structure for enemy data
        /// </summary>
        public struct EnemyData
        {
            public bool IsStopped;
            public List<Vector2> LastWaypoints;
            public int LastWaypointTick;
            public int StopTick;
            public float AvgTick;
            public float AvgPathLenght;
            public int Count;
            public int LastAATick;
            public int LastWindupTick;
            public bool IsWindupChecked;
            public int OrbwalkCount;
            public float AvgOrbwalkTime;
            public float LastAngleDiff;

            public EnemyData(List<Vector2> wp)
            {
                IsStopped = false;
                LastWaypoints = wp;
                LastWaypointTick = 0;
                StopTick = 0;
                AvgTick = 0;
                AvgPathLenght = 0;
                Count = 0;
                LastAATick = 0;
                LastWindupTick = 0;
                IsWindupChecked = false;
                OrbwalkCount = 0;
                AvgOrbwalkTime = 0;
                LastAngleDiff = 0;
            }
        }

        public static Dictionary<uint, EnemyData> EnemyInfo = new Dictionary<uint, EnemyData>();

        /// <summary>
        /// Initialize PathTracker services
        /// </summary>
        public static void Initialize()
        {
            foreach (var enemy in GameObjects.EnemyHeroes)
                EnemyInfo.Add(enemy.NetworkId, new EnemyData(new List<Vector2>()));

            AIBaseClient.OnNewPath += AIBaseClient_OnNewPath;
            AIBaseClient.OnDoCast += AIBaseClient_OnDoCast;
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
        }

        /// <summary>
        /// OnNewPath event for average reaction time calculations
        /// </summary>
        private static void AIBaseClient_OnNewPath(AIBaseClient sender, AIBaseClientNewPathEventArgs args)
        {
            if (!sender.IsEnemy || !(sender.Type == GameObjectType.AIHeroClient && sender.IsValid) || args.IsDash)
                return;

            var enemy = EnemyInfo[sender.NetworkId];

            if (args.Path.Length < 2)
            {
                if (!enemy.IsStopped)
                {
                    enemy.StopTick = Environment.TickCount;
                    enemy.LastWaypointTick = Environment.TickCount;
                    enemy.IsStopped = true;
                    enemy.Count = 0;
                    enemy.AvgTick = 0;
                    enemy.AvgPathLenght = 0;
                    enemy.LastAngleDiff = 360;
                }
            }
            else
            {
                List<Vector2> wp = args.Path.Select(p => p.ToVector2()).ToList();
                List<Vector2> sample1 = new List<Vector2>();
                wp.Insert(0, sender.PreviousPosition.ToVector2());

                for (int i = 0; i < wp.Count - 1; i++)
                {
                    Vector2 direction = (wp[i + 1] - wp[i]).Normalized();
                    sample1.Add(direction);
                }

                List<Vector2> sample2 = new List<Vector2>();
                for (int i = 0; i < enemy.LastWaypoints.Count - 1; i++)
                {
                    Vector2 direction = (enemy.LastWaypoints[i + 1] - enemy.LastWaypoints[i]).Normalized();
                    sample2.Add(direction);
                }

                if (sample1.Count() > 0 && sample2.Count() > 0)
                {
                    float sample1_avg = sample1.Average(p => p.AngleBetween(Vector2.Zero));
                    float sample2_avg = sample2.Average(p => p.AngleBetween(Vector2.Zero));
                    enemy.LastAngleDiff = Math.Abs(sample2_avg - sample1_avg);
                }
                if (!enemy.LastWaypoints.SequenceEqual(wp))
                {
                    if (!enemy.IsStopped)
                    {
                        enemy.AvgTick = (enemy.Count * enemy.AvgTick + (Environment.TickCount - enemy.LastWaypointTick)) / ++enemy.Count;
                        enemy.AvgPathLenght = ((enemy.Count - 1) * enemy.AvgPathLenght + wp.PathLength()) / enemy.Count;
                    }
                    enemy.LastWaypointTick = Environment.TickCount;
                    enemy.IsStopped = false;
                    enemy.LastWaypoints = wp;

                    if (!enemy.IsWindupChecked)
                    {
                        if (Environment.TickCount - enemy.LastAATick < 300)
                            enemy.AvgOrbwalkTime = (enemy.AvgOrbwalkTime * enemy.OrbwalkCount + (enemy.LastWaypointTick - enemy.LastWindupTick)) / ++enemy.OrbwalkCount;
                        enemy.IsWindupChecked = true;
                    }
                }
            }

            EnemyInfo[sender.NetworkId] = enemy;
        }

        /// <summary>
        /// OnProcessSpellCast event for aa windup check
        /// </summary>
        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (Orbwalker.IsAutoAttack(args.SData.Name) && sender.IsEnemy && sender.Type == GameObjectType.AIHeroClient && sender.IsValid)
            {
                var enemy = EnemyInfo[sender.NetworkId];

                enemy.LastWindupTick = Environment.TickCount;
                enemy.IsWindupChecked = false;
                EnemyInfo[sender.NetworkId] = enemy;
            }
        }

        /// <summary>
        /// OnDoCast event for aa windup check
        /// </summary>
        private static void AIBaseClient_OnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (Orbwalker.IsAutoAttack(args.SData.Name) && sender.IsEnemy && sender.Type == GameObjectType.AIHeroClient && sender.IsValid)
            {
                var enemy = EnemyInfo[sender.NetworkId];

                enemy.LastAATick = Environment.TickCount;
                EnemyInfo[sender.NetworkId] = enemy;
            }
        }
    }
}
