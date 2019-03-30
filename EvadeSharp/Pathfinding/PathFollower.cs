using System;
using System.Collections.Generic;
using EnsoulSharp;
using EnsoulSharp.SDK;
using SharpDX;

namespace Evade.Pathfinding
{
    public static class PathFollower
    {
        public static List<Vector2> Path = new List<Vector2>();

        public static bool IsFollowing
        {
            get
            {
                return Path.Count > 0;
            }
        }

        public static void KeepFollowingPath(EventArgs args)
        {
            if (Path.Count > 0)
            {
                while (Path.Count > 0 && Program.PlayerPosition.Distance(Path[0]) < 80)
                {
                    Path.RemoveAt(0);
                }

                if (Path.Count > 0)
                {
                    ObjectManager.Player.SendMovePacket(Path[0]);
                }
            }
        }

        public static void Follow(List<Vector2> path)
        {
            Path = path;
        }

        public static void Stop()
        {
            Path = new List<Vector2>();
        }
    }
}