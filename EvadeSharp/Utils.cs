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
using EnsoulSharp;
using EnsoulSharp.SDK;
using SharpDX;
using Color = System.Drawing.Color;

#endregion

namespace Evade
{
    public static class Utils
    {
        public static int TickCount
        {
            get { return (int)(Game.Time * 1000f); }
        }

        public static List<Vector2> To2DList(this Vector3[] v)
        {
            var result = new List<Vector2>();
            foreach (var point in v)
            {
                result.Add(point.ToVector2());
            }
            return result;
        }

        public static void SendMovePacket(this AIBaseClient v, Vector2 point)
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, point.ToVector3(), false);
        }

        public static AIBaseClient Closest(List<AIBaseClient> targetList, Vector2 from)
        {
            var dist = float.MaxValue;
            AIBaseClient result = null;

            foreach (var target in targetList)
            {
                var distance = Vector2.DistanceSquared(from, target.Position.ToVector2());
                if (distance < dist)
                {
                    dist = distance;
                    result = target;
                }
            }

            return result;
        }

        public static bool LineSegmentsCross(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            var denominator = ((b.X - a.X) * (d.Y - c.Y)) - ((b.Y - a.Y) * (d.X - c.X));

            if (denominator == 0)
            {
                return false;
            }

            var numerator1 = ((a.Y - c.Y) * (d.X - c.X)) - ((a.X - c.X) * (d.Y - c.Y));

            var numerator2 = ((a.Y - c.Y) * (b.X - a.X)) - ((a.X - c.X) * (b.Y - a.Y));

            if (numerator1 == 0 || numerator2 == 0)
            {
                return false;
            }

            var r = numerator1 / denominator;
            var s = numerator2 / denominator;

            return (r > 0 && r < 1) && (s > 0 && s < 1);
        }

        /// <summary>
        /// Returns when the unit will be able to move again
        /// </summary>
        public static int ImmobileTime(AIBaseClient unit)
        {
            var result = 0f;

            foreach (var buff in unit.Buffs)
            {
                if (buff.IsActive &&
                    (buff.Type == BuffType.Stun || buff.Type == BuffType.Taunt || buff.Type == BuffType.Polymorph
                    || buff.Type == BuffType.Fear || buff.Type == BuffType.Charm || buff.Type == BuffType.Suppression
                    || buff.Type == BuffType.Flee || buff.Type == BuffType.Knockup || buff.Type == BuffType.Knockback
                    || buff.Type == BuffType.Disarm || buff.Type == BuffType.Asleep))
                {
                    result = Math.Max(result, buff.EndTime);
                }
            }

            return (result == 0f) ? -1 : (int)(Utils.TickCount + (result - Game.Time) * 1000);
        }

        public static void DrawLineInWorld(Vector3 start, Vector3 end, int width, Color color)
        {
            var from = Drawing.WorldToScreen(start);
            var to = Drawing.WorldToScreen(end);
            Drawing.DrawLine(from.X, from.Y, to.X, to.Y, width, color);
        }
    }

    internal class SpellList<T> : List<T>
    {
        public event EventHandler OnAdd;

        public new void Add(T item)
        {
            if (OnAdd != null)
            {
                OnAdd(this, null);
            }

            base.Add(item);
        }
    }
}