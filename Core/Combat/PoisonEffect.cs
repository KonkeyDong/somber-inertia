using SomberInertia.Core.Units;

namespace SomberInertia.Core.Combat;

public class PoisonEffect : StatusEffect
{
    public PoisonEffect()
    {
        Duration = int.MaxValue; // virtual infinite value
    }

    // In the original game, Poison dealt a pathetic 
    // HP of damage per turn. This made Poison more of
    // an annoyance than a threat. Dealing 1/8 of your
    // max HP of damage per turn should add more excitement.
    // Minimum damage is 2 HP if life pool is small (mages).
    public override void Process(Unit unit)
    {
        Duration--;

        var damage = (int)(unit.HP.Max / 8);
        var maxDamage = Math.Max(2, damage);

        Logger.Info($"Poison damage dealt to unit [{unit.GetDisplayName()}] is [{maxDamage}].");
        unit.TakeDamage(maxDamage);
    }
}