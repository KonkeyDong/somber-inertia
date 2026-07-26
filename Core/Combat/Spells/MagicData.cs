using System.Text;
using SomberInertia.Enums;

namespace SomberInertia.Core.Combat.Spells;

public struct MagicData
{
    public MagicName Name;
    public int Level;
    public int MPCost;
    public MagicType MagicType;
    public Range DistanceRange;
    public Range TargetRange;
    public bool Offensive;
    public MagicEffectType EffectType;
    public int EffectValue; // damage or heal amount

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