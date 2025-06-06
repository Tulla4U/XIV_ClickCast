using System;
using ImGuiNET;

namespace SamplePlugin.Util;

public static class MouseUtil
{
    public static bool IsPressed(MouseButton button)
    {
        var index = button switch
        {
            MouseButton.Left => 0,
            MouseButton.Right => 1,
            MouseButton.Middle => 2,
            MouseButton.Button4 => 3,
            MouseButton.Button5 => 4,
            _ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
        };
        return ImGui.GetIO().MouseDown[index];
    }
}

public enum MouseButton
{
    Left,
    Middle,
    Right,
    Button4,
    Button5
}
