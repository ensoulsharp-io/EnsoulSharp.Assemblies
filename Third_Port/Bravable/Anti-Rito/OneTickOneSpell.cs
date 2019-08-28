using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;

namespace Anti_Rito
{
    public static class OneTickOneSpell
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static LastSpellCast LastSpell = new LastSpellCast();
        public static List<LastSpellCast> LastSpellsCast = new List<LastSpellCast>();
        public static int BlockedCount = 0;
        public static Menu menu, config, OneSpell;


        public static void Init()
        {
            menu = new Menu("Anti Rito", "Anti Rito", true);
            menu.Add(new MenuSeparator("Anti Rito", "Anti Rito"));
            menu.Add(new MenuSeparator("Version", "Version: " + "1.0.0.0"));
            menu.Add(new MenuSeparator("Credit", "MostlyPride"));
            menu.Add(new MenuSeparator("Rep", "+Rep If you use this :)"));

            OneSpell = menu.Add(new Menu("OneSpellOneTick", "OneSpellOneTick"));
            OneSpell.Add(new MenuSeparator("OneSpellOneTick Settings", "OneSpellOneTick Settings"));
            OneSpell.Add(new MenuBool("Enable", "Enable"));
            OneSpell.Add(new MenuBool("Drawing", "Draw Block Count"));

            //OneSpell.Add(new MenuBool("Recast", "Re-cast blocked spell after a delay?");

            menu.Attach();


            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Drawing.OnDraw += onDrawArgs =>
            {
                if (OneSpell["Drawing"].GetValue<MenuBool>().Enabled)
                {
                    Drawing.DrawText(180, 100, System.Drawing.Color.Aqua, "Blocked" + BlockedCount + "Spells");
                    //(Drawing.Width - 180, 100, System.Drawing.Color.Lime, "Blocked " + BlockedCount + " Spells");
                }
            };
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!OneSpell["Enable"].GetValue<MenuBool>().Enabled)
                return;
            if (!sender.Owner.IsMe)
                return;
            if (!(new SpellSlot[] {SpellSlot.Q,SpellSlot.W,SpellSlot.E,SpellSlot.R,SpellSlot.Summoner1,SpellSlot.Summoner2
                ,SpellSlot.Item1,SpellSlot.Item2,SpellSlot.Item3,SpellSlot.Item4,SpellSlot.Item5,SpellSlot.Item6,SpellSlot.Trinket})
                .Contains(args.Slot))
                return;
            if (Environment.TickCount - LastSpell.CastTick < 50)
            {
                args.Process = false;
                BlockedCount += 1;
            }
            else
            {
                LastSpell = new LastSpellCast() { Slot = args.Slot, CastTick = Environment.TickCount };
            }
            if (LastSpellsCast.Any(x => x.Slot == args.Slot))
            {
                LastSpellCast spell = LastSpellsCast.FirstOrDefault(x => x.Slot == args.Slot);
                if (spell != null)
                {
                    if (Environment.TickCount - spell.CastTick <= 250 + Game.Ping)
                    {
                        args.Process = false;
                        BlockedCount += 1;
                    }
                    else
                    {
                        LastSpellsCast.RemoveAll(x => x.Slot == args.Slot);
                        LastSpellsCast.Add(new LastSpellCast() { Slot = args.Slot, CastTick = Environment.TickCount });
                    }
                }
                else
                {
                    LastSpellsCast.Add(new LastSpellCast() { Slot = args.Slot, CastTick = Environment.TickCount });
                }
            }
            else
            {
                LastSpellsCast.Add(new LastSpellCast() { Slot = args.Slot, CastTick = Environment.TickCount });
            }
        }
        public class LastSpellCast
        {
            public SpellSlot Slot = SpellSlot.Unknown;
            public int CastTick = 0;
        }
    }
}
