using System;

using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;

using SebbyLib;

using SharpDX;

namespace OneKeyToWin_AIO_Sebby.Core
{
    class MissileReturn
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static Menu Config = Program.Config;

        private string MissileName, MissileReturnName;
        private Spell QWER;
        private Menu menu;
        private Vector3 MissileEndPos;
        public AIHeroClient Target;
        public MissileClient Missile;

        public MissileReturn(string missile, string missileReturnName, Spell qwer)
        {
            menu = Config[Player.CharacterName] as Menu;

            var aaos = new Menu("AAOS", "Auto AIM OKTW system");
            aaos.Add(new MenuBool("aim", "Auto aim returned missile", true, Player.CharacterName));
            (menu[qwer.Slot + "Config"] as Menu).Add(aaos);
            (menu["Draw"] as Menu).Add(new MenuBool("drawHelper", "Show " + qwer.Slot + " helper", true, Player.CharacterName));

            MissileName = missile;
            MissileReturnName = missileReturnName;
            QWER = qwer;
            Target = null;

            GameObject.OnMissileCreate += GameObject_OnMissileCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            AIBaseClient.OnDoCast += AIBaseClient_OnDoCast;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (Missile != null && Missile.IsValid && (menu["Draw"] as Menu).GetValue<MenuBool>("drawHelper").Enabled)
                OktwCommon.DrawLineRectangle(Missile.Position, Player.Position, (int)QWER.Width, 1, System.Drawing.Color.White);
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if ((menu[QWER.Slot + "Config"]["AAOS"] as Menu).GetValue<MenuBool>("aim").Enabled)
            {
                var posPred = CalculateReturnPos();
                if (posPred != Vector3.Zero)
                    Orbwalker.SetOrbwalkerPosition(posPred);
                else
                    Orbwalker.SetOrbwalkerPosition(Game.CursorPos);
            }
            else
                Orbwalker.SetOrbwalkerPosition(Game.CursorPos);
        }

        private void AIBaseClient_OnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.Slot == QWER.Slot)
            {
                MissileEndPos = args.To;
            }
        }

        private void GameObject_OnMissileCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsEnemy || sender.Type != GameObjectType.MissileClient || !sender.IsValid)
                return;

            var missile = (MissileClient)sender;

            if (missile.SData.Name != null)
            {
                if (missile.SData.Name.ToLower() == MissileName.ToLower() || missile.SData.Name.ToLower() == MissileReturnName.ToLower())
                {
                    Missile = missile;
                }
            }
        }

        private void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.IsEnemy || sender.Type != GameObjectType.MissileClient || !sender.IsValid)
                return;

            var missile = (MissileClient)sender;

            if (missile.SData.Name != null)
            {
                if (missile.SData.Name.ToLower() == MissileName.ToLower() || missile.SData.Name.ToLower() == MissileReturnName.ToLower())
                {
                    Missile = null;
                }
            }
        }

        public Vector3 CalculateReturnPos()
        {
            if (Missile != null && Missile.IsValid && Target.IsValidTarget())
            {
                var finishPosition = Missile.Position;
                if (Missile.SData.Name.ToLower() == MissileName.ToLower())
                {
                    finishPosition = MissileEndPos;
                }

                var misToPlayer = Player.Distance(finishPosition);
                var tarToPlayer = Player.Distance(Target);

                if (misToPlayer > tarToPlayer)
                {
                    var misToTarget = Target.Distance(finishPosition);

                    if (misToTarget < QWER.Range && misToTarget > 50)
                    {
                        var cursorToTarget = Target.Distance(Player.Position.Extend(Game.CursorPos, 100));
                        var ext = finishPosition.Extend(Target.PreviousPosition, cursorToTarget + misToTarget);

                        if (ext.Distance(Player) < 800 && ext.CountEnemyHeroesInRange(400) < 2)
                        {
                            return ext;
                        }
                    }
                }
            }
            return Vector3.Zero;
        }
    }
}
