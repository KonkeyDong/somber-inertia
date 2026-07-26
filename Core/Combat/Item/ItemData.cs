using SomberInertia.Enums;
using System.Text;

namespace SomberInertia.Core.Combat.Item;

public struct ItemData
{
    public ItemName Name;
    public ItemType Type;
    public int Price;
    public int Attack; // 0 for non-weapons
    public Range DistanceRange;
    public Job AllowedJobs;
    public bool Cursed;

    public ItemEffectType EffectType;
    public int EffectValue; // e.g. heal amount
    public MagicName SpellName; // MagicName.NoSpell if none

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
