using SomberInertia.Core.Units;
using SomberInertia.Enums;

namespace SomberInertia.Core.Combat.StatusEffect;

public static class StatusEffectSystem
{
    private static readonly Random _random = new Random();

    public static StatusEffectSlot Create(StatusEffectType type)
    {
        switch (type)
        {
            case StatusEffectType.Poison:
                return new StatusEffectSlot
                {
                    Type = StatusEffectType.Poison,
                    Duration = int.MaxValue
                };

            case StatusEffectType.Sleep:
                return new StatusEffectSlot
                {
                    Type = StatusEffectType.Sleep,
                    Duration = _random.Next(GameConstants.StatusEffects.SLEEP_DURATION)
                };

            default:
                return StatusEffectSlot.Empty;
        }
    }

    public static void ProcessPoison(Unit unit)
    {
        if (!unit.HasStatus(StatusEffectType.Poison))
        {
            return;
        }

        var damage = (int)(unit.HP.Max / GameConstants.StatusEffects.POISON_DAMAGE_DENOMINATOR);
        var finalDamage = Math.Max(2, damage);

        Logger.Info($"Poison damage dealt to unit [{unit.GetDisplayName()}] is [{finalDamage}].");
        unit.TakeDamage(finalDamage);
    }

    public static void ProcessSleep(Unit unit)
    {
        var index = unit.FindStatusIndex(StatusEffectType.Sleep);
        if (index < 0)
        {
            return;
        }

        var slot = unit.StatusEffects[index];
        slot.Duration--;
        unit.StatusEffects[index] = slot;

        if (slot.Duration < 0)
        {
            Logger.Info($"Sleep status on unit [{unit.GetDisplayName()}] has exhausted; removing.");
            unit.RemoveStatus(StatusEffectType.Sleep);
        }
    }
}