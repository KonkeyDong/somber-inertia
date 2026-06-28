using SomberInertia.Core.Units;

namespace SomberInertia.Core.Combat.StatusEffect;

public abstract class StatusEffect
{
    public int Duration { get; protected set; }

    public virtual void Process(Unit unit) { }
}