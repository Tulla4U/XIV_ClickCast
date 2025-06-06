using System;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using SamplePlugin.Util;

namespace SamplePlugin.Windows;

public class ActionAssignmentWindow : Window, IDisposable
{
    private Configuration Configuration;
    private Plugin Plugin;
    private int _selectedJobIndex = 0;

    public ActionAssignmentWindow(Plugin plugin) : base("Action Assignment Window###AAS")
    {
        Flags = ImGuiWindowFlags.NoCollapse;

        Size = new Vector2(232, 500);
        SizeCondition = ImGuiCond.Always;
        Plugin = plugin;
        Configuration = plugin.Configuration;
    }


    public void Dispose()
    {
        // TODO release managed resources here
    }

    public override void Draw()
    {
        string[] jobOptions = ["WHM", "SGE"];
        if (ImGui.Combo("Job", ref _selectedJobIndex, jobOptions.ToArray(), jobOptions.Length)) { }

        foreach (var mouseButton in Enum.GetValues<MouseButton>())
        {
            DrawActionSelector(mouseButton, null);

            foreach (var keyModifier in Enum.GetValues<KeyModifier>())
            {
                DrawActionSelector(mouseButton, keyModifier);
            }
        }
        
        

        // foreach (var actionAssignment in Configuration.WhiteMageActionAssignment)
        // {
        //     ImGui.TextUnformatted(
        //         $"{JobActions.WhiteMageActions.First(x => x.actionId == actionAssignment.ActionId).actionName} {actionAssignment.MouseButton.ToString()} {string.Join(',', actionAssignment.KeyModifiers.Select(x => x.ToString()))}");
        // }
    }

    private void DrawActionSelector(MouseButton mouseButton, KeyModifier? keyModifier)
    {
        var currentAssignment =
            Configuration.WhiteMageActionAssignment.FirstOrDefault(x => keyModifier != null
                                                                            ? x.KeyModifiers.Contains(
                                                                                (KeyModifier)keyModifier)
                                                                            : x.KeyModifiers.Length == 0 &&
                                                                              x.MouseButton == mouseButton);
        int selectedActionIndex = currentAssignment != null
                                      ? JobActions.WhiteMageActions.FindIndex(x => x.actionId ==
                                                                                  currentAssignment.ActionId)
                                      : -1;
        if (ImGui.Combo($"{mouseButton.ToString()}{(keyModifier != null ? " - " : "")}{keyModifier.ToString()}", ref selectedActionIndex,
                        JobActions.WhiteMageActions.Select(x => x.actionName).ToArray(),
                        JobActions.WhiteMageActions.Count))
        {
            if (currentAssignment != null)
            {
                Configuration.WhiteMageActionAssignment.Remove(currentAssignment);
                currentAssignment = currentAssignment with
                {
                    ActionId = JobActions.WhiteMageActions[selectedActionIndex].actionId
                };
            }
            else
            {
                currentAssignment = new ActionAssignment(JobActions.WhiteMageActions[selectedActionIndex].actionId,
                                                         mouseButton,
                                                         keyModifier != null ? [(KeyModifier)keyModifier] : []);
            }

            Configuration.WhiteMageActionAssignment.Add(currentAssignment);
            Configuration.Save();
        }
    }
}
