// Copyright 2014 - 2014 Esk0r
// Skillshot.cs is part of Evade.
// 
// Evade is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Evade is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Evade. If not, see <http://www.gnu.org/licenses/>.

#region

using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Core.UI.IMenu;
using EnsoulSharp.SDK.Core.UI.IMenu.Values;
using SharpDX;
using Color = System.Drawing.Color;
using GamePath = System.Collections.Generic.List<SharpDX.Vector2>;

#endregion

namespace Evade
{
    public enum SkillShotType
    {
        SkillshotCircle,
        SkillshotLine,
        SkillshotMissileLine,
        SkillshotCone,
        SkillshotRing,
        SkillshotArc
    }

    public enum DetectionType
    {
        RecvPacket,
        ProcessSpell
    }

    public struct FoundIntersection
    {
        public Vector2 ComingFrom;
        public float Distance;
        public Vector2 Point;
        public int Time;
        public bool Valid;

        public FoundIntersection(float distance, int time, Vector2 point ,Vector2 comingFrom)
        {
            Distance = distance;
            ComingFrom = comingFrom;
            Valid = (point.X != 0) && (point.Y != 0);
            Point = point + Config.GridSize * (ComingFrom - point).Normalized();
            Time = time;
        }
    }

    public struct SafePathResult
    {
        public FoundIntersection Intersection;
        public bool IsSafe;

        public SafePathResult(bool isSafe, FoundIntersection intersection)
        {
            IsSafe = isSafe;
            Intersection = intersection;
        }
    }

    public class Skillshot
    {
        public DetectionType DetectionType;
        public bool ForceDisabled;

        public Vector2 Direction;
        public Vector2 Start;
        public Vector2 End;
        public Vector2 MissilePosition;

        public Geometry.Polygon DrawingPolygon;
        public Geometry.Polygon Polygon;
        public Geometry.Arc Arc;
        public Geometry.Circle Circle;
        public Geometry.Rectangle Rectangle;
        public Geometry.Ring Ring;
        public Geometry.Sector Sector;

        public SpellData SpellData;

        public int StartTick;

        private int _helperTick = 0;

        private bool _cachedValue = false;
        private int _cachedValueTick = 0;

        private Vector2 _collisionEnd = Vector2.Zero;
        private int _lastCollisionCalc = 0;

        private bool _speedUp = false;

        public Geometry.Polygon EvadePolygon { get; set; }
        public Geometry.Polygon PathFindingPolygon { get; set; }
        public Geometry.Polygon PathFindingInnerPolygon { get; set; }
        public AIBaseClient Unit { get; set; }

        public Skillshot(DetectionType detectionType,
            SpellData spellData,
            int startT,
            Vector2 start,
            Vector2 end,
            AIBaseClient unit)
        {
            DetectionType = detectionType;
            SpellData = spellData;
            StartTick = startT;
            Start = start;
            End = end;
            MissilePosition = start;
            Direction = (end - start).Normalized();

            Unit = unit;

            // Create the spatial object for each type of skillshot.
            switch (spellData.Type)
            {
                case SkillShotType.SkillshotCircle:
                    Circle = new Geometry.Circle(CollisionEnd, spellData.Radius);
                    break;
                case SkillShotType.SkillshotLine:
                    Rectangle = new Geometry.Rectangle(Start, CollisionEnd, spellData.Radius);
                    break;
                case SkillShotType.SkillshotMissileLine:
                    Rectangle = new Geometry.Rectangle(Start, CollisionEnd, spellData.Radius);
                    break;
                case SkillShotType.SkillshotCone:
                    Sector = new Geometry.Sector(start, CollisionEnd - start, spellData.RawRadius * (float)Math.PI / 180, spellData.Range);
                    break;
                case SkillShotType.SkillshotRing:
                    Ring = new Geometry.Ring(CollisionEnd, spellData.Radius, spellData.RingRadius);
                    break;
                case SkillShotType.SkillshotArc:
                    Arc = new Geometry.Arc(start, end, Config.SkillShotsExtraRadius + (int)ObjectManager.Player.BoundingRadius);
                    break;
            }

            // Create the polygon.
            UpdatePolygon();
        }

        public Vector2 Perpendicular
        {
            get { return Direction.Perpendicular(); }
        }

        public Vector2 CollisionEnd
        {
            get
            {
                if (_collisionEnd.IsValid())
                {
                    return _collisionEnd;
                }

                if (IsGlobal)
                {
                    return GlobalGetMissilePosition(0) +
                        Direction * SpellData.MissileSpeed *
                        (0.5f + SpellData.Radius * 2 / ObjectManager.Player.MoveSpeed);
                }

                return End;
            }
        }

