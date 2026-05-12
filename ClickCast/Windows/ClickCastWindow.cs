using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ClickCast.Util;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Party;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace ClickCast.Windows;

public class ClickCastWindow : Window, IDisposable
{
    private readonly Configuration configuration;
    public event Action? OnActionAssigmentWindowToggle;

    public ClickCastWindow(Plugin plugin) : base("CC###CC", ImGuiWindowFlags.NoCollapse |
                                                            ImGuiWindowFlags.NoScrollWithMouse)
    {
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
    private MouseButton _lastMouseButton = MouseButton.None;

    private uint? DetermineAction()
    {
        var pressedMouseButton = MouseUtil.GetPressedButton();
        if (pressedMouseButton == MouseButton.None)
        {
            _lastMouseButton = MouseButton.None;
            return null;
        }

        if (_lastMouseButton == pressedMouseButton)
        {
            return null;
        }
        _lastMouseButton = pressedMouseButton;

        var jobName = Plugin.ObjectTable?.LocalPlayer?.ClassJob.Value.Abbreviation;
        var actionId = configuration.GetActionsForJob(jobName.ToString() ?? "DRG")
                                    .Where(x => x.MouseButton == pressedMouseButton);


        return actionId.FirstOrDefault(x => x.KeyModifiers.Contains(GetActiveModifier()))?.ActionId;
    }

    private List<ActionAssignment> GetActionsForModifier()
    {
        var jobName = Plugin.ObjectTable?.LocalPlayer?.ClassJob.Value.Abbreviation;
        return configuration.GetActionsForJob(jobName.ToString() ?? "DRG")
                     .Where(x => x.KeyModifiers.Contains(GetActiveModifier()))
                     .ToList();
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

        if (Plugin.ObjectTable.LocalPlayer?.TargetObject is not IPlayerCharacter partyMember)
        {
            return;
        }

        RenderPlayer(partyMember);
    }

    private void DrawDebugUi()
    {
        var localPlayer = Plugin.ObjectTable.LocalPlayer;
        if (localPlayer is null)
        {
            return;
        }
        // ImGui.TextUnformatted($"Hovered Action {Plugin.GameGui.HoveredAction.ActionId}");
        // ImGui.TextUnformatted($"Selected Action {selectedActionId}");
        RenderPlayer(localPlayer);

        AddTarget();

        if (ImGui.Button("Toggle Assignment Window"))
        {
            OnActionAssigmentWindowToggle?.Invoke();
        }
    }

    private void RenderPlayer(IPlayerCharacter playerCharacter)
    {
        if (playerCharacter?.GameObjectId is null)
        {
            return;
        }
        var dispellableEffect = playerCharacter.StatusList.Any(x => x.GameData.Value.CanDispel);
        RenderPlayer(playerCharacter.CurrentHp, playerCharacter.MaxHp, playerCharacter.Name.ToString(),
                     playerCharacter.ClassJob.Value.Abbreviation.ExtractText(), playerCharacter.GameObjectId, dispellableEffect);
    }

    private void RenderPlayer(IPartyMember partyMember)
    {
        if (partyMember.GameObject is null)
        {
            return;
        }

        var dispellableEffect = partyMember.Statuses.Any(x => x.GameData.Value.CanDispel);
        RenderPlayer(partyMember.CurrentHP, partyMember.MaxHP, partyMember.Name.ToString(), partyMember.ClassJob.Value.Abbreviation.ExtractText(), partyMember.GameObject.GameObjectId, dispellableEffect);
    }

    private void RenderPlayer(uint currentHp, uint maxHp, string name, string jobName, ulong objectId, bool hasDispellableEffect)
    {
        var barWidth = ImGui.GetWindowWidth() - 20;
        ImGui.BeginGroup();
        var hpPercentage = configuration.ClickCastSettings.TrackHpOnBar ? (float)currentHp / maxHp : 1f;
        ImGui.PushStyleColor(ImGuiCol.PlotHistogram, JobColours.GetJobColour(jobName));
        ImGui.ProgressBar(hpPercentage, new Vector2(barWidth, configuration.ClickCastSettings.BarHeight), "");
        ImGui.PopStyleColor();

        if (hasDispellableEffect)
        {
            var position = ImGui.GetCursorPos();
            position.Y -= configuration.ClickCastSettings.BarHeight + 4;
            ImGui.SetCursorPos(position);

            ImGui.PushStyleColor(ImGuiCol.PlotHistogram, new Vector4(255, 255, 255, 1));
            ImGui.ProgressBar(hpPercentage, new Vector2(barWidth, 6), "");
        }
       
        
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
                if (configuration.ClickCastSettings.ShowActionInfo)
                {
                    DrawActionInfo();
                }
            }
        }
    }

    private void DrawActionInfo()
    {
        var actions = GetActionsForModifier()
            .OrderBy(x => x.MouseButton);
        ImGui.BeginTooltip();
        foreach (var action in actions)
        {
            ImGui.TextUnformatted($"{action.MouseButton}: {action.ActionName}");
        }
        ImGui.EndTooltip();
    }

    private void DrawPartyList(IList<IPartyMember> partyMembers)
    {
        foreach (var partyMember in partyMembers)
        {
            RenderPlayer(partyMember);
        }

        if (partyMembers.FirstOrDefault(x => x.GameObject?.GameObjectId ==
                                             Plugin.ObjectTable.LocalPlayer?.TargetObjectId) == null)
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
