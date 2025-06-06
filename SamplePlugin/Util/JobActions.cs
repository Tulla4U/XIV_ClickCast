using System;
using System.Collections.Generic;

namespace SamplePlugin.Util;

public static class JobActions
{
    public static List<(uint actionId, string actionName)> WhiteMageActions { get; } =
    [
        (120, "Cure I"),
        (135, "Cure II"),
        (131, "Cure III"),
        (137, "Regen"),
        (25862, "Liturgy of the Bell"),
        (3570, "Tetragrammaton"),
        (16531, "Afflatus Solace"),
        (7432, "Divine Benison"),
        (25861, "Aquaveil"),
        (7568, "Esuna"),
        (140, "Benediction"),
        (7571, "Rescue"),
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
            (7568, "Esuna"),
            (7571, "Rescue"),
            (24296, "Druochole "),
            (24317, "Krasis"),
        ];

    public static List<(uint actionId, string actionName)> GetActionsForJob(string jobName) => jobName switch
    {
        "SGE" => SageActions,
        "WHM" => WhiteMageActions,
        _ => throw new NotImplementedException()
    };
}