        public bool IsGlobal
        {
            get { return SpellData.RawRange >= 20000; }
        }

        /// <summary>
        /// Returns the value from this skillshot menu.
        /// </summary>
        public T GetValue<T>(string name) where T : MenuItem
        {
            return Config.skillShots?[SpellData.MenuItemName]?[name + SpellData.MenuItemName]?.GetValue<T>();
        }

        /// <summary>
        /// Returns if the skillshot has expired.
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            if (SpellData.MissileAccel != 0)
            {
                return Utils.TickCount <= StartTick + 5000;
            }

            return Utils.TickCount <=
                StartTick + SpellData.Delay + SpellData.ExtraDuration +
                1000 * (Start.Distance(End) / SpellData.MissileSpeed);
        }

        public bool Evade()
        {
            if (ForceDisabled)
            {
                return false;
            }

            if (Utils.TickCount - _cachedValueTick < 100)
            {
                return _cachedValue;
            }

            if (!GetValue<MenuBool>("IsDangerous").Value && Config.Menu["OnlyDangerous"].GetValue<MenuKeyBind>().Active)
            {
                _cachedValue = false;
                _cachedValueTick = Utils.TickCount;
                return _cachedValue;
            }

            _cachedValue = GetValue<MenuBool>("Enabled").Value;
            _cachedValueTick = Utils.TickCount;

            return _cachedValue;
        }

        public void Game_OnGameUpdate()
        {
            // Even if it doesnt consume a lot of resources with 20 updatest second works.
            if (SpellData.CollisionObjects != null && SpellData.CollisionObjects.Count() > 0 &&
                Utils.TickCount - _lastCollisionCalc > 50 && Config.collision["EnableCollision"].GetValue<MenuBool>().Value)
            {
                _lastCollisionCalc = Utils.TickCount;
                _collisionEnd = Collision.GetCollisionPoint(this);
            }

            // Update the missile position each time the game updates.
            if (SpellData.Type == SkillShotType.SkillshotMissileLine)
            {
                Rectangle = new Geometry.Rectangle(GetMissilePosition(0), CollisionEnd, SpellData.Radius);
                UpdatePolygon();
            }

            if (SpellData.MissileFollowsUnit)
            {
                if (Unit.IsVisible)
                {
                    End = Unit.Position.ToVector2();
                    Direction = (End - Start).Normalized();
                    UpdatePolygon();
                }
            }

            if (SpellData.FollowCaster)
            {
                switch (SpellData.Type)
                {
                    case SkillShotType.SkillshotCircle:
                        Circle.Center = Unit.Position.ToVector2();
                        UpdatePolygon();
                        break;
                    case SkillShotType.SkillshotLine:
                        Start = Unit.Position.ToVector2();
                        End = Start + Direction * SpellData.Range;
                        Rectangle = new Geometry.Rectangle(Start, End, SpellData.Radius);
                        UpdatePolygon();
                        break;
                    case SkillShotType.SkillshotCone:
                        Sector.Center = Unit.Position.ToVector2();
                        UpdatePolygon();
                        break;
                }
            }

            if (SpellData.SpellName == "AatroxQ3")
            {
                Circle.Center = Unit.Position.ToVector2() + Direction * 3 * Unit.BoundingRadius;
                UpdatePolygon();
            }

            if (SpellData.SpellName == "JinxR" && !_speedUp)
            {
                SpellData.MissileSpeed = 1700;
                if (Utils.TickCount - StartTick >= 450)
                {
                    _speedUp = true;
                    SpellData.MissileSpeed += 500;
                }
            }

            if (SpellData.SpellName == "SionR")
            {
                if (_helperTick == 0)
                {
                    _helperTick = StartTick;
                }

                SpellData.MissileSpeed = (int)Unit.MoveSpeed;
                if (Unit.IsValidTarget(float.MaxValue, false))
                {
                    if (!Unit.HasBuff("SionR") && Utils.TickCount - _helperTick > 600)
                    {
                        StartTick = 0;
                    }
                    else
                    {
                        StartTick = Utils.TickCount - SpellData.Delay;
                        Start = Unit.Position.ToVector2();
                        End = Unit.Position.ToVector2() + 1000 * Unit.Direction.ToVector2().Perpendicular();
                        Direction = (End - Start).Normalized();
                        UpdatePolygon();
                    }
                }
                else
                {
                    StartTick = 0;
                }
            }

            if ((SpellData.SpellName == "XayahQMissile1" || SpellData.SpellName == "XayahQMissile2") && !_speedUp)
            {
                SpellData.MissileSpeed = 400;
                if (Utils.TickCount - StartTick >= SpellData.Delay + 60)
                {
                    _speedUp = true;
                    SpellData.MissileSpeed += 3500;
                }
            }
        }

