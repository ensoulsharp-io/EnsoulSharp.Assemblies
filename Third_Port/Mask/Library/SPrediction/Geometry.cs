/*
 Copyright 2015 - 2015 SPrediction
 Geometry.cs is part of SPrediction
 
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

using EnsoulSharp;
using EnsoulSharp.SDK;

using SharpDX;

namespace SPrediction
{
    /// <summary>
    /// SPrediction Geometry class
    /// </summary>
    public static class Geometry
    {
        //from Esk0r's evade's geometry class, orginal code: https://github.com/Esk0r/LeagueSharp/blob/master/Evade/Geometry.cs
        public class Polygon
        {
            public List<Vector2> Points = new List<Vector2>();

            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="poly">Polygons to combine</param>
            public Polygon(params Polygon[] poly)
            {
                for (int i = 0; i < poly.Length; i++)
                    Points.AddRange(poly[i].Points);
            }

            /// <summary>
            /// Adds point to polygon
            /// </summary>
            /// <param name="point">Point</param>
            public void Add(Vector2 point)
            {
                Points.Add(point);
            }

            /// <summary>
            /// Draws polygon
            /// </summary>
            /// <param name="width">Line width</param>
            public void Draw(int width = 1)
            {
                for (var i = 0; i < Points.Count; i++)
                {
                    var nextIndex = (Points.Count - 1 == i) ? 0 : (i + 1);
                    var start = Points[i].ToVector3();
                    var end = Points[nextIndex].ToVector3();
                    var from = Drawing.WorldToScreen(start);
                    var to = Drawing.WorldToScreen(end);
                    Drawing.DrawLine(from[0], from[1], to[0], to[1], width, System.Drawing.Color.White);
                }
            }
        }

        /// <summary>
        /// Circle class
        /// </summary>
        internal class Circle
        {
            private const int CircleLineSegmentN = 22;
            public Vector2 Center;
            public float Radius;

            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="x">center x</param>
            /// <param name="y">center y</param>
            /// <param name="r">radius</param>
            public Circle(float x, float y, float r)
            {
                Center = new Vector2(x, y);
                Radius = r;
            }

            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="c">Center</param>
            /// <param name="r">radius</param>
            public Circle(Vector2 c, float r)
            {
                Center = c;
                Radius = r;
            }

            /// <summary>
            /// Gets Circle as Polygon
            /// </summary>
            public Polygon Polygons
            {
                get
                {
                    var result = new Polygon();
                    var outRadius = (Radius) / (float)Math.Cos(2 * Math.PI / CircleLineSegmentN);

                    for (var i = 1; i <= CircleLineSegmentN; i++)
                    {
                        var angle = i * 2 * Math.PI / CircleLineSegmentN;
                        Vector2 point = new Vector2(
                            Center.X + outRadius * (float)Math.Cos(angle), Center.Y + outRadius * (float)Math.Sin(angle));
                        result.Add(point);
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// Rectangle class
        /// </summary>
        internal class Rectangle
        {
            public Vector2 Direction;
            public Vector2 Perpendicular;
            public Vector2 REnd;
            public Vector2 RStart;
            public float Width;

            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="start">Start pos</param>
            /// <param name="end">End pos</param>
            /// <param name="width">Scale</param>
            public Rectangle(Vector2 start, Vector2 end, float width)
            {
                RStart = start;
                REnd = end;
                Width = width;
                Direction = (end - start).Normalized();
                Perpendicular = Direction.Perpendicular();
            }

            /// <summary>
            /// Gets Rectangle as polygon
            /// </summary>
            public Polygon Polygons
            {
                get
                {
                    var result = new Polygon();

                    result.Add(RStart + Width * Perpendicular);
                    result.Add(RStart - Width * Perpendicular);
                    result.Add(REnd - Width * Perpendicular);
                    result.Add(REnd + Width * Perpendicular);

                    return result;
                }
            }
        }

        /// <summary>
        /// Sector class
        /// </summary>
        internal class Sector
        {
            private const int CircleLineSegmentN = 22;
            public float Angle;
            public Vector2 Center;
            public Vector2 Direction;
            public float Radius;

            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="center">Center</param>
            /// <param name="direction">Direction</param>
            /// <param name="angle">Angle</param>
            /// <param name="radius">Radius</param>
            public Sector(Vector2 center, Vector2 direction, float angle, float radius)
            {
                Center = center;
                Direction = direction;
                Angle = angle;
                Radius = radius;
            }

            /// <summary>
            /// Gets sector as polygon
            /// </summary>
            public Polygon Polygons
            {
                get
                {
                    var result = new Polygon();
                    var outRadius = Radius / (float)Math.Cos(2 * Math.PI / CircleLineSegmentN);

                    result.Add(Center);
                    var Side1 = Direction.Rotated(-Angle * 0.5f);

                    for (var i = 0; i <= CircleLineSegmentN; i++)
                    {
                        var cDirection = Side1.Rotated(i * Angle / CircleLineSegmentN).Normalized();
                        result.Add(new Vector2(Center.X + outRadius * cDirection.X, Center.Y + outRadius * cDirection.Y));
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// Arc class
        /// </summary>
        internal class Arc
        {
            public Vector2 Center;
            public Vector2 Direction;
            public float Width;
            public float Height;
            public float Angle;

            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="x">Center x</param>
            /// <param name="y">Center y</param>
            /// <param name="direction">Direction</param>
            /// <param name="angle">Angle</param>
            /// <param name="w">Width</param>
            /// <param name="h">Height</param>
            public Arc(float x, float y, Vector2 direction, float angle, float w, float h)
            {
                Center = new Vector2(x, y);
                Direction = direction;
                Angle = angle;
                Width = w;
                Height = h;
            }

            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="c">Center</param>
            /// <param name="direction">Direction</param>
            /// <param name="angle">Angle</param>
            /// <param name="w">Width</param>
            /// <param name="h">Height</param>
            public Arc(Vector2 c, Vector2 direction, float angle, float w, float h)
            {
                Center = c;
                Direction = direction;
                Angle = angle;
                Width = w;
                Height = h;
            }

            /// <summary>
            /// Gets arc as polygon
            /// </summary>
            public Polygon Polygons
            {
                get
                {
                    Polygon result = new Polygon();

                    double aStep;            // Angle Step (rad)

                    // Angle step in rad
                    if (Width < Height)
                    {
                        if (Width < 1.0e-4)
                            aStep = 1.0;
                        else
                            aStep = Math.Asin(2.0 / Width);
                    }
                    else
                    {
                        if (Height < 1.0e-4)
                            aStep = 1.0;
                        else
                            aStep = Math.Asin(2.0 / Height);
                    }

                    if (aStep < 0.05)
                        aStep = 0.05;

                    Vector2 v1 = new Vector2(Center.X + (float)Math.Cos(0) * Width, Center.Y - (float)Math.Sin(0) * Height);

                    float rotAngle = (float)Math.Atan2(Direction.Y - v1.Y, Direction.X - v1.X) - (float)(Math.PI * 180.0 / 180.0);
                    for (double a = 0; a <= Angle; a += aStep)
                        result.Add(new Vector2(Center.X + (float)Math.Cos(a) * Width, Center.Y - (float)Math.Sin(a) * Height).RotateAroundPoint(v1, rotAngle));

                    return result;
                }
            }
        }

        /// <summary>
        /// Gets position with time
        /// </summary>
        /// <param name="t">Time</param>
        /// <param name="speed">Move speed</param>
        /// <param name="delay">Delay to start</param>
        /// <returns></returns>
        internal static Vector2 PositionAfter(List<Vector2> self, float t, float speed, float delay = 0)
        {
            var distance = Math.Max(0, t - delay) * speed / 1000;
            for (var i = 0; i <= self.Count - 2; i++)
            {
                var from = self[i];
                var to = self[i + 1];
                var d = (int)to.Distance(from);
                if (d > distance)
                {
                    return from + distance * (to - from).Normalized();
                }
                distance -= d;
            }
            return self[self.Count - 1];
        }

        /// <summary>
        /// Gets closest on circle point
        /// </summary>
        /// <param name="center">Circle center</param>
        /// <param name="radius">Circle radius</param>
        /// <param name="pointStart">Point start</param>
        /// <param name="pointEnd">Point end</param>
        /// <returns></returns>
        internal static Vector2 ClosestCirclePoint(Vector2 center, float radius, Vector2 point)
        {
            Vector2 v = (point - center).Normalized();
            return center + v * radius;
        }

        internal static Vector2 Deviation(Vector2 point1, Vector2 point2, double angle)
        {
            angle *= Math.PI / 180.0;
            Vector2 temp = Vector2.Subtract(point2, point1);
            Vector2 result = new Vector2(0);
            result.X = (float)(temp.X * Math.Cos(angle) - temp.Y * Math.Sin(angle)) / 4;
            result.Y = (float)(temp.X * Math.Sin(angle) + temp.Y * Math.Cos(angle)) / 4;
            result = Vector2.Add(result, point1);
            return result;
        }

        internal static bool IsBetween(this Vector2 b, Vector2 a, Vector2 c)
        {
            return a.Distance(c) + c.Distance(b) - a.Distance(b) < float.Epsilon;
        }
    }
}
