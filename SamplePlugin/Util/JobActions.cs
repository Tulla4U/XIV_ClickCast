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
    ];
}
