using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using SamplePlugin.Util;

namespace SamplePlugin;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool IsConfigWindowMovable { get; set; } = true;
    public bool SomePropertyToBeSavedAndWithADefault { get; set; } = true;

    public List<ActionAssignment> WhiteMageActionAssignment { get; set; } =
    [
        new(135, MouseButton.Left, [KeyModifier.None]),
        new(131, MouseButton.Left, [KeyModifier.Shift]),
        new(137, MouseButton.Left, [KeyModifier.Control]),
        new(16531, MouseButton.Right, [KeyModifier.None]),
        new(140, MouseButton.Right, [KeyModifier.Shift]),
        new(3570, MouseButton.Middle, [KeyModifier.None]),
        new(7568, MouseButton.Middle, [KeyModifier.Shift]),
        new(25861, MouseButton.Button4, [KeyModifier.None]),
        new(7432, MouseButton.Button5, [KeyModifier.None]),
        new(120, MouseButton.Button5, [KeyModifier.Shift]),
    ];

    public List<ActionAssignment> SageActionAssignment { get; set; } = [];

    // the below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
    
    public List<ActionAssignment> GetActionsForJob(string jobName) => jobName switch
    {
        "SGE" => SageActionAssignment,
        "WHM" => WhiteMageActionAssignment,
        _ => throw new NotImplementedException()
    };
}
