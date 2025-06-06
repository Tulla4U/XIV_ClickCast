using System;
using SamplePlugin.Util;

namespace SamplePlugin;

[Serializable]
public record ActionAssignment(uint ActionId, MouseButton MouseButton, KeyModifier[]  KeyModifiers);

[Serializable]
public enum KeyModifier
{
    Shift = 1,
    Control = 2,
    Alt = 3,
}
