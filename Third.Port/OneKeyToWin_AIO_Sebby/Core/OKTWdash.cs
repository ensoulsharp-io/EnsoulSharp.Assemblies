using System.Collections.Generic;
using System.Linq;

namespace OneKeyToWin_AIO_Sebby.Core
{
    using EnsoulSharp;
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;
    using SebbyLib;
    using SharpDX;

    class OKTWdash : Program
    {
        private readonly MenuList dashMode = new MenuList("dashMode", "Dash MODE", new[] { "Game Cursor", "Side", "Safe position" }, 2);
        private readonly MenuSlider enemyCheck = new MenuSlider("enemyCheck", "Block dash in x enemies", 3, 0, 5);
        private readonly MenuBool wallCheck = new MenuBool("wallCheck", "Block dash in wall");
        private readonly MenuBool turretCheck = new MenuBool("turretCheck", "Block dash under turret");
        private readonly MenuBool aaCheck = new MenuBool("aaCheck", "Dash only in AA range");

        private readonly MenuList gapcloserMode = new MenuList("gapcloserMode", "Gapcloser MODE", new[] { "Game Cursor", "Away - safe position", "Disable" }, 1);
        private readonly List<MenuBool> egcChampions = new List<MenuBool>();

        private Spell dashSpell;

        public OKTWdash(Spell qwer)
        {
            dashSpell = qwer;

            var local = Config[Player.CharacterName] as Menu;
            var config = local[qwer.Slot.ToString().ToLower() + "Config"] as Menu;

            config.Add(dashMode);
            config.Add(enemyCheck);
            config.Add(wallCheck);
            config.Add(turretCheck);
            config.Add(aaCheck);

            var gapcloser = new Menu("gapcloser", "Gapcloser") { gapcloserMode };

            foreach (var enemy in GameObjects.EnemyHeroes)
            {
                var egcChampion = new MenuBool("egcChampion" + enemy.CharacterName, enemy.CharacterName);
                egcChampions.Add(egcChampion);
                gapcloser.Add(egcChampion);
            }

            config.Add(gapcloser);

            AntiGapcloser.OnGapcloser += AntiGapcloser_OnGapcloser;
        }

        private void AntiGapcloser_OnGapcloser(AIHeroClient sender, AntiGapcloser.GapcloserArgs args)
        {
            if (!OktwCommon.CheckGapcloser(sender,args))
            {
                return;
            }

            if (dashSpell.IsReady() && egcChampions.Any(e => e.Enabled && e.Name == "egcChampion" + sender.CharacterName))
            {
                switch (gapcloserMode.Index)
                {
                    case 0:
                        {
                            var bestPoint = Player.Position.Extend(Game.CursorPos, dashSpell.Range);

                            if (IsGoodPosition(bestPoint))
                            {
                                dashSpell.Cast(bestPoint);
                            }

                            break;
                        }
                    case 1:
                        {
                            var points = OktwCommon.CirclePoints(10, dashSpell.Range, Player.Position);
                            var bestPoint = Player.Position.Extend(sender.Position, -dashSpell.Range);
                            var enemies = bestPoint.CountEnemyHeroesInRange(dashSpell.Range);

                            foreach (var point in points)
                            {
                                var count = point.CountEnemyHeroesInRange(dashSpell.Range);

                                if (count < enemies)
                                {
                                    bestPoint = point;
                                    enemies = count;
                                }
                                else if (count == enemies && Game.CursorPos.Distance(point) < Game.CursorPos.Distance(bestPoint))
                                {
                                    bestPoint = point;
                                    enemies = count;
                                }
                            }

                            if (IsGoodPosition(bestPoint))
                            {
                                dashSpell.Cast(bestPoint);
                            }

                            break;
                        }
                }
            }
        }

        public Vector3 CastDash(bool asap = false)
        {
            var bestPoint = Vector3.Zero;

            switch (dashMode.Index)
            {
                case 0:
                    {
                        bestPoint = Player.Position.Extend(Game.CursorPos, dashSpell.Range);
                        break;
                    }
                case 1:
                    {
                        var orbT = Orbwalker.GetTarget();

                        if (orbT != null && orbT.Type == GameObjectType.AIHeroClient)
                        {
                            var start = Player.Position.ToVector2();
                            var end = orbT.Position.ToVector2();
                            var dir = (end - start).Normalized().Perpendicular();
                            var dis = Player.Distance(orbT);

                            var rightEndPos = end + dir * dis;
                            var leftEndPos = end - dir * dis;

                            var rEndPos = new Vector3(rightEndPos.X, rightEndPos.Y, Player.Position.Z);
                            var lEndPos = new Vector3(leftEndPos.X, leftEndPos.Y, Player.Position.Z);

                            if (Game.CursorPos.Distance(rEndPos) < Game.CursorPos.Distance(lEndPos))
                            {
                                bestPoint = Player.Position.Extend(rEndPos, dashSpell.Range);
                            }
                            else
                            {
                                bestPoint = Player.Position.Extend(lEndPos, dashSpell.Range);
                            }
                        }

                        break;
                    }
                case 2:
                    {
                        bestPoint = Player.Position.Extend(Game.CursorPos, dashSpell.Range);

                        var points = OktwCommon.CirclePoints(15, dashSpell.Range, Player.Position);
                        var enemies = bestPoint.CountEnemyHeroesInRange(350);

                        foreach (var point in points)
                        {
                            var count = point.CountEnemyHeroesInRange(350);
                            if (!InAARange(point))
                            {
                                continue;
                            }
                            if (point.IsUnderAllyTurret())
                            {
                                bestPoint = point;
                                enemies = count - 1;
                            }
                            else if (count < enemies)
                            {
                                bestPoint = point;
                                enemies = count;
                            }
                            else if (count == enemies && Game.CursorPos.Distance(point) < Game.CursorPos.Distance(bestPoint))
                            {
                                bestPoint = point;
                                enemies = count;
                            }
                        }

                        break;
                    }
            }

            if (bestPoint.IsZero)
            {
                return Vector3.Zero;
            }

            var isGoodPos = IsGoodPosition(bestPoint);

            if (asap && isGoodPos)
            {
                return bestPoint;
            }
            else if (isGoodPos && InAARange(bestPoint))
            {
                return bestPoint;
            }

            return Vector3.Zero;
        }

        public bool InAARange(Vector3 point)
        {
            if (!aaCheck.Enabled)
            {
                return true;
            }

            var target = Orbwalker.GetTarget();

            if (target != null && target.Type == GameObjectType.AIHeroClient)
            {
                return point.Distance(target.Position) < Player.AttackRange;
            }
            else
            {
                return point.CountEnemyHeroesInRange(Player.AttackRange) > 0;
            }
        }

        public bool IsGoodPosition(Vector3 dashPos)
        {
            if (wallCheck.Enabled)
            {
                var segment = dashSpell.Range / 5;
                for (var i = 1; i <= 5; i++)
                {
                    if (Player.Position.Extend(dashPos, i * segment).IsWall())
                    {
                        return false;
                    }
                }
            }

            if (turretCheck.Enabled)
            {
                if (dashPos.IsUnderEnemyTurret())
                {
                    return false;
                }
            }

            var enemyCountDashPos = dashPos.CountEnemyHeroesInRange(600);

            if (enemyCheck.Value > enemyCountDashPos)
            {
                return true;
            }

            var enemyCountPlayer = Player.CountEnemyHeroesInRange(400);

            if (enemyCountDashPos <= enemyCountPlayer)
            {
                return true;
            }

            return false;
        }
    }
}