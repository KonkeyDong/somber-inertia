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

    /// <summary>Presentation order: typically caster first, then other friendlies.</summary>
    public List<(Unit Unit, BattleUnitSpriteSet Sprites)> PresentationUnits { get; private set; } = new();

    public BattleUnitSpriteSet CasterSprites { get; private set; } = new();

    public Unit Target => Targets.Count > 0 ? Targets[0] : Caster;

    public bool IsSelfTarget => Targets.Count == 1 && ReferenceEquals(Targets[0], Caster);

    public bool IsPartyWide => Targets.Count > 1;

    public Vector2 CasterSpritePosition => GameConstants.Battle.GetSpritePosition(Caster);

    public ItemContext(Unit caster, List<Unit> targets, Grid grid, int itemSlotIndex)
    {
        Caster = caster;
        Targets = targets ?? new List<Unit>();
        Grid = grid;
        ItemSlotIndex = itemSlotIndex;
    }

    /// <summary>
    /// Load force-side battle idle sprites for every target (and ensure caster is present).
    /// </summary>
    public void LoadBattleSprites()
    {
        PresentationUnits = new List<(Unit, BattleUnitSpriteSet)>();

        foreach (var unit in Targets)
        {
            var set = BattleUnitSpriteManager.Get(unit);
            set.BasePosition = GameConstants.Battle.GetSpritePosition(unit);
            PresentationUnits.Add((unit, set));
        }

        // EnterBattleScreen / exit draw the caster set explicitly.
        var casterEntry = PresentationUnits.FirstOrDefault(p => ReferenceEquals(p.Unit, Caster));
        if (casterEntry.Unit != null)
        {
            CasterSprites = casterEntry.Sprites;
        }
        else
        {
            CasterSprites = BattleUnitSpriteManager.Get(Caster);
            CasterSprites.BasePosition = CasterSpritePosition;
        }

        Logger.Debug(
            $"ItemContext sprites loaded. Targets={Targets.Count}, PartyWide={IsPartyWide}, " +
            $"Caster pos={CasterSpritePosition}");
    }

    public void Reset()
    {
        var reset = new HashSet<BattleUnitSpriteSet>();
        foreach (var (_, sprites) in PresentationUnits)
        {
            if (reset.Add(sprites))
            {
                sprites.Reset();
            }
        }

        if (reset.Add(CasterSprites))
        {
            CasterSprites.Reset();
        }

        PresentationUnits.Clear();
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
