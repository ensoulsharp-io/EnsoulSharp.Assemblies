using System;
using System.Linq;
using System.Windows.Forms;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Core.UI;
using EnsoulSharp.SDK.Core.UI.IMenu.Values;
using EnsoulSharp.SDK.Core.Utils;
using EnsoulSharp.SDK.Core.Wrappers.Damages;
using SebbyLib;
using Menu = EnsoulSharp.SDK.Core.UI.IMenu.Menu;

namespace OneKeyToWin_AIO_Sebby.Core
{
    class Activator
    {
        private Menu Config = Program.Config;
        private AIHeroClient Player { get { return ObjectManager.Player; } }

        public static Items.Item
            // Clean
            Mikaels = new Items.Item(3222, 600f),
            Quicksilver = new Items.Item(3140, 0),
            Mercurial = new Items.Item(3139, 0),
            Dervish = new Items.Item(3137, 0),
            // Regen
            Potion = new Items.Item(2003, 0),
            ManaPotion = new Items.Item(2004, 0),
            Flask = new Items.Item(2041, 0),
            Biscuit = new Items.Item(2010, 0),
            Refillable = new Items.Item(2031, 0),
            Hunter = new Items.Item(2032, 0),
            Corrupting = new Items.Item(2033, 0),
            // Offensive
            Botrk = new Items.Item(3153, 550f),
            Cutlass = new Items.Item(3144, 550f),
            Youmuus = new Items.Item(3142, 650f),
            Hydra = new Items.Item(3074, 440f),
            Hydra2 = new Items.Item(3077, 440f),
            HydraTitanic = new Items.Item(3748, 150f),
            Hextech = new Items.Item(3146, 700f),
            Protobelt = new Items.Item(3152, 850f),
            GLP800 = new Items.Item(3030, 800f),
            // Defensive
            Zhonya = new Items.Item(3157, 0),
            Stopwatch = new Items.Item(2420, 0),
            RepStopwatch = new Items.Item(2423, 0),
            Seraph = new Items.Item(3040, 0),
            Solari = new Items.Item(3190, 600f),
            Randuin = new Items.Item(3143, 400f);

        private SpellSlot Heal { get { return Player.GetSpellSlot("SummonerHeal"); } }
        private SpellSlot Barrier { get { return Player.GetSpellSlot("SummonerBarrier"); } }
        private SpellSlot Ignite { get { return Player.GetSpellSlot("SummonerDot"); } }
        private SpellSlot Exhaust { get { return Player.GetSpellSlot("SummonerExhaust"); } }
        private SpellSlot Cleanse { get { return Player.GetSpellSlot("SummonerBoost"); } }
        private SpellSlot Smite
        {
            get
            {
                var slot = Player.GetSpellSlot("SummonerSmite");

                if (slot == SpellSlot.Unknown) { slot = Player.GetSpellSlot("S5_SummonerSmitePlayerGanker"); }
                if (slot == SpellSlot.Unknown) { slot = Player.GetSpellSlot("S5_SummonerSmiteDue1"); }

                return slot;
            }
        }

