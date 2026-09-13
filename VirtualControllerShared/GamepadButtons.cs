using System;
using System.Collections.Generic;
using System.Text;
using VirtualControllerNative.Interop;

namespace VirtualControllerShared
{
    public static class GamepadButtons
    {
        public static IReadOnlyList<GamepadButton> Buttons = Enum.GetValues<GamepadButton>().OfType<GamepadButton>().Where(x => x != GamepadButton.None).ToList();

        public static GamepadButton GetGamepadButton(int buttonIdx)
        {
            return (GamepadButton)(1 << buttonIdx);
        }

        public static IReadOnlyDictionary<string, GamepadButton> ButtonSVGMappings = new Dictionary<string, GamepadButton>()
        {
            { "buttonA", GamepadButton.A },
            { "buttonB", GamepadButton.B },
            { "buttonX", GamepadButton.X },
            { "buttonY", GamepadButton.Y },
            { "buttonLeft", GamepadButton.DPadLeft },
            { "buttonRight", GamepadButton.DPadRight },
            { "buttonDown", GamepadButton.DPadDown },
            { "buttonUp", GamepadButton.DPadUp },
            { "buttonLS", GamepadButton.LeftThumb },
            { "buttonRS", GamepadButton.RightThumb },
            { "buttonLB", GamepadButton.LeftShoulder },
            { "buttonRB", GamepadButton.RightShoulder },
            { "buttonLT", GamepadButton.LeftTrigger },
            { "buttonRT", GamepadButton.RightTrigger },
        };

        public static IReadOnlyDictionary<GamepadButton, string> ButtonLabels = new Dictionary<GamepadButton, string>()
        {
            { GamepadButton.A, "A" },
            { GamepadButton.B, "B" },
            { GamepadButton.X, "X" },
            { GamepadButton.Y, "Y" },
            { GamepadButton.DPadLeft, "←" },
            { GamepadButton.DPadRight, "→" },
            { GamepadButton.DPadDown, "↓" },
            { GamepadButton.DPadUp, "↑" },
            { GamepadButton.LeftThumb, "LS" },
            { GamepadButton.RightThumb, "RS" },
            { GamepadButton.LeftShoulder, "LB" },
            { GamepadButton.RightShoulder, "RB" },
            { GamepadButton.LeftTrigger, "LT" },
            { GamepadButton.RightTrigger, "RT" },
            { GamepadButton.Start, "START" },
            { GamepadButton.Back, "BACK" },
        };
    }
}
