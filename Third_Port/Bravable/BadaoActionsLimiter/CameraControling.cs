using System;
using EnsoulSharp;

namespace BadaoActionsLimiter
{
    public static class CameraControling
    {
        private static readonly Random _random = new Random(DateTime.Now.Millisecond);
        public static void BadaoActivate()
        {

        }
        public static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
        //    if (!Program.Config["CameraControl"].GetValue<MenuBool>().Enabled)
        //        return;
        //    if (args.StartPosition.Distance(ObjectManager.Player.Position) <= 20)
        //        return;

        //    Vector3 start = args.StartPosition;

        //    if (ObjectManager.Player.CharacterName == "Yasuo" && args.Slot == SpellSlot.W)
        //        start = ObjectManager.Player.Position.Extend(args.StartPosition, 250);

        //    if (!start.IsOnScreen())
        //    {
        //        var pos = NavMesh.Ca.Position;
        //        Vector3 NewCameraPos = new Vector3(start.X + _random.Next(400), start.Y + _random.Next(400), Camera.Position.Z);
        //        Camera.Position = NewCameraPos;
        //        Utility.DelayAction.Add(300, () => Camera.Position = pos);
        //    }
        }
    }
}