        public void LoadOKTW()
        {
            var activator = new Menu("activator", "Activator OKTW©");

            #region Summoner

            var summoner = new Menu("summoner", "Summoners");

            var smite = new Menu("smite", "Smite");
            smite.Add(new MenuBool("SmiteEnemy", "Auto Smite enemy under 50% hp", true));
            smite.Add(new MenuBool("SmiteEnemyKS", "Auto Smite enemy KS", true));
            smite.Add(new MenuKeyBind("Smite", "Auto Smite mobs OKTW", Keys.N, KeyBindType.Toggle));
            smite.Add(new MenuBool("Rdragon", "Dragon", true, Player.CharacterName));
            smite.Add(new MenuBool("Rbaron", "Baron", true, Player.CharacterName));
            smite.Add(new MenuBool("Rherald", "Herald", true, Player.CharacterName));
            smite.Add(new MenuBool("Rred", "Red", true, Player.CharacterName));
            smite.Add(new MenuBool("Rblue", "Blue", true, Player.CharacterName));
            smite.GetValue<MenuKeyBind>("Smite").Permashow(true);
            summoner.Add(smite);

            var exhaust = new Menu("exhaust", "Exhaust");
            exhaust.Add(new MenuBool("Exhaust", "Exhaust", true));
            exhaust.Add(new MenuBool("Exhaust1", "Exhaust if Channeling Important Spell", true));
            exhaust.Add(new MenuBool("Exhaust2", "Always in combo"));
            summoner.Add(exhaust);

            var heal = new Menu("heal", "Heal");
            heal.Add(new MenuBool("Heal", "Heal", true));
            heal.Add(new MenuBool("AllyHeal", "AllyHeal", true));
            summoner.Add(heal);

            summoner.Add(new MenuBool("Barrier", "Barrier", true));
            summoner.Add(new MenuBool("Ignite", "Ignite", true));
            summoner.Add(new MenuBool("Cleanse", "Cleanse", true));

            activator.Add(summoner);

            #endregion

            activator.Add(new MenuBool("pots", "Potion, ManaPotion, Flask, Biscuit, Refillable, Hunter, Corrupting", true));

            #region Offensives

            var offensives = new Menu("offensives", "Offensives");

            var botrk = new Menu("botrk", "Botrk");
            botrk.Add(new MenuBool("Botrk", "Botrk", true));
            botrk.Add(new MenuBool("BotrkKS", "Botrk KS", true));
            botrk.Add(new MenuBool("BotrkCombo", "Botrk always in combo", true));
            offensives.Add(botrk);

            var cutlass = new Menu("cutlass", "Cutlass");
            cutlass.Add(new MenuBool("Cutlass", "Cutlass", true));
            cutlass.Add(new MenuBool("CutlassKS", "Cutlass KS", true));
            cutlass.Add(new MenuBool("CutlassCombo", "Cutlass always in combo", true));
            offensives.Add(cutlass);

            var hextech = new Menu("hextech", "Hextech");
            hextech.Add(new MenuBool("Hextech", "Hextech", true));
            hextech.Add(new MenuBool("HextechKS", "Hextech KS", true));
            hextech.Add(new MenuBool("HextechCombo", "Hextach always in combo", true));
            offensives.Add(hextech);

            var protobelt = new Menu("protobelt", "Protobelt");
            protobelt.Add(new MenuBool("Protobelt", "Protobelt", true));
            protobelt.Add(new MenuBool("ProtobeltKS", "Protobelt KS", true));
            protobelt.Add(new MenuBool("ProtobeltCombo", "Protobelt always in combo", true));
            offensives.Add(protobelt);

            var glp800 = new Menu("glp800", "GLP800");
            glp800.Add(new MenuBool("GLP800", "GLP800", true));
            glp800.Add(new MenuBool("GLP800KS", "GLP800 KS", true));
            glp800.Add(new MenuBool("GLP800Combo", "GLP800 always in combo", true));
            offensives.Add(glp800);

            var youmuus = new Menu("youmuus", "Youmuus");
            youmuus.Add(new MenuBool("Youmuus", "Youmuus", true));
            youmuus.Add(new MenuBool("YoumuusR", "TwitchR, AsheQ", true));
            youmuus.Add(new MenuBool("YoumuusKS", "Youmuus KS", true));
            youmuus.Add(new MenuBool("YoumuusCombo", "Youmuus always in combo"));
            offensives.Add(youmuus);

            var hydra = new Menu("hydra", "Hydra");
            hydra.Add(new MenuBool("Hydra", "Hydra", true));
            offensives.Add(hydra);

            var hydratitanic = new Menu("hydratitanic", "HydraTitanic");
            hydratitanic.Add(new MenuBool("HydraTitanic", "Hydra Titanic", true));
            offensives.Add(hydratitanic);

            activator.Add(offensives);

            #endregion

            #region Defensive

            var defensives = new Menu("defensives", "Defensives");

            defensives.Add(new MenuBool("Randuin", "Randuin", true));

            var zhonya = new Menu("zhonya", "Zhonya");
            zhonya.Add(new MenuBool("Zhonya", "Zhonya", true));
            foreach (var enemy in GameObjects.EnemyHeroes)
            {
                var spell = enemy.Spellbook.Spells[3];
                zhonya.Add(new MenuBool("spellZ" + spell.SData.Name, enemy.CharacterName + ": " + spell.Name, spell.SData.TargettingType == SpellDataTargetType.Unit));
            }
            defensives.Add(zhonya);

            defensives.Add(new MenuBool("Seraph", "Seraph", true));
            defensives.Add(new MenuBool("Solari", "Solari", true));

            activator.Add(defensives);

            #endregion

            #region Cleansers

            var cleansers = new Menu("cleansers", "Cleansers");

            cleansers.Add(new MenuBool("Clean", "Quicksilver, Miskaels, Mercurial, Dervish", true));

            var mikaelsally = new Menu("Mikaelsallys", "Mikaels allys");
            foreach (var ally in GameObjects.AllyHeroes)
                mikaelsally.Add(new MenuBool("MikaelsAlly" + ally.CharacterName, ally.CharacterName, true));
            cleansers.Add(mikaelsally);

            cleansers.Add(new MenuSlider("CSSdelay", "Delay x ms", 0, 0, 1000));
            cleansers.Add(new MenuSlider("cleanHP", "Use only under % HP", 80, 0, 100));

            var bufftype = new Menu("bufftype", "Buff type");
            bufftype.Add(new MenuBool("Stun", "Stun", true));
            bufftype.Add(new MenuBool("Snare", "Snare", true));
            bufftype.Add(new MenuBool("Charm", "Charm", true));
            bufftype.Add(new MenuBool("Flee", "Flee", true));
            bufftype.Add(new MenuBool("Suppression", "Suppression", true));
            bufftype.Add(new MenuBool("Taunt", "Taunt", true));
            bufftype.Add(new MenuBool("Blind", "Blind", true));
            cleansers.Add(bufftype);

            activator.Add(cleansers);

            #endregion

            Config.Add(activator);

            Game.OnUpdate += Game_OnUpdate;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            AIBaseClient.OnDoCast += AIBaseClient_OnDoCast;
            Variables.Orbwalker.OnAction += Orbwalker_OnAction;
        }

