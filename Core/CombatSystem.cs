namespace SomberInertia.Core;
using Raylib_cs;

public static class CombatSystem
{
    public static void Attack(Unit attacker, Unit defender)
    {
        Logger.Info($"{attacker.Name} attacks {defender.Name}.");
        if (!CheckHit(attacker, defender))
        {
            Logger.Info("   Attack missed!");

            return;
        }

        Logger.Info("   Attack hits!");
        var baseDamage = (attacker.Attack + attacker.Weapon.Attack) - defender.Defense;
        if (baseDamage <= 0)
        {
            Logger.Info($"{attacker.Name}'s attack [{attacker.Attack}] is less than or equal to {defender.Name}'s defense [{defender.Defense}]. Minimum damage is 1");
            defender.TakeDamage(1);

            return;
        }

        Logger.Info($"  Base attack damage: [{baseDamage}].");
        
        // 75% - 125% damage variance
        var variance = Raylib.GetRandomValue(75, 125);
        var variantDamage = (baseDamage * variance) / 100;

        Logger.Info($"Variant damage: [{variantDamage}].");

        defender.TakeDamage(Math.Max(variantDamage, 1));
    }

    private static bool CheckHit(Unit attacker, Unit defender)
    {
        return !Chance(16); // if not zero, a hit.
    }

    // If chance is zero, then return true.
    // The callee function will determine what to do with that value.
    private static bool Chance(int denominator)
    {
        if (denominator <= 1)
        {
            Logger.Error("CombatSystem::Chance(): denominator must be greater than 1.");
            throw new ArgumentOutOfRangeException(nameof(denominator), denominator, "Denominator must be greater than 1.");
        }

        Logger.Debug($"CombatSystem::Chance({denominator})");

        var result = Raylib.GetRandomValue(0, denominator - 1);
        Logger.Info($"CombatSystem::Chance() roll: [{result}].");

        return result == 0;
    }
}