using System;
using EnsoulSharp;
using EnsoulSharp.SDK;

namespace BadaoActionsLimiter
{
    public static class AttackBlock
    {
        public static int AttackBlockCount;
        public static int LastAutoAttack;

        public static void BadaoActivate()
        {
            Spellbook.OnStopCast += Spellbook_OnStopCast;
            Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
            AIBaseClient.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Spellbook_OnStopCast(AIBaseClient sender, SpellbookStopCastEventArgs args)
        {
            if (sender.IsValid && sender.IsMe && args.DestroyMissile && args.KeepAnimationPlaying)
            {
                LastAutoAttack = 0;
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;
            if (Orbwalker.IsAutoAttack(args.SData.Name))
            {
                LastAutoAttack = 0;
            }
            if (Orbwalker.IsAutoAttackReset(args.SData.Name))
            {
                LastAutoAttack = 0;
            }
        }

        private static void Obj_AI_Base_OnIssueOrder(AIBaseClient sender, PlayerIssueOrderEventArgs args)
        {
            if (!sender.IsMe)
                return;
            if (args.Order != GameObjectOrder.AttackUnit)
                return;
            var limitTick = 1f / ObjectManager.Player.AttackDelay > 4.5f ? 6 : 5;
            if (Environment.TickCount - LastAutoAttack < 100)
            {
                args.Process = false;
                AttackBlockCount += 1;
            }
            else
            {
                LastAutoAttack = Environment.TickCount;
            }
        }
    }
}
