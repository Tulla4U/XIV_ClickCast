using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace ClickCast.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Configuration configuration;

    // We give this window a constant ID using ###.
    // This allows for labels to be dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base("A Wonderful Configuration Window###With a constant ID")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(232, 150);
        SizeCondition = ImGuiCond.FirstUseEver;

        configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        // Flags must be added or removed before Draw() is being called, or they won't apply
        if (configuration.IsConfigWindowMovable)
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }
    }

    public override void Draw()
    {
        // can't ref a property, so use a local copy
        var showTextOnBars = configuration.ClickCastSettings.ShowTextOnBars;
        if (ImGui.Checkbox("Show text on bars", ref showTextOnBars))
        {
            configuration.ClickCastSettings.ShowTextOnBars = showTextOnBars;
            configuration.Save();
        }        
        var trackHpOnBar = configuration.ClickCastSettings.TrackHpOnBar;
        if (ImGui.Checkbox("Adjust bar to health percentage", ref trackHpOnBar))
        {
            configuration.ClickCastSettings.TrackHpOnBar = trackHpOnBar;
            configuration.Save();
        }        
        var transparentBackground = configuration.ClickCastSettings.TransparentBackground;
        if (ImGui.Checkbox("Transparent cast window", ref transparentBackground))
        {
            configuration.ClickCastSettings.TransparentBackground = transparentBackground;
            configuration.Save();
        }        
        var includeTarget = configuration.ClickCastSettings.IncludeTarget;
        if (ImGui.Checkbox("Include Target in list", ref includeTarget))
        {
            configuration.ClickCastSettings.IncludeTarget = includeTarget;
            configuration.Save();
        }

        var barHeight = configuration.ClickCastSettings.BarHeight;
        if (ImGui.SliderFloat("Bar Height", ref barHeight, 5, 100))
        {
            configuration.ClickCastSettings.BarHeight = barHeight;
            configuration.Save();
        }
    }
}
