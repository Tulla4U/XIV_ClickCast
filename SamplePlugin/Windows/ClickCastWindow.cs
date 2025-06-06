using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Party;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using SamplePlugin.Util;

namespace SamplePlugin.Windows;

public class ClickCastWindow : Window, IDisposable
{
    private ActionAssignmentWindow _actionAssignmentWindow;
    private Plugin Plugin;

    public ClickCastWindow(Plugin plugin) : base("Click Casting Window###")
    {
        Plugin = plugin;
        Flags = ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(232, 500);
        SizeCondition = ImGuiCond.Always;
        _actionAssignmentWindow = new(Plugin, actionAssignments);
        Plugin.WindowSystem.AddWindow(_actionAssignmentWindow);
    }

    public void Dispose()
    {
        _actionAssignmentWindow.Dispose();
    }

    private uint lastActionId = 0;
    private uint selectedActionId = 0;


    private List<ActionAssignment> actionAssignments =
    [
        new(135, MouseButton.Left, []),
        new(131, MouseButton.Left, [KeyModifier.Shift]),
        new(137, MouseButton.Left, [KeyModifier.Control]),
        new(16531, MouseButton.Right, []),
        new(140, MouseButton.Right, [KeyModifier.Shift]),
        new(3570, MouseButton.Middle, []),
        new(7568, MouseButton.Middle, [KeyModifier.Shift]),
        new(25861, MouseButton.Button4, []),
        new(7432, MouseButton.Button5, []),
        new(120, MouseButton.Button5, [KeyModifier.Shift]),
    ];

    private uint? DetermineAction() // TODO: fix 
    {
        var pressedMouseButton = MouseUtil.GetPressedButton();
        if (pressedMouseButton == MouseButton.None)
        {
            return null;
        }

        var actionId = actionAssignments.Where(x => x.MouseButton == pressedMouseButton);
        if (ImGui.GetIO().KeyShift)
        {
            actionId = actionId.Where(x => x.KeyModifiers.Contains(KeyModifier.Shift));
        }

        if (ImGui.GetIO().KeyCtrl)
        {
            actionId = actionId.Where(x => x.KeyModifiers.Contains(KeyModifier.Control));
        }

        if (ImGui.GetIO().KeyAlt)
        {
            actionId = actionId.Where(x => x.KeyModifiers.Contains(KeyModifier.Alt));
        }

        return actionId.FirstOrDefault()?.ActionId;
    }

    private void DrawDebugUi()
    {
        const float barWidth = 222f;
        const float barHeight = 50f;
        var localPlayer = Plugin.ClientState.LocalPlayer;

        ImGui.BeginGroup();
        // ImGui.Selectable($"{localPlayer.Name} {localPlayer.CurrentHp} / {localPlayer.MaxHp}", false, ImGuiSelectableFlags.None);
        var hpPercentage = (float)localPlayer.CurrentHp / localPlayer.MaxHp;
        ImGui.ProgressBar(hpPercentage, new(barWidth, barHeight), "");
        var barText = $"{localPlayer.Name}\n{localPlayer.CurrentHp}/{localPlayer.MaxHp}";
        var textSize = ImGui.CalcTextSize(barText);
        var position = ImGui.GetCursorPos();
        position.X += (barWidth - textSize.X) / 2;
        position.Y -= barHeight;

        ImGui.SetCursorPos(position);
        ImGui.TextUnformatted(barText);

        // Stupid workaround for placing multiline text on progressbar
        // ImGui.SetCursorPosY(ImGui.GetCursorPosY() - ImGui.GetFontSize() * 2); // Adjust position for two lines
        // ImGui.Text("Loading...\nPlease wait...");                               // Use \n for a new line


        ImGui.EndGroup();

        var hover = ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled);

        unsafe
        {
            if (hover)
            {
                var actionId = DetermineAction();
                if (actionId.HasValue)
                {
                    ActionManager.Instance()->UseAction(ActionType.Action, (uint)actionId,
                                                        localPlayer.GameObjectId);
                }
            }
        }
    }

    private void DrawPartyList(IList<IPartyMember> partyMembers)
    {
        const float barWidth = 222f;
        const float barHeight = 50f;
        foreach (var partyMember in partyMembers)
        {
            ImGui.BeginGroup();
            // ImGui.Selectable($"{localPlayer.Name} {localPlayer.CurrentHp} / {localPlayer.MaxHp}", false, ImGuiSelectableFlags.None);
            var hpPercentage = (float)partyMember.CurrentHP / partyMember.MaxHP;
            ImGui.ProgressBar(hpPercentage, new(barWidth, barHeight), "");
            var barText = $"{partyMember.Name}\n{partyMember.CurrentHP}/{partyMember.MaxHP}";
            var textSize = ImGui.CalcTextSize(barText);
            var position = ImGui.GetCursorPos();
            position.X += (barWidth - textSize.X) / 2;
            position.Y -= barHeight;

            ImGui.SetCursorPos(position);
            ImGui.TextUnformatted(barText);

            // Stupid workaround for placing multiline text on progressbar
            // ImGui.SetCursorPosY(ImGui.GetCursorPosY() - ImGui.GetFontSize() * 2); // Adjust position for two lines
            // ImGui.Text("Loading...\nPlease wait...");                               // Use \n for a new line


            ImGui.EndGroup();

            var hover = ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled);

            unsafe
            {
                if (hover)
                {
                    var actionId = DetermineAction();
                    if (actionId.HasValue)
                    {
                        ActionManager.Instance()->UseAction(ActionType.Action, (uint)actionId,
                                                            partyMember.ObjectId);
                    }
                }
            }
        }
    }

    public override void Draw()
    {
        var localPlayer = Plugin.ClientState.LocalPlayer;
        if (localPlayer.CastActionId != 0)
        {
            lastActionId = localPlayer.CastActionId;
        }

        if (ImGui.Button("Toggle Assignment Window"))
        {
            _actionAssignmentWindow.Toggle();
        }

        ImGui.TextUnformatted($"Last Action {lastActionId}");
        ImGui.TextUnformatted($"Hovered Action {Plugin.GameGui.HoveredAction.ActionID}");
        ImGui.TextUnformatted($"Selected Action {selectedActionId}");

        var party = Plugin.PartyList.ToList();
        if (party.Count == 0)
        {
            DrawDebugUi();
        }
        else
        {
            DrawPartyList(party);
        }


        // ImGui.GetWindowDrawList().AddRectFilled(pos, pos + new Vector2(barWidth, barHeight), 1);
    }
}