        private void Orbwalker_OnAction(object sender, OrbwalkingActionArgs args)
        {
            if (args.Type == OrbwalkingType.AfterAttack)
            {
                if (Config["activator"]["offensives"]["hydratitanic"].GetValue<MenuBool>("HydraTitanic").Value && Program.Combo && HydraTitanic.IsReady && args.Target.Type == GameObjectType.AIHeroClient)
                {
                    HydraTitanic.Cast();
                }
            }
        }

        private void AIBaseClient_OnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (!sender.IsEnemy || sender.Type != GameObjectType.AIHeroClient)
                return;

            if (sender.Distance(Player.Position) > 1600)
                return;

            if (ZhonyaReady() && Config["activator"]["defensives"]["zhonya"].GetValue<MenuBool>("Zhonya").Value)
            {
                var e = Config["activator"]["defensives"]["zhonya"]["spellZ" + args.SData.Name];
                if (e != null && (e as MenuBool).Value)
                {
                    if (args.Target != null && args.Target.NetworkId == Player.NetworkId)
                    {
                        ZhonyaTryCast();
                    }
                    else if (args.To.Distance(Player.Position) < Player.BoundingRadius / 2)
                    {
                        ZhonyaTryCast();
                    }
                }
            }

            if (CanUse(Exhaust) && Config["activator"]["summoner"]["exhaust"].GetValue<MenuBool>("Exhaust").Value)
            {
                foreach (var ally in GameObjects.AllyHeroes.Where(a => a.IsValidTarget(700, false) && a.HealthPercent < 50))
                {
                    var dmg = 0d;
                    if (args.Target != null && args.Target.NetworkId == ally.NetworkId)
                    {
                        dmg += (sender as AIHeroClient).GetSpellDamage(ally, args.Slot);
                    }
                    else
                    {
                        if (args.To.Distance(ally.Position) < ally.BoundingRadius / 2)
                            dmg += (sender as AIHeroClient).GetSpellDamage(ally, args.Slot);
                        else
                            continue;
                    }

                    if (ally.Health - dmg < ally.CountEnemyHeroesInRange(700) * ally.Level * 40)
                        Player.Spellbook.CastSpell(Exhaust, sender);
                }
            }
        }

        private bool ZhonyaReady()
        {
            return (Zhonya.IsReady || Stopwatch.IsReady || RepStopwatch.IsReady);
        }

