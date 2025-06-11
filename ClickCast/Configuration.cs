using System;
using System.Collections.Generic;
using ClickCast.Util;
using Dalamud.Configuration;

namespace ClickCast;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public ClickCastSettings ClickCastSettings { get; set; } = new()
    {
        BarHeight = 25f,
        BarSpacing = 2f,
        ShowTextOnBars = false,
        TrackHpOnBar = true,
        TrasparentBackground = true
    };

    public bool IsConfigWindowMovable { get; set; } = true;

    public List<ActionAssignment> WhiteMageActionAssignment { get; set; } =
        [];

    public List<ActionAssignment> SageActionAssignment { get; set; } = [];
    public List<ActionAssignment> AstrologianAssignment { get; set; } = [];
    public List<ActionAssignment> ScholarAssignment { get; set; } = [];
    public List<ActionAssignment> WarriorAssignments { get; set; } = [];

    // the below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }

    public List<ActionAssignment> GetActionsForJob(string jobName) => jobName switch
    {
        "SGE" => SageActionAssignment,
        "WHM" => WhiteMageActionAssignment,
        "AST" => AstrologianAssignment,
        "SCH" => ScholarAssignment,
        "WAR"  => WarriorAssignments,
        _ => throw new NotImplementedException()
    };
}

[Serializable]
public class ClickCastSettings
{
    public float BarHeight { get; set; }
    public bool ShowTextOnBars { get; set; }
    public float BarSpacing { get; set; }
    public bool TrackHpOnBar { get; set; }
    public bool TrasparentBackground { get; set; }
}
