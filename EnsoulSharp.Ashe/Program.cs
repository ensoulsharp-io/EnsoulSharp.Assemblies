namespace EnsoulSharp.Ashe
{
    using EnsoulSharp.SDK;

    internal class Program
    {
        private static void Main(string[] args)
        {
            Events.OnLoad += (sender, e) =>
            {
                if (Player.Instance.CharacterName != "Ashe")
                    return;

                Ashe.OnLoad();
            };
        }
    }
}
