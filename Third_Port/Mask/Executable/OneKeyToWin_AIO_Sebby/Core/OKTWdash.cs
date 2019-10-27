using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;

using SebbyLib;

using SharpDX;

namespace OneKeyToWin_AIO_Sebby.Core
{
    class OKTWdash
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static Menu Config = Program.Config;
        private static Spell DashSpell;

        private Menu menu;

        public OKTWdash(Spell qwer)
        {
            DashSpell = qwer;

            menu = Config[Player.CharacterName][qwer.Slot + "Config"] as Menu;

            menu.Add(new MenuList("DashMode", "Dash MODE", new[] { "Game Cursor", "Side", "Safe position" }, 2, Player.CharacterName));
            menu.Add(new MenuSlider("EnemyCheck", "Block dash in x enemies", 3, 0, 5, Player.CharacterName));
            menu.Add(new MenuBool("WallCheck", "Block dash in wall", true, Player.CharacterName));
            menu.Add(new MenuBool("TurretCheck", "Block dash under turret", true, Player.CharacterName));
            menu.Add(new MenuBool("AAcheck", "Dash only in AA range", true, Player.CharacterName));

            var gap = new Menu("gap", "Gapcloser");

            gap.Add(new MenuList("GapcloserMode", "Gapcloser MODE", new[] { "Game Cursor", "Away - safe position", "Disable" }, 1, Player.CharacterName));
            foreach (var enemy in GameObjects.EnemyHeroes)
                gap.Add(new MenuBool("EGCchampion" + enemy.CharacterName, enemy.CharacterName, true, Player.CharacterName));

            menu.Add(gap);

            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
        }

        private void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {
            if (DashSpell.IsReady() && menu["gap"].GetValue<MenuBool>("EGCchampion" + sender.CharacterName).Enabled)
            {
                var gapcloserMode = menu["gap"].GetValue<MenuList>("GapcloserMode").Index;
                if (gapcloserMode == 0)
                {
                    var bestpoint = Player.Position.Extend(Game.CursorPos, DashSpell.Range);
                    if (IsGoodPosition(bestpoint))
                        DashSpell.Cast(bestpoint);
                }
                else if (gapcloserMode == 1)
                {
                    var points = OktwCommon.CirclePoints(10, DashSpell.Range, Player.Position);
                    var bestpoint = Player.Position.Extend(sender.Position, -DashSpell.Range);
                    var enemies = bestpoint.CountEnemyHeroesInRange(DashSpell.Range);
                    foreach (var point in points)
                    {
                        var count = point.CountEnemyHeroesInRange(DashSpell.Range);
                        if (count < enemies)
                        {
                            bestpoint = point;
                            enemies = count;
                        }
                        else if (count == enemies && point.DistanceToCursor() < bestpoint.DistanceToCursor())
                        {
                            bestpoint = point;
                            enemies = count;
                        }
                    }
                    if (IsGoodPosition(bestpoint))
                        DashSpell.Cast(bestpoint);
                }
            }
        }

        public Vector3 CastDash(bool asap = false)
        {
            var DashMode = menu.GetValue<MenuList>("DashMode").Index;
            var bestpoint = Vector3.Zero;

            if (DashMode == 0)
            {
                bestpoint = Player.Position.Extend(Game.CursorPos, DashSpell.Range);
            }
            else if (DashMode == 1)
            {
                var orbT = Orbwalker.GetTarget();
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

                    if (rEndPos.DistanceToCursor() < lEndPos.DistanceToCursor())
                    {
                        bestpoint = Player.Position.Extend(rEndPos, DashSpell.Range);
                    }
                    else
                    {
                        bestpoint = Player.Position.Extend(lEndPos, DashSpell.Range);
                    }
                }
            }
            else if (DashMode == 2)
            {
                var points = OktwCommon.CirclePoints(15, DashSpell.Range, Player.Position);
                bestpoint = Player.Position.Extend(Game.CursorPos, DashSpell.Range);
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
                        bestpoint = point;
                        enemies = count;
                    }
                    else if (count == enemies && point.DistanceToCursor() < bestpoint.DistanceToCursor())
                    {
                        bestpoint = point;
                        enemies = count;
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
            if (!menu.GetValue<MenuBool>("AAcheck").Enabled)
                return true;

            var target = Orbwalker.GetTarget();

            if (target != null && target.Type == GameObjectType.AIHeroClient)
            {
                return point.Distance(target) < Player.AttackRange;
            }
            else
            {
                return point.CountEnemyHeroesInRange(Player.AttackRange) > 0;
            }
        }

        public bool IsGoodPosition(Vector3 dashPos)
        {
            if (menu.GetValue<MenuBool>("WallCheck").Enabled)
            {
                var segment = DashSpell.Range / 5;
                for (var i = 1; i <= 5; i++)
                    if (Player.Position.Extend(dashPos, i * segment).IsWall())
                        return false;
            }

            if (menu.GetValue<MenuBool>("TurretCheck").Enabled)
            {
                if (dashPos.IsUnderEnemyTurret())
                    return false;
            }

            var enemyCheck = menu.GetValue<MenuSlider>("EnemyCheck").Value;
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
