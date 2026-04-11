using System.Numerics;

namespace ClickCast.Util;

public static class JobColours
{
    public static Vector4 GetJobColour(string job) => job switch
    {
        "VPR" => GetVector(16, 130, 16),
        "DRG" or "LNC" => GetVector(81, 116, 221),
        "BLM" or "THM" => GetVector(165, 121, 214),
        "MNK" or "PGL" => GetVector(214, 156, 0),
        "SAM" => GetVector(228, 109, 4),
        "NIN" or "ROG" => GetVector(174, 25, 100),
        "RPR" => GetVector(150, 90, 144),
        "PCT" => GetVector(252, 146, 225),
        "RDM" => GetVector(232, 123, 123),
        "BRD" or "ARC"  => GetVector(145, 186, 94),
        "SMN" or "ACN"  => GetVector(45, 155, 120),
        "DNC" => GetVector(226, 176, 175),
        "MCH" => GetVector(110, 225, 214),
        "AST" => GetVector(225, 231, 74),
        "DRK" => GetVector(209, 38, 204),
        "GNB" => GetVector(154, 141, 80),
        "PLD" or "GLA" => GetVector(168, 210, 230),
        "SGE" => GetVector(128, 160, 240),
        "SCH" => GetVector(134, 87, 255),
        "WAR" or "MRD" => GetVector(207, 38, 33),
        "WHM" or "CNJ" => GetVector(255, 240, 220),
        _ => GetVector(255, 180, 200),
    };

    private static Vector4 GetVector(int r, int g, int b) => new Vector4(r / 255f, g / 255f, b / 255f, 1f);
}