        public void UpdatePolygon()
        {
            switch (SpellData.Type)
            {
                case SkillShotType.SkillshotCircle:
                    Polygon = Circle.ToPolygon();
                    EvadePolygon = Circle.ToPolygon(Config.ExtraEvadeDistance);
                    PathFindingPolygon = Circle.ToPolygon(Config.PathFindingDistance);
                    PathFindingInnerPolygon = Circle.ToPolygon(Config.PathFindingDistance2);
                    DrawingPolygon = Circle.ToPolygon(
                        0,
                        !SpellData.AddHitbox
                            ? SpellData.Radius
                            : (SpellData.Radius - ObjectManager.Player.BoundingRadius));
                    break;
                case SkillShotType.SkillshotLine:
                    Polygon = Rectangle.ToPolygon();
                    EvadePolygon = Rectangle.ToPolygon(Config.ExtraEvadeDistance);
                    PathFindingPolygon = Rectangle.ToPolygon(Config.PathFindingDistance);
                    PathFindingInnerPolygon = Rectangle.ToPolygon(Config.PathFindingDistance2);
                    DrawingPolygon = Rectangle.ToPolygon(
                        0,
                        !SpellData.AddHitbox
                            ? SpellData.Radius
                            : (SpellData.Radius - ObjectManager.Player.BoundingRadius));
                    break;
                case SkillShotType.SkillshotMissileLine:
                    Polygon = Rectangle.ToPolygon();
                    EvadePolygon = Rectangle.ToPolygon(Config.ExtraEvadeDistance);
                    PathFindingPolygon = Rectangle.ToPolygon(Config.PathFindingDistance);
                    PathFindingInnerPolygon = Rectangle.ToPolygon(Config.PathFindingDistance2);
                    DrawingPolygon = Rectangle.ToPolygon(
                        0,
                        !SpellData.AddHitbox
                            ? SpellData.Radius
                            : (SpellData.Radius - ObjectManager.Player.BoundingRadius));
                    break;
                case SkillShotType.SkillshotCone:
                    Polygon = Sector.ToPolygon();
                    EvadePolygon = Sector.ToPolygon(Config.ExtraEvadeDistance);
                    PathFindingPolygon = Sector.ToPolygon(Config.PathFindingDistance);
                    PathFindingInnerPolygon = Sector.ToPolygon(Config.PathFindingDistance2);
                    DrawingPolygon = Polygon;
                    break;
                case SkillShotType.SkillshotRing:
                    Polygon = Ring.ToPolygon();
                    EvadePolygon = Ring.ToPolygon(Config.ExtraEvadeDistance);
                    PathFindingPolygon = Ring.ToPolygon(Config.PathFindingDistance);
                    PathFindingInnerPolygon = Ring.ToPolygon(Config.PathFindingDistance2);
                    DrawingPolygon = Polygon;
                    break;
                case SkillShotType.SkillshotArc:
                    Polygon = Arc.ToPolygon();
                    EvadePolygon = Arc.ToPolygon(Config.ExtraEvadeDistance);
                    PathFindingPolygon = Arc.ToPolygon(Config.PathFindingDistance);
                    PathFindingInnerPolygon = Arc.ToPolygon(Config.PathFindingDistance2);
                    DrawingPolygon = Polygon;
                    break;
            }
        }

        /// <summary>
        /// Returns the missile position after time time.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public Vector2 GlobalGetMissilePosition(int time)
        {
            var t = Math.Max(0, Utils.TickCount + time - StartTick - SpellData.Delay);
            t = (int)Math.Max(0, Math.Min(End.Distance(Start), t * SpellData.MissileSpeed / 1000));
            return Start + Direction * t;
        }

