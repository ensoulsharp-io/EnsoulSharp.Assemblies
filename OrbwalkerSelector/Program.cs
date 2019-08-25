namespace OrbwalkerSelector
{
    using System.Drawing;

    using EnsoulSharp.Common;
    using EnsoulSharp.SDK;

    class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += (arg) =>
            {
                var menu =
                    new Menu("Orbwalker Selector", "ORBTS", true)
                        .SetFontStyle(FontStyle.Bold, SharpDX.Color.DeepSkyBlue);
                var item =
                    new MenuItem("orbts", "Selector")
                        .SetValue(new StringList(new[] { "SDK", "Common" }))
                        .SetFontStyle(FontStyle.Bold, SharpDX.Color.Orange);

                item.ValueChanged += (sender, e) =>
                {
                    var idx = e.GetNewValue<StringList>().SelectedIndex;
                    if (idx == 0)
                    {
                        Orbwalking.Attack = false;
                        Orbwalking.Move = false;
                        Orbwalker.AttackState = true;
                        Orbwalker.MovementState = true;
                    }
                    else if (idx == 1)
                    {
                        Orbwalker.AttackState = false;
                        Orbwalker.MovementState = false;
                        Orbwalking.Attack = true;
                        Orbwalking.Move = true;
                    }
                };

                menu.AddItem(item);

                var i = item.GetValue<StringList>().SelectedIndex;
                if (i == 0)
                {
                    Orbwalking.Attack = false;
                    Orbwalking.Move = false;
                    Orbwalker.AttackState = true;
                    Orbwalker.MovementState = true;
                }
                else if (i == 1)
                {
                    Orbwalker.AttackState = false;
                    Orbwalker.MovementState = false;
                    Orbwalking.Attack = true;
                    Orbwalking.Move = true;
                }
            };
        }
    }
}