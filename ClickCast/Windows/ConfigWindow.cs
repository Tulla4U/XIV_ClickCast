using System;
using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace ClickCast.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base("A Wonderful Configuration Window###With a constant ID")
    {
        Flags = ImGuiWindowFlags.NoCollapse;

        Size = new Vector2(232, 120);
        SizeCondition = ImGuiCond.FirstUseEver;

        Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        // Flags must be added or removed before Draw() is being called, or they won't apply
        if (Configuration.IsConfigWindowMovable)
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
        var showTextOnBars = Configuration.ClickCastSettings.ShowTextOnBars;
        if (ImGui.Checkbox("Show text on bars", ref showTextOnBars))
        {
            Configuration.ClickCastSettings.ShowTextOnBars = showTextOnBars;
            Configuration.Save();
        }        
        var trackHpOnBar = Configuration.ClickCastSettings.TrackHpOnBar;
        if (ImGui.Checkbox("Adjust bar to health percentage", ref trackHpOnBar))
        {
            Configuration.ClickCastSettings.TrackHpOnBar = trackHpOnBar;
            Configuration.Save();
        }        
        var transparentBackground = Configuration.ClickCastSettings.TrasparentBackground;
        if (ImGui.Checkbox("Transparent cast window", ref transparentBackground))
        {
            Configuration.ClickCastSettings.TrasparentBackground = transparentBackground;
            Configuration.Save();
        }

        var barHeight = Configuration.ClickCastSettings.BarHeight;
        if (ImGui.SliderFloat("Bar Height", ref barHeight, 5, 100))
        {
            Configuration.ClickCastSettings.BarHeight = barHeight;
            Configuration.Save();
        }

    }
}
