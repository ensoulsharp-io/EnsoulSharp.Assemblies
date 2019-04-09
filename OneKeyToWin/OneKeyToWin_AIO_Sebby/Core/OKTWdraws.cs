using System;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Core.UI.IMenu;
using EnsoulSharp.SDK.Core.UI.IMenu.Values;
using EnsoulSharp.SDK.Core.Utils;
using SebbyLib;
using SharpDX;
using SharpDX.Direct3D9;

namespace OneKeyToWin_AIO_Sebby.Core
{
    class OKTWdraws
    {
        private static Menu Config = Program.Config;
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static Font Tahoma13, Tahoma13B, TextBold;
        public Spell Q, W, E, R;
        private float IntroTimer = Game.Time;
        private Render.Sprite Intro;

        public void LoadOKTW()
        {
            (Config["aboutoktw"] as Menu).Add(new MenuBool("logo", "Intro logo OKTW", true));

            if (Config["aboutoktw"].GetValue<MenuBool>("logo").Value)
            {
                Intro = new Render.Sprite(LoadImg("intro"), new Vector2(Drawing.Width / 2 - 500, Drawing.Height / 2 - 350));
                Intro.Add(0);
                Intro.OnDraw();
            }

            DelayAction.Add(7000, () => Intro.Remove());

            var ud = Config["utilitydraws"] as Menu;

            ud.Add(new MenuBool("disableDraws", "DISABLE UTILITY DRAWS"));

            var eig = new Menu("enemyinfogrid", "Enemy info grid");
            eig.Add(new MenuBool("championInfo", "Game Info", true));
            eig.Add(new MenuBool("ShowKDA", "Show flash and R CD", true));
            eig.Add(new MenuBool("ShowRecall", "Show recall", true));
            eig.Add(new MenuSlider("posX", "posX", 70, 0, 100));
            eig.Add(new MenuSlider("posY", "posY", 10, 0, 100));
            ud.Add(eig);

            ud.Add(new MenuBool("GankAlert", "Gank Alert", true));
            ud.Add(new MenuBool("HpBar", "Dmg indicators BAR OKTW© style", true));
            ud.Add(new MenuBool("ShowClicks", "Show enemy clicks", true));
            ud.Add(new MenuBool("SS", "SS notification", true));
            ud.Add(new MenuBool("RF", "R and Flash notification", true));
            ud.Add(new MenuBool("showWards", "Show hidden objects, wards", true));
            ud.Add(new MenuBool("minimap", "Mini-map hack", true));

            Tahoma13B = new Font(Drawing.Direct3DDevice, new FontDescription
            { FaceName = "Tahoma", Height = 14, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });

            Tahoma13 = new Font(Drawing.Direct3DDevice, new FontDescription
            { FaceName = "Tahoma", Height = 14, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });

            TextBold = new Font(Drawing.Direct3DDevice, new FontDescription
            { FaceName = "Impact", Height = 30, Weight = FontWeight.Normal, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            Drawing.OnEndScene += Drawing_OnEndScene;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static System.Drawing.Bitmap LoadImg(string imgName)
        {
            var bitmap = Resource1.ResourceManager.GetObject(imgName) as System.Drawing.Bitmap;
            if (bitmap == null)
            {
                Logging.Write()(LogLevel.Warn, imgName + ".png not found.");
            }
            return bitmap;
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            if (Config["utilitydraws"].GetValue<MenuBool>("disableDraws").Value)
                return;

            if (Config["utilitydraws"].GetValue<MenuBool>("minimap").Value)
            {
                foreach (var enemy in GameObjects.EnemyHeroes)
                {
                    if (!enemy.IsVisible)
                    {
                        var ChampionInfoOne = OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                        if (ChampionInfoOne != null)
                        {
                            var wts = Drawing.WorldToMinimap(ChampionInfoOne.LastVisiblePos);
                            Program.DrawFontTextScreen(Tahoma13, enemy.CharacterName[0].ToString() + enemy.CharacterName[1].ToString(), wts[0], wts[1], Color.Yellow, true);
                        }
                    }
                }
            }

            if (Config["utilitydraws"].GetValue<MenuBool>("showWards").Value)
            {
                foreach (var obj in OKTWward.HiddenObjList)
                {
                    if (obj.type == 1)
                    {
                        Program.DrawCircle(obj.pos, 100, System.Drawing.Color.Yellow, 3, 20, true);
                    }

                    if (obj.type == 2)
                    {
                        Program.DrawCircle(obj.pos, 100, System.Drawing.Color.HotPink, 3, 20, true);
                    }

                    if (obj.type == 3)
                    {
                        Program.DrawCircle(obj.pos, 100, System.Drawing.Color.Orange, 3, 20, true);
                    }
                }
            }

            var HpBar = Config["utilitydraws"].GetValue<MenuBool>("HpBar").Value;
            var Width = 104;
            var Height = 11;
            var XOffset = -45;
            var YOffset = -24;

            foreach (var enemy in GameObjects.EnemyHeroes)
            {
                if (HpBar && enemy.IsHPBarRendered && Render.OnScreen(Drawing.WorldToScreen(enemy.Position)))
                {
                    var barPos = enemy.HPBarPosition;

                    float QdmgDraw = 0, WdmgDraw = 0, EdmgDraw = 0, RdmgDraw = 0, damage = 0;

                    if (Q.IsReady())
                        damage = damage + Q.GetDamage(enemy);

                    if (W.IsReady() && Player.CharacterName != "Kalista")
                        damage = damage + W.GetDamage(enemy);

                    if (E.IsReady())
                        damage = damage + E.GetDamage(enemy);

                    if (R.IsReady())
                        damage = damage + R.GetDamage(enemy);

                    if (Q.IsReady())
                        QdmgDraw = (Q.GetDamage(enemy) / damage);

                    if (W.IsReady() && Player.CharacterName != "Kalista")
                        WdmgDraw = (W.GetDamage(enemy) / damage);

                    if (E.IsReady())
                        EdmgDraw = (E.GetDamage(enemy) / damage);

                    if (R.IsReady())
                        RdmgDraw = (R.GetDamage(enemy) / damage);

                    var percentHealthAfterDamage = Math.Max(0, enemy.Health - damage) / enemy.MaxHealth;

                    var yPos = barPos.Y + YOffset;
                    var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
                    var xPosCurrentHp = barPos.X + XOffset + Width * enemy.Health / enemy.MaxHealth;

                    var differenceInHP = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + XOffset + (Width * percentHealthAfterDamage);

                    for (var i = 0; i < differenceInHP; i++)
                    {
                        if (Q.IsReady() && i < QdmgDraw * differenceInHP)
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.Cyan);
                        else if (W.IsReady() && i < (QdmgDraw + WdmgDraw) * differenceInHP)
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.Orange);
                        else if (E.IsReady() && i < (QdmgDraw + WdmgDraw + EdmgDraw) * differenceInHP)
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.Yellow);
                        else if (R.IsReady() && i < (QdmgDraw + WdmgDraw + EdmgDraw + RdmgDraw) * differenceInHP)
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.YellowGreen);
                    }
                }
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (Config["utilitydraws"].GetValue<MenuBool>("disableDraws").Value)
                return;

            if (Config["utilitydraws"].GetValue<MenuBool>("showWards").Value)
            {
                var circleSize = 30;
                foreach (var obj in OKTWward.HiddenObjList.Where(obj => Render.OnScreen(Drawing.WorldToScreen(obj.pos))))
                {
                    if (obj.type == 1)
                    {
                        OktwCommon.DrawTriangleOKTW(circleSize, obj.pos, System.Drawing.Color.Yellow);
                        Program.DrawFontTextMap(Tahoma13, "" + (int)(obj.endTime - Game.Time), obj.pos, SharpDX.Color.Yellow, true);
                    }

                    if (obj.type == 2)
                    {
                        OktwCommon.DrawTriangleOKTW(circleSize, obj.pos, System.Drawing.Color.HotPink);
                        Program.DrawFontTextMap(Tahoma13, "VW", obj.pos, SharpDX.Color.HotPink, true);
                    }

                    if (obj.type == 3)
                    {
                        OktwCommon.DrawTriangleOKTW(circleSize, obj.pos, System.Drawing.Color.Orange);
                        Program.DrawFontTextMap(Tahoma13, "! " + (int)(obj.endTime - Game.Time), obj.pos, SharpDX.Color.Orange, true);
                    }
                }
            }

            var championInfo = Config["utilitydraws"]["enemyinfogrid"].GetValue<MenuBool>("championInfo").Value;
            var GankAlert = Config["utilitydraws"].GetValue<MenuBool>("GankAlert").Value;
            var ShowKDA = Config["utilitydraws"]["enemyinfogrid"].GetValue<MenuBool>("ShowKDA").Value;
            var ShowRecall = Config["utilitydraws"]["enemyinfogrid"].GetValue<MenuBool>("ShowRecall").Value;
            var ShowClicks = Config["utilitydraws"].GetValue<MenuBool>("ShowClicks").Value;
            var RF = Config["utilitydraws"].GetValue<MenuBool>("RF").Value;
            var posX = Config["utilitydraws"]["enemyinfogrid"].GetValue<MenuSlider>("posX").Value * 0.01f * Drawing.Width;
            var posY = Config["utilitydraws"]["enemyinfogrid"].GetValue<MenuSlider>("posY").Value * 0.01f * Drawing.Height;
            var positionDraw = 0f;
            var positionGang = 500f;
            var FillColor = System.Drawing.Color.GreenYellow;
            var Color = System.Drawing.Color.Azure;
            var offset = 0f;

            foreach (var enemy in GameObjects.EnemyHeroes)
            {
                offset += 0.15f;

                if (Config["utilitydraws"].GetValue<MenuBool>("SS").Value)
                {
                    if (!enemy.IsVisible && !enemy.IsDead)
                    {
                        var ChampionInfoOne = OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                        if (ChampionInfoOne != null && enemy != Program.jungler)
                        {
                            if ((int)(Game.Time * 10) % 2 == 0 && Game.Time - ChampionInfoOne.LastVisibleTime > 3 && Game.Time - ChampionInfoOne.LastVisibleTime < 7)
                            {
                                Program.DrawFontTextScreen(TextBold, "SS " + enemy.CharacterName + " " + (int)(Game.Time - ChampionInfoOne.LastVisibleTime), Drawing.Width * offset, Drawing.Height * 0.02f, SharpDX.Color.Orange);
                            }
                            if (Game.Time - ChampionInfoOne.LastVisibleTime >= 7)
                            {
                                Program.DrawFontTextScreen(TextBold, "SS " + enemy.CharacterName + " " + (int)(Game.Time - ChampionInfoOne.LastVisibleTime), Drawing.Width * offset, Drawing.Height * 0.02f, SharpDX.Color.OrangeRed);
                            }
                        }
                    }
                }

                if (enemy.IsValidTarget() && ShowClicks)
                {
                    var lastWaypoint = enemy.GetWaypoints().Last().ToVector3();
                    if (lastWaypoint.IsValid())
                    {
                        Program.drawLine(enemy.Position, lastWaypoint, 1, System.Drawing.Color.Red);

                        if (enemy.GetWaypoints().Count() > 1)
                            Program.DrawFontTextMap(Tahoma13, enemy.CharacterName, lastWaypoint, SharpDX.Color.WhiteSmoke, true);
                    }
                }

                var kolor = System.Drawing.Color.GreenYellow;

                if (enemy.IsDead)
                    kolor = System.Drawing.Color.Gray;
                else if (!enemy.IsVisible)
                    kolor = System.Drawing.Color.OrangeRed;

                var kolorHP = System.Drawing.Color.GreenYellow;

                if (enemy.IsDead)
                    kolorHP = System.Drawing.Color.Gray;
                else if (enemy.HealthPercent < 30)
                    kolorHP = System.Drawing.Color.Red;
                else if (enemy.HealthPercent < 60)
                    kolorHP = System.Drawing.Color.Orange;

                if (championInfo)
                {
                    positionDraw += 15;
                    Program.DrawFontTextScreen(Tahoma13, "" + enemy.Level, posX - 25, posY + positionDraw, SharpDX.Color.White);
                    Program.DrawFontTextScreen(Tahoma13, enemy.CharacterName, posX, posY + positionDraw, SharpDX.Color.White);

                    if (ShowRecall)
                    {
                        var ChampionInfoOne = OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                        if (Game.Time - ChampionInfoOne.FinishRecallTime < 4)
                        {
                            Program.DrawFontTextScreen(Tahoma13, "FINISH", posX - 90, posY + positionDraw, SharpDX.Color.GreenYellow);
                        }
                        else if (ChampionInfoOne.StartRecallTime <= ChampionInfoOne.AbortRecallTime && Game.Time - ChampionInfoOne.AbortRecallTime < 4)
                        {
                            Program.DrawFontTextScreen(Tahoma13, "ABORT", posX - 90, posY + positionDraw, SharpDX.Color.Yellow);
                        }
                        else if (Game.Time - ChampionInfoOne.StartRecallTime < 8)
                        {
                            var recallPercent = (int)((Game.Time - ChampionInfoOne.StartRecallTime) / 8 * 100);
                            var recallX1 = posX - 90;
                            var recallY1 = posY + positionDraw + 6;
                            var recallX2 = (recallX1 + recallPercent / 2) + 1;
                            var recallY2 = posY + positionDraw + 6;
                            Drawing.DrawLine(recallX1, recallY1, recallX1 + 50, recallY2, 8, System.Drawing.Color.Red);
                            Drawing.DrawLine(recallX1, recallY1, recallX2, recallY2, 8, System.Drawing.Color.White);
                        }
                    }

                    var fSlot = enemy.Spellbook.Spells[4];

                    if (fSlot.Name != "SummonerFlash")
                        fSlot = enemy.Spellbook.Spells[5];

                    if (fSlot.Name == "SummonerFlash")
                    {
                        var fT = fSlot.CooldownExpires - Game.Time;
                        if (ShowKDA)
                        {
                            if (fT < 0)
                                Program.DrawFontTextScreen(Tahoma13, "F rdy", posX + 130, posY + positionDraw, SharpDX.Color.GreenYellow);
                            else
                                Program.DrawFontTextScreen(Tahoma13, "F " + (int)fT, posX + 130, posY + positionDraw, SharpDX.Color.Yellow);
                        }
                        if (RF)
                        {
                            if (fT < 2 && fT > -3)
                                Program.DrawFontTextScreen(TextBold, enemy.CharacterName + " FLASH READY!", Drawing.Width * offset, Drawing.Height * 0.1f, SharpDX.Color.Yellow);
                            else if (fSlot.Cooldown - fT < 5)
                                Program.DrawFontTextScreen(TextBold, enemy.CharacterName + " FLASH LOST!", Drawing.Width, Drawing.Height * 0.1f, SharpDX.Color.Red);
                        }
                    }

                    if (enemy.Level > 5)
                    {
                        var rSlot = enemy.Spellbook.Spells[3];
                        var t = rSlot.CooldownExpires - Game.Time;
                        if (ShowKDA)
                        {
                            if (t < 0)
                                Program.DrawFontTextScreen(Tahoma13, "R rdy", posX + 165, posY + positionDraw, SharpDX.Color.GreenYellow);
                            else
                                Program.DrawFontTextScreen(Tahoma13, "R " + (int)t, posX + 165, posY + positionDraw, SharpDX.Color.Yellow);
                        }
                        if (RF)
                        {
                            if (t < 2 && t > -3)
                                Program.DrawFontTextScreen(TextBold, enemy.CharacterName + " R READY!", Drawing.Width * offset, Drawing.Height * 0.2f, SharpDX.Color.YellowGreen);
                            else if (rSlot.Cooldown - t < 5)
                                Program.DrawFontTextScreen(TextBold, enemy.CharacterName + " R LOST!", Drawing.Width * offset, Drawing.Height * 0.1f, SharpDX.Color.Red);
                        }
                    }
                    else if (ShowKDA)
                        Program.DrawFontTextScreen(Tahoma13, "R ", posX + 165, posY + positionDraw, SharpDX.Color.Yellow);
                }

                var Distance = Player.Distance(enemy.Position);

                if (GankAlert && !enemy.IsDead && Distance > 1200)
                {
                    var wts = Drawing.WorldToScreen(Player.Position.Extend(enemy.Position, positionGang));

                    wts[0] = wts[0];
                    wts[1] = wts[1] + 15;

                    if (enemy.HealthPercent > 0)
                        Drawing.DrawLine(wts[0], wts[1], wts[0] + enemy.HealthPercent / 2 + 1, wts[1], 8, kolorHP);

                    if (enemy.HealthPercent < 100)
                        Drawing.DrawLine(wts[0] + enemy.HealthPercent / 2, wts[1], wts[0] + 50, wts[1], 8, System.Drawing.Color.White);

                    if (enemy.IsVisible)
                    {
                        if (Program.jungler.NetworkId == enemy.NetworkId)
                            Program.DrawFontTextMap(Tahoma13B, enemy.CharacterName, Player.Position.Extend(enemy.Position, positionGang), SharpDX.Color.OrangeRed);
                        else
                            Program.DrawFontTextMap(Tahoma13, enemy.CharacterName, Player.Position.Extend(enemy.Position, positionGang), SharpDX.Color.White);
                    }
                    else
                    {
                        var ChampionInfoOne = OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                        if (ChampionInfoOne != null)
                        {
                            if (Game.Time - ChampionInfoOne.LastVisibleTime > 3 && Game.Time - ChampionInfoOne.LastVisibleTime < 7)
                                Program.DrawFontTextMap(Tahoma13, "SS " + enemy.CharacterName + " " + (int)(Game.Time - ChampionInfoOne.LastVisibleTime), Player.Position.Extend(enemy.Position, positionGang), SharpDX.Color.Yellow);
                            else
                                Program.DrawFontTextMap(Tahoma13, "SS " + enemy.CharacterName + " " + (int)(Game.Time - ChampionInfoOne.LastVisibleTime), Player.Position.Extend(enemy.Position, positionGang), SharpDX.Color.YellowGreen);
                        }
                        else
                            Program.DrawFontTextMap(Tahoma13, "SS " + enemy.CharacterName, Player.Position.Extend(enemy.Position, positionGang), SharpDX.Color.LightYellow);
                    }

                    if (Distance < 3500 && enemy.IsVisible && !Render.OnScreen(Drawing.WorldToScreen(enemy.Position)) && Program.jungler != null)
                    {
                        if (Program.jungler.NetworkId == enemy.NetworkId)
                            Program.drawLine(Player.Position.Extend(enemy.Position, 100), Player.Position.Extend(enemy.Position, positionGang - 100), (int)((3500 - Distance) / 280), System.Drawing.Color.Crimson);
                        else
                        {
                            if (enemy.IsFacing(Player))
                                Program.drawLine(Player.Position.Extend(enemy.Position, 100), Player.Position.Extend(enemy.Position, positionGang - 100), (int)((3500 - Distance) / 280), System.Drawing.Color.Orange);
                            else
                                Program.drawLine(Player.Position.Extend(enemy.Position, 100), Player.Position.Extend(enemy.Position, positionGang - 100), (int)((3500 - Distance) / 280), System.Drawing.Color.Gold);
                        }
                    }
                    else if (Distance < 3500 && !enemy.IsVisible && !Render.OnScreen(Drawing.WorldToScreen(Player.Position.Extend(enemy.Position, Distance + 500))))
                    {
                        var need = OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                        if (need != null && Game.Time - need.LastVisibleTime < 5)
                        {
                            Program.drawLine(Player.Position.Extend(enemy.Position, 100), Player.Position.Extend(enemy.Position, positionGang - 100), (int)((3500 - Distance) / 300), System.Drawing.Color.Gray);
                        }
                    }
                }

                positionGang = positionGang + 100;
            }

            if (Program.AIOmode == 2)
            {
                Program.DrawFontTextScreen(TextBold, "OKTW AIO only utility mode ON", Drawing.Width * 0.5f, Drawing.Height * 0.7f, SharpDX.Color.Cyan, true);
            }
        }
    }
}