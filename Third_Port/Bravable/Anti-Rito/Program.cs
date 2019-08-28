using System;
using EnsoulSharp;
using EnsoulSharp.SDK;

namespace Anti_Rito
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad()
        {
            OneTickOneSpell.Init();
            Chat.Print("Anti-Riot by Live To Rise <3");
        }
    }
}