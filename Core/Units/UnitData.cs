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
    public Job DefaultJob; // Job.Any for monsters / unused

    public override string ToString()
    {
        return $"{Name.GetDisplayName()} | Move: {MovementType}/{Movement} | " +
               $"HP {BaseHP} MP {BaseMP} ATK {BaseAttack} DEF {BaseDefense} SPD {BaseSpeed} | Job: {DefaultJob}";
    }
}