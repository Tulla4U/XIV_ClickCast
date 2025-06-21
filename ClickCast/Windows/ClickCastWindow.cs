using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ClickCast.Util;
using Dalamud.Game.ClientState.Objects.SubKinds;
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

        if (configuration.ClickCastSettings.TransparentBackground)
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

    private void AddTarget()
    {
        if (!configuration.ClickCastSettings.IncludeTarget)
        {
            return;
        }

        if (Plugin.ClientState.LocalPlayer?.TargetObject is not IPlayerCharacter partyMember)
        {
            return;
        }

        RenderPlayer(partyMember.CurrentHp, partyMember.MaxHp, partyMember.Name.ToString(),
                     partyMember.ClassJob.Value.Abbreviation.ExtractText(), partyMember.GameObjectId);
    }

    private void DrawDebugUi()
    {
        var localPlayer = Plugin.ClientState.LocalPlayer;
        // ImGui.TextUnformatted($"Hovered Action {Plugin.GameGui.HoveredAction.ActionID}");
        // ImGui.TextUnformatted($"Selected Action {selectedActionId}");
        RenderPlayer(localPlayer.CurrentHp, localPlayer.MaxHp, localPlayer.Name.ToString(),
                     localPlayer.ClassJob.Value.Abbreviation.ExtractText(), localPlayer.GameObjectId);

        AddTarget();

        if (ImGui.Button("Toggle Assignment Window"))
        {
            OnActionAssigmentWindowToggle?.Invoke();
        }
    }

    private void RenderPlayer(uint currentHp, uint maxHp, string name, string jobName, ulong objectId)
    {
        var barWidth = ImGui.GetWindowWidth() - 20;
        ImGui.BeginGroup();
        var hpPercentage = configuration.ClickCastSettings.TrackHpOnBar ? (float)currentHp / maxHp : 1f;
        ImGui.PushStyleColor(ImGuiCol.PlotHistogram, JobColours.GetJobColour(jobName));
        ImGui.ProgressBar(hpPercentage, new Vector2(barWidth, configuration.ClickCastSettings.BarHeight), "");
        ImGui.PopStyleColor();
        if (configuration.ClickCastSettings.ShowTextOnBars)
        {
            var barText = $"{name}\n{currentHp}/{maxHp}";
            var textSize = ImGui.CalcTextSize(barText);
            var position = ImGui.GetCursorPos();
            position.X += (barWidth - textSize.X) / 2;
            position.Y -= configuration.ClickCastSettings.BarHeight;

            ImGui.SetCursorPos(position);
            ImGui.TextUnformatted(barText);
        }

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
                                                        objectId);
                }
            }
        }
    }

    private void DrawPartyList(IList<IPartyMember> partyMembers)
    {
        foreach (var partyMember in partyMembers)
        {
            RenderPlayer(partyMember.CurrentHP, partyMember.MaxHP, partyMember.Name.ToString(),
                         partyMember.ClassJob.Value.Abbreviation.ExtractText(), partyMember.GameObject.GameObjectId);
        }

        if (partyMembers.FirstOrDefault(x => x.GameObject?.GameObjectId ==
                                             Plugin.ClientState.LocalPlayer?.TargetObjectId) == null)
        {
            AddTarget();
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
        if (!configuration.ClickCastSettings.TransparentBackground)
        {
            Flags &= ~ImGuiWindowFlags.NoBackground;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoBackground;
        }
    }
}
