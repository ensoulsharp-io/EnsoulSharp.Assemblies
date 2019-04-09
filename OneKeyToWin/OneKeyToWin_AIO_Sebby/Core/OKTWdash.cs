using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Core.UI.IMenu;
using EnsoulSharp.SDK.Core.UI.IMenu.Values;
using SebbyLib;
using SharpDX;

namespace OneKeyToWin_AIO_Sebby.Core
{
    class OKTWdash
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static Menu Config = Program.Config;
        private Spell DashSpell;

        public OKTWdash(Spell qwer)
        {
            DashSpell = qwer;

            (Config[Player.CharacterName][qwer.Slot + "config"] as Menu).Add(new MenuList<string>("DashMode", "Dash MODE", new[] { "Game Cursor", "Side", "Safe position" }, Player.CharacterName) { Index = 2 });
            (Config[Player.CharacterName][qwer.Slot + "config"] as Menu).Add(new MenuSlider("EnemyCheck", "Block dash if x enemies", 3, 0, 5, Player.CharacterName));
            (Config[Player.CharacterName][qwer.Slot + "config"] as Menu).Add(new MenuBool("WallCheck", "Block dash in wall", true, Player.CharacterName));
            (Config[Player.CharacterName][qwer.Slot + "config"] as Menu).Add(new MenuBool("TurretCheck", "Block dash under turret", true, Player.CharacterName));
            (Config[Player.CharacterName][qwer.Slot + "config"] as Menu).Add(new MenuBool("AAcheck", "Dash only in AA range", true, Player.CharacterName));

            var gapcloser = new Menu("gapcloser", "Gapcloser");
            gapcloser.Add(new MenuList<string>("GapcloserMode", "Gapcloser MODE", new[] { "Game Cursor", "Away - safe position", "Disable" }, Player.CharacterName) { Index = 1 });
            foreach (var enemy in GameObjects.EnemyHeroes)
                gapcloser.Add(new MenuBool("EGCchampion" + enemy.CharacterName, enemy.CharacterName, true, Player.CharacterName));
            (Config[Player.CharacterName][qwer.Slot + "config"] as Menu).Add(gapcloser);

            Events.OnGapCloser += OnGapCloser;
        }

