namespace EnsoulSharp.Xerath
{
    using EnsoulSharp.SDK;

    public class Program
    {   
        // thanks NightMoon open source for this Xerath
        // all code was basic on his Xerath, i just only update it and make it work on EnsoulSharp
        // source is from L# version, but seems it work fine for me

        private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad()
        {
            if (ObjectManager.Player.CharacterName != "Xerath")
            {
                return;
            }

            Xerath.OnLoad();
        }
    }
}
