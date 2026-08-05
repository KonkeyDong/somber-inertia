using SomberInertia.Enums;

namespace SomberInertia.Core.Units;

public struct UnitData
{
    public UnitName Name;
    public MovementType MovementType;
    public int Movement;
    public int BaseHP;
    public int BaseMP;
    public int BaseAttack;
    public int BaseDefense;
    public int BaseSpeed;

    public bool Friendly;

    // ForceMembers gain levels while monsters don't gain experience or level up.
    public int Level;

    // Monsters will just have a generic "Monster" job.
    public Job DefaultJob;

    public override string ToString()
    {
        return $"{Name.GetDisplayName()} | Move: {MovementType}/{Movement} | " +
               $"HP {BaseHP} MP {BaseMP} ATK {BaseAttack} DEF {BaseDefense} SPD {BaseSpeed} | " +
               $"Friendly: {Friendly} Lvl {Level} | Job: {DefaultJob}";
    }
}