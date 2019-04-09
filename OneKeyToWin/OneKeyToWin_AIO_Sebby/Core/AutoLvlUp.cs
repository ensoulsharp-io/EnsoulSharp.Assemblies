using System;
using EnsoulSharp;
using EnsoulSharp.SDK.Core.UI.IMenu;
using EnsoulSharp.SDK.Core.UI.IMenu.Values;
using EnsoulSharp.SDK.Core.Utils;

namespace OneKeyToWin_AIO_Sebby.Core
{
    class AutoLvlUp
    {
        private Menu Config = Program.Config;
        private AIHeroClient Player { get { return ObjectManager.Player; } }
        private int lvl1, lvl2, lvl3, lvl4;

        public void LoadOKTW()
        {
            var autolvlup = new Menu("autolvlup", "AutoLvlUp OKTW©");

            autolvlup.Add(new MenuBool("AutoLvl", "ENABLE", true));
            autolvlup.Add(new MenuList<string>("1", "1", new[] { "Q", "W", "E", "R" }, Player.CharacterName) { Index = 3 });
            autolvlup.Add(new MenuList<string>("2", "2", new[] { "Q", "W", "E", "R" }, Player.CharacterName) { Index = 1 });
            autolvlup.Add(new MenuList<string>("3", "3", new[] { "Q", "W", "E", "R" }, Player.CharacterName) { Index = 1 });
            autolvlup.Add(new MenuList<string>("4", "4", new[] { "Q", "W", "E", "R" }, Player.CharacterName) { Index = 1 });
            autolvlup.Add(new MenuSlider("LvlStart", "Auto LVL start", 2, 1, 6, Player.CharacterName));

            Config.Add(autolvlup);

            Game.OnUpdate += Game_OnUpdate;
            AIHeroClient.OnLevelUp += AIHeroClient_OnLevelUp;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (!Program.LagFree(0) || !Config["autolvlup"].GetValue<MenuBool>("AutoLvl").Value)
                return;
            lvl1 = (Config["autolvlup"]["1"] as MenuList<string>).Index;
            lvl2 = (Config["autolvlup"]["2"] as MenuList<string>).Index;
            lvl3 = (Config["autolvlup"]["3"] as MenuList<string>).Index;
            lvl4 = (Config["autolvlup"]["4"] as MenuList<string>).Index;
        }

        private void AIHeroClient_OnLevelUp(AIHeroClient sender, AIHeroClientLevelUpEventArgs args)
        {
            if (!sender.IsMe || !Config["autolvlup"].GetValue<MenuBool>("AutoLvl").Value || Player.Level < Config["autolvlup"].GetValue<MenuSlider>("LvlStart").Value)
                return;
            if (lvl2 == lvl3 || lvl2 == lvl4 || lvl3 == lvl4)
                return;
            var delay = 700;
            DelayAction.Add(delay, () => Up(lvl1));
            DelayAction.Add(delay + 50, () => Up(lvl2));
            DelayAction.Add(delay + 100, () => Up(lvl3));
            DelayAction.Add(delay + 150, () => Up(lvl4));
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (Player.Level == 1 && Config["autolvlup"].GetValue<MenuBool>("AutoLvl").Value)
            {
                if ((lvl2 == lvl3 || lvl2 == lvl4 || lvl3 == lvl4) && (int)Game.Time % 2 == 0)
                {
                    Program.drawText("AutoLvlUp: PLEASE SET ABILITY SEQENCE", Player.Position, System.Drawing.Color.OrangeRed, -200, true);
                }
            }
        }

        private void Up(int index)
        {
            if (Player.Level < 4)
            {
                if (index == 0 && Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                    Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (index == 1 && Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                    Player.Spellbook.LevelSpell(SpellSlot.W);
                if (index == 2 && Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                    Player.Spellbook.LevelSpell(SpellSlot.E);
            }
            else
            {
                if (index == 0)
                    Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (index == 1)
                    Player.Spellbook.LevelSpell(SpellSlot.W);
                if (index == 2)
                    Player.Spellbook.LevelSpell(SpellSlot.E);
                if (index == 3)
                    Player.Spellbook.LevelSpell(SpellSlot.R);
            }
        }
    }
}