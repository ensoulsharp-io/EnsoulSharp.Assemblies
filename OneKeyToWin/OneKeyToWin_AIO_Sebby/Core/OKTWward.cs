using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Core.UI.IMenu;
using EnsoulSharp.SDK.Core.UI.IMenu.Values;
using SharpDX;

namespace OneKeyToWin_AIO_Sebby.Core
{
    class HiddenObj
    {
        public int type;
        //0 - missile
        //1 - normal
        //2 - pink
        //3 - teemo trap
        public float endTime { get; set; }
        public Vector3 pos { get; set; }
    }

    class OKTWward
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static Menu Config = Program.Config;
        private static Spell Q, W, E, R;

        public static List<HiddenObj> HiddenObjList = new List<HiddenObj>();

        private Items.Item
            WardTotem = new Items.Item(3340, 600f),
            OracleLens = new Items.Item(3364, 800f),
            FarsightOrb = new Items.Item(3363, 4000f),
            StealthWard = new Items.Item(2056, 600f),
            ControlWard = new Items.Item(2055, 600f),
            NomadsMedallion = new Items.Item(3096, 600f),
            RotAscended = new Items.Item(3069, 600f),
            Frostfang = new Items.Item(3098, 600f),
            RotWatchers = new Items.Item(3092, 600f),
            TargonsBrace = new Items.Item(3097, 600f),
            RotAspect = new Items.Item(3401, 600f);

        public void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            var autoward = new Menu("autoward", "AutoWard OKTW©");

            autoward.Add(new MenuBool("AutoWard", "Auto Ward", true));
            autoward.Add(new MenuBool("AutoBuy", "Auto buy blue trinket after lvl 9"));
            autoward.Add(new MenuBool("AutoWardBlue", "Auto Blue Trinket", true));
            autoward.Add(new MenuBool("AutoWardCombo", "Only combo mode", true));
            autoward.Add(new MenuBool("AutoWardPink", "Auto ControlWard, OracleLens", true));

            Config.Add(autoward);

            Game.OnUpdate += Game_OnUpdate;
            GameObject.OnAssign += GameObject_OnAssign;
            GameObject.OnDelete += GameObject_OnDelete;
            AIBaseClient.OnDoCast += AIBaseClient_OnDoCast;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (!Program.LagFree(0) || Player.IsRecalling() || Player.IsDead)
                return;

            foreach (var obj in HiddenObjList)
            {
                if (obj.endTime < Game.Time)
                {
                    HiddenObjList.Remove(obj);
                    return;
                }
            }

            if (Config["autoward"].GetValue<MenuBool>("AutoBuy").Value && Player.InFountain() && !FarsightOrb.IsOwned() && Player.Level >= 9 && MenuGUI.IsShopOpen)
                FarsightOrb.Buy();

            if (Player.HasBuff("rengarrvision"))
                CastVisionWards(Player.Position);

            if (GameObjects.EnemyHeroes.Any(e => e.CharacterName == "Vayne" && e.IsValidTarget(1000) && e.HasBuff("vaynetumblefade")))
                CastVisionWards(Player.Position);

