using SomberInertia.Core.Units;
using SomberInertia.Enums;

using Raylib_cs;

namespace SomberInertia.Core.Combat;

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

        var variantDamage = CalculateVariance(baseDamage);

        defender.TakeDamage(Math.Max(variantDamage, 1));
    }

    public static void MagicAttack(Unit attacker, Unit defender, int baseDamage, MagicType magicType)
    {
        Logger.Info($"{attacker.Name} performs a {magicType.ToString()} magic attack upon {defender.Name}.");

        // Magic attacks never miss, but they cost MP to cast (unless casted by using an item).
        // Magic attacks can crit and a defender can be weak or strong against a magic attack.

        var variantDamage = CalculateVariance(baseDamage);

        defender.TakeDamage(Math.Max(variantDamage, 1));
    }

    private static bool CheckHit(Unit attacker, Unit defender) => !Chance(16);

    private static int CalculateVariance(int baseAmount)
    {
        Logger.Info($"  Base amount: [{baseAmount}].");
        
        var variance = Raylib.GetRandomValue(75, 125);
        var variantDamage = (baseAmount * variance) / 100;

        Logger.Info($"  Variant amount: [{variantDamage}].");

        return variantDamage;
    }

    private static bool Chance(int denominator)
    {
        if (denominator <= 1)
        {
            Logger.Error("CombatSystem::Chance(): denominator must be greater than 1.");
        }

        var result = Raylib.GetRandomValue(0, denominator - 1);
        Logger.Info($"CombatSystem::Chance() roll: [{result}].");

        return result == 0;
    }
}