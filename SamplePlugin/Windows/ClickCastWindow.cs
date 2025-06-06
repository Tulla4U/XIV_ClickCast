using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using SamplePlugin.Util;

namespace SamplePlugin.Windows;

public class ClickCastWindow : Window, IDisposable
{
    public ClickCastWindow() : base("Click Casting Window###")
    {
        Flags = ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(232, 500);
        SizeCondition = ImGuiCond.Always;
    }

    public void Dispose()
    {
        DiableSpellListening();
    }

    private uint lastActionId = 0;
    private uint selectedActionId = 0;

    private List<(uint actionId, string actionName)> whiteMageActions =
        [(135, "Cure II"), (131, "Cure III"), (137, "Regen")];

    public override void Draw()
    {
        var localPlayer = Plugin.ClientState.LocalPlayer;
        if (localPlayer.CastActionId != 0)
        {
            lastActionId = localPlayer.CastActionId;
        }

        ImGui.TextUnformatted($"Last Action {lastActionId}");
        ImGui.TextUnformatted($"Hovered Action {Plugin.GameGui.HoveredAction.ActionID}");
        ImGui.TextUnformatted($"Selected Action {selectedActionId}");

        // if (ImGui.Button("Toggle Spell Selector"))
        // {
        //     if (!spellSelectorActive)
        //     {
        //         EnableSpellListening();
        //     }
        //     else
        //     {
        //         DiableSpellListening();
        //     }
        // }

        {
            ImGui.BeginGroup();
            ImGui.Selectable($"test", false, ImGuiSelectableFlags.None);

            
            ImGui.EndGroup();

            var hover = ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled);
            var left = hover && MouseUtil.IsPressed(MouseButton.Left);
            var right = hover && MouseUtil.IsPressed(MouseButton.Right);
            var middle = hover && MouseUtil.IsPressed(MouseButton.Middle);
            var mouse4 = hover && MouseUtil.IsPressed(MouseButton.Button4);
            var mouse5 = hover && MouseUtil.IsPressed(MouseButton.Button5);

            unsafe
            {
                if (left)
                {
                    if (ImGui.GetIO().KeyCtrl) { }
                    else if (ImGui.GetIO().KeyShift) { }
                    else if (ImGui.GetIO().KeyAlt) { }
                    else if (ImGui.GetIO().KeyCtrl && ImGui.GetIO().KeyShift) { }
                    else
                    {
                        ActionManager.Instance()->UseAction(ActionType.Action, 131, localPlayer.GameObjectId);
                    }
                }
                else if (right)
                {
                    ActionManager.Instance()->UseAction(ActionType.Action, 135, localPlayer.GameObjectId);
                }
                else if (middle)
                {
                    ActionManager.Instance()->UseAction(ActionType.Action, 137, localPlayer.GameObjectId);
                }
                else if (mouse4)
                {
                    ActionManager.Instance()->UseAction(ActionType.Action, 137, localPlayer.GameObjectId);
                }
                else if (mouse5)
                {
                    ActionManager.Instance()->UseAction(ActionType.Action, 137, localPlayer.GameObjectId);
                }
            }
        }
        var party = Plugin.PartyList.ToList();
        if (party.Count == 0)
        {
            return;
        }

        const float barWidth = 150f;
        const float barHeight = 100f;
        foreach (var partyMember in party)
        {
            // var pos = new Vector2(50 + (barWidth + 10f), 150);
            ImGui.BeginGroup();
            ImGui.Selectable($"{partyMember.Name} {partyMember.CurrentHP}/{partyMember.MaxHP}", false,
                             ImGuiSelectableFlags.None);


            // unsafe
            // {
            //     //Plugin.TargetManager.Target = localPlayer;
            //     // 135
            //     if (ImGui.GetIO().KeyCtrl)
            //     {
            //         ActionManager.Instance()->UseAction(ActionType.Action, 137, partyMember.ObjectId);
            //     }
            //     else if (ImGui.GetIO().KeyShift)
            //     {
            //         ActionManager.Instance()->UseAction(ActionType.Action, 131, partyMember.ObjectId);
            //     }
            //     else if (ImGui.GetIO().KeyAlt) { }
            //     else if (ImGui.GetIO().KeyCtrl && ImGui.GetIO().KeyShift) { }
            //     else
            //     {
            //         ActionManager.Instance()->UseAction(ActionType.Action, 135, partyMember.ObjectId);
            //     }
            // }
            ImGui.EndGroup();

            var hover = ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled);
            var left = hover && ImGui.IsMouseClicked(ImGuiMouseButton.COUNT);
            var right = hover && ImGui.IsMouseClicked(ImGuiMouseButton.Right);
            unsafe
            {
                if (left)
                {
                    if (ImGui.GetIO().KeyCtrl)
                    {
                        ActionManager.Instance()->UseAction(ActionType.Action, 137, partyMember.ObjectId);
                    }
                    else if (ImGui.GetIO().KeyShift)
                    {
                        ActionManager.Instance()->UseAction(ActionType.Action, 131, partyMember.ObjectId);
                    }
                    else if (ImGui.GetIO().KeyAlt) { }
                    else if (ImGui.GetIO().KeyCtrl && ImGui.GetIO().KeyShift) { }
                    else
                    {
                        ActionManager.Instance()->UseAction(ActionType.Action, 135, partyMember.ObjectId);
                    }
                }
                else if (right) { }
            }

            // ImGui.GetWindowDrawList().AddRectFilled(pos, pos + new Vector2(barWidth, barHeight), 1);
        }
    }

    private bool spellSelectorActive = false;

    private void EnableSpellListening()
    {
        Plugin.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "ActionMenu", SpellClickHandler);
    }

    private void DiableSpellListening()
    {
        Plugin.AddonLifecycle.UnregisterListener(SpellClickHandler);
    }

    private void SpellClickHandler(AddonEvent type, AddonArgs args)
    {
        unsafe
        {
            var addon = (AtkUnitBase*)args.Addon;
            for (uint i = 1; i < 60; i += 1)
            {
                var targetNode = addon->GetNodeById(i);
                if (targetNode == null) continue;

                targetNode->NodeFlags |= NodeFlags.EmitsEvents | NodeFlags.RespondToMouse | NodeFlags.HasCollision;
                Plugin.EventManager.AddEvent((nint)addon, (nint)i, AddonEventType.MouseClick, SelectSpell);
            }
        }
    }

    private void SelectSpell(AddonEventType type, IntPtr addon, IntPtr node)
    {
        unsafe
        {
            var addonId = ((AtkUnitBase*)node)->Id;
            switch (type)
            {
                case AddonEventType.MouseClick:
                    selectedActionId = Plugin.GameGui.HoveredAction.ActionID;
                    break;
            }
        }
    }
}
