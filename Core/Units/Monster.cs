using SomberInertia.Enums;

namespace SomberInertia.Core.Units;

public class Monster : Unit
{
    protected override string AssetRoot => $"Assets/Sprites/Monsters/{Name}/";

    public Monster(string texturePath, string name, MovementType movementType, int movement)
        : base(texturePath, name, movementType, movement)
        {
            Name = name;
            MovementType = movementType;
            Movement = movement;

            HP = new Stat(10);
            MP = new Stat(10);

            Friendly = false;
            Promoted = false;
            EquipWeapon(WeaponManager.Create(WeaponName.Unarmed));

            Logger.Info($"Monster created -> {Name} ({MovementType}), Movement: {Movement}.");
        }
}