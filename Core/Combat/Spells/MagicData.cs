using System.Text;
using SomberInertia.Enums;

namespace SomberInertia.Core.Combat.Spells;

/// <summary>Immutable spell definition from <see cref="MagicDatabase"/>.</summary>
public readonly struct MagicData
{
    public MagicName Name { get; init; }
    public int Level { get; init; }
    public int MPCost { get; init; }
    public MagicType MagicType { get; init; }
    public Range DistanceRange { get; init; }
    public Range TargetRange { get; init; }
    public bool Offensive { get; init; }
    public MagicEffectType EffectType { get; init; }
    public int EffectValue { get; init; } // damage or heal amount

    public MagicFamily Family => Name.ToFamily();

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"MagicData [{Name}]:");
        sb.AppendLine($"  Display     = [{Name.GetDisplayName()}]");
        sb.AppendLine($"  Level       = [{Level}]");
        sb.AppendLine($"  MPCost      = [{MPCost}]");
        sb.AppendLine($"  Type        = [{MagicType}]");
        sb.AppendLine($"  Distance    = [{DistanceRange}]");
        sb.AppendLine($"  Target      = [{TargetRange}]");
        sb.AppendLine($"  Offensive   = [{Offensive}]");
        sb.AppendLine($"  EffectType  = [{EffectType}]");
        sb.AppendLine($"  EffectValue = [{EffectValue}]");
        return sb.ToString();
    }
}
