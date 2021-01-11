using System.Collections.Generic;

namespace OneKeyToWin_AIO_Sebby
{
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;

    class Base : Program
    {
        public static Menu Local, HarassMenu, FarmMenu;
        public static List<MenuBool> HarassList = new List<MenuBool>();
        public static MenuBool manaDisable;
        public static MenuBool harassMixed = new MenuBool("harassMixed", "Spell-harass only in mixed mode", false);

        public static float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;

        public static bool FarmSpells
        {
            get
            {
                return true;
            }
        }

        static Base()
        {
            Local = new Menu(Player.CharacterName, Player.CharacterName);

            manaDisable = new MenuBool("manaDisable", "Disable mana manager in combo", false);

            HarassMenu = new Menu("harass", "Harass");
            foreach (var enemy in GameObjects.EnemyHeroes)
            {
                var harass = new MenuBool("harass" + enemy.CharacterName, enemy.CharacterName);
                HarassList.Add(harass);
                HarassMenu.Add(harass);
            }

            FarmMenu = new Menu("farm", "Farm");

            Local.Add(HarassMenu);
            Local.Add(FarmMenu);

            Config.Add(new Menu("extra", "Extra settings OKTW©")
            {
                manaDisable,
                harassMixed
            });
            Config.Add(Local);
        }
    }
}