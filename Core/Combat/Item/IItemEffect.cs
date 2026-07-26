namespace SomberInertia.Core.Combat.Item;

public interface IItemEffect
{
    void Execute(ItemContext context);
}

public class HealEffect : IItemEffect
{
    private readonly int _healAmount;

    public HealEffect(int healAmount)
    {
        _healAmount = healAmount;
    }

    public void Execute(ItemContext context)
    {
        foreach (var target in context.Targets)
        {
            // Only heal friendly units
            if (context.User.Friendly == target.Friendly)
            {
                // TODO: Replace with proper Heal method when available
                Logger.Info($"HealEffect: Healing {target.GetDisplayName()} for {_healAmount} HP.");
                // CombatSystem.Heal(target, _healAmount);
            }
        }
    }
}

public class RemovePoisonEffect : IItemEffect
{
    public void Execute(ItemContext context)
    {
        foreach (var target in context.Targets)
        {
            if (context.User.Friendly == target.Friendly)
            {
                Logger.Info($"RemovePoisonEffect: Removing poison from {target.GetDisplayName()}.");
                // target.RemoveStatus<PoisonEffect>();
            }
        }
    }
}

public class EscapeEffect : IItemEffect
{
    public void Execute(ItemContext context)
    {
        Logger.Info("EscapeEffect: Escaping from battle (not yet implemented).");
        // TODO: Signal that the battle should end
    }
}