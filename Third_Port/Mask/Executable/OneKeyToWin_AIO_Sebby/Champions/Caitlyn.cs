using System;
using System.Linq;
using System.Windows.Forms;

using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;

using SebbyLib;

using SharpDX;

using Menu = EnsoulSharp.SDK.MenuUI.Menu;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Caitlyn : Program
    {
        private float QCastTime = 0;

        public Caitlyn()
        {
            Q = new Spell(SpellSlot.Q, 1300);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 3500);

            Q.SetSkillshot(0.63f, 60, 2200, false, false, SkillshotType.Line);
            W.SetSkillshot(1.5f, 75, float.MaxValue, false, false, SkillshotType.Circle);
            E.SetSkillshot(0.15f, 70, 1600, true, false, SkillshotType.Line);
            R.SetSkillshot(1.375f, 40, 3200, false, false, SkillshotType.Line);

            var wrapper = new Menu(Player.CharacterName, Player.CharacterName);

            var draw = new Menu("draw", "Draw");
            draw.Add(new MenuBool("noti", "Show notification & line", false, Player.CharacterName));
            draw.Add(new MenuBool("qRange", "Q range", false, Player.CharacterName));
            draw.Add(new MenuBool("wRange", "W range", false, Player.CharacterName));
            draw.Add(new MenuBool("eRange", "E range", false, Player.CharacterName));
            draw.Add(new MenuBool("rRange", "R range", false, Player.CharacterName));
            draw.Add(new MenuBool("onlyRdy", "Draw only ready spells", true, Player.CharacterName));
            wrapper.Add(draw);

            var q = new Menu("QConfig", "Q Config");
            q.Add(new MenuBool("autoQ2", "Auto Q", true, Player.CharacterName));
            q.Add(new MenuBool("autoQ", "Reduce Q use", true, Player.CharacterName));
            q.Add(new MenuBool("Qaoe", "Q aoe", true, Player.CharacterName));
            q.Add(new MenuBool("Qslow", "Q slow", true, Player.CharacterName));
            wrapper.Add(q);

            var w = new Menu("WConfig", "W Config");
            w.Add(new MenuBool("autoW", "Auto W on hard CC", true, Player.CharacterName));
            w.Add(new MenuBool("telE", "Auto W teleport", true, Player.CharacterName));
            w.Add(new MenuBool("forceW", "Force W before E", false, Player.CharacterName));
            w.Add(new MenuBool("bushW", "Auto W bush after enemy enter", true, Player.CharacterName));
            w.Add(new MenuBool("bushW2", "Auto W bush and turret if full ammo", true, Player.CharacterName));
            w.Add(new MenuBool("Wspell", "W on special spell detection", true, Player.CharacterName));
            var wgap = new Menu("wgap", "W Gap Closer");
            wgap.Add(new MenuList("WmodeGC", "Gap Closer position mode", new[] { "Dash end position", "My hero position" }, 0, Player.CharacterName));
            var wgaplist = new Menu("wgaplist", "Cast on enemy:");
            foreach (var enemy in GameObjects.EnemyHeroes)
                wgaplist.Add(new MenuBool("WGCchampion" + enemy.CharacterName, enemy.CharacterName, true, Player.CharacterName));
            wgap.Add(wgaplist);
            w.Add(wgap);
            wrapper.Add(w);

            var e = new Menu("EConfig", "E Config");
            e.Add(new MenuBool("autoE", "Auto E", true, Player.CharacterName));
            e.Add(new MenuBool("Ehitchance", "Auto E dash and immobile target", true, Player.CharacterName));
            e.Add(new MenuBool("harassEQ", "TRY E + Q", true, Player.CharacterName));
            e.Add(new MenuBool("EQks", "Ks E + Q + AA", true, Player.CharacterName));
            e.Add(new MenuKeyBind("useE", "Dash E HotKeySmartcast", Keys.T, KeyBindType.Press, Player.CharacterName));
            var egap = new Menu("egap", "E Gap Closer");
            egap.Add(new MenuList("EmodeGC", "Gap Closer position mode", new[] { "Dash end position", "Cursor position", "Enemy position" }, 2, Player.CharacterName));
            var egaplist = new Menu("egaplist", "Cast on enemy:");
            foreach (var enemy in GameObjects.EnemyHeroes)
                egaplist.Add(new MenuBool("EGCchampion" + enemy.CharacterName, enemy.CharacterName, true, Player.CharacterName));
            egap.Add(egaplist);
            e.Add(egap);
            wrapper.Add(e);

            var r = new Menu("RConfig", "R Config");
            r.Add(new MenuBool("autoR", "Auto R KS", true, Player.CharacterName));
            r.Add(new MenuSlider("Rcol", "R collision width [400]", 400, 1, 1000, Player.CharacterName));
            r.Add(new MenuSlider("Rrange", "R minimum range [1000]", 1000, 1, 1500, Player.CharacterName));
            r.Add(new MenuKeyBind("useR", "Semi-manual cast R key", Keys.T, KeyBindType.Press, Player.CharacterName));
            r.Add(new MenuBool("Rturret", "Don't R under enemy turret", true, Player.CharacterName));
            wrapper.Add(r);

            var farm = new Menu("farm", "Farm");
            farm.Add(new MenuBool("farmQ", "Lane clear Q", true, Player.CharacterName));
            farm.Add(new MenuSlider("LCMana", "Lane clear minimum Mana", 80, 30, 100, Player.CharacterName));
            farm.Add(new MenuSlider("LCMinions", "Lane clear minimum Minions", 2, 0, 10, Player.CharacterName));
            wrapper.Add(farm);

            Config.Add(wrapper);

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            AIBaseClient.OnDoCast += AIBaseClient_OnDoCast;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.W)
            {
                if (GameObjects.ParticleEmitters.Any(obj => obj.IsValid && obj.Position.Distance(args.StartPosition) < 300 && obj.Name.ToLower().Contains("yordleTrap_idle_green".ToLower())))
                    args.Process = false;
            }
            if (args.Slot == SpellSlot.E && Player.Mana > RMANA + WMANA  && Config[Player.CharacterName]["WConfig"].GetValue<MenuBool>("forceW").Enabled)
            {
                W.Cast(Player.Position.Extend(args.EndPosition, Player.Distance(args.EndPosition) + 50));
                DelayAction.Add(10, () => E.Cast(args.EndPosition));
            }
        }

        private void AIBaseClient_OnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && (args.SData.Name == "CaitlynPiltoverPeacemaker" || args.SData.Name == "CaitlynEntrapment"))
            {
                QCastTime = Game.Time;
            }

            if (!W.IsReady() || !sender.IsEnemy || sender.Type != GameObjectType.AIHeroClient || !sender.IsValidTarget(W.Range) || !Config[Player.CharacterName]["WConfig"].GetValue<MenuBool>("Wspell").Enabled)
                return;

            if ((sender as AIHeroClient).IsCastingImporantSpell())
                W.Cast(sender.Position);
        }

        private void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {
            if (Player.Mana > RMANA + WMANA)
            {
                if (E.IsReady() && sender.IsValidTarget(E.Range) && Config[Player.CharacterName]["EConfig"]["egap"]["egaplist"].GetValue<MenuBool>("EGCchampion" + sender.CharacterName).Enabled)
                {
                    var EmodeGC = Config[Player.CharacterName]["EConfig"]["egap"].GetValue<MenuList>("EmodeGC").Index;
                    if (EmodeGC == 0)
                        E.Cast(args.EndPosition);
                    else if (EmodeGC == 1)
                        E.Cast(Game.CursorPos);
                    else
                        E.Cast(sender.PreviousPosition);
                }
                else if (W.IsReady() && sender.IsValidTarget(W.Range) && Config[Player.CharacterName]["WConfig"]["wgap"]["wgaplist"].GetValue<MenuBool>("WGCchampion" + sender.CharacterName).Enabled)
                {
                    var WmodeGC = Config[Player.CharacterName]["WConfig"]["wgap"].GetValue<MenuList>("WmodeGC").Index;
                    if (WmodeGC == 0)
                        W.Cast(args.EndPosition);
                    else
                        W.Cast(Player.PreviousPosition);
                }
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            var onlyRdy = Config[Player.CharacterName]["draw"].GetValue<MenuBool>("onlyRdy");

            if (Config[Player.CharacterName]["draw"].GetValue<MenuBool>("qRange").Enabled)
            {
                if (onlyRdy)
                {
                    if (Q.IsReady())
                        Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1);
                }
                else
                    Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1);
            }

            if (Config[Player.CharacterName]["draw"].GetValue<MenuBool>("wRange").Enabled)
            {
                if (onlyRdy)
                {
                    if (W.IsReady())
                        Render.Circle.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Orange, 1);
                }
                else
                    Render.Circle.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Orange, 1);
            }

            if (Config[Player.CharacterName]["draw"].GetValue<MenuBool>("eRange").Enabled)
            {
                if (onlyRdy)
                {
                    if (E.IsReady())
                        Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Yellow, 1);
                }
                else
                    Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Yellow, 1);
            }

            if (Config[Player.CharacterName]["draw"].GetValue<MenuBool>("rRange").Enabled)
            {
                if (onlyRdy)
                {
                    if (R.IsReady())
                        Render.Circle.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Gray, 1);
                }
                else
                    Render.Circle.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Gray, 1);
            }

            if (Config[Player.CharacterName]["draw"].GetValue<MenuBool>("noti").Enabled)
            {
                var tr = TargetSelector.GetTarget(R.Range);

                if (tr.IsValidTarget() && R.IsReady())
                {
                    var rDamage = R.GetDamage(tr);
                    if (rDamage > tr.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "Ult can kill: " + tr.CharacterName + " have: " + tr.Health + " hp");
                        drawLine(tr.Position, Player.Position, 10, System.Drawing.Color.Yellow);
                    }
                }

                var tw = TargetSelector.GetTarget(W.Range);

                if (tw.IsValidTarget())
                {
                    if (Q.GetDamage(tw) > tw.Health)
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.4f, System.Drawing.Color.Red, "Q can kill: " + tw.CharacterName + " have: " + tw.Health + " hp");
                }
            }
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsRecalling())
                return;

            if (Config[Player.CharacterName]["RConfig"].GetValue<MenuKeyBind>("useR").Active && R.IsReady())
            {
                var t = TargetSelector.GetTarget(R.Range);
                if (t.IsValidTarget())
                    R.CastOnUnit(t);
            }

            if (LagFree(0))
            {
                SetMana();
                R.Range = (500 * R.Level) + 1500;
            }

            var orbT = Orbwalker.GetTarget() as AIHeroClient;
            if (orbT != null && orbT.Health - OktwCommon.GetIncomingDamage(orbT) < Player.GetAutoAttackDamage(orbT) * 2)
                return;

            if (LagFree(1) && E.IsReady() && Orbwalker.CanMove(40, false))
                LogicE();
            if (LagFree(2) && W.IsReady() && Orbwalker.CanMove(40, false))
                LogicW();
            if (LagFree(3) && Q.IsReady() && Orbwalker.CanMove(40, false) && Config[Player.CharacterName]["QConfig"].GetValue<MenuBool>("autoQ2").Enabled)
                LogicQ();
            if (LagFree(4) && R.IsReady() && Config[Player.CharacterName]["RConfig"].GetValue<MenuBool>("autoR").Enabled && !Player.IsUnderEnemyTurret() && Game.Time - QCastTime > 1)
                LogicR();
        }

        private void LogicQ()
        {
            if (Combo && Player.IsWindingUp)
                return;
            var t = TargetSelector.GetTarget(Q.Range);
            if (t.IsValidTarget())
            {
                var q = Config[Player.CharacterName]["QConfig"];
                if (GetRealDistance(t) > bonusRange() + 250 && !t.InAutoAttackRange() && OktwCommon.GetKsDamage(t, Q) > t.Health && Player.CountEnemyHeroesInRange(400) == 0)
                {
                    CastSpell(Q, t);
                }
                else if (Combo && Player.Mana > RMANA + QMANA + EMANA + 10 && Player.CountEnemyHeroesInRange(bonusRange() + 100 + t.BoundingRadius) == 0 && !q.GetValue<MenuBool>("autoQ").Enabled)
                {
                    CastSpell(Q, t);
                }
                if ((Combo || Harass) && Player.Mana > RMANA + QMANA && Player.CountEnemyHeroesInRange(400) == 0)
                {
                    foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsValidTarget(Q.Range) && (!OktwCommon.CanMove(enemy) || enemy.HasBuff("caitlynyordletrapinternal"))))
                        Q.Cast(enemy, true);
                    if (Player.CountEnemyHeroesInRange(bonusRange()) == 0 && OktwCommon.CanHarass())
                    {
                        if (t.HasBuffOfType(BuffType.Slow) && q.GetValue<MenuBool>("Qslow").Enabled)
                            Q.Cast(t);
                        if (q.GetValue<MenuBool>("Qaoe").Enabled)
                            Q.CastIfWillHit(t, 2, true);
                    }
                }
            }
            else if (LaneClear && Player.Mana > RMANA + QMANA)
            {
                var farm = Config[Player.CharacterName]["farm"] as Menu;
                if (farm.GetValue<MenuBool>("farmQ").Enabled && Player.ManaPercent > farm.GetValue<MenuSlider>("LCMana").Value)
                {
                    var minionList = Cache.GetMinions(Player.PreviousPosition, Q.Range);
                    var farmLocation = Q.GetLineFarmLocation(minionList, Q.Width);
                    if (farmLocation.MinionsHit > farm.GetValue<MenuSlider>("LCMinions").Value)
                        Q.Cast(farmLocation.Position);
                }
            }
        }

        private void LogicW()
        {
            if (Player.Mana > RMANA + WMANA)
            {
                var w = Config[Player.CharacterName]["WConfig"];

                if (w.GetValue<MenuBool>("autoW").Enabled)
                {
                    foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy) && !enemy.HasBuff("caitlynyordletrapinternal")))
                    {
                        W.Cast(enemy);
                    }
                }

                if (w.GetValue<MenuBool>("telE").Enabled)
                {
                    var trapPos = OktwCommon.GetTrapPos(W.Range);
                    if (!trapPos.IsZero)
                        W.Cast(trapPos);
                }

                if (!Orbwalker.CanMove(40, false))
                    return;

                if ((int)(Game.Time * 10) % 2 == 0 && w.GetValue<MenuBool>("bushW2").Enabled)
                {
                    if (Player.Spellbook.GetSpell(SpellSlot.W).Ammo == new int[] { 0, 3, 3, 4, 4, 5 }[W.Level] && Player.CountEnemyHeroesInRange(1000) == 0)
                    {
                        var points = OktwCommon.CirclePoints(8, W.Range, Player.Position);
                        foreach (var point in points)
                        {
                            if (NavMesh.IsWallOfType(point, CollisionFlags.Wall, 0) || NavMesh.IsWallOfType(point, CollisionFlags.Grass, 0)  || point.IsUnderEnemyTurret())
                            {
                                if (!OktwCommon.CirclePoints(8, 150, point).Any(x => x.IsWall()))
                                {
                                    W.Cast(point);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void LogicE()
        {
            if (Combo && Player.IsWindingUp)
                return;

            var e = Config[Player.CharacterName]["EConfig"];

            if (e.GetValue<MenuBool>("autoE").Enabled)
            {
                var t = TargetSelector.GetTarget(E.Range);

                if (t.IsValidTarget())
                {
                    var positionT = Player.PreviousPosition - (t.Position - Player.PreviousPosition);

                    if (Player.Position.Extend(positionT, 400).CountEnemyHeroesInRange(700) < 2)
                    {
                        var eDmg = E.GetDamage(t);
                        var qDmg = Q.GetDamage(t);
                        if (e.GetValue<MenuBool>("EQks").Enabled && qDmg + eDmg + Player.GetAutoAttackDamage(t) > t.Health && Player.Mana > EMANA + QMANA)
                        {
                            CastSpell(E, t);
                        }
                        else if ((Harass || Combo) && e.GetValue<MenuBool>("harassEQ").Enabled && Player.Mana > EMANA + QMANA + RMANA)
                        {
                            CastSpell(E, t);
                        }
                    }

                    if (Player.Mana > RMANA + EMANA)
                    {
                        if (e.GetValue<MenuBool>("Ehitchance").Enabled)
                        {
                            E.CastIfHitchanceEquals(t, HitChance.Dash);
                        }
                        if (Player.Health < Player.MaxHealth * 0.3)
                        {
                            if (GetRealDistance(t) < 500)
                                E.Cast(t, true);
                            if (Player.CountEnemyHeroesInRange(250) > 0)
                                E.Cast(t, true);
                        }
                    }
                }
            }

            if (e.GetValue<MenuKeyBind>("useE").Active)
            {
                E.Cast(Player.PreviousPosition - (Game.CursorPos - Player.PreviousPosition), true);
            }
        }

        private void LogicR()
        {
            var r = Config[Player.CharacterName]["RConfig"];

            if (Player.IsUnderEnemyTurret() && r.GetValue<MenuBool>("Rturret").Enabled)
                return;

            var cast = false;

            foreach (var target in GameObjects.EnemyHeroes.Where(target => target.IsValidTarget(R.Range) && Player.Distance(target) > r.GetValue<MenuSlider>("Rrange") && target.CountEnemyHeroesInRange(r.GetValue<MenuSlider>("Rcol")) == 1 && target.CountAllyHeroesInRange(500) == 0 && OktwCommon.ValidUlt(target)))
            {
                if (OktwCommon.GetKsDamage(target, R) > target.Health)
                {
                    cast = true;
                    var output = R.GetPrediction(target);
                    var direction = (output.CastPosition.ToVector2() - Player.Position.ToVector2()).Normalized();
                    foreach (var enemy in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget()))
                    {
                        if (enemy.NetworkId == target.NetworkId || !cast)
                            continue;
                        var prediction = R.GetPrediction(enemy);
                        var predictionPosition = prediction.CastPosition;
                        var v = output.CastPosition - Player.PreviousPosition;
                        var w = predictionPosition - Player.PreviousPosition;
                        var c1 = Vector3.Dot(w, v);
                        var c2 = Vector3.Dot(v, v);
                        var b = c1 / c2;
                        var pb = Player.PreviousPosition + b * v;
                        var length = Vector3.Distance(predictionPosition, pb);
                        if (length < (r.GetValue<MenuSlider>("Rcol").Value + enemy.BoundingRadius) && Player.Distance(predictionPosition) < Player.Distance(target.PreviousPosition))
                            cast = false;
                    }
                    if (cast)
                        R.CastOnUnit(target);
                }
            }
        }

        private float GetRealDistance(GameObject target)
        {
            return Player.PreviousPosition.Distance(target.Position) + Player.BoundingRadius + target.BoundingRadius;
        }

        private float bonusRange()
        {
            return 720f + Player.BoundingRadius;
        }

        private void SetMana()
        {
            if ((Config["extraSet"].GetValue<MenuBool>("manaDisable").Enabled && Combo) || Player.HealthPercent < 20)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
                return;
            }

            QMANA = Q.Instance.SData.ManaArray[Math.Max(0, Q.Level - 1)];
            WMANA = W.Instance.SData.ManaArray[Math.Max(0, W.Level - 1)];
            EMANA = E.Instance.SData.ManaArray[Math.Max(0, E.Level - 1)];

            if (!R.IsReady())
                RMANA = QMANA - Player.PARRegenRate * Q.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.ManaArray[Math.Max(0, R.Level - 1)];
        }       
    }
}
