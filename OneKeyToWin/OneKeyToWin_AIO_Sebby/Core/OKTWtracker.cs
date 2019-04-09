using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Core.Utils;
using SharpDX;

namespace OneKeyToWin_AIO_Sebby.Core
{
    class ChampionInfo
    {
        public uint NetworkId { get; set; }

        public Vector3 LastVisiblePos { get; set; }
        public float LastVisibleTime { get; set; }
        public Vector3 PredictedPos { get; set; }

        public float StartRecallTime { get; set; }
        public float AbortRecallTime { get; set; }
        public float FinishRecallTime { get; set; }

        public ChampionInfo()
        {
            LastVisibleTime = Game.Time;
            StartRecallTime = 0;
            AbortRecallTime = 0;
            FinishRecallTime = 0;
        }
    }

    class OKTWtracker
    {
        public static List<ChampionInfo> ChampionInfoList = new List<ChampionInfo>();
        public static AIHeroClient jungler;
        private Vector3 EnemySpawn = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.IsEnemy).Position;

        public void LoadOKTW()
        {
            foreach (var hero in GameObjects.EnemyHeroes)
            {
                ChampionInfoList.Add(new ChampionInfo() { NetworkId = hero.NetworkId, LastVisiblePos = hero.Position });
                if (IsJungler(hero))
                    jungler = hero;
            }

            Game.OnUpdate += OnUpdate;
            Events.OnTeleport += OnTeleport;
        }

        private static void OnTeleport(object sender, TeleportEventArgs args)
        {
            var unit = args.Object as AIHeroClient;

            if (unit == null || !unit.IsValid || unit.IsAlly)
                return;

            var ChampionInfoOne = ChampionInfoList.Find(x => x.NetworkId == unit.NetworkId);

            if (args.Type == TeleportType.Recall)
            {
                switch (args.Status)
                {
                    case TeleportStatus.Start:
                        ChampionInfoOne.StartRecallTime = Game.Time;
                        break;
                    case TeleportStatus.Abort:
                        ChampionInfoOne.AbortRecallTime = Game.Time;
                        break;
                    case TeleportStatus.Finish:
                        ChampionInfoOne.FinishRecallTime = Game.Time;
                        ChampionInfoOne.LastVisiblePos = GameObjects.EnemySpawnPoints.FirstOrDefault().Position;
                        break;
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (!Program.LagFree(0))
                return;

            foreach (var enemy in GameObjects.EnemyHeroes.Where(e => e.IsValid))
            {
                var ChampionInfoOne = ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                if (enemy.IsDead)
                {
                    if (ChampionInfoOne != null)
                    {
                        ChampionInfoOne.LastVisiblePos = EnemySpawn;
                        ChampionInfoOne.LastVisibleTime = Game.Time;
                        ChampionInfoOne.PredictedPos = EnemySpawn;
                    }
                }
                else if (enemy.IsVisible)
                {
                    var prepos = enemy.Position;

                    if (enemy.IsMoving)
                        prepos = prepos.Extend(enemy.GetWaypoints().Last().ToVector3(), 125);

                    if (ChampionInfoOne == null)
                        ChampionInfoList.Add(new ChampionInfo() { NetworkId = enemy.NetworkId, LastVisiblePos = enemy.Position, LastVisibleTime = Game.Time, PredictedPos = prepos });
                    else
                    {
                        ChampionInfoOne.LastVisiblePos = enemy.Position;
                        ChampionInfoOne.LastVisibleTime = Game.Time;
                        ChampionInfoOne.PredictedPos = prepos;
                    }
                }
            }
        }

        private bool IsJungler(AIHeroClient hero) { return hero.Spellbook.Spells.Any(spell => spell.Name.ToLower().Contains("smite")); }
    }
}