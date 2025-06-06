using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using SamplePlugin.Util;

namespace SamplePlugin.Windows;

public class ActionAssignmentWindow : Window, IDisposable
{
    private List<ActionAssignment> actionAssignments;
    private Plugin plugin;
    public ActionAssignmentWindow(Plugin plugin, List<ActionAssignment> actionAssignments) : base("Action Assignment Window###")
    {
        Flags = ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(232, 500);
        SizeCondition = ImGuiCond.Always;
        this.plugin = plugin;
        this.actionAssignments = actionAssignments;
    }


    public void Dispose()
    {
        // TODO release managed resources here
    }

    public override void Draw()
    {
        foreach (ActionAssignment actionAssignment in actionAssignments)
        {
            ImGui.TextUnformatted($"{JobActions.WhiteMageActions.First(x => x.actionId == actionAssignment.ActionId).actionName} {actionAssignment.MouseButton.ToString()} {string.Join(',', actionAssignment.KeyModifiers.Select(x => x.ToString()))}");
        }
    }
}
