using SomberInertia.Enums;

namespace SomberInertia.Core.Units;

public class ForceMember : Unit
{
    public override bool Promoted { get; set; }
    public Job Job { get; set; }
    public int Exp { get; set; }
    public int Level { get; set; }

    protected override string AssetRoot =>
        $"Assets/Sprites/Characters/{Name.GetBaseName()}/{(Promoted ? "Promoted" : "Unpromoted")}";

    public ForceMember(UnitName name)
        : base(name)
    {
        Friendly = true;
        Promoted = false;
        Level = 1;
        Exp = 0;
        Job = UnitDatabase.Get(name).DefaultJob;

        Logger.Info($"Force Member created -> {Name.GetDisplayName()} ({MovementType}), Movement: {Movement}.");
    }
}