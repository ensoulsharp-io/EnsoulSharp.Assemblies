using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Core.UI;
using EnsoulSharp.SDK.Core.UI.IMenu.Values;
using EnsoulSharp.SDK.Core.Utils;
using EnsoulSharp.SDK.Core.Wrappers.Damages;
using SebbyLib;
using SharpDX;
using SharpDX.Direct3D9;
using Menu = EnsoulSharp.SDK.Core.UI.IMenu.Menu;

namespace OneKeyToWin_AIO_Sebby
{
    internal class Program
    {
        private static string OktwNews = "OneKeyToWin transplant succeed";

        public static Menu Config;

        public static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static Spell Q, W, E, R, DrawSpell;

        public static float JungleTime, DrawSpellTime = 0;
        public static int timer, tickIndex = 0;
        public static AIHeroClient jungler = Player;
        public static Obj_SpawnPoint enemySpawn;
        public static PredictionOutput DrawSpellPos;

        public static bool SPredictionLoad = false;
        public static int AIOmode = 0;
        private static float spellFarmTimer = 0;
        private static Font TextBold;

        static void Main(string[] args)
        {
            Events.OnLoad += GameOnOnGameLoad;
        }

        public static void debug(string msg)
        {
            if (Config["aboutoktw"].GetValue<MenuBool>("debug").Value)
            {
                Console.WriteLine(msg);
            }
            if (Config["aboutoktw"].GetValue<MenuBool>("debugChat").Value)
            {
                Chat.PrintChat(msg);
            }
        }

        private static void GameOnOnGameLoad(object sender, EventArgs args)
        {
            TextBold = new Font(Drawing.Direct3DDevice, new FontDescription
            { FaceName = "Impact", Height = 30, Weight = FontWeight.Normal, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });

            enemySpawn = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.IsEnemy);
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            Config = new Menu("OneKeyToWin AIO", "OneKeyToWin_AIO " + Player.CharacterName, true);

            #region MENU ABOUT OKTW

            var about = new Menu("aboutoktw", "About OKTW©");
            about.Add(new MenuBool("debug", "Debug"));
            about.Add(new MenuBool("debugChat", "Debug Chat"));
            about.Add(new Menu("0", "OneKeyToWin© by Sebby"));
            about.Add(new Menu("1", "visit joduska.me"));
            about.Add(new Menu("2", "DONATE: kaczor.sebastian@gmail.com"));
            about.Add(new MenuBool("print", "OKTW NEWS in chat", true));
            Config.Add(about);

            #endregion

            Config.Add(new MenuList<string>("AIOmode", "AIO mode", new[] { "Utility and Champion", "Only Champion", "Only Utility" }, Player.CharacterName) { Index = 0 });

            AIOmode = Config.GetValue<MenuList<string>>("AIOmode").Index;

            if (AIOmode != 1)
            {
                var utilityDraws = new Menu("utilitydraws", "Utility, Draws OKTW©");
                var gankTimer = new Menu("ganktimer", "GankTimer");
                gankTimer.Add(new MenuBool("enabled", "Enabled", true));
                gankTimer.Add(new Menu("1", "RED - be careful"));
                gankTimer.Add(new Menu("2", "ORANGE - you have time"));
                gankTimer.Add(new Menu("3", "GREEN - jungler visible"));
                gankTimer.Add(new Menu("4", "CYAN jungler dead - take objectives"));
                utilityDraws.Add(gankTimer);
                Config.Add(utilityDraws);
            }

            var predictionmode = new Menu("predictionmode", "Prediction Mode");
            predictionmode.Add(new MenuList<string>("Qpred", "Q Prediction MODE", new[] { "SDK", "OKTW© PREDICTION", "SPrediction press F5 if not loaded (outdated)", "Exory prediction" }) { Index = 1 });
            predictionmode.Add(new MenuList<string>("QHitChance", "Q Hit Chance", new[] { "Very High", "High", "Medium" }, Player.CharacterName) { Index = 0 });
            predictionmode.Add(new MenuList<string>("Wpred", "W Prediction MODE", new[] { "SDK", "OKTW© PREDICTION", "SPrediction press F5 if not loaded (outdated)", "Exory prediction" }) { Index = 1 });
            predictionmode.Add(new MenuList<string>("WHitChance", "W Hit Chance", new[] { "Very High", "High", "Medium" }, Player.CharacterName) { Index = 0 });
            predictionmode.Add(new MenuList<string>("Epred", "E Prediction MODE", new[] { "SDK", "OKTW© PREDICTION", "SPrediction press F5 if not loaded (outdated)", "Exory prediction" }) { Index = 1 });
            predictionmode.Add(new MenuList<string>("EHitChance", "E Hit Chance", new[] { "Very High", "High", "Medium" }, Player.CharacterName) { Index = 0 });
            predictionmode.Add(new MenuList<string>("Rpred", "R Prediction MODE", new[] { "SDK", "OKTW© PREDICTION", "SPrediction press F5 if not loaded (outdated)", "Exory prediction" }) { Index = 1 });
            predictionmode.Add(new MenuList<string>("RHitChance", "R Hit Chance", new[] { "Very High", "High", "Medium" }, Player.CharacterName) { Index = 0 });
            predictionmode.Add(new MenuBool("debugPred", "Draw Aiming OKTW© PREDICTION"));
            Config.Add(predictionmode);

