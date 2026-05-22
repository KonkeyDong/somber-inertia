using SomberInertia.Enums;
using SomberInertia.Graphics;

namespace SomberInertia.Core.Units;

public class ForceMember : Unit
{
    public override bool Promoted { get; set; }
    public Job Job { get; set; }
    public int Exp { get; set; } // experience

    protected override string AssetRoot =>
        $"Assets/Sprites/Characters/{Name}/{(Promoted ? "Promoted" : "Unpromoted")}";

    public ForceMember(string texturePath, string name, MovementType movementType, int movement)
        : base(texturePath, name, movementType, movement)
    {
        Name = name;
        MovementType = movementType;
        Movement = movement * 2;

        HP = new Stat(10);
        MP = new Stat(10);

        Friendly = true;
        Promoted = false;
        EquipWeapon(WeaponManager.Create(WeaponName.Unarmed));

        Logger.Info($"Force Member created -> {Name} ({MovementType}), Movement: {Movement}.");
    }
}