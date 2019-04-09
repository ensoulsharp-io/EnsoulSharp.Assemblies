using System;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Core.UI.IMenu;
using EnsoulSharp.SDK.Core.UI.IMenu.Values;
using EnsoulSharp.SDK.Core.Utils;
using SebbyLib;
using SharpDX;

namespace OneKeyToWin_AIO_Sebby.Core
{
    class MissileReturn
    {
        public AIHeroClient Target;
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static Menu Config = Program.Config;
        private string MissileName, MissileReturnName;
        private Spell QWER;
        public MissileClient Missile;
        private Vector3 MissileEndPos;

        public MissileReturn(string missileName, string missileReturnName, Spell qwer)
        {
            var autoaim = new Menu("autoaim", "Auto AIM OKTW system");
            autoaim.Add(new MenuBool("aim", "Auto aim returned missile", true, Player.CharacterName));

            (Config[Player.CharacterName][qwer.Slot + "config"] as Menu).Add(autoaim);
            (Config[Player.CharacterName]["draw"] as Menu).Add(new MenuBool("drawHelper", "Show " + qwer.Slot + " helper", true, Player.CharacterName));

            MissileName = missileName;
            MissileReturnName = missileReturnName;
            QWER = qwer;

            GameObject.OnAssign += GameObject_OnAssign;
            GameObject.OnDelete += GameObject_OnDelete;
            AIBaseClient.OnDoCast += AIBaseClient_OnDoCast;
            Drawing.OnDraw += Drawing_OnDraw;
            Variables.Orbwalker.OnAction += Orbwalker_OnAction;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (Missile != null && Missile.IsValid && Config[Player.CharacterName]["draw"].GetValue<MenuBool>("drawHelper").Value)
                OktwCommon.DrawLineRectangle(Missile.Position, Player.Position, (int)QWER.Width, 1, System.Drawing.Color.White);
        }

        private void Orbwalker_OnAction(object sender, OrbwalkingActionArgs args)
        {
            if (args.Type == OrbwalkingType.Movement && Config[Player.CharacterName][QWER.Slot + "config"]["autoaim"].GetValue<MenuBool>("aim").Value)
            {
                var posPred = CalculateReturnPos();
                if (posPred != Vector3.Zero)
                    args.Position = posPred;
                else
                    args.Position = Game.CursorPosRaw;
            }
        }

        private void AIBaseClient_OnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.Slot == QWER.Slot)
            {
                MissileEndPos = args.To;
            }
        }

        private void GameObject_OnAssign(GameObject sender, EventArgs args)
        {
            if (sender.IsEnemy || sender.Type != GameObjectType.MissileClient || !sender.IsValid)
                return;

            var missile = sender as MissileClient;

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
            if (sender.IsEnemy || sender.Type != GameObjectType.MissileClient && !sender.IsValid)
                return;

            var missile = sender as MissileClient;

            if (missile.SData.Name != null)
            {
                if (missile.SData.Name.ToLower() == MissileReturnName.ToLower())
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
                        var cursorToTarget = Target.Distance(Player.Position.Extend(Game.CursorPosRaw, 100));
                        var ext = finishPosition.Extend(Target.Position, cursorToTarget + misToTarget);

                        if (ext.Distance(Player.Position) < 800 && ext.CountEnemyHeroesInRange(400) < 2)
                        {
                            if (Config[Player.CharacterName]["draw"].GetValue<MenuBool>("drawHelper").Value)
                                Render.Circle.DrawCircle(ext, 100, System.Drawing.Color.White, 1);
                            return ext;
                        }
                    }
                }
            }
            return Vector3.Zero;
        }
    }
}