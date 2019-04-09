using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Core.Utils;
using SharpDX;

namespace Evade.Benchmarking
{
    public static class Benchmark
    {
        private static Vector2 startPoint;
        private static Vector2 endPoint;

        public static void Initialize()
        {
            Game.OnWndProc += Game_OnWndProc;
        }

        static void SpawnLineSkillShot(Vector2 start, Vector2 end)
        {
            SkillshotDetector.TriggerOnDetectSkillshot(
                DetectionType.ProcessSpell,
                SpellDatabase.GetByName("TestLineSkillShot"),
                Utils.TickCount,
                start,
                end,
                ObjectManager.Player);

            DelayAction.Add(5000, () => SpawnLineSkillShot(start, end));
        }

        static void SpawnCircleSkillShot(Vector2 start, Vector2 end)
        {
            SkillshotDetector.TriggerOnDetectSkillshot(
                DetectionType.ProcessSpell,
                SpellDatabase.GetByName("TestCircleSkillShot"),
                Utils.TickCount,
                start,
                end,
                ObjectManager.Player);

            DelayAction.Add(5000, () => SpawnCircleSkillShot(start, end));
        }

        static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (uint)WindowsMessages.LBUTTONDOWN)
            {
                startPoint = Game.CursorPosRaw.ToVector2();
            }

            if (args.Msg == (uint)WindowsMessages.LBUTTONUP)
            {
                endPoint = Game.CursorPosRaw.ToVector2();
            }

            if (args.Msg == (uint)WindowsMessages.KEYUP && args.WParam == 'L') // line missile skillshot
            {
                SpawnLineSkillShot(startPoint, endPoint);   
            }

            if (args.Msg == (uint)WindowsMessages.KEYUP && args.WParam == 'I') // circular skillshot
            {
                SpawnCircleSkillShot(startPoint, endPoint);
            }
        }
    }
}