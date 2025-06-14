using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ClickCast.Util;
using Dalamud.Game.ClientState.Party;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;

namespace ClickCast.Windows;

public class ClickCastWindow : Window, IDisposable
{
    private readonly Configuration configuration;
    public event Action? OnActionAssigmentWindowToggle;

    public ClickCastWindow(Plugin plugin) : base("CC###CC")
    {
        Flags = ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoScrollWithMouse;
        Size = new Vector2(232, 500);
        SizeCondition = ImGuiCond.FirstUseEver;
        configuration = plugin.Configuration;
        
        if (configuration.ClickCastSettings.TrasparentBackground)
        {
            Flags |= ImGuiWindowFlags.NoBackground;
        }
    }

    public void Dispose() { }

    private uint selectedActionId = 0;

    private uint? DetermineAction()
    {
        var pressedMouseButton = MouseUtil.GetPressedButton();
        if (pressedMouseButton == MouseButton.None)
        {
            return null;
        }

        var jobName = Plugin.ClientState.LocalPlayer.ClassJob.Value.Abbreviation;
        var actionId = configuration.GetActionsForJob(jobName.ExtractText())
                                    .Where(x => x.MouseButton == pressedMouseButton);


        return actionId.FirstOrDefault(x => x.KeyModifiers.Contains(GetActiveModifier()))?.ActionId;
    }

    private KeyModifier GetActiveModifier()
    {
        if (ImGui.GetIO().KeyShift)
        {
            return KeyModifier.Shift;
        }

        if (ImGui.GetIO().KeyCtrl)
        {
            return KeyModifier.Control;
        }

        if (ImGui.GetIO().KeyAlt)
        {
            return KeyModifier.Alt;
        }

        return KeyModifier.None;
    }

    private void DrawDebugUi()
    {
        var localPlayer = Plugin.ClientState.LocalPlayer;
        // ImGui.TextUnformatted($"Hovered Action {Plugin.GameGui.HoveredAction.ActionID}");
        // ImGui.TextUnformatted($"Selected Action {selectedActionId}");

        var barWidth = ImGui.GetWindowWidth() - 20;

        ImGui.BeginGroup();
        // ImGui.Selectable($"{localPlayer.Name} {localPlayer.CurrentHp} / {localPlayer.MaxHp}", false, ImGuiSelectableFlags.None);
        var hpPercentage = configuration.ClickCastSettings.TrackHpOnBar ? (float)localPlayer.CurrentHp / localPlayer.MaxHp : 1f;
        ImGui.PushStyleColor(ImGuiCol.PlotHistogram, JobColours.GetJobColour(localPlayer.ClassJob.Value.Abbreviation.ExtractText()));
        ImGui.ProgressBar(hpPercentage, new(barWidth, configuration.ClickCastSettings.BarHeight), "");
        ImGui.PopStyleColor();

        if (configuration.ClickCastSettings.ShowTextOnBars)
        {
            var barText = $"{localPlayer.Name}\n{localPlayer.CurrentHp}/{localPlayer.MaxHp}";
            var textSize = ImGui.CalcTextSize(barText);
            var position = ImGui.GetCursorPos();
            position.X += (barWidth - textSize.X) / 2;
            position.Y -= configuration.ClickCastSettings.BarHeight;

            ImGui.SetCursorPos(position);
            ImGui.TextUnformatted(barText);
        }

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
        
        
        if (ImGui.Button("Toggle Assignment Window"))
        {
            OnActionAssigmentWindowToggle?.Invoke();
        }
    }

    private void DrawPartyList(IList<IPartyMember> partyMembers)
    {
        var barWidth = ImGui.GetWindowWidth() - 20;
        foreach (var partyMember in partyMembers)
        {
            ImGui.BeginGroup();
            // ImGui.Selectable($"{localPlayer.Name} {localPlayer.CurrentHp} / {localPlayer.MaxHp}", false, ImGuiSelectableFlags.None);
            var hpPercentage = configuration.ClickCastSettings.TrackHpOnBar ? (float)partyMember.CurrentHP / partyMember.MaxHP : 1f;
            ImGui.PushStyleColor(ImGuiCol.PlotHistogram, JobColours.GetJobColour(partyMember.ClassJob.Value.Abbreviation.ExtractText()));
            ImGui.ProgressBar(hpPercentage, new(barWidth, configuration.ClickCastSettings.BarHeight), "");
            ImGui.PopStyleColor();
            if (configuration.ClickCastSettings.ShowTextOnBars)
            {
                var barText = $"{partyMember.Name}\n{partyMember.CurrentHP}/{partyMember.MaxHP}";
                var textSize = ImGui.CalcTextSize(barText);
                var position = ImGui.GetCursorPos();
                position.X += (barWidth - textSize.X) / 2;
                position.Y -= configuration.ClickCastSettings.BarHeight;

                ImGui.SetCursorPos(position);
                ImGui.TextUnformatted(barText);
            }

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
        var party = Plugin.PartyList.ToList();
        if (party.Count == 0)
        {
            DrawDebugUi();
        }
        else
        {
            DrawPartyList(party);
        }
    }
    
    public override void PreDraw()
    {
        // Flags must be added or removed before Draw() is being called, or they won't apply
        if (!configuration.ClickCastSettings.TrasparentBackground)
        {
            Flags &= ~ImGuiWindowFlags.NoBackground;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoBackground;
        }
    }
}
