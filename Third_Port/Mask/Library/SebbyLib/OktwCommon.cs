using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Prediction;

using SharpDX;

namespace SebbyLib
{
    public class OktwCommon
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static List<UnitIncomingDamage> IncomingDamageList = new List<UnitIncomingDamage>();
        public static bool blockMove = false, blockAttack = false, blockSpells = false;

        static OktwCommon()
        {
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
            AIBaseClient.OnDoCast += AIBaseClient_OnDoCast;
            Game.OnWndProc += Game_OnWndProc;
            Game.OnUpdate += Game_OnUpdate;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            AIBaseClient.OnIssueOrder += Player_OnIssueOrder;
        }

        public static double GetIncomingDamage(AIHeroClient target, float time = 0.5f, bool skillshots = true)
        {
            double totalDamage = 0;

            foreach (var damage in IncomingDamageList.Where(damage => damage.TargetNetworkId == target.NetworkId && Game.Time - time < damage.Time))
            {
                if (skillshots || !damage.Skillshot)
                {
                    totalDamage += damage.Damage;
                }
            }

            return totalDamage;
        }

        public static bool CanHarass()
        {
            return (!Player.IsWindingUp && !Player.IsUnderEnemyTurret() && Orbwalker.CanMove(50, false));
        }

        public static bool ShouldWait()
        {
            var attackCalc = (int)(Player.AttackDelay * 1000);
            return
                Cache.GetMinions(Player.Position, 0).Any(
                    minion => HealthPrediction.GetPrediction(minion, attackCalc, 30, HealthPrediction.HealthPredictionType.Simulated) <= Player.GetAutoAttackDamage(minion));
        }

        public static float GetEchoLudenDamage(AIHeroClient target)
        {
            float totalDamage = 0;

            if (Player.GetBuffCount("itemmagicshankcharge") == 100)
            {
                totalDamage += (float)Player.CalculateDamage(target, DamageType.Magical, 100 + 0.1 * Player.TotalMagicalDamage);
            }

            return totalDamage;
        }

