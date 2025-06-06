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
        // Do not use .Text() or any other formatted function like TextWrapped(), or SetTooltip().
        // These expect formatting parameter if any part of the text contains a "%", which we can't
        // provide through our bindings, leading to a Crash to Desktop.
        // Replacements can be found in the ImGuiHelpers Class
        // ImGui.TextUnformatted($"The random config bool is {Plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");
        //
        // if (ImGui.Button("Show Settings"))
        // {
        //     Plugin.ToggleConfigUI();
        // }

        ImGui.Spacing();

        // Normally a BeginChild() would have to be followed by an unconditional EndChild(),
        // ImRaii takes care of this after the scope ends.
        // This works for all ImGui functions that require specific handling, examples are BeginTable() or Indent().
        using var child = ImRaii.Child("SomeChildWithAScrollbar", Vector2.Zero, true);
        // Check if this child is drawing
        if (child.Success)
        {
            // ImGui.TextUnformatted("Have a goat:");
            // var goatImage = Plugin.TextureProvider.GetFromFile(GoatImagePath).GetWrapOrDefault();
            // if (goatImage != null)
            // {
            //     using (ImRaii.PushIndent(55f))
            //     {
            //         ImGui.Image(goatImage.ImGuiHandle, new Vector2(goatImage.Width, goatImage.Height));
            //     }
            // }
            // else
            // {
            //     ImGui.TextUnformatted("Image not found.");
            // }
            //
            // ImGuiHelpers.ScaledDummy(20.0f);
            //
            // var localPlayer = Plugin.ClientState.LocalPlayer;
            // if (localPlayer == null)
            // {
            //     ImGui.TextUnformatted("Our local player is currently not loaded.");
            //     return;
            // }
            //
            // if (!localPlayer.ClassJob.IsValid)
            // {
            //     ImGui.TextUnformatted("Our current job is currently not valid.");
            //     return;
            // }


            

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
