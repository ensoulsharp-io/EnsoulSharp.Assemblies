using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;

using SebbyLib;
using SPrediction;

using SharpDX;
using SharpDX.Direct3D9;

using Menu = EnsoulSharp.SDK.MenuUI.Menu;
using Font = SharpDX.Direct3D9.Font;

namespace OneKeyToWin_AIO_Sebby
{
    internal class Program
    {
        private static string OktwNews = "Successful Porting by Mask";

        public static Menu Config;
        public static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static Spell Q, W, E, R, Q1, W1, E1, R1;
        public static float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;

        public static int tickIndex = 0;
        public static bool SPredictionLoad = false;

        private static Font TextBold;
        private static Render.Sprite Intro;
        private static float spellFarmTimer = 0;
        private static float dodgeTime = 0;
        private static float dodgeRange = 420;
        private static float DrawSpellTime = 0;
        private static Spell DrawSpell;
        private static SpellPrediction.PredictionOutput DrawSpellPos;

        static void Main(string[] args) { GameEvent.OnGameLoad += OnGameLoad; }

        private static void OnGameLoad()
        {
            TextBold = new Font(Drawing.Direct3DDevice, new FontDescription
            { FaceName = "Impact", Height = 30, Weight = FontWeight.Normal, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            Config = new Menu("OneKeyToWin_AIO" + ObjectManager.Player.CharacterName, "OneKeyToWin AIO", true);

            #region MENU ABOUT OKTW

            var about = new Menu("about", "About OKTW©");
            about.Add(new MenuBool("debug", "Debug", false));
            about.Add(new MenuBool("debugChat", "Debug Chat", false));
            about.Add(new MenuSeparator("0", "OneKeyToWin© by Sebby"));
            about.Add(new MenuSeparator("1", "visit joduska.me"));
            about.Add(new MenuSeparator("2", "DONATE: kaczor.sebastian@gmail.com"));
            about.Add(new MenuBool("print", "OKTW NEWS in chat", true));
            about.Add(new MenuBool("logo", "Intro logo OKTW", true));
            Config.Add(about);

            #endregion

            #region PREDICTION MODE

            var pred = new Menu("predmode", "Prediction MODE");
            pred.Add(new MenuList("Qpred", "Q Prediction MODE", new[] { "Common prediction", "OKTW© PREDICTION", "SPrediction press F5 if not loaded", "Exory prediction" }, 1, ObjectManager.Player.CharacterName));
            pred.Add(new MenuList("QHitChance", "Q Hit Chance", new[] { "Very High", "High", "Medium" }, 0, ObjectManager.Player.CharacterName));
            pred.Add(new MenuList("Wpred", "W Prediction MODE", new[] { "Common prediction", "OKTW© PREDICTION", "SPrediction press F5 if not loaded", "Exory prediction" }, 1, ObjectManager.Player.CharacterName));
            pred.Add(new MenuList("WHitChance", "W Hit Chance", new[] { "Very High", "High", "Medium" }, 0, ObjectManager.Player.CharacterName));
            pred.Add(new MenuList("Epred", "E Prediction MODE", new[] { "Common prediction", "OKTW© PREDICTION", "SPrediction press F5 if not loaded", "Exory prediction" }, 1, ObjectManager.Player.CharacterName));
            pred.Add(new MenuList("EHitChance", "E Hit Chance", new[] { "Very High", "High", "Medium" }, 0, ObjectManager.Player.CharacterName));
            pred.Add(new MenuList("Rpred", "R Prediction MODE", new[] { "Common prediction", "OKTW© PREDICTION", "SPrediction press F5 if not loaded", "Exory prediction" }, 1, ObjectManager.Player.CharacterName));
            pred.Add(new MenuList("RHitChance", "R Hit Chance", new[] { "Very High", "High", "Medium" }, 0, ObjectManager.Player.CharacterName));

            pred.Add(new MenuBool("debugPred", "Draw Aiming OKTW© PREDICTION", false));

            Config.Add(pred);

            if (pred.GetValue<MenuList>("Qpred").Index == 2 ||
                pred.GetValue<MenuList>("Wpred").Index == 2 ||
                pred.GetValue<MenuList>("Epred").Index == 2 ||
                pred.GetValue<MenuList>("Rpred").Index == 2)
            {
                SPrediction.Prediction.Initialize(pred);
                SPredictionLoad = true;
                pred.Add(new MenuSeparator("322", "SPREDICTION LOADED"));
            }
            else
                pred.Add(new MenuSeparator("322", "SPREDICTION NOT LOADED"));

            #endregion

            #region EXTRA SETTINGS

            var extra = new Menu("extraSet", "Extra settings OKTW©");
            extra.Add(new MenuBool("supportMode", "Support Mode", false, ObjectManager.Player.CharacterName));
            extra.Add(new MenuBool("comboDisableMode", "Disable auto-attack in combo mode", false, ObjectManager.Player.CharacterName));
            extra.Add(new MenuBool("manaDisable", "Disable mana manager in combo", false, ObjectManager.Player.CharacterName));
            extra.Add(new MenuBool("collAA", "Disable auto-attack if Yasuo wall collision", true, ObjectManager.Player.CharacterName));

            var anti = new Menu("antimelee", "Anti-Melee Positioning Assistant OKTW©");
            anti.Add(new MenuBool("positioningAssistant", "Anti-Melee Positioning Assistant OKTW©", false));
            var assist = new Menu("antimeleeassist", "Positioning Assistant:");
            foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsMelee))
                assist.Add(new MenuBool("posAssistant" + enemy.CharacterName, enemy.CharacterName, true));
            anti.Add(new MenuBool("positioningAssistantDraw", "Show notification", true));
            anti.Add(assist);
            extra.Add(anti);

            extra.Add(new MenuBool("harassLaneclear", "Skill-Harass in lane clear", true));
            extra.GetValue<MenuBool>("supportMode").Enabled = false;

            Config.Add(extra);

            #endregion

            #region HARASS

            var harass = new Menu("harass", "Harass");
            foreach (var enemy in GameObjects.EnemyHeroes)
                harass.Add(new MenuBool("harass" + enemy.CharacterName, enemy.CharacterName, true));
            Config.Add(harass);

            #endregion

            #region FARM

            var farm = new Menu("farm", "Farm");
            var spellFarm = new Menu("spellfarm", "SPELLS FARM TOGGLE");
            spellFarm.Add(new MenuBool("spellFarm", "OKTW spells farm", true));
            spellFarm.Add(new MenuList("spellFarmMode", "SPELLS FARM TOGGLE MODE", new[] { "Scroll down", "Scroll press", "Key toggle", "Disable" }) { Index = 1 });
            spellFarm.Add(new MenuKeyBind("spellFarmKeyToggle", "Key toggle", Keys.N, KeyBindType.Toggle));
            spellFarm.Add(new MenuBool("showNot", "Show notification", true));
            spellFarm.GetValue<MenuBool>("spellFarm").Permashow(true);
            farm.Add(spellFarm);
            Config.Add(farm);

            #endregion

            #region DRAW

            var draw = new Menu("draw", "Draws OKTW©");
            draw.Add(new MenuBool("disableDraws", "DISABLE DRAWS", false));
            Config.Add(draw);

            #endregion

            #region LOAD CHAMPIONS

            switch (Player.CharacterName)
            {
                case "Caitlyn":
                    new Champions.Caitlyn();
                    break;
                case "Graves":
                    new Champions.Graves();
                    break;
                case "Jinx":
                    new Champions.Jinx();
                    break;
            }

            #endregion

            #region LOGO

            if (about.GetValue<MenuBool>("logo").Enabled)
            {
                Intro = new Render.Sprite(LoadImg("intro"), new Vector2(Drawing.Width / 2 - 500, Drawing.Height / 2 - 350));
                Intro.Add(0);
                Intro.OnDraw();

                DelayAction.Add(7000, () => Intro.Remove());
            }

            #endregion

            Config.Attach();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
            Orbwalker.OnAction += Orbwalker_OnAction;

            if (about.GetValue<MenuBool>("print").Enabled)
            {
                Chat.Print("<font size='30'>OneKeyToWin</font> <font color='#b756c5'>by Sebby</font>");
                Chat.Print("<font color='#b756c5'>OKTW NEWS: </font>" + OktwNews);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!SPredictionLoad && (int)Game.Time % 2 == 0 &&
                (Config["predmode"].GetValue<MenuList>("Qpred").Index == 2 ||
                Config["predmode"].GetValue<MenuList>("Wpred").Index == 2 ||
                Config["predmode"].GetValue<MenuList>("Epred").Index == 2 ||
                Config["predmode"].GetValue<MenuList>("Rpred").Index == 2))
            {
                drawText("PRESS F5 TO LOAD SPREDICTION", Player.Position, System.Drawing.Color.Yellow, -300);
            }

            if (spellFarmTimer + 1 > Game.Time && Config["farm"]["spellfarm"].GetValue<MenuBool>("showNot").Enabled)
            {
                if (Config["farm"]["spellfarm"].GetValue<MenuBool>("spellFarm").Enabled)
                    DrawFontTextScreen(TextBold, "SPELLS FARM ON", Drawing.Width * 0.5f, Drawing.Height * 0.4f, SharpDX.Color.GreenYellow);
                else
                    DrawFontTextScreen(TextBold, "SPELLS FARM OFF", Drawing.Width * 0.5f, Drawing.Height * 0.4f, SharpDX.Color.OrangeRed);
            }

            if (Config["draw"].GetValue<MenuBool>("disableDraws").Enabled)
                return;

            if (Game.Time - dodgeTime < 0.01 && (int)(Game.Time * 10) % 2 == 0 && !Player.IsMelee
                && Config["extraSet"]["antimelee"].GetValue<MenuBool>("positioningAssistant").Enabled
                && Config["extraSet"]["antimelee"].GetValue<MenuBool>("positioningAssistantDraw").Enabled)
            {
                Render.Circle.DrawCircle(Player.Position, dodgeRange, System.Drawing.Color.DimGray, 1);
                drawText("Anti-Melee Positioning Assistant", Player.Position, System.Drawing.Color.Gray);
            }

            if (Game.Time - DrawSpellTime < 0.5 && Config["predmode"].GetValue<MenuBool>("debugPred").Enabled &&
                (Config["predmode"].GetValue<MenuList>("Qpred").Index == 1 ||
                Config["predmode"].GetValue<MenuList>("Wpred").Index == 1 ||
                Config["predmode"].GetValue<MenuList>("Epred").Index == 1 ||
                Config["predmode"].GetValue<MenuList>("Rpred").Index == 1))
            {
                if (DrawSpell.Type == SkillshotType.Line)
                    OktwCommon.DrawLineRectangle(DrawSpellPos.CastPosition, Player.Position, (int)DrawSpell.Width, 1, System.Drawing.Color.DimGray);
                if (DrawSpell.Type == SkillshotType.Circle)
                    Render.Circle.DrawCircle(DrawSpellPos.CastPosition, DrawSpell.Width, System.Drawing.Color.DimGray, 1);

                drawText("Aiming " + DrawSpellPos.Hitchance, Player.Position.Extend(DrawSpellPos.CastPosition, 400), System.Drawing.Color.Gray);
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (LagFree(0) && Config["farm"]["spellfarm"].GetValue<MenuList>("spellFarmMode").Index == 2)
            {
                var spellFarmKeyToggle = Config["farm"]["spellfarm"].GetValue<MenuKeyBind>("spellFarmKeyToggle");
                var spellFarm = Config["farm"]["spellfarm"].GetValue<MenuBool>("spellFarm");

                if (spellFarmKeyToggle.Active != spellFarm.Enabled)
                {
                    var wrapper = Config[Player.CharacterName] as Menu;
                    if (wrapper != null)
                    {
                        var farm = wrapper["farm"] as Menu;
                        if (farm != null)
                        {
                            var q = farm["farmQ"] as MenuBool;
                            var w = farm["farmW"] as MenuBool;
                            var e = farm["farmE"] as MenuBool;
                            var r = farm["farmR"] as MenuBool;

                            if (q != null)
                                q.Enabled = !q.Enabled;
                            if (w != null)
                                w.Enabled = !w.Enabled;
                            if (e != null)
                                e.Enabled = !e.Enabled;
                            if (r != null)
                                r.Enabled = !r.Enabled;
                        }
                    }

                    spellFarm.Enabled = !spellFarm.Enabled;
                    spellFarmTimer = Game.Time;
                }
            }

            PositionHelper();

            tickIndex++;

            if (tickIndex > 4)
                tickIndex = 0;
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            var idx = Config["farm"]["spellfarm"].GetValue<MenuList>("spellFarmMode").Index;

            if (idx == 3)
                return;

            if ((idx == 0 && args.Msg == (uint)WindowsMessages.MOUSEWHEEL)
                || idx == 1 && args.Msg == (uint)WindowsMessages.MBUTTONUP)
            {
                var spellFarm = Config["farm"]["spellfarm"].GetValue<MenuBool>("spellFarm");
                var wrapper = Config[Player.CharacterName] as Menu;

                if (wrapper != null)
                {
                    var farm = wrapper["farm"] as Menu;
                    if (farm != null)
                    {
                        var q = farm["farmQ"] as MenuBool;
                        var w = farm["farmW"] as MenuBool;
                        var e = farm["farmE"] as MenuBool;
                        var r = farm["farmR"] as MenuBool;

                        if (q != null)
                            q.Enabled = !q.Enabled;
                        if (w != null)
                            w.Enabled = !w.Enabled;
                        if (e != null)
                            e.Enabled = !e.Enabled;
                        if (r != null)
                            r.Enabled = !r.Enabled;
                    }
                }

                spellFarm.Enabled = !spellFarm.Enabled;
                spellFarmTimer = Game.Time;
            }
        }

        private static void Orbwalker_OnAction(object sender, OrbwalkerActionArgs args)
        {
            if (args.Type == OrbwalkerType.BeforeAttack)
            {
                if (Combo && Config["extraSet"].GetValue<MenuBool>("comboDisableMode").Enabled)
                {
                    var t = args.Target as AIHeroClient;
                    if (t != null &&
                        4 * Player.GetAutoAttackDamage(t) < t.Health - OktwCommon.GetIncomingDamage(t) &&
                        !t.HasBuff("LuxIlluminatingFraulein") &&
                        !Player.HasBuff("sheen"))
                        args.Process = false;
                }

                if (!Player.IsMelee && Config["extraSet"].GetValue<MenuBool>("collAA").Enabled && OktwCommon.CollisionYasuo(Player.PreviousPosition, args.Target.Position))
                {
                    args.Process = false;
                }

                if (Orbwalker.ActiveMode == OrbwalkerMode.Harass && Config["extraSet"].GetValue<MenuBool>("supportMode").Enabled)
                {
                    if (args.Target.Type == GameObjectType.AIMinionClient)
                        args.Process = false;
                }
            }
        }

        private static void PositionHelper()
        {
            if (Player.CharacterName == "Draven" || !Config["extraSet"]["antimelee"].GetValue<MenuBool>("positioningAssistant").Enabled)
                return;

            if (Player.IsMelee)
            {
                Orbwalker.SetOrbwalkerPosition(Vector3.Zero);
                return;
            }

            foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsMelee && enemy.IsValidTarget(dodgeRange) && enemy.IsFacing(Player) && Config["extraSet"]["antimelee"]["antimeleeassist"].GetValue<MenuBool>("posAssistant" + enemy.CharacterName).Enabled))
            {
                var points = OktwCommon.CirclePoints(20, 250, Player.Position);

                if (Player.FlatMagicDamageMod > Player.FlatPhysicalDamageMod)
                    OktwCommon.blockAttack = true;

                var bestPoint = Vector3.Zero;

                foreach (var point in points)
                {
                    if (point.IsWall() || point.IsUnderEnemyTurret())
                    {
                        Orbwalker.SetOrbwalkerPosition(Vector3.Zero);
                        return;
                    }

                    if (enemy.Distance(point) > dodgeRange  && (bestPoint == Vector3.Zero || point.DistanceToCursor() < bestPoint.DistanceToCursor()))
                    {
                        bestPoint = point;
                    }
                }

                if (enemy.Distance(bestPoint) > dodgeRange)
                {
                    Orbwalker.SetOrbwalkerPosition(bestPoint);
                }
                else
                {
                    var fastPoint = enemy.PreviousPosition.Extend(Player.PreviousPosition, dodgeRange);
                    if (fastPoint.CountEnemyHeroesInRange(dodgeRange) <= Player.CountEnemyHeroesInRange(dodgeRange))
                    {
                        Orbwalker.SetOrbwalkerPosition(fastPoint);
                    }
                }

                dodgeTime = Game.Time;
                return;
            }