            AutoWardLogic();
        }

        private void AutoWardLogic()
        {
            foreach (var enemy in GameObjects.EnemyHeroes.Where(e => e.IsValid && !e.IsVisible && !e.IsDead))
            {
                var need = OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);

                if (need == null || need.PredictedPos == null)
                    continue;

                var PPDistance = need.PredictedPos.Distance(Player.Position);

                if (PPDistance > 1400)
                    continue;

                var timer = Game.Time - need.LastVisibleTime;

                if (timer > 1 && timer < 3)
                {
                    if (Program.Combo && PPDistance < 1500 && Player.CharacterName == "Quinn" && W.IsReady() && Config[Player.CharacterName].GetValue<MenuBool>("autoW").Value)
                    {
                        W.Cast();
                    }

                    if (Program.Combo && PPDistance < 800 && Player.CharacterName == "Karthus" && Q.IsReady() && Player.CountEnemyHeroesInRange(900) == 0)
                    {
                        Q.Cast(need.PredictedPos);
                    }

                    if (Program.Combo && PPDistance < 1400 && Player.CharacterName == "Ashe" && E.IsReady() && Player.CountEnemyHeroesInRange(800) == 0 && Config[Player.CharacterName]["Econfig"].GetValue<MenuBool>("autoE").Value)
                    {
                        E.Cast(Player.Position.Extend(need.PredictedPos, 5000));
                    }

                    if (Program.Combo && PPDistance < 800 && Player.CharacterName == "MissFortune" && E.IsReady() && Player.Mana > 200)
                    {
                        E.Cast(Player.Position.Extend(need.PredictedPos, 800));
                    }

                    if (!Player.IsWindingUp && PPDistance < 800 && Player.CharacterName == "Caitlyn" && W.IsReady() && Player.Mana > 200 && Config[Player.CharacterName]["Wconfig"].GetValue<MenuBool>("bushW").Value && Variables.TickCount - W.LastCastAttemptT > 2000)
                    {
                        W.Cast(need.PredictedPos);
                    }

                    if (!Player.IsWindingUp && PPDistance < 150 + R.Level * 250 && Player.CharacterName == "Teemo" && R.IsReady() && Player.Mana > 200 && Config[Player.CharacterName]["Rconfig"].GetValue<MenuBool>("bushR").Value && Variables.TickCount - W.LastCastAttemptT > 2000)
                    {
                        R.Cast(need.PredictedPos);
                    }

                    if (!Player.IsWindingUp && PPDistance < 750 && Player.CharacterName == "Jhin" && E.IsReady() && Player.Mana > 200 && Config[Player.CharacterName]["Econfig"].GetValue<MenuBool>("bushE").Value && Variables.TickCount - E.LastCastAttemptT > 2000)
                    {
                        E.Cast(need.PredictedPos);
                    }
                }

                if (timer < 4)
                {
                    if (!Program.Combo && Program.AIOmode != 2 && Config["autoward"].GetValue<MenuBool>("AutoWardCombo").Value)
                        return;

                    if (NavMesh.IsWallOfGrass(need.PredictedPos, 0))
                    {
                        if (PPDistance < 600 && Config["autoward"].GetValue<MenuBool>("AutoWard").Value)
                        {
                            if (WardTotem.IsReady)
                            {
                                WardTotem.Cast(need.PredictedPos);
                                need.LastVisibleTime = Game.Time - 5;
                            }
                            else if (StealthWard.IsReady)
                            {
                                StealthWard.Cast(need.PredictedPos);
                                need.LastVisibleTime = Game.Time - 5;
                            }
                            else if (NomadsMedallion.IsReady)
                            {
                                NomadsMedallion.Cast(need.PredictedPos);
                                need.LastVisibleTime = Game.Time - 5;
                            }
                            else if (RotAscended.IsReady)
                            {
                                RotAscended.Cast(need.PredictedPos);
                                need.LastVisibleTime = Game.Time - 5;
                            }
                            else if (Frostfang.IsReady)
                            {
                                Frostfang.Cast(need.PredictedPos);
                                need.LastVisibleTime = Game.Time - 5;
                            }
                            else if (RotWatchers.IsReady)
                            {
                                RotWatchers.Cast(need.PredictedPos);
                                need.LastVisibleTime = Game.Time - 5;
                            }
                            else if (TargonsBrace.IsReady)
                            {
                                TargonsBrace.Cast(need.PredictedPos);
                                need.LastVisibleTime = Game.Time - 5;
                            }
                            else if (RotAspect.IsReady)
                            {
                                RotAspect.Cast(need.PredictedPos);
                                need.LastVisibleTime = Game.Time - 5;
                            }
                        }

                        if (Config["autoward"].GetValue<MenuBool>("AutoWardBlue").Value)
                        {
                            if (FarsightOrb.IsReady)
                            {
                                FarsightOrb.Cast(need.PredictedPos);
                                need.LastVisibleTime = Game.Time - 5;
                            }
                        }
                    }
                }
            }
        }

        private void GameObject_OnAssign(GameObject sender, EventArgs args)
        {
            if (!sender.IsEnemy)
                return;

            var missile = sender as MissileClient;
            if (missile != null)
            {
                if (!missile.SpellCaster.IsVisible)
                {
                    if ((missile.SData.Name == "BantamTrap" || missile.SData.Name == "BantamTrapBounceSpell") && !HiddenObjList.Exists(x => missile.EndPosition == x.pos))
                        AddWard("teemorcast", missile.EndPosition);
                }
            }

            var minion = sender as AIMinionClient;
            if (minion != null)
            {
                if ((sender.Name.ToLower() == "sightward" || sender.Name.ToLower() == "visionward") && !HiddenObjList.Exists(x => x.pos.Distance(sender.Position) < 100))
                {
                    HiddenObjList.Add(new HiddenObj() { type = 1, pos = sender.Position, endTime = Game.Time + minion.Mana });
                }
            }
        }

        private void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            var minion = sender as AIMinionClient;
            if (minion != null && minion.Health < 100)
            {
                foreach (var obj in HiddenObjList)
                {
                    if (obj.pos == sender.Position)
                    {
                        HiddenObjList.Remove(obj);
                        return;
                    }
                    else if (obj.type == 3 && obj.pos.Distance(sender.Position) < 100)
                    {
                        HiddenObjList.Remove(obj);
                        return;
                    }
                    else if (obj.type == 1 && obj.pos.Distance(sender.Position) < 400)
                    {
                        if (sender.Name.ToLower() == "sightward" || sender.Name.ToLower() == "visionward")
                        {
                            HiddenObjList.Remove(obj);
                            return;
                        }
                    }
                }
            }
        }

        private void AIBaseClient_OnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender is AIHeroClient && sender.IsEnemy)
            {
                if (args.Target == null)
                    AddWard(args.SData.Name.ToLower(), args.To, sender as AIHeroClient);

                if ((OracleLens.IsReady || ControlWard.IsReady) && sender.Distance(Player.Position) < 1200)
                {
                    switch (args.SData.Name.ToLower())
                    {
                        case "deceive":
                            CastVisionWards(sender.Position);
                            break;
                        case "khazixr":
                            CastVisionWards(sender.Position);
                            break;
                        case "talonr":
                            CastVisionWards(sender.Position);
                            break;
                        case "monkeykingdecoy":
                            CastVisionWards(sender.Position);
                            break;
                        case "rengarr":
                            CastVisionWards(sender.Position);
                            break;
                        case "twitchhideinshadows":
                            CastVisionWards(sender.Position);
                            break;
                    }
                }
            }
        }

        private void AddWard(string name, Vector3 posCast, AIHeroClient sender = null)
        {
            switch (name)
            {
                case "trinkettotemlvl1":
                    HiddenObjList.Add(new HiddenObj() { type = 1, pos = posCast, endTime = Game.Time + 90 + 30 / 17 * (sender.Level - 1) });
                    break;
                case "lootedtrinkettotem":
                    HiddenObjList.Add(new HiddenObj() { type = 1, pos = posCast, endTime = Game.Time + 90 + 30 / 17 * (sender.Level - 1) });
                    break;
                case "itemghostward":
                    HiddenObjList.Add(new HiddenObj() { type = 1, pos = posCast, endTime = Game.Time + 150 });
                    break;
                case "teemorcast":
                    HiddenObjList.Add(new HiddenObj() { type = 3, pos = posCast, endTime = Game.Time + 300 });
                    break;
                case "jackinthebox":
                    HiddenObjList.Add(new HiddenObj() { type = 3, pos = posCast, endTime = Game.Time + 60 + 0.05f * sender.TotalMagicalDamage });
                    break;
            }
        }

        private void CastVisionWards(Vector3 position)
        {
            if (Config["autoward"].GetValue<MenuBool>("AutoWardPink").Value)
            {
                if (Player.Distance(position) < OracleLens.Range && OracleLens.IsReady)
                    OracleLens.Cast();
                else if (ControlWard.IsReady)
                    ControlWard.Cast(Player.Position.Extend(position, ControlWard.Range));
            }
        }
    }
}