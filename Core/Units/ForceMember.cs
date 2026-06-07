using SomberInertia.Enums;
using SomberInertia.Graphics;
using SomberInertia.Core.Combat;

namespace SomberInertia.Core.Units;

public class ForceMember : Unit
{
    public override bool Promoted { get; set; }
    public Job Job { get; set; }
    public int Exp { get; set; } // experience
    public int Level { get; set; }
    
    protected override string AssetRoot =>
        $"Assets/Sprites/Characters/{Name.GetBaseName()}/{(Promoted ? "Promoted" : "Unpromoted")}";

    public ForceMember(UnitName name, MovementType movementType, int movement)
        : base(name, movementType, movement)
    {
        Name = name;
        MovementType = movementType;
        Movement = movement * 2;

        HP = new Stat(10);
        MP = new Stat(10);

        Friendly = true;
        Promoted = false;
        EquipWeapon(WeaponManager.Create(WeaponName.Unarmed));

        Logger.Info($"Force Member created -> {Name.GetDisplayName()} ({MovementType}), Movement: {Movement}.");
    }
}