        private void ZhonyaTryCast()
        {
            if (Player.HasBuffOfType(BuffType.PhysicalImmunity) || Player.HasBuffOfType(BuffType.SpellImmunity)
                || (Player.Spellbook.Spells[3].IsReady && Player.CharacterName == "Kayle")
                || Player.IsZombie || Player.IsInvulnerable || Player.HasBuffOfType(BuffType.Invulnerability)
                || Player.HasBuff("KindredRNoDeathBuff")
                || Player.HasBuffOfType(BuffType.SpellShield)
                || Player.AllShield > OktwCommon.GetIncomingDamage(Player))
            {

            }
            else
            {
                if (Zhonya.IsReady)
                {
                    Zhonya.Cast();
                }
                else if (RepStopwatch.IsReady)
                {
                    RepStopwatch.Cast();
                }
                else if (Stopwatch.IsReady)
                {
                    Stopwatch.Cast();
                }
            }
        }

        private void Survival()
        {
            if (Player.HealthPercent < 60 && (Seraph.IsReady || ZhonyaReady() || CanUse(Barrier)))
            {
                var dmg = OktwCommon.GetIncomingDamage(Player, 1);
                var enemys = Player.CountEnemyHeroesInRange(800);
                if (dmg > 0 || enemys > 0)
                {
                    if (CanUse(Barrier) && Config["activator"]["summoner"].GetValue<MenuBool>("Barrier").Value)
                    {
                        var value = 95 + Player.Level * 20;
                        if (dmg > value && Player.HealthPercent < 50)
                            Player.Spellbook.CastSpell(Barrier, Player);
                        else if (Player.Health - dmg < enemys * Player.Level * 20)
                            Player.Spellbook.CastSpell(Barrier, Player);
                        else if (Player.Health - dmg < Player.Level * 10)
                            Player.Spellbook.CastSpell(Barrier, Player);
                    }

                    if (Seraph.IsReady && Config["activator"]["defensives"].GetValue<MenuBool>("Seraph").Value)
                    {
                        var value = 150 + Player.Mana * 0.15;
                        if (dmg > value && Player.HealthPercent < 50)
                            Seraph.Cast();
                        else if (Player.Health - dmg < enemys * Player.Level * 20)
                            Seraph.Cast();
                        else if (Player.Health - dmg < Player.Level * 10)
                            Seraph.Cast();
                    }

                    if (ZhonyaReady() && Config["activator"]["defensives"]["zhonya"].GetValue<MenuBool>("Zhonya").Value)
                    {
                        if (dmg > Player.Level * 35)
                        {
                            ZhonyaTryCast();
                        }
                        else if (Player.Health - dmg < enemys * Player.Level * 20)
                        {
                            ZhonyaTryCast();
                        }
                        else if (Player.Health - dmg < Player.Level * 10)
                        {
                            ZhonyaTryCast();
                        }
                    }
                }
            }

            if (!Solari.IsReady && !CanUse(Heal))
                return;

            foreach (var ally in GameObjects.AllyHeroes.Where(a => a.IsValidTarget(700, false) && a.HealthPercent < 50))
            {
                var dmg = OktwCommon.GetIncomingDamage(ally, 1);
                var enemys = ally.CountEnemyHeroesInRange(700);
                if (dmg == 0 && enemys == 0)
                    continue;

                if (CanUse(Heal) && Config["activator"]["summoner"]["heal"].GetValue<MenuBool>("Heal").Value)
                {
                    if (!Config["activator"]["summoner"]["heal"].GetValue<MenuBool>("AllyHeal").Value && !ally.IsMe)
                        return;

                    if (ally.Health - dmg < enemys * ally.Level * 15)
                        Player.Spellbook.CastSpell(Heal, ally);
                    else if (ally.Health - dmg < ally.Level * 10)
                        Player.Spellbook.CastSpell(Heal, ally);
                }

                if (Config["activator"]["defensives"].GetValue<MenuBool>("Solari").Value && Solari.IsReady && Player.Distance(ally.Position) < Solari.Range)
                {
                    var value = 30 + 15 * Player.Level + 0.2 * Player.BonusHealth;
                    if (dmg > value && Player.HealthPercent < 50)
                        Solari.Cast();
                    else if (ally.Health - dmg < enemys * ally.Level * 15)
                        Solari.Cast();
                    else if (ally.Health - dmg < ally.Level * 10)
                        Solari.Cast();
                }
            }
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!Youmuus.IsReady || !Config["activator"]["offensives"]["youmuus"].GetValue<MenuBool>("YoumuusR").Value)
                return;
            if (args.Slot == SpellSlot.R && Player.CharacterName == "Twitch")
                Youmuus.Cast();
            if (args.Slot == SpellSlot.Q && Player.CharacterName == "Ashe")
                Youmuus.Cast();
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (Player.InFountain() || Player.IsRecalling() || Player.IsDead)
            {
                return;
            }

