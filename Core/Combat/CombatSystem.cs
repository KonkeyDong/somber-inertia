using SomberInertia.Core.Units;
using SomberInertia.Enums;

using Raylib_cs;

namespace SomberInertia.Core.Combat;

public static class CombatSystem
{

    public static void Attack(Unit attacker, Unit defender)
    {
        Logger.Warning("DEPRECATED: use AttackV2 and pass an AttackContext object.");
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

    public static void CalculateAttackOutcome(AttackContext context)
    {
        if (Miss(16))
        {
            Logger.Info("   Attack missed!");

            context.Hit = false;
            context.Crit = false;
            context.Damage = 0;
            return;
        }

        context.Hit = true;

        Logger.Warning("Critical hit not implemented.");
        context.Crit = false;

        Logger.Info("   Attack hits!");
        var baseDamage = (context.Attacker.Attack + context.Attacker.Weapon.Attack) - context.Defender.Defense;
        if (baseDamage <= 0)
        {
            Logger.Info($"{context.Attacker.Name}'s attack [{context.Attacker.Attack}] is less than or equal to {context.Defender.Name}'s defense [{context.Defender.Defense}]. Minimum damage is 1");
            // defender.TakeDamage(1);

            context.Damage = 1;
            
            return;
        }

        var variantDamage = CalculateVariance(baseDamage);

        // defender.TakeDamage(Math.Max(variantDamage, 1));

        context.Damage = Math.Max(variantDamage, 1);
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
    private static bool Miss(int denominator) => Chance(denominator);

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