        public static bool IsSpellHeroCollision(AIHeroClient t, Spell QWER, int extraWidth = 50)
        {
            foreach (var hero in GameObjects.EnemyHeroes.Where(h => h.IsValidTarget(QWER.Range + QWER.Width, true, QWER.RangeCheckFrom) && t.NetworkId != h.NetworkId))
            {
                var prediction = QWER.GetPrediction(hero);
                var powCalc = Math.Pow(QWER.Width + extraWidth + hero.BoundingRadius, 2);
                if (prediction.UnitPosition.ToVector2().DistanceSquared(QWER.From.ToVector2(), prediction.CastPosition.ToVector2(), true) <= powCalc
                    || prediction.UnitPosition.ToVector2().DistanceSquared(QWER.From.ToVector2(), t.Position.ToVector2(), true) <= powCalc)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CanHitSkillShot(AIBaseClient target, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (args.Target == null && target.IsValidTarget(float.MaxValue, false))
            {
                var pred = SpellPrediction.GetPrediction(target, 0.25f).CastPosition;
                if (args.SData.LineWidth > 0)
                {
                    var powCalc = Math.Pow(args.SData.LineWidth + target.BoundingRadius, 2);
                    if (pred.ToVector2().DistanceSquared(args.To.ToVector2(), args.Start.ToVector2(), true) <= powCalc
                        || target.Position.ToVector2().DistanceSquared(args.To.ToVector2(), args.Start.ToVector2(), true) <= powCalc)
                    {
                        return true;
                    }
                }
                else if (target.Distance(args.To) < 50 + target.BoundingRadius
                    || pred.Distance(args.To) < 50 + target.BoundingRadius)
                {
                    return true;
                }
            }
            return false;
        }

        public static float GetKsDamage(AIHeroClient t, Spell QWER)
        {
            var totalDmg = QWER.GetDamage(t);
            totalDmg -= t.HPRegenRate;

            if (totalDmg > t.Health)
            {
                if (t.CharacterName == "Blitzcrank" && !t.HasBuff("manabarriercooldown") && !t.HasBuff("manabarrier"))
                {
                    totalDmg -= t.Mana * 0.3f;
                }
            }

            totalDmg += (float)GetIncomingDamage(t);
            return totalDmg;
        }

        public static bool ValidUlt(AIHeroClient target)
        {
            return !(target.HasBuffOfType(BuffType.PhysicalImmunity) || target.HasBuffOfType(BuffType.SpellImmunity)
                || target.IsInvulnerable || target.HasBuffOfType(BuffType.Invulnerability) || target.HasBuff("KindredRNoDeathBuff")
                || target.HasBuffOfType(BuffType.SpellShield) || target.Health - GetIncomingDamage(target) < 1);
        }

        public static bool CanMove(AIHeroClient target)
        {
            return !(target.MoveSpeed < 50 || target.IsStunned
                || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Snare)
                || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Suppression)
                || target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Knockback) || target.HasBuffOfType(BuffType.Asleep)
                || (target.IsCastingImporantSpell() && !target.IsMoving)
                || target.IsRecalling());
        }

        public static int GetBuffCount(AIBaseClient target, string buffName)
        {
            foreach (var buff in target.Buffs.Where(buff => buff.Name.ToLower() == buffName.ToLower()))
            {
                return ((buff.Count == 0) ? 1 : buff.Count);
            }
            return 0;
        }

        public static int CountEnemyMinions(AIBaseClient target, float range)
        {
            return GameObjects.EnemyMinions.Where(m => m.Team != GameObjectTeam.Neutral && m.IsValidTarget(range, false, target.Position)).Count();
        }

        public static float GetPassiveTime(AIBaseClient target, string buffName)
        {
            return target.Buffs.OrderByDescending(buff => buff.EndTime - Game.Time)
                .Where(buff => buff.Name.ToLower() == buffName.ToLower())
                .Select(buff => buff.EndTime)
                .FirstOrDefault() - Game.Time;
        }

        public static Vector3 GetTrapPos(float range)
        {
            foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsValid && enemy.Distance(Player.Position) < range && (enemy.HasBuff("bardrstasis") || enemy.HasBuff("zhonyasringshield"))))
            {
                return enemy.Position;
            }

            foreach (var obj in ObjectManager.Get<EffectEmitter>().Where(obj => obj.IsValid && obj.Position.Distance(Player.Position) < range))
            {
                var name = obj.Name.ToLower();

                if (name.Contains("Gatemarker_Red".ToLower()) || name.Contains("global_ss_teleport_target_red.troy".ToLower())
                    || name.Contains("R_Tar_Ground_Enemy".ToLower()) || name.Contains("R_indicator_red".ToLower()))
                {
                    return obj.Position;
                }
            }

            return Vector3.Zero;
        }

        public static bool IsMovingInSameDirection(AIBaseClient source, AIBaseClient target)
        {
            var sourceLW = source.GetWaypoints().Last().ToVector3();

            if (sourceLW == source.Position || !source.IsMoving)
            {
                return false;
            }

            var targetLW = target.GetWaypoints().Last().ToVector3();

            if (targetLW == target.Position || !target.IsMoving)
            {
                return false;
            }

            var pos1 = sourceLW.ToVector2() - source.Position.ToVector2();
            var pos2 = targetLW.ToVector2() - target.Position.ToVector2();
            var getAngle = pos1.AngleBetween(pos2);

            return (getAngle < 25);
        }

        public static bool CollisionYasuo(Vector3 from, Vector3 to)
        {
            if (!GameObjects.EnemyHeroes
                        .Any(
                            hero => hero.IsValidTarget(float.MaxValue, false) && hero.CharacterName == "Yasuo"))
            {
                return false;
            }

            foreach (var effectEmitter in GameObjects.ParticleEmitters)
            {
                if (effectEmitter.IsValid &&
                    Regex.IsMatch(effectEmitter.Name, @"Yasuo_.+_w_windwall_enemy_\d", RegexOptions.IgnoreCase))
                {
                    var wall = effectEmitter;
                    var level = wall.Name.Substring(wall.Name.Length - 2, 2);
                    var wallWidth = 350 + 50 * Convert.ToInt32(level);
                    var wallDirection = wall.Position.Perpendicular().ToVector2();
                    var wallStart = wall.Position.ToVector2() + wallWidth / 2 * wallDirection;
                    var wallEnd = wallStart - wallWidth * wallDirection;

                    if (wallStart.Intersection(wallEnd, to.ToVector2(), from.ToVector2()).Intersects)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static void DrawTriangleOKTW(float radius, Vector3 position, System.Drawing.Color color, float bold = 1)
        {
            var positionV2 = Drawing.WorldToScreen(position);
            var a = new Vector2(positionV2.X + radius, positionV2.Y + radius / 2);
            var b = new Vector2(positionV2.X - radius, positionV2.Y + radius / 2);
            var c = new Vector2(positionV2.X, positionV2.Y - radius);
            Drawing.DrawLine(a[0], a[1], b[0], b[1], bold, color);
            Drawing.DrawLine(b[0], b[1], c[0], c[1], bold, color);
            Drawing.DrawLine(c[0], c[1], a[0], a[1], bold, color);
        }

        public static void DrawLineRectangle(Vector3 start2, Vector3 end2, int radius, float width, System.Drawing.Color color)
        {
            var start = start2.ToVector2();
            var end = end2.ToVector2();
            var dir = (end - start).Normalized();
            var pDir = dir.Perpendicular();

            var rightStartPos = start + pDir * radius;
            var leftStartPos = start - pDir * radius;
            var rightEndPos = end + pDir * radius;
            var leftEndPos = end - pDir * radius;

            var rStartPos = Drawing.WorldToScreen(new Vector3(rightStartPos.X, rightStartPos.Y, Player.Position.Z));
            var lStartPos = Drawing.WorldToScreen(new Vector3(leftStartPos.X, leftStartPos.Y, Player.Position.Z));
            var rEndPos = Drawing.WorldToScreen(new Vector3(rightEndPos.X, rightEndPos.Y, Player.Position.Z));
            var lEndPos = Drawing.WorldToScreen(new Vector3(leftEndPos.X, leftEndPos.Y, Player.Position.Z));

            Drawing.DrawLine(rStartPos, rEndPos, width, color);
            Drawing.DrawLine(lStartPos, lEndPos, width, color);
            Drawing.DrawLine(rStartPos, lStartPos, width, color);
            Drawing.DrawLine(lEndPos, rEndPos, width, color);
        }

        public static List<Vector3> CirclePoints(float CircleLineSegmentN, float radius, Vector3 position)
        {
            List<Vector3> points = new List<Vector3>();
            for (var i = 1; i <= CircleLineSegmentN; i++)
            {
                var angle = i * 2 * Math.PI / CircleLineSegmentN;
                var point = new Vector3(position.X + radius * (float)Math.Cos(angle), position.Y + radius * (float)Math.Sin(angle), position.Z);
                points.Add(point);
            }
            return points;
        }

        private static void Game_OnWndProc(GameWndProcEventArgs args)
        {
            if (args.Msg == (uint)WindowsMessages.CONTEXTMENU && blockMove)
            {
                blockMove = false;
                blockAttack = false;
                Orbwalker.AttackState = true;
                Orbwalker.MovementState = true;
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
        }

        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            var hero = sender as AIHeroClient;
            if (args.Target != null && args.SData != null && hero != null)
            {
                if (args.Target.Type == GameObjectType.AIHeroClient && !sender.IsMelee && args.Target.Team != sender.Team)
                {
                    IncomingDamageList.Add(new UnitIncomingDamage { Damage = hero.GetSpellDamage((AIBaseClient)args.Target, args.Slot), TargetNetworkId = args.Target.NetworkId, Time = Game.Time, Skillshot = false });
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            var time = Game.Time - 2;
            IncomingDamageList.RemoveAll(damage => time < damage.Time);
        }

        private static void AIBaseClient_OnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            var hero = sender as AIHeroClient;
            if (args.SData == null || hero == null)
            {
                return;
            }

            var target = args.Target as AIBaseClient;
            if (target != null)
            {
                if (target.Type == GameObjectType.AIHeroClient && sender.IsMelee && target.Team != sender.Team)
                {
                    IncomingDamageList.Add(new UnitIncomingDamage { Damage = hero.GetSpellDamage(target, args.Slot), TargetNetworkId = args.Target.NetworkId, Time = Game.Time, Skillshot = false });
                }
            }
            else
            {
                foreach (var champion in GameObjects.Heroes.Where(c => !c.IsDead && c.IsVisible && c.Team != sender.Team && c.Distance(sender) < 2000))
                {
                    if (CanHitSkillShot(champion, args))
                    {
                        IncomingDamageList.Add(new UnitIncomingDamage { Damage = hero.GetSpellDamage(champion, args.Slot), TargetNetworkId = champion.NetworkId, Time = Game.Time, Skillshot = true });
                    }
                }
            }
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (blockSpells)
            {
                args.Process = false;
            }
        }

        private static void Player_OnIssueOrder(AIBaseClient sender, AIBaseClientIssueOrderEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (blockMove && args.Order != GameObjectOrder.AttackUnit)
            {
                args.Process = false;
            }
            if (blockAttack && args.Order == GameObjectOrder.AttackUnit)
            {
                args.Process = false;
            }
        }
    }

    class UnitIncomingDamage
    {
        public uint TargetNetworkId { get; set; }
        public float Time { get; set; }
        public double Damage { get; set; }
        public bool Skillshot { get; set; }
    }
}