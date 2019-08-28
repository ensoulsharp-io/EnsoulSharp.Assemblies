using System;
using EnsoulSharp;
using EnsoulSharp.SDK;

namespace BadaoActionsLimiter
{
    public static class MovementBlock
    {
        public static int MovementBlockCount;
        public static int LastMovement;
        public static void BadaoActivate()
        {
            AIBaseClient.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
        }

        private static void Obj_AI_Base_OnIssueOrder(AIBaseClient sender, PlayerIssueOrderEventArgs args)
        {
            if (!sender.IsMe)
                return;
            if (args.Order != GameObjectOrder.MoveTo)
                return;
            if (Environment.TickCount - LastMovement < 100)
            {
                args.Process = false;
                MovementBlockCount += 1;
            }
            else
            {
                LastMovement = Environment.TickCount;
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;
            if (!Orbwalker.IsAutoAttack(args.SData.Name))
                return;

            LastMovement = 0;
        }
    }
}