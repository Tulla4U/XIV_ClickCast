using System;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace ClickCast.Windows;

public class MainWindow : Window, IDisposable
{

    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(Plugin plugin)
        : base("My Amazing Window##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.Spacing();
        
        using var child = ImRaii.Child("SomeChildWithAScrollbar", Vector2.Zero, true);
        // Check if this child is drawing
        if (child.Success)
        {
            var playersTargetingSomething = Plugin.ObjectTable.Where(x => x is IPlayerCharacter)
                                                  .Where(x => x.TargetObject is IPlayerCharacter)
                                                  .OrderBy(x => x.Name.ToString())
                                                  .ToList();
            var targetCount = playersTargetingSomething.GroupBy(x => x.TargetObject!.Name.ToString());

            if (ImGui.CollapsingHeader("Targeting"))
            {
                foreach (var player in playersTargetingSomething)
                {
                    ImGui.TextUnformatted($"{player.Name} -> {player.TargetObject!.Name}");
                }
            }

            if (ImGui.CollapsingHeader("Targeted count"))
            {
                foreach (var target in targetCount.OrderByDescending(x => x.Count()))
                {
                    ImGui.TextUnformatted($"{target.Key} - {target.Count()}");
                }
            }
        }
    }
}
