using SomberInertia.Enums;

namespace SomberInertia.Core.Combat.StatusEffect;

public struct StatusEffectSlot
{
    public StatusEffectType Type;
    public int Duration; // int.MaxValue = permanent until cured

    public static StatusEffectSlot Empty => new StatusEffectSlot
    {
        Type = StatusEffectType.None,
        Duration = 0
    };

    public bool IsActive => Type != StatusEffectType.None;
}