            Cleansers();
            Smitee();
            Survival();

            if (!Program.LagFree(0))
                return;

            if (Config["activator"].GetValue<MenuBool>("pots").Value)
                PotionManagement();

            Ignitee();
            Exhaustt();
            Offensive();
            Defensive();
            ZhonyaCast();
        }

        private double SmiteDmg()
        {
            var dmg = new[] { 390, 410, 430, 450, 480, 510, 540, 570, 600, 640, 680, 720, 760, 800, 850, 900, 950, 1000 };
            return dmg[Math.Max(0, Player.Level - 1)];
        }

        private double IgniteDmg()
        {
            return (50 + 20 * Player.Level);
        }

        private void Smitee()
        {
            if (CanUse(Smite))
            {
                var mobs = GameObjects.Jungle.Where(e => e.IsValidTarget(520, false));
                if (mobs.Count() == 0 && (Player.GetSpellSlot("S5_SummonerSmitePlayerGanker") != SpellSlot.Unknown || Player.GetSpellSlot("S5_SummonerSmiteDue1") != SpellSlot.Unknown))
                {
                    var enemy = Variables.TargetSelector.GetTarget(500, DamageType.True);
                    if (enemy.IsValidTarget())
                    {
                        if (enemy.HealthPercent < 50 && Config["activator"]["summoner"]["smite"].GetValue<MenuBool>("SmiteEnemy").Value)
                            Player.Spellbook.CastSpell(Smite, enemy);

                        var smiteDmg = SmiteDmg();

                        if (Config["activator"]["summoner"]["smite"].GetValue<MenuBool>("SmiteEnemyKS").Value && enemy.Health - OktwCommon.GetIncomingDamage(enemy) < smiteDmg)
                            Player.Spellbook.CastSpell(Smite, enemy);
                    }
                }
                if (mobs.Count() > 0 && Config["activator"]["summoner"]["smite"].GetValue<MenuKeyBind>("Smite").Active)
                {
                    var smite = Config["activator"]["summoner"]["smite"];
                    foreach (var mob in mobs)
                    {
                        if (((mob.CharacterName.ToLower().Contains("dragon") && smite.GetValue<MenuBool>("Rdragon"))
                            || (mob.CharacterName == "SRU_Baron" && smite.GetValue<MenuBool>("Rbaron"))
                            || (mob.CharacterName == "SRU_RiftHerald" && smite.GetValue<MenuBool>("Rherald"))
                            || (mob.CharacterName == "SRU_Red" && smite.GetValue<MenuBool>("Rred"))
                            || (mob.CharacterName == "SRU_Blue" && smite.GetValue<MenuBool>("Rblue")))
                            && mob.Health <= SmiteDmg())
                        {
                            Player.Spellbook.CastSpell(Smite, mob);
                        }
                    }
                }
            }
        }

        private void Exhaustt()
        {
            var exhaust = Config["activator"]["summoner"]["exhaust"];
            if (CanUse(Exhaust) && exhaust.GetValue<MenuBool>("Exhaust").Value)
            {
                if (exhaust.GetValue<MenuBool>("Exhaust1").Value)
                {
                    foreach (var enemy in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(650) && e.IsCastingInterruptableSpell()))
                    {
                        Player.Spellbook.CastSpell(Exhaust, enemy);
                    }
                }

                if (exhaust.GetValue<MenuBool>("Exhaust2").Value && Program.Combo)
                {
                    var t = Variables.TargetSelector.GetTarget(650, DamageType.Physical);
                    if (t.IsValidTarget())
                    {
                        Player.Spellbook.CastSpell(Exhaust, t);
                    }
                }
            }
        }

        private void Ignitee()
        {
            if (CanUse(Ignite) && Config["activator"]["summoner"].GetValue<MenuBool>("Ignite").Value)
            {
                foreach (var enemy in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(600)))
                {
                    var pred = enemy.Health - OktwCommon.GetIncomingDamage(enemy);
                    var IgnDmg = IgniteDmg();

                    if (pred <= 2 * IgnDmg && OktwCommon.ValidUlt(enemy))
                    {
                        if (pred <= IgnDmg && enemy.CountAllyHeroesInRange(450) < 2)
                        {
                            var enemyPred = Movement.GetPrediction(enemy, 0.1f).CastPosition;
                            if (Player.Position.Distance(enemyPred) > 500 || NavMesh.IsWallOfGrass(enemyPred, 0))
                                Player.Spellbook.CastSpell(Ignite, enemy);
                        }

                        if (enemy.PercentLifeStealMod > 0.1)
                            Player.Spellbook.CastSpell(Ignite, enemy);

                        if (enemy.HasBuff("RegenerationPotion") || enemy.HasBuff("Item2010")
                            || enemy.HasBuff("ItemCrystalFlask") || enemy.HasBuff("ItemCrystalFlaskJungle") || enemy.HasBuff("ItemDarkCrystalFlask"))
                            Player.Spellbook.CastSpell(Ignite, enemy);

                        if (enemy.Health > Player.Health)
                            Player.Spellbook.CastSpell(Ignite, enemy);
                    }
                }
            }
        }

        private void ZhonyaCast()
        {
            if (Config["activator"]["defensives"]["zhonya"].GetValue<MenuBool>("Zhonya").Value && ZhonyaReady())
            {
                var time = 10f;
                if (Player.HasBuff("zedrdeathmark"))
                {
                    time = OktwCommon.GetPassiveTime(Player, "zedrdeathmark");
                }
                if (Player.HasBuff("fizzrbomb"))
                {
                    time = OktwCommon.GetPassiveTime(Player, "fizzrbomb");
                }
                if (Player.HasBuff("vladimirhemoplaguedebuff"))
                {
                    time = OktwCommon.GetPassiveTime(Player, "vladimirhemoplaguedebuff");
                }
                if (time < 1 && time >0)
                {
                    ZhonyaTryCast();
                }
            }
        }

        private void Cleansers()
        {
            if (!Quicksilver.IsReady && !Mikaels.IsReady && !Mercurial.IsReady && !Dervish.IsReady && !Cleanse.IsReady())
                return;

            if (Player.HealthPercent >= Config["activator"]["cleansers"].GetValue<MenuSlider>("cleanHP").Value || !Config["activator"]["cleansers"].GetValue<MenuBool>("Clean").Value)
                return;

            var bt = Config["activator"]["cleansers"]["bufftype"];

            if (Mikaels.IsReady)
            {
                var mikaels = Config["activator"]["cleansers"]["Mikaelsallys"];
                foreach (var ally in GameObjects.AllyHeroes.Where(a => a.IsValidTarget(Mikaels.Range,false)))
                {
                    var e = mikaels["MikaelsAlly" + ally.CharacterName] as MenuBool;
                    if (e != null && e.Value && ally.HealthPercent < Config["activator"]["cleansers"].GetValue<MenuSlider>("cleanHP").Value)
                    {
                        if (ally.HasBuffOfType(BuffType.Stun) && bt.GetValue<MenuBool>("Stun").Value)
                            Mikaels.Cast(ally);
                        if (ally.HasBuffOfType(BuffType.Snare) && bt.GetValue<MenuBool>("Snare").Value)
                            Mikaels.Cast(ally);
                        if (ally.HasBuffOfType(BuffType.Charm) && bt.GetValue<MenuBool>("Charm").Value)
                            Mikaels.Cast(ally);
                        if (ally.HasBuffOfType(BuffType.Flee) && bt.GetValue<MenuBool>("Flee").Value)
                            Mikaels.Cast(ally);
                        if (ally.HasBuffOfType(BuffType.Suppression) && bt.GetValue<MenuBool>("Suppression").Value)
                            Mikaels.Cast(ally);
                        if (ally.HasBuffOfType(BuffType.Taunt) && bt.GetValue<MenuBool>("Taunt").Value)
                            Mikaels.Cast(ally);
                        if (ally.HasBuffOfType(BuffType.Blind) && bt.GetValue<MenuBool>("Blind").Value)
                            Mikaels.Cast(ally);
                    }
                }
            }

            if (Player.HasBuffOfType(BuffType.Stun) && bt.GetValue<MenuBool>("Stun").Value)
                Clean();
            if (Player.HasBuffOfType(BuffType.Snare) && bt.GetValue<MenuBool>("Snare").Value)
                Clean();
            if (Player.HasBuffOfType(BuffType.Charm) && bt.GetValue<MenuBool>("Charm").Value)
                Clean();
            if (Player.HasBuffOfType(BuffType.Flee) && bt.GetValue<MenuBool>("Flee").Value)
                Clean();
            if (Player.HasBuffOfType(BuffType.Suppression) && bt.GetValue<MenuBool>("Suppression").Value)
                Clean();
            if (Player.HasBuffOfType(BuffType.Taunt) && bt.GetValue<MenuBool>("Taunt").Value)
                Clean();
            if (Player.HasBuffOfType(BuffType.Blind) && bt.GetValue<MenuBool>("Blind").Value)
                Clean();
        }

        private void Clean()
        {
            if (Quicksilver.IsReady)
                DelayAction.Add(Config["activator"]["cleansers"].GetValue<MenuSlider>("CSSdelay").Value, () => Quicksilver.Cast());
            else if (Mercurial.IsReady)
                DelayAction.Add(Config["activator"]["cleansers"].GetValue<MenuSlider>("CSSdelay").Value, () => Mercurial.Cast());
            else if (Dervish.IsReady)
                DelayAction.Add(Config["activator"]["cleansers"].GetValue<MenuSlider>("CSSdelay").Value, () => Dervish.Cast());
            else if (CanUse(Cleanse) && Config["activator"]["summoner"].GetValue<MenuBool>("Cleanse").Value)
                DelayAction.Add(Config["activator"]["cleansers"].GetValue<MenuSlider>("CSSdelay").Value, () => Player.Spellbook.CastSpell(Cleanse, Player));
        }

        private void Defensive()
        {
            if (Randuin.IsReady && Config["activator"]["defensives"].GetValue<MenuBool>("Randuin").Value && Player.CountEnemyHeroesInRange(Randuin.Range) > 0)
            {
                Randuin.Cast();
            }
        }

        private void Offensive()
        {
            var botrk = Config["activator"]["offensives"]["botrk"];
            if (Botrk.IsReady && botrk.GetValue<MenuBool>("Botrk").Value)
            {
                var t = Variables.TargetSelector.GetTarget(Botrk.Range, DamageType.Magical);
                if (t.IsValidTarget())
                {
                    if (botrk.GetValue<MenuBool>("BotrkKS").Value && Player.CalculateDamage(t, DamageType.Magical, 100) > t.Health - OktwCommon.GetIncomingDamage(t))
                        Botrk.Cast(t);
                    if (botrk.GetValue<MenuBool>("BotrkCombo").Value && Program.Combo)
                        Botrk.Cast(t);
                }
            }

            var glp800 = Config["activator"]["offensives"]["glp800"];
            if (GLP800.IsReady && glp800.GetValue<MenuBool>("GLP800").Value)
            {
                var t = Variables.TargetSelector.GetTarget(GLP800.Range, DamageType.Magical);
                if (t.IsValidTarget())
                {
                    if (glp800.GetValue<MenuBool>("GLP800KS").Value && Player.CalculateDamage(t, DamageType.Magical, 100 + 100f / 17 * (Player.Level - 1) + 0.2 * Player.TotalMagicalDamage) > t.Health - OktwCommon.GetIncomingDamage(t))
                        GLP800.Cast(Movement.GetPrediction(t, 0.5f).CastPosition);
                    if (glp800.GetValue<MenuBool>("GLP800Combo").Value && Program.Combo)
                        GLP800.Cast(Movement.GetPrediction(t, 0.5f).CastPosition);
                }
            }

            var protobelt = Config["activator"]["offensives"]["protobelt"];
            if (Protobelt.IsReady && protobelt.GetValue<MenuBool>("Protobelt").Value)
            {
                var t = Variables.TargetSelector.GetTarget(Protobelt.Range, DamageType.Magical);
                if (t.IsValidTarget())
                {
                    if (protobelt.GetValue<MenuBool>("ProtobeltKS").Value && Player.CalculateDamage(t, DamageType.Magical, 75 + 75f / 17 * (Player.Level - 1) + 0.25 * Player.TotalMagicalDamage) > t.Health - OktwCommon.GetIncomingDamage(t))
                        Protobelt.Cast(Movement.GetPrediction(t, 0.5f).CastPosition);
                    if (protobelt.GetValue<MenuBool>("ProtobeltCombo").Value && Program.Combo)
                        Protobelt.Cast(Movement.GetPrediction(t, 0.5f).CastPosition);
                }
            }

            var hextech = Config["activator"]["offensives"]["hextech"];
            if (Hextech.IsReady && hextech.GetValue<MenuBool>("Hextech").Value)
            {
                var t = Variables.TargetSelector.GetTarget(Hextech.Range, DamageType.Magical);
                if (t.IsValidTarget())
                {
                    if (hextech.GetValue<MenuBool>("HextechKS").Value && Player.CalculateDamage(t, DamageType.Magical, 175 + 78f / 17 * (Player.Level - 1) + 0.3 * Player.TotalMagicalDamage) > t.Health - OktwCommon.GetIncomingDamage(t))
                        Hextech.Cast(t);
                    if (hextech.GetValue<MenuBool>("HextechCombo").Value && Program.Combo)
                        Hextech.Cast(t);
                }
            }

            var cutlass = Config["activator"]["offensives"]["cutlass"];
            if (Cutlass.IsReady && cutlass.GetValue<MenuBool>("Cutlass").Value)
            {
                var t = Variables.TargetSelector.GetTarget(Cutlass.Range, DamageType.Magical);
                if (t.IsValidTarget())
                {
                    if (cutlass.GetValue<MenuBool>("CutlassKS").Value && Player.CalculateDamage(t, DamageType.Magical, 100) > t.Health - OktwCommon.GetIncomingDamage(t))
                        Cutlass.Cast(t);
                    if (cutlass.GetValue<MenuBool>("CutlassCombo").Value && Program.Combo)
                        Cutlass.Cast(t);
                }
            }

            var youmuus = Config["activator"]["offensives"]["youmuus"];
            if (Youmuus.IsReady && youmuus.GetValue<MenuBool>("Youmuus").Value && Program.Combo)
            {
                var t = Variables.Orbwalker.GetTarget();
                if (t.IsValidTarget() && t is AIHeroClient)
                {
                    if (youmuus.GetValue<MenuBool>("YoumuusKS").Value && t.Health < Player.MaxHealth)
                        Youmuus.Cast();
                    if (youmuus.GetValue<MenuBool>("YoumuusCombo").Value)
                        Youmuus.Cast();
                }
            }

            if (Config["activator"]["offensives"]["hydra"].GetValue<MenuBool>("Hydra").Value)
            {
                if (Hydra.IsReady && Player.CountEnemyHeroesInRange(Hydra.Range) > 0)
                    Hydra.Cast();
                else if (Hydra2.IsReady && Player.CountEnemyHeroesInRange(Hydra2.Range) > 0)
                    Hydra2.Cast();
            }
        }

        private void PotionManagement()
        {
            if (Player.Health + 250 > Player.MaxHealth)
                return;

            if (Player.HealthPercent > 50 && Player.CountEnemyHeroesInRange(700) == 0)
                return;

            if (Player.HasBuff("RegenerationPotion")
                || Player.HasBuff("Item2010")
                || Player.HasBuff("ItemCrystalFlask")
                || Player.HasBuff("ItemCrystalFlaskJungle")
                || Player.HasBuff("ItemDarkCrystalFlask"))
                return;

            if (Refillable.IsReady)
                Refillable.Cast();
            else if (Potion.IsReady)
                Potion.Cast();
            else if (Biscuit.IsReady)
                Biscuit.Cast();
            else if (Hunter.IsReady)
                Hunter.Cast();
            else if (Corrupting.IsReady)
                Corrupting.Cast();
        }

        private bool CanUse(SpellSlot sum)
        {
            return (sum != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(sum) == SpellState.Ready);
        }
    }
}