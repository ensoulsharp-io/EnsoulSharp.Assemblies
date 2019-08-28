using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;

namespace BadaoActionsLimiter
{
    public static class SpellBlock
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static List<Action> Actions = new List<Action>();
        public static List<SpellSlot> Spells = new List<SpellSlot>() {SpellSlot.Q,SpellSlot.W,SpellSlot.E,SpellSlot.R,SpellSlot.Item1,SpellSlot.Item2,
        SpellSlot.Item3,SpellSlot.Item4,SpellSlot.Item5,SpellSlot.Item6,SpellSlot.Trinket};
        public static int SpellBlockCount;
        public static void BadaoActivate()
        {
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            AIBaseClient.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }
        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!sender.Owner.IsMe)
                return;
            if (!Spells.Contains(args.Slot))
                return;
            var action = Actions.FirstOrDefault(x => x.Slot == args.Slot);
            if (action == null)
            {
                Actions.Add(new Action() { Slot = args.Slot, Tick = Environment.TickCount });
                CameraControling.Spellbook_OnCastSpell(sender, args);
            }
            else
            {
                if (Environment.TickCount - action.Tick >= 200 && !Actions.Any(x => x.Slot != args.Slot && x.Tick == Environment.TickCount))
                {
                    action.Tick = Environment.TickCount;
                    CameraControling.Spellbook_OnCastSpell(sender, args);
                }
                else
                {
                    args.Process = false;
                    SpellBlockCount += 1;
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;

            if (!Spells.Contains(args.Slot))
                return;

            var action = Actions.FirstOrDefault(x => x.Slot == args.Slot);
            if (action != null)
            {
                action.Tick = 0;
            }

        }
    }
}
