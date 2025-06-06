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

    public static MouseButton GetPressedButton()
    {
        var mouseIndex = ImGui.GetIO().MouseDown;
        if (mouseIndex[0])
        {
            return MouseButton.Left;
        }

        if (mouseIndex[1])
        {
            return MouseButton.Right;
        }

        if (mouseIndex[2])
        {
            return MouseButton.Middle;
        }

        if (mouseIndex[3])
        {
            return MouseButton.Button4;
        }

        if (mouseIndex[4])
        {
            return MouseButton.Button5;
        }
        return MouseButton.None;
    }
}

public enum MouseButton
{
    Left = 0,
    Right = 1,
    Middle = 2,
    Button4 = 3,
    Button5 = 4,
    None = 99
}
