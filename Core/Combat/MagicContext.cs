using SomberInertia.Core.Units;
using System.Numerics;

namespace SomberInertia.Core.Combat;

public class MagicContext
{
    public Unit Caster { get; }
    public List<Unit> Targets { get; } // 1 or multiple
    public Grid Grid { get; }
    public Vector2 TargetWorldPosition { get; } // where the spell was aimed

    public MagicContext(Unit caster, List<Unit> targets, Grid grid, Vector2 targetWorldPosition)
    {
        Caster = caster;
        Targets = targets ?? new List<Unit>();
        Grid = grid;
        TargetWorldPosition = targetWorldPosition;
    }
}