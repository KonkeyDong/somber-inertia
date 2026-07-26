using SomberInertia.Enums;

namespace SomberInertia.Core.Units;

public class Monster : Unit
{
    protected override string AssetRoot => $"Assets/Sprites/Monsters/{Name.GetBaseName()}/";

    public Monster(UnitName name)
        : base(name)
    {
        Friendly = false;
        Promoted = false;

        Logger.Info($"Monster created -> {Name.GetDisplayName()} ({MovementType}), Movement: {Movement}.");
    }
}