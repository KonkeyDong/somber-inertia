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

        var variance = Raylib.GetRandomValue(75, 125);
        var variantDamage = (baseDamage * variance) / 100;

        Logger.Info($"Variant damage: [{variantDamage}].");

        defender.TakeDamage(Math.Max(variantDamage, 1));
    }

    private static bool CheckHit(Unit attacker, Unit defender) => !Chance(16);


    private static bool Chance(int denominator)
    {
        if (denominator <= 1)
        {
            Logger.Error("CombatSystem::Chance(): denominator must be greater than 1.");
            throw new ArgumentOutOfRangeException(nameof(denominator));
        }

        var result = Raylib.GetRandomValue(0, denominator - 1);
        Logger.Info($"CombatSystem::Chance() roll: [{result}].");

        return result == 0;
    }
}