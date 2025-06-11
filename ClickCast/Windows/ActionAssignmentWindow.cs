using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ClickCast.Util;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace ClickCast.Windows;

public class ActionAssignmentWindow : Window, IDisposable
{
    private Configuration Configuration;
    private int _selectedJobIndex = 0;
    string[] _jobOptions = ["WHM", "SGE", "AST", "SCH", "WAR"];

    public ActionAssignmentWindow(Plugin plugin) : base("Action Assignment Window###AAS")
    {
        Flags = ImGuiWindowFlags.NoCollapse;

        Size = new Vector2(300, 500);
        SizeCondition = ImGuiCond.FirstUseEver;
        Configuration = plugin.Configuration;
    }


    public void Dispose()
    {
        
    }

    public override void Draw()
    {
        if (ImGui.Combo("Job", ref _selectedJobIndex, _jobOptions.ToArray(), _jobOptions.Length)) { }

        foreach (var mouseButton in Enum.GetValues<MouseButton>())
        {
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

    private void DrawActionSelector(MouseButton mouseButton, KeyModifier keyModifier)
    {
        var currentAssignment =
            JobActionAssignments.FirstOrDefault(x => x.KeyModifiers.Contains(
                                                         keyModifier)
                                                     &&
                                                     x.MouseButton == mouseButton);
        var selectedActionIndex = currentAssignment != null
                                      ? SelectedJobActions.FindIndex(x => x.actionId ==
                                                                          currentAssignment.ActionId)
                                      : -1;
        if (ImGui.Combo(
                $"{mouseButton.ToString()}{(keyModifier != KeyModifier.None ? " - " + keyModifier : "")}",
                ref selectedActionIndex,
                SelectedJobActions.Select(x => x.actionName).ToArray(),
                SelectedJobActions.Count))
        {
            if (currentAssignment != null)
            {
                RemoveAssignment(currentAssignment);
                currentAssignment = currentAssignment with
                {
                    ActionId = SelectedJobActions[selectedActionIndex].actionId
                };
            }
            else
            {
                currentAssignment = new ActionAssignment(SelectedJobActions[selectedActionIndex].actionId,
                                                         mouseButton, [keyModifier]);
            }

            AddAssignment(currentAssignment);
            Configuration.Save();
        }
    }

    private List<(uint actionId, string actionName)> SelectedJobActions =>
        JobActions.GetActionsForJob(_jobOptions[_selectedJobIndex]);

    private List<ActionAssignment> JobActionAssignments =>
        Configuration.GetActionsForJob(_jobOptions[_selectedJobIndex]);

    private void AddAssignment(ActionAssignment assignment)
    {
        JobActionAssignments.Add(assignment);
    }

    private void RemoveAssignment(ActionAssignment assignment)
    {
        JobActionAssignments.Remove(assignment);
    }
}
