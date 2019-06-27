namespace EnsoulSharp.Kalista
{
    using EnsoulSharp.SDK;

    internal class Program
    {
        private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += delegate 
            {
                if (ObjectManager.Player.CharacterName != "Kalista")
                    return;

                Kalista.OnLoad();
            };
        }
    }
}