        /// <summary>
        /// Returns the missile position after time time.
        /// </summary>
        public Vector2 GetMissilePosition(int time)
        {
            var t = Math.Max(0, Utils.TickCount + time - StartTick - SpellData.Delay);
            var x = 0;

            // Missile with acceleration = 0.
            if (SpellData.MissileAccel == 0)
            {
                x = t * SpellData.MissileSpeed / 1000;
            }
            // Missile with constant acceleration.
            else
            {
                var t1 = (SpellData.MissileAccel > 0
                    ? SpellData.MissileMaxSpeed
                    : SpellData.MissileMinSpeed - SpellData.MissileSpeed) * 1000f / SpellData.MissileAccel;

                if (t <= t1)
                {
                    x = (int)(t * SpellData.MissileSpeed / 1000d + 0.5d * SpellData.MissileAccel * Math.Pow(t / 1000d, 2));
                }
                else
                {
                    x = (int)(t1 * SpellData.MissileSpeed / 1000d + 0.5d * SpellData.MissileAccel * Math.Pow(t1 / 1000d, 2)
                        + (t - t1) / 1000d * (SpellData.MissileAccel < 0 ? SpellData.MissileMinSpeed : SpellData.MissileMaxSpeed));
                }
            }

            t = (int)Math.Max(0, Math.Min(CollisionEnd.Distance(Start), x));
            return Start + Direction * t;
        }

        /// <summary>
        /// Returns if the skillshot will hit you when trying to blink to the point.
        /// </summary>
        public bool IsSafeToBlink(Vector2 point, int timeOffset, int delay = 0)
        {
            timeOffset /= 2;

            if (IsSafe(Program.PlayerPosition))
            {
                return true;
            }

            // Skillshots with missile
            if (SpellData.Type == SkillShotType.SkillshotMissileLine)
            {
                var missilePositionAfterBlink = GetMissilePosition(delay + timeOffset);
                var myPositionProjection = Program.PlayerPosition.ProjectOn(Start, End);

                if (missilePositionAfterBlink.Distance(End) < myPositionProjection.SegmentPoint.Distance(End))
                {
                    return false;
                }

                return true;
            }

            // Skillshots without missile
            var timeToExplode = SpellData.Delay + SpellData.ExtraDuration +
                (int)(1000 * Start.Distance(End) / SpellData.MissileSpeed) -
                (Utils.TickCount - StartTick);

            return timeToExplode > timeOffset + delay;
        }

