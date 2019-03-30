using System.Collections.Generic;
using SharpDX;

namespace Evade.Pathfinding
{
    public class Node
    {
        public Vector2 Point;
        public List<Node> Neightbours;

        public Node(Vector2 point)
        {
            Point = point;
            Neightbours = new List<Node>();
        }
    }
}