using SomberInertia.Enums;

namespace SomberInertia.Core.Combat;

public class Magic
{
    public string Name { get; set; }
    public int Level { get; set; }
    public int MPCost { get; set; }
    public MagicType MagicType { get; set; }
    public Range DistanceRange { get; set; }
    public Range TargetRange { get; set; }

    public Magic(string name, int level, int mPCost, MagicType magicType, Range distanceRange, Range targetRange)
    {
        if (level <= 0 || level > 4)
        {
            Logger.Error("Magic level has to be between 1 and 4.");
        }

        if (mPCost < 0)
        {
            Logger.Error("MP Cost cannot be less than 0.");
        }

        Name = name;
        Level = level;
        MPCost = mPCost;
        MagicType = magicType;
        DistanceRange = distanceRange;
        TargetRange = targetRange;
    }
}