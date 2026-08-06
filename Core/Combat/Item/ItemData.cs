using SomberInertia.Enums;
using System.Text;

namespace SomberInertia.Core.Combat.Item;

/// <summary>Immutable item definition from <see cref="ItemDatabase"/>.</summary>
public readonly struct ItemData
{
    public ItemName Name { get; init; }
    public ItemType Type { get; init; }
    public int Price { get; init; }
    public int Attack { get; init; } // 0 for non-weapons
    public Range DistanceRange { get; init; }
    public Job AllowedJobs { get; init; }
    public bool Cursed { get; init; }

    public ItemEffectType EffectType { get; init; }
    public int EffectValue { get; init; } // e.g. heal amount
    public MagicName SpellName { get; init; } // MagicName.NoSpell if none

    public int SellPrice => (int)(Price * 0.75f);

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"ItemData [{Name}]:");
        sb.AppendLine($"  Display Name = [{Name.GetDisplayName()}]");
        sb.AppendLine($"  Type         = [{Type}]");
        sb.AppendLine($"  Attack       = [{Attack}]");
        sb.AppendLine($"  Price        = [{Price}] (Sell: {SellPrice})");
        sb.AppendLine($"  Range        = [{DistanceRange}]");
        sb.AppendLine($"  AllowedJobs  = [{AllowedJobs}]");
        sb.AppendLine($"  Cursed       = [{Cursed}]");
        sb.AppendLine($"  EffectType   = [{EffectType}]");
        sb.AppendLine($"  EffectValue  = [{EffectValue}]");
        sb.AppendLine($"  SpellName    = [{SpellName}]");
        return sb.ToString();
    }
}
