using SamplePlugin.Util;

namespace SamplePlugin;

public record ActionAssignment(uint ActionId, MouseButton MouseButton, KeyModifier[]  KeyModifiers);

public enum KeyModifier
{
    Shift = 1,
    Control = 2,
    Alt = 3,
}
