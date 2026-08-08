using System.Numerics;
using System.Text;
using SomberInertia.Core.Units;
using SomberInertia.Graphics;

namespace SomberInertia.Core.Combat.Item;

public class ItemContext
{
    public Unit Caster { get; }
    public List<Unit> Targets { get; }
    public Grid Grid { get; }
    public int ItemSlotIndex { get; }

    public BattleUnitSpriteSet CasterSprites { get; private set; } = new();
    public BattleUnitSpriteSet TargetSprites { get; private set; } = new();

    public Unit Target => Targets.Count > 0 ? Targets[0] : Caster;

    public bool IsSelfTarget => Targets.Count == 1 && ReferenceEquals(Targets[0], Caster);

    public Vector2 CasterSpritePosition => GameConstants.Battle.GetSpritePosition(Caster);

    public Vector2 TargetSpritePosition => GameConstants.Battle.GetSpritePosition(Target);

    public ItemContext(Unit caster, List<Unit> targets, Grid grid, int itemSlotIndex)
    {
        Caster = caster;
        Targets = targets ?? new List<Unit>();
        Grid = grid;
        ItemSlotIndex = itemSlotIndex;
    }

    /// <summary>Load force-side battle idle sprites for caster and target (may be the same unit).</summary>
    public void LoadBattleSprites()
    {
        CasterSprites = BattleUnitSpriteManager.Get(Caster);
        CasterSprites.BasePosition = CasterSpritePosition;

        if (IsSelfTarget)
        {
            TargetSprites = CasterSprites;
        }
        else
        {
            TargetSprites = BattleUnitSpriteManager.Get(Target);
            TargetSprites.BasePosition = TargetSpritePosition;
        }

        Logger.Debug(
            $"ItemContext sprites loaded. Self={IsSelfTarget}. " +
            $"Caster pos={CasterSpritePosition}, Target pos={TargetSpritePosition}");
    }

    public void Reset()
    {
        CasterSprites.Reset();
        if (!IsSelfTarget)
        {
            TargetSprites.Reset();
        }
        else
        {
            // Shared reference already reset above.
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine("ItemContext:");
        sb.AppendLine($"Caster/User = [{Caster.GetDisplayName()}]; Target Count = [{Targets.Count}]");
        sb.AppendLine($"ItemSlot = {Caster.GetItemAtIndex(ItemSlotIndex)}");
        sb.AppendLine("Unfolding targets:");
        foreach (var target in Targets)
        {
            sb.AppendLine($"  => {target.GetDisplayName()}");
        }

        return sb.ToString();
    }
}
