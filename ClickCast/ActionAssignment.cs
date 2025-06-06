using System;
using ClickCast.Util;

namespace ClickCast;

[Serializable]
public record ActionAssignment(uint ActionId, MouseButton MouseButton, KeyModifier[]  KeyModifiers);

[Serializable]
public enum KeyModifier
{
    None = 0,
    Shift = 1,
    Control = 2,
    Alt = 3
}