        /// <summary>
        /// Returns if the skillshot will hit the unit if the unit follows the path.
        /// </summary>
        public SafePathResult IsSafePath(GamePath path, int timeOffset, int speed = -1, int delay = 0, AIBaseClient unit = null)
        {
            var Distance = 0f;
            timeOffset += Game.Ping / 2;

            speed = (speed == -1) ? (int)ObjectManager.Player.MoveSpeed : speed;

            if (unit == null)
            {
                unit = ObjectManager.Player;
            }

            var allIntersections = new List<FoundIntersection>();
            for (var i = 0; i <= path.Count - 2; i++)
            {
                var from = path[i];
                var to = path[i + 1];
                var segmentIntersections = new List<FoundIntersection>();

                for (var j = 0; j <= Polygon.Points.Count - 1; j++)
                {
                    var sideStart = Polygon.Points[j];
                    var sideEnd = Polygon.Points[j == (Polygon.Points.Count - 1) ? 0 : j + 1];

                    var intersection = from.Intersection(to, sideStart, sideEnd);

                    if (intersection.Intersects)
                    {
                        segmentIntersections.Add(
                            new FoundIntersection(
                                Distance + intersection.Point.Distance(from),
                                (int)((Distance + intersection.Point.Distance(from)) * 1000 / speed),
                                intersection.Point, from));
                    }
                }

                var sortedList = segmentIntersections.OrderBy(o => o.Distance).ToList();
                allIntersections.AddRange(sortedList);

                Distance += from.Distance(to);
            }

            // Skillshot with missile
            if (SpellData.Type == SkillShotType.SkillshotMissileLine ||
                SpellData.Type == SkillShotType.SkillshotArc)
            {
                // Outside the skillshot
                if (IsSafe(Program.PlayerPosition))
                {
                    // No intersections -> Safe
                    if (allIntersections.Count == 0)
                    {
                        return new SafePathResult(true, new FoundIntersection());
                    }

                    if (SpellData.DontCross)
                    {
                        return new SafePathResult(false, allIntersections[0]);
                    }

                    for (var i = 0; i <= allIntersections.Count - 1; i = i + 2)
                    {
                        var enterIntersection = allIntersections[i];
                        var enterIntersectionProjection = enterIntersection.Point.ProjectOn(Start, End).SegmentPoint;

                        // Intersection with no exit point.
                        if (i == allIntersections.Count - 1)
                        {
                            var missilePositionOnIntersection = GetMissilePosition(enterIntersection.Time - timeOffset);
                            return new SafePathResult(
                                (End.Distance(missilePositionOnIntersection) + 50 <=
                                End.Distance(enterIntersectionProjection)) &&
                                ObjectManager.Player.MoveSpeed < SpellData.MissileSpeed, allIntersections[0]);
                        }

                        var exitIntersection = allIntersections[i + 1];
                        var exitIntersectionProjection = exitIntersection.Point.ProjectOn(Start, End).SegmentPoint;

                        var missilePosOnEnter = GetMissilePosition(enterIntersection.Time - timeOffset);
                        var missilePosOnExit = GetMissilePosition(enterIntersection.Time + timeOffset);

                        // Missile didnt pass.
                        if (missilePosOnEnter.Distance(End) + 50 > enterIntersectionProjection.Distance(End))
                        {
                            if (missilePosOnExit.Distance(End) <= exitIntersectionProjection.Distance(End))
                            {
                                return new SafePathResult(false, allIntersections[0]);
                            }
                        }
                    }

                    return new SafePathResult(true, allIntersections[0]);
                }

                // Inside the skillshot.
                if (allIntersections.Count == 0)
                {
                    return new SafePathResult(false, new FoundIntersection());
                }

                if (allIntersections.Count > 0)
                {
                    // Check only for the exit point.
                    var exitIntersection = allIntersections[0];
                    var exitIntersectionProjection = exitIntersection.Point.ProjectOn(Start, End).SegmentPoint;

                    var missilePosOnExit = GetMissilePosition(exitIntersection.Time + timeOffset);

                    if (missilePosOnExit.Distance(End) <= exitIntersectionProjection.Distance(End))
                    {
                        return new SafePathResult(false, allIntersections[0]);
                    }
                }
            }

            if (IsSafe(Program.PlayerPosition))
            {
                if (allIntersections.Count == 0)
                {
                    return new SafePathResult(true, new FoundIntersection());
                }

                if (SpellData.DontCross)
                {
                    return new SafePathResult(false, allIntersections[0]);
                }
            }
            else
            {
                if (allIntersections.Count == 0)
                {
                    return new SafePathResult(false, new FoundIntersection());
                }
            }

            var timeToExplode = (SpellData.DontAddExtraDuration ? 0 : SpellData.ExtraDuration) + SpellData.Delay +
                (int)(1000 * Start.Distance(End) / SpellData.MissileSpeed) -
                (Utils.TickCount - StartTick);

            var myPositionWhenExplodes = path.PositionAfter(timeToExplode, speed, delay);

            if (!IsSafe(myPositionWhenExplodes))
            {
                return new SafePathResult(false, allIntersections[0]);
            }

            var myPositionWhenExplodesWithOffset = path.PositionAfter(timeToExplode, speed, timeOffset);

            return new SafePathResult(IsSafe(myPositionWhenExplodesWithOffset), allIntersections[0]);
        }

        public bool IsSafe(Vector2 point)
        {
            return Polygon.IsOutside(point);
        }

        public bool IsDanger(Vector2 point)
        {
            return !IsSafe(point);
        }

        /// <summary>
        /// Returns if the skillshot is about to hit the unit in the next time seconds.
        /// </summary>
        public bool IsAboutToHit(int time, AIBaseClient unit)
        {
            if (SpellData.Type == SkillShotType.SkillshotMissileLine)
            {
                var missilePos = GetMissilePosition(0);
                var missilePosAfterT = GetMissilePosition(time);

                var projection = unit.Position.ToVector2().ProjectOn(missilePos, missilePosAfterT);

                if (projection.IsOnSegment && projection.SegmentPoint.Distance(unit.Position) < SpellData.Radius)
                {
                    return true;
                }

                return false;
            }

            if (!IsSafe(unit.Position.ToVector2()))
            {
                var timeToExplode = SpellData.Delay + SpellData.ExtraDuration +
                    (int)(1000 * Start.Distance(End) / SpellData.MissileSpeed) -
                    (Utils.TickCount - StartTick);

                if (timeToExplode <= time)
                {
                    return true;
                }
            }

            return false;
        }

        public void Draw(Color color, Color missileColor, int width = 1)
        {
            if (!GetValue<MenuBool>("Draw").Value)
            {
                return;
            }

            DrawingPolygon.Draw(color, width);

            if (SpellData.Type == SkillShotType.SkillshotMissileLine)
            {
                var position = Rectangle.RStart;
                Utils.DrawLineInWorld(
                    (position + SpellData.Radius * Direction.Perpendicular()).ToVector3(),
                    (position - SpellData.Radius * Direction.Perpendicular()).ToVector3(), 2, missileColor);
            }
        }
    }
}