using System;
using System.Collections.Generic;
using System.Linq;

namespace ClickCast.Util;

public static class JobActions
{
    public static List<(uint actionId, string actionName)> WhiteMageActions { get; } =
    [
        (120, "Cure I"),
        (135, "Cure II"),
        (131, "Cure III"),
        (137, "Regen"),
        (3570, "Tetragrammaton"),
        (16531, "Afflatus Solace"),
        (7432, "Divine Benison"),
        (25861, "Aquaveil"),
        Esuna,
        (140, "Benediction"),
        Rescue,
        (125, "Raise"),
    ];

    public static List<(uint actionId, string actionName)> SageActions { get; } =
    [
        (24284, "Diagnosis"),
        (24285, "Kardia"),
        (24287, "Egeiro"),
        (24291, "Eukrasian Diagnosis"),
        (24295, "Icarus"),
        (24303, "Taurochole"),
        (24305, "Haima"),
        Esuna,
        Rescue,
        (24296, "Druochole "),
        (24317, "Krasis"),
    ];

    private static (uint actinId, string actinName) Esuna => (7568, "Esuna");
    private static (uint actinId, string actinName) Rescue => (7571, "Rescue");

    private static (uint actinId, string actinName) Shirk => (7537, "Shirk");

    public static List<(uint actionId, string actionName)> AstrologianActions { get; } =
    [
        (3594, "Benefic"),
        (3610, "Benefic II"),
        (3603, "Ascend"),
        (3614, "Essential Dignity"),
        (37019, "Play I"),
        (37020, "Play II"),
        (37021, "Play III"),
        (3595, "Aspected Benefic"),
        (3612, "Synastry"),
        (16556, "Celestial Intersection"),
        (25873, "Exaltation"),
        Esuna,
        Rescue,
    ];

    public static List<(uint actionId, string actionName)> ScholarActions { get; } =
    [
        (190, "Physick"),
        (173, "Resurrection"),
        (185, "Adloquium"),
        (189, "Lustrate"),
        (3585, "Deployment Tactics"),
        (7434, "Excogitation"),
        (7437, "Aetherpact"),
        (25867, "Protraction"),
        (37015, "Manifestation"),
        Esuna,
        Rescue,
    ];

    public static List<(uint actionId, string actionName)> WarriorActions { get; } =
    [
        (16464, "Nascent Flash"),
        Shirk
    ];

    public static List<(uint actionId, string actionName)> GetActionsForJob(string jobName) => jobName switch
    {
        "SGE" => SageActions.OrderBy(x => x.actionName).ToList(),
        "WHM" => WhiteMageActions.OrderBy(x => x.actionName).ToList(),
        "AST" => AstrologianActions.OrderBy(x => x.actionName).ToList(),
        "SCH" => ScholarActions.OrderBy(x => x.actionName).ToList(),
        "WAR" => WarriorActions.OrderBy(x => x.actionName).ToList(),
        _ => throw new NotImplementedException()
    };
}
