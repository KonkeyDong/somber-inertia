namespace SomberInertia.Core.Combat;

public interface IMagicEffect
{
    void Execute(MagicContext context, Magic magic);
}

public class DamageEffect : IMagicEffect
{
    private readonly int _damage;

    public DamageEffect(int damage)
    {
        _damage = damage;
    }

    public void Execute(MagicContext context, Magic magic)
    {
        foreach (var target in context.Targets)
        {
            if (context.Caster.Friendly != target.Friendly) // only damage enemies
            {
                CombatSystem.MagicAttack(context.Caster, target, _damage, magic.MagicType);
            }
        }
    }
}

public class HealEffect : IMagicEffect
{
    private readonly int _healAmount;

    public HealEffect(int healAmount)
    {
        _healAmount = healAmount;
    }

    public void Execute(MagicContext context, Magic magic)
    {
        foreach (var target in context.Targets)
        {
            if (context.Caster.Friendly != target.Friendly) // only damage enemies
            {
                Logger.Warning("HealEffect: change CombatSystem.MagicAttack to Heal.");
                CombatSystem.MagicAttack(context.Caster, target, _healAmount, magic.MagicType);
            }
        }
    }
}