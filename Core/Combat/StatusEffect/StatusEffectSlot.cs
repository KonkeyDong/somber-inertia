using SomberInertia.Enums;

namespace SomberInertia.Core.Combat.StatusEffect;

/// <summary>
/// Active status on a unit. Mutable: tick duration via copy-modify-write on the list entry.
/// Not readonly because <see cref="Duration"/> changes each turn for sleep, etc.
/// </summary>
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