            if (AIOmode != 2)
            {
                var extraSet = new Menu("extraSet", "Extra settings OKTW©");
                extraSet.Add(new MenuBool("supportMode", "Support Mode", false, Player.CharacterName));
                extraSet.Add(new MenuBool("comboDisableMode", "Disable auto-attack in combo mode", false, Player.CharacterName));
                extraSet.Add(new MenuBool("manaDisable", "Disable mana manager in combo", false, Player.CharacterName));
                extraSet.Add(new MenuBool("collAA", "Disable auto-attack if Yasuo wall collision", true, Player.CharacterName));
                extraSet.Add(new MenuBool("harassLaneclear", "Skill-Harass in lane clear", true));
                Config.Add(extraSet);
                extraSet.GetValue<MenuBool>("supportMode").Value = false;

                #region LOAD CHAMPIONS

                #endregion

                var player = Config[Player.CharacterName] as Menu;
                if (player == null)
                {
                    player = new Menu(Player.CharacterName, Player.CharacterName);
                    Config.Add(player);
                }
                var farm = player["farm"] as Menu;
                if (farm == null)
                {
                    farm = new Menu("farm", "Farm");
                    player.Add(farm);
                }
                var spellsfarmtoggle = new Menu("spellsfarmtoggle", "SPELLS FARM TOGGLE");
                spellsfarmtoggle.Add(new MenuBool("spellFarm", "OKTW spells farm", true));
                spellsfarmtoggle.Add(new MenuList<string>("spellFarmMode", "SPELLS FARM TOGGLE MODE", new[] { "Scroll down", "Scroll press", "Key toggle", "Disable" }) { Index = 1 });
                spellsfarmtoggle.Add(new MenuKeyBind("spellFarmKeyToggle", "Key toggle", Keys.N, KeyBindType.Toggle));
                spellsfarmtoggle.Add(new MenuBool("showNot", "Show notification", true));
                farm.Add(spellsfarmtoggle);
                spellsfarmtoggle.GetValue<MenuBool>("spellFarm").Permashow(true);
            }

            foreach (var hero in GameObjects.EnemyHeroes)
            {
                if (IsJungler(hero))
                {
                    jungler = hero;
                }
            }

            if (AIOmode != 1)
            {
                new Core.Activator().LoadOKTW();
                new Core.AutoLvlUp().LoadOKTW();
                new Core.OKTWdraws().LoadOKTW();
                new Core.OKTWtracker().LoadOKTW();
                new Core.OKTWward().LoadOKTW();
            }


            Config.Add(new Menu("aiomodes", "!!! PRESS F5 TO RELOAD MODE !!!"));
            Config.Attach();

            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
            Drawing.OnDraw += Drawing_OnDraw;
            Variables.Orbwalker.OnAction += Orbwalker_OnAction;

            if (Config["aboutoktw"].GetValue<MenuBool>("print").Value)
            {
                Chat.PrintChat("<font size='30'>OneKeyToWin</font> <font color='#b756c5'>by Sebby</font>");
                Chat.PrintChat("<font color='#b756c5'>OKTW NEWS: </font>" + OktwNews);
            }
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.WParam == 16)
            {
                (Config["aiomodes"] as Menu).Visible = AIOmode != Config.GetValue<MenuList<string>>("AIOmode").Index;
            }

            if (AIOmode == 2)
            {
                return;
            }

            var sft = Config[Player.CharacterName]["farm"]["spellsfarmtoggle"];
            var idx = sft.GetValue<MenuList<string>>("spellFarmMode").Index;
            var spellFarm = sft.GetValue<MenuBool>("spellFarm");

            if (spellFarm == null || idx == 3)
            {
                return;
            }

            if ((idx == 0 && args.Msg == (uint)WindowsMessages.MOUSEHWHEEL) || (idx == 1 && args.Msg == (uint)WindowsMessages.MBUTTONUP))
            {
                if (!spellFarm.Value)
                {
                    spellFarm.Value = true;
                    spellFarmTimer = Game.Time;

                    var farmQ = Config[Player.CharacterName]["farm"]["farmQ"];
                    if (farmQ != null)
                        (farmQ as MenuBool).Value = true;
                    var farmW = Config[Player.CharacterName]["farm"]["farmW"];
                    if (farmW != null)
                        (farmW as MenuBool).Value = true;
                    var farmE = Config[Player.CharacterName]["farm"]["farmE"];
                    if (farmE != null)
                        (farmE as MenuBool).Value = true;
                    var farmR = Config[Player.CharacterName]["farm"]["farmR"];
                    if (farmR != null)
                        (farmR as MenuBool).Value = true;
                }
                else
                {
                    spellFarm.Value = false;
                    spellFarmTimer = Game.Time;

                    var farmQ = Config[Player.CharacterName]["farm"]["farmQ"];
                    if (farmQ != null)
                        (farmQ as MenuBool).Value = false;
                    var farmW = Config[Player.CharacterName]["farm"]["farmW"];
                    if (farmW != null)
                        (farmW as MenuBool).Value = false;
                    var farmE = Config[Player.CharacterName]["farm"]["farmE"];
                    if (farmE != null)
                        (farmE as MenuBool).Value = false;
                    var farmR = Config[Player.CharacterName]["farm"]["farmR"];
                    if (farmR != null)
                        (farmR as MenuBool).Value = false;
                }
            }
        }

        private static void Orbwalker_OnAction(object sender, OrbwalkingActionArgs args)
        {
            if (args.Type == OrbwalkingType.BeforeAttack)
            {
                if (AIOmode == 2)
                {
                    return;
                }

                if (Combo && Config["extraSet"].GetValue<MenuBool>("comboDisableMode").Value)
                {
                    var t = args.Target as AIHeroClient;
                    if (args.Target == null ||
                        (4 * Player.GetAutoAttackDamage(t) < t.Health - OktwCommon.GetIncomingDamage(t) && !t.HasBuff("LuxIlluminatingFraulein") && !Player.HasBuff("sheen")))
                    {
                        args.Process = false;
                    }
                }

                if (Math.Abs(Player.GetProjectileSpeed() - float.MaxValue) < float.Epsilon && OktwCommon.CollisionYasuo(Player.Position,args.Target.Position) && Config["extraSet"].GetValue<MenuBool>("collAA"))
                {
                    args.Process = false;
                }

                if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Hybrid && Config["extraSet"].GetValue<MenuBool>("supportMode").Value)
                {
                    if (args.Target.Type == GameObjectType.AIMinionClient)
                    {
                        args.Process = false;
                    }
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (AIOmode != 2)
            {
                var sft = Config[Player.CharacterName]["farm"]["spellsfarmtoggle"];
                var idx = sft.GetValue<MenuList<string>>("spellFarmMode").Index;
                var spellFarm = sft.GetValue<MenuBool>("spellFarm");
                var spellFarmKeyToggle = sft.GetValue<MenuKeyBind>("spellFarmKeyToggle");

                if (LagFree(0) && idx != 3 && spellFarm != null && idx == 2 && spellFarmKeyToggle.Active != spellFarm.Value)
                {
                    if (spellFarmKeyToggle.Active)
                    {
                        spellFarm.Value = true;
                        spellFarmTimer = Game.Time;

                        var farmQ = Config[Player.CharacterName]["farm"]["farmQ"];
                        if (farmQ != null)
                            (farmQ as MenuBool).Value = true;
                        var farmW = Config[Player.CharacterName]["farm"]["farmW"];
                        if (farmW != null)
                            (farmW as MenuBool).Value = true;
                        var farmE = Config[Player.CharacterName]["farm"]["farmE"];
                        if (farmE != null)
                            (farmE as MenuBool).Value = true;
                        var farmR = Config[Player.CharacterName]["farm"]["farmR"];
                        if (farmR != null)
                            (farmR as MenuBool).Value = true;
                    }
                    else
                    {
                        spellFarm.Value = false;
                        spellFarmTimer = Game.Time;

                        var farmQ = Config[Player.CharacterName]["farm"]["farmQ"];
                        if (farmQ != null)
                            (farmQ as MenuBool).Value = false;
                        var farmW = Config[Player.CharacterName]["farm"]["farmW"];
                        if (farmW != null)
                            (farmW as MenuBool).Value = false;
                        var farmE = Config[Player.CharacterName]["farm"]["farmE"];
                        if (farmE != null)
                            (farmE as MenuBool).Value = false;
                        var farmR = Config[Player.CharacterName]["farm"]["farmR"];
                        if (farmR != null)
                            (farmR as MenuBool).Value = false;
                    }
                }
            }

            tickIndex++;

            if (tickIndex > 4)
                tickIndex = 0;

            if (!LagFree(0))
                return;

            JunglerTimer();
        }

        public static void JunglerTimer()
        {
            if (AIOmode != 1 && Config["utilitydraws"]["ganktimer"].GetValue<MenuBool>("enabled").Value && jungler != null && jungler.IsValid)
            {
                if (jungler.IsDead)
                {
                    timer = (int)(enemySpawn.Position.Distance(Player.Position) / 370);
                }
                else if(jungler.IsVisible)
                {
                    var Way = 0f;
                    var JunglerPath = Player.GetPath(Player.Position, jungler.Position);
                    var PointStart = Player.Position;
                    if (JunglerPath == null)
                        return;
                    foreach (var point in JunglerPath)
                    {
                        var PSDistance = PointStart.Distance(point);
                        if (PSDistance > 0)
                        {
                            Way += PSDistance;
                            PointStart = point;
                        }
                    }
                    timer = (int)(Way / jungler.MoveSpeed);
                }
            }
        }

        public static bool LagFree(int offset)
        {
            return (tickIndex == offset);
        }

        public static bool Farm
        {
            get
            {
                return (Variables.Orbwalker.ActiveMode == OrbwalkingMode.LaneClear && Config["extraSet"].GetValue<MenuBool>("harassLaneclear").Value)
                    || Variables.Orbwalker.ActiveMode == OrbwalkingMode.Hybrid;
            }
        }

        public static bool None
        {
            get
            {
                return (Variables.Orbwalker.ActiveMode == OrbwalkingMode.None);
            }
        }

        public static bool Combo
        {
            get
            {
                return ((AIOmode == 2) ? Player.IsMoving : (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo));
            }
        }

        public static bool LaneClear
        {
            get
            {
                return (Variables.Orbwalker.ActiveMode == OrbwalkingMode.LaneClear);
            }
        }

        private static bool IsJungler(AIHeroClient hero)
        {
            return hero.Spellbook.Spells.Any(spell => spell.Name.ToLower().Contains("smite"));
        }

        public static void CastSpell(Spell QWER, AIBaseClient target)
        {
            var predIndex = 0;
            var hitchance = HitChance.Low;
            var pred = Config["predictionmode"];

            if (QWER.Slot == SpellSlot.Q)
            {
                predIndex = pred.GetValue<MenuList<string>>("Qpred").Index;
                var idx = pred.GetValue<MenuList<string>>("QHitChance").Index;
                if (idx == 0)
                    hitchance = HitChance.VeryHigh;
                else if (idx == 1)
                    hitchance = HitChance.High;
                else if (idx == 2)
                    hitchance = HitChance.Low;
            }
            else if (QWER.Slot == SpellSlot.W)
            {
                predIndex = pred.GetValue<MenuList<string>>("Wpred").Index;
                var idx = pred.GetValue<MenuList<string>>("WHitChance").Index;
                if (idx == 0)
                    hitchance = HitChance.VeryHigh;
                else if (idx == 1)
                    hitchance = HitChance.High;
                else if (idx == 2)
                    hitchance = HitChance.Low;
            }
            else if (QWER.Slot == SpellSlot.E)
            {
                predIndex = pred.GetValue<MenuList<string>>("Epred").Index;
                var idx = pred.GetValue<MenuList<string>>("EHitChance").Index;
                if (idx == 0)
                    hitchance = HitChance.VeryHigh;
                else if (idx == 1)
                    hitchance = HitChance.High;
                else if (idx == 2)
                    hitchance = HitChance.Low;
            }
            else if (QWER.Slot == SpellSlot.R)
            {
                predIndex = pred.GetValue<MenuList<string>>("Rpred").Index;
                var idx = pred.GetValue<MenuList<string>>("RHitChance").Index;
                if (idx == 0)
                    hitchance = HitChance.VeryHigh;
                else if (idx == 1)
                    hitchance = HitChance.High;
                else if (idx == 2)
                    hitchance = HitChance.Low;
            }

            if (predIndex == 0)
            {
                var poutput = QWER.GetPrediction(target);

                if (QWER.Speed != float.MaxValue && OktwCommon.CollisionYasuo(Player.Position, poutput.CastPosition))
                    return;

                if (poutput.Hitchance >= hitchance)
                    QWER.Cast(poutput.CastPosition);
            }
            else if (predIndex == 1)
            {
                var coreType2 = SkillshotType.SkillshotLine;
                var aoe2 = false;

                if (QWER.Type == SkillshotType.SkillshotCircle)
                {
                    coreType2 = SkillshotType.SkillshotCircle;
                    aoe2 = true;
                }

                if (QWER.Width > 80 && !QWER.Collision)
                    aoe2 = true;

                var predInput2 = new PredictionInput
                {
                    AoE = aoe2,
                    Collision = QWER.Collision,
                    Speed = QWER.Speed,
                    Delay = QWER.Delay,
                    Range = QWER.Range,
                    From = Player.Position,
                    Radius = QWER.Width,
                    Unit = target,
                    Type = coreType2
                };

                var poutput2 = Movement.GetPrediction(predInput2);

                if (QWER.Speed != float.MaxValue && OktwCommon.CollisionYasuo(Player.Position, poutput2.CastPosition))
                    return;

                if (hitchance == HitChance.VeryHigh)
                {
                    if (poutput2.Hitchance >= HitChance.VeryHigh)
                        QWER.Cast(poutput2.CastPosition);
                    else if (predInput2.AoE && poutput2.AoeTargetsHitCount > 1 && poutput2.Hitchance >= HitChance.High)
                        QWER.Cast(poutput2.CastPosition);
                }
                else if (poutput2.Hitchance >= hitchance)
                {
                    QWER.Cast(poutput2.CastPosition);
                }

                if (Game.Time - DrawSpellTime > 0.5)
                {
                    DrawSpell = QWER;
                    DrawSpellTime = Game.Time;
                }

                DrawSpellPos = poutput2;
            }
            else if (predIndex == 2)
            {

            }
            else if (predIndex == 3)
            {
                if (QWER.Type == SkillshotType.SkillshotCircle)
                    Core.PredictionAio.CCast(QWER, target, hitchance);
                else if (QWER.Type == SkillshotType.SkillshotLine)
                    Core.PredictionAio.LCast(QWER, target, hitchance);
                else if (QWER.Type == SkillshotType.SkillshotCone)
                    Core.PredictionAio.ConeCast(QWER, target, hitchance);
                else
                    QWER.Cast(target);
            }
        }

        public static void drawText(string msg, Vector3 Hero, System.Drawing.Color color, int weight = 0, bool center = false)
        {
            var wts = Drawing.WorldToScreen(Hero);
            if (center)
                Drawing.DrawText(wts[0] - Drawing.GetTextEntent(msg, 15).Width / 2, wts[1] + weight, color, msg);
            else
                Drawing.DrawText(wts[0] - msg.Length * 5, wts[1] + weight, color, msg);
        }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, System.Drawing.Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        public static void DrawFontTextScreen(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor, bool vCenter = false)
        {
            if (vCenter)
                vFont.DrawText(null, vText, (int)vPosX - vFont.MeasureText(null, vText, FontDrawFlags.Center).Width / 2, (int)vPosY, vColor);
            else
                vFont.DrawText(null, vText, (int)vPosX, (int)vPosY, vColor);
        }

        public static void DrawFontTextMap(Font vFont, string vText, Vector3 Pos, ColorBGRA vColor, bool vCenter = false)
        {
            var wts = Drawing.WorldToScreen(Pos);
            if (vCenter)
                vFont.DrawText(null, vText, (int)wts[0] - vFont.MeasureText(null, vText, FontDrawFlags.Center).Width / 2, (int)wts[1], vColor);
            else
                vFont.DrawText(null, vText, (int)wts[0], (int)wts[1], vColor);
        }

        public static void DrawCircle(Vector3 center, float radius, System.Drawing.Color color, int thickness =5,int quality = 30,bool onMinimap = false)
        {
            if (!onMinimap)
            {
                Render.Circle.DrawCircle(center, radius, color, thickness);
                return;
            }

            var pointList = new List<Vector3>();
            for (var i = 0; i < quality; i++)
            {
                var angle = i * Math.PI * 2 / quality;
                pointList.Add(
                    new Vector3(
                        center.X + radius * (float)Math.Cos(angle),
                        center.Y + radius * (float)Math.Sin(angle),
                        center.Z));
            }

            for (var i = 0; i < pointList.Count; i++)
            {
                var a = pointList[i];
                var b = pointList[i == pointList.Count - 1 ? 0 : i + 1];

                var aonScreen = Drawing.WorldToMinimap(a);
                var bonScreen = Drawing.WorldToMinimap(b);

                Drawing.DrawLine(aonScreen.X, aonScreen.Y, bonScreen.X, bonScreen.Y, thickness, color);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!SPredictionLoad && (int)Game.Time % 2 == 0 &&
                (Config["predictionmode"].GetValue<MenuList<string>>("Qpred").Index == 2 ||
                Config["predictionmode"].GetValue<MenuList<string>>("Wpred").Index == 2 ||
                Config["predictionmode"].GetValue<MenuList<string>>("Epred").Index == 2 ||
                Config["predictionmode"].GetValue<MenuList<string>>("Rpred").Index == 2))
                drawText("SPREDICTION NOT UPDATED YET", Player.Position, System.Drawing.Color.Yellow, -300, true);

            var sft = Config[Player.CharacterName]?["farm"]["spellsfarmtoggle"];
            var sf = sft?.GetValue<MenuBool>("spellFarm");
            if (AIOmode != 2 && spellFarmTimer + 1 > Game.Time && sft != null && sft.GetValue<MenuBool>("showNot").Value && sf != null)
                DrawFontTextScreen(TextBold, sf.Value ? "SPELLS FARM ON" : "SPELLS FARM OFF", Drawing.Width * 0.5f, Drawing.Height * 0.4f, sf.Value ? Color.GreenYellow : Color.OrangeRed);

            if (AIOmode == 1 || Config["utilitydraws"].GetValue<MenuBool>("disableDraws").Value)
                return;

            if (Game.Time - DrawSpellTime < 0.5 && Config["predictionmode"].GetValue<MenuBool>("debugPred").Value &&
                (Config["predictionmode"].GetValue<MenuList<string>>("Qpred").Index == 1 ||
                Config["predictionmode"].GetValue<MenuList<string>>("Wpred").Index == 1 ||
                Config["predictionmode"].GetValue<MenuList<string>>("Epred").Index == 1 ||
                Config["predictionmode"].GetValue<MenuList<string>>("Rpred").Index == 1))
            {
                if (DrawSpell.Type == SkillshotType.SkillshotLine)
                    OktwCommon.DrawLineRectangle(DrawSpellPos.CastPosition, Player.Position, (int)DrawSpell.Width, 1, System.Drawing.Color.DimGray);
                if (DrawSpell.Type == SkillshotType.SkillshotCircle)
                    Render.Circle.DrawCircle(DrawSpellPos.CastPosition, DrawSpell.Width, System.Drawing.Color.DimGray, 1);

                drawText("Aiming " + DrawSpellPos.Hitchance, Player.Position.Extend(DrawSpellPos.CastPosition, 400), System.Drawing.Color.Gray);
            }

            if (Config["utilitydraws"]["ganktimer"].GetValue<MenuBool>("enabled").Value && jungler != null)
            {
                if (jungler == Player)
                    drawText("Jungler not detected", Player.Position, System.Drawing.Color.Yellow, 100, true);
                else if (jungler.IsDead)
                    drawText("Jungler dead " + timer, Player.Position, System.Drawing.Color.Cyan, 100, true);
                else if (jungler.IsVisible)
                    drawText("Jungler visible " + timer, Player.Position, System.Drawing.Color.GreenYellow, 100, true);
                else
                {
                    if (timer > 0)
                        drawText("Junger in jungle " + timer, Player.Position, System.Drawing.Color.Orange, 100, true);
                    else if ((int)(Game.Time * 10) % 2 == 0)
                        drawText("BE CAREFUL " + timer, Player.Position, System.Drawing.Color.OrangeRed, 100, true);
                    if (Game.Time - JungleTime >= 1)
                    {
                        timer = timer - 1;
                        JungleTime = Game.Time;
                    }
                }
            }
        }
    }
}