        private void OnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            var g = Config[Player.CharacterName][DashSpell.Slot + "config"]["gapcloser"] as Menu;
            var e = g["EGCchampion" + args.Sender.CharacterName] as MenuBool;
            if (DashSpell.IsReady() && e != null && e.Value)
            {
                var gapcloserMode = g.GetValue<MenuList<string>>("GapcloserMode").Index;
                if (gapcloserMode == 0)
                {
                    var bestpoint = Player.Position.Extend(Game.CursorPosRaw, DashSpell.Range);
                    if (IsGoodPosition(bestpoint))
                        DashSpell.Cast(bestpoint);
                }
                else if(gapcloserMode == 1)
                {
                    var points = OktwCommon.CirclePoints(10, DashSpell.Range, Player.Position);
                    var bestpoint = Player.Position.Extend(args.Sender.FearLeashPoint, -DashSpell.Range);
                    var enemies = bestpoint.CountEnemyHeroesInRange(DashSpell.Range);
                    foreach (var point in points)
                    {
                        var count = point.CountEnemyHeroesInRange(DashSpell.Range);
                        if (count < enemies)
                        {
                            enemies = count;
                            bestpoint = point;
                        }
                        else if(count == enemies && Game.CursorPosRaw.Distance(point) < Game.CursorPosRaw.Distance(bestpoint))
                        {
                            enemies = count;
                            bestpoint = point;
                        }
                    }
                    if (IsGoodPosition(bestpoint))
                        DashSpell.Cast(bestpoint);
                }
            }
        }

        public Vector3 CastDash(bool asap = false)
        {
            var dashMode = Config[Player.CharacterName][DashSpell.Slot + "config"].GetValue<MenuList<string>>("DashMode").Index;
            var bestpoint = Vector3.Zero;
            if (dashMode == 0)
            {
                bestpoint = Player.Position.Extend(Game.CursorPosRaw, DashSpell.Range);
            }
            else if (dashMode == 1)
            {
                var orbT = Variables.Orbwalker.GetTarget();
                if (orbT != null && orbT is AIHeroClient)
                {
                    var start = Player.Position.ToVector2();
                    var end = orbT.Position.ToVector2();
                    var dir = (end - start).Normalized();
                    var pDir = dir.Perpendicular();

                    var rightEndPos = end + pDir * Player.Distance(orbT);
                    var leftEndPos = end - pDir * Player.Distance(orbT);

                    var rEndPos = new Vector3(rightEndPos.X, rightEndPos.Y, Player.Position.Z);
                    var lEndPos = new Vector3(leftEndPos.X, leftEndPos.Y, Player.Position.Z);

                    if (Game.CursorPosRaw.Distance(rEndPos) < Game.CursorPosRaw.Distance(lEndPos))
                    {
                        bestpoint = Player.Position.Extend(rEndPos, DashSpell.Range);
                    }
                    else
                    {
                        bestpoint = Player.Position.Extend(lEndPos, DashSpell.Range);
                    }
                }
            }
            else if (dashMode == 2)
            {
                var points = OktwCommon.CirclePoints(15, DashSpell.Range, Player.Position);
                bestpoint = Player.Position.Extend(Game.CursorPosRaw, DashSpell.Range);
                var enemies = bestpoint.CountEnemyHeroesInRange(350);
                foreach (var point in points)
                {
                    var count = point.CountEnemyHeroesInRange(350);
                    if (!InAARange(point))
                        continue;
                    if (point.IsUnderAllyTurret())
                    {
                        bestpoint = point;
                        enemies = count - 1;
                    }
                    else if (count < enemies)
                    {
                        enemies = count;
                        bestpoint = point;
                    }
                    else if (count == enemies && Game.CursorPosRaw.Distance(point) < Game.CursorPosRaw.Distance(bestpoint))
                    {
                        enemies = count;
                        bestpoint = point;
                    }
                }
            }

            if (bestpoint.IsZero)
                return Vector3.Zero;

            var isGoodPos = IsGoodPosition(bestpoint);

            if (asap && isGoodPos)
            {
                return bestpoint;
            }
            else if (isGoodPos && InAARange(bestpoint))
            {
                return bestpoint;
            }

            return Vector3.Zero;
        }

        public bool InAARange(Vector3 point)
        {
            if (!Config[Player.CharacterName][DashSpell.Slot + "config"].GetValue<MenuBool>("AAcheck").Value)
                return true;
            else
            {
                var t = Variables.Orbwalker.GetTarget();
                if (t != null && t.Type == GameObjectType.AIHeroClient)
                {
                    return point.Distance(t.Position) < Player.AttackRange;
                }
                else
                {
                    return point.CountEnemyHeroesInRange(Player.AttackRange) > 0;
                }
            }
        }

        public bool IsGoodPosition(Vector3 dashPos)
        {
            if (Config[Player.CharacterName][DashSpell.Slot + "config"].GetValue<MenuBool>("WallCheck").Value)
            {
                var segment = DashSpell.Range / 5;
                for (var i = 1; i <= 5; i++)
                {
                    if (Player.Position.Extend(dashPos, i * segment).IsWall())
                        return false;
                }
            }

            if (Config[Player.CharacterName][DashSpell.Slot + "config"].GetValue<MenuBool>("TurretCheck").Value)
            {
                if (dashPos.IsUnderEnemyTurret())
                    return false;
            }

            var enemyCheck = Config[Player.CharacterName][DashSpell.Slot + "config"].GetValue<MenuSlider>("EnemyCheck").Value;
            var enemyCountDashPos = dashPos.CountEnemyHeroesInRange(600);

            if (enemyCheck > enemyCountDashPos)
                return true;

            var enemyCountPlayer = Player.CountEnemyHeroesInRange(400);

            if (enemyCountDashPos <= enemyCountPlayer)
                return true;

            return false;
        }
    }
}