            Orbwalker.SetOrbwalkerPosition(Vector3.Zero);
            OktwCommon.blockAttack = false;
        }

        public static void CastSpell(Spell QWER, AIBaseClient target)
        {
            var predIndex = 0;
            var hitchance = HitChance.Low;

            if (QWER.Slot == SpellSlot.Q)
            {
                predIndex = Config["predmode"].GetValue<MenuList>("Qpred").Index;
                var QHitChance = Config["predmode"].GetValue<MenuList>("QHitChance").Index;
                if (QHitChance == 0)
                    hitchance = HitChance.VeryHigh;
                else if (QHitChance == 1)
                    hitchance = HitChance.High;
                else if (QHitChance == 2)
                    hitchance = HitChance.Medium;
            }
            else if (QWER.Slot == SpellSlot.W)
            {
                predIndex = Config["predmode"].GetValue<MenuList>("Wpred").Index;
                var WHitChance = Config["predmode"].GetValue<MenuList>("WHitChance").Index;
                if (WHitChance == 0)
                    hitchance = HitChance.VeryHigh;
                else if (WHitChance == 1)
                    hitchance = HitChance.High;
                else if (WHitChance == 2)
                    hitchance = HitChance.Medium;
            }
            else if (QWER.Slot == SpellSlot.E)
            {
                predIndex = Config["predmode"].GetValue<MenuList>("Epred").Index;
                var EHitChance = Config["predmode"].GetValue<MenuList>("EHitChance").Index;
                if (EHitChance == 0)
                    hitchance = HitChance.VeryHigh;
                else if (EHitChance == 1)
                    hitchance = HitChance.High;
                else if (EHitChance == 2)
                    hitchance = HitChance.Medium;
            }
            else if (QWER.Slot == SpellSlot.R)
            {
                predIndex = Config["predmode"].GetValue<MenuList>("Rpred").Index;
                var RHitChance = Config["predmode"].GetValue<MenuList>("RHitChance").Index;
                if (RHitChance == 0)
                    hitchance = HitChance.VeryHigh;
                else if (RHitChance == 1)
                    hitchance = HitChance.High;
                else if (RHitChance == 2)
                    hitchance = HitChance.Medium;
            }

            if (predIndex == 3)
            {
                if (QWER.Type == SkillshotType.Circle)
                {
                    Core.PredictionAio.CCast(QWER, target, hitchance);
                }
                else if (QWER.Type == SkillshotType.Line)
                {
                    Core.PredictionAio.LCast(QWER, target, hitchance);
                }
                else if (QWER.Type == SkillshotType.Cone)
                {
                    Core.PredictionAio.ConeCast(QWER, target, hitchance);
                }
                else
                {
                    QWER.CastIfHitchanceMinimum(target, hitchance);
                }
            }
            else if (predIndex == 2)
            {
                if (target is AIHeroClient)
                {
                    QWER.SPredictionCast(target as AIHeroClient, hitchance);
                }
                else
                {
                    QWER.CastIfHitchanceMinimum(target, hitchance);
                }
            }
            else if (predIndex == 1)
            {
                var aoe = false;

                if (QWER.Type == SkillshotType.Circle)
                {
                    aoe = true;
                }

                if (QWER.Width > 80 && !QWER.Collision)
                {
                    aoe = true;
                }

                var predInput = new SpellPrediction.PredictionInput
                {
                    Aoe = aoe,
                    Collision = QWER.Collision,
                    Speed = QWER.Speed,
                    Delay = QWER.Delay,
                    Range = QWER.Range,
                    From = Player.PreviousPosition,
                    RangeCheckFrom = Player.PreviousPosition,
                    Radius = QWER.Width,
                    Unit = target,
                    Type = QWER.Type
                };

                var predOutput = SpellPrediction.GetPrediction(predInput);

                if (QWER.Speed != float.MaxValue && OktwCommon.CollisionYasuo(Player.PreviousPosition, predOutput.CastPosition))
                    return;

                if (predOutput.Hitchance >= hitchance)
                    QWER.Cast(predOutput.CastPosition);

                if (Game.Time - DrawSpellTime > 0.5)
                {
                    DrawSpell = QWER;
                    DrawSpellTime = Game.Time;
                }

                DrawSpellPos = predOutput;
            }
            else if (predIndex == 0)
            {
                QWER.CastIfHitchanceMinimum(target, hitchance);
            }
        }

        public static bool LagFree(int offset)
        {
            return (tickIndex == offset);
        }

        public static bool Harass
        {
            get
            {
                return (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear && Config["extraSet"].GetValue<MenuBool>("harassLaneclear").Enabled
                    || Orbwalker.ActiveMode == OrbwalkerMode.Harass);
            }
        }

        public static bool None
        {
            get
            {
                return (Orbwalker.ActiveMode == OrbwalkerMode.None);
            }
        }

        public static bool Combo
        {
            get
            {
                return (Orbwalker.ActiveMode == OrbwalkerMode.Combo);
            }
        }

        public static bool LaneClear
        {
            get
            {
                return (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear);
            }
        }

        public static Bitmap LoadImg(string imgName)
        {
            var bitmap = Resource1.ResourceManager.GetObject(imgName) as Bitmap;
            if (bitmap == null)
            {
                Console.WriteLine(imgName + ".png not found.");
            }
            return bitmap;
        }

        public static void debug(string msg)
        {
            if (Config["about"].GetValue<MenuBool>("debug").Enabled)
            {
                Console.WriteLine(msg);
            }
            if (Config["about"].GetValue<MenuBool>("debugChat").Enabled)
            {
                Chat.Print(msg);
            }
        }

        public static void drawText(string msg, Vector3 Hero, System.Drawing.Color color, int weight = 0)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] + weight, color, msg);
        }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, System.Drawing.Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        public static void DrawFontTextScreen(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int)vPosX, (int)vPosY, vColor);
        }
    }
}
