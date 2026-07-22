using SomberInertia.Enums;

namespace SomberInertia.Core.Combat.Spells;

public class Magic
{
    public MagicName Name { get; set; }
    public int Level { get; set; }
    public int MPCost { get; set; }
    public MagicType MagicType { get; set; }
    public Range DistanceRange { get; set; }
    public Range TargetRange { get; set; }
    public bool Offensive { get; set; }

    private readonly IMagicEffect _effect;

    public Magic(MagicName name, int level, int MPCost, MagicType magicType, Range distanceRange, Range targetRange, bool offensive, IMagicEffect effect)
    {
        if (level <= 0 || level > 4)
        {
            Logger.Error("Magic level has to be between 1 and 4.");
        }

        if (MPCost < 0)
        {
            Logger.Error("MP Cost cannot be less than 0.");
        }

        Name = name;
        Level = level;
        this.MPCost = MPCost;
        MagicType = magicType;
        DistanceRange = distanceRange;
        TargetRange = targetRange;
        Offensive = offensive;
        _effect = effect;
    }

    public void Cast(MagicContext context) => _effect.Execute(context, this);
    public Magic Clone() => new Magic(Name, Level, MPCost, MagicType, DistanceRange, TargetRange, Offensive, _effect);
    public override string ToString() => $"{Name.GetDisplayName()}: level = [{Level}]; MP cost: [{MPCost}]; DistanceRange: [{DistanceRange.ToString()}]; TargetRange: [{TargetRange.ToString()}].";
}