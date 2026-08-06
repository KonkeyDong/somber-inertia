using SomberInertia.Enums;

namespace SomberInertia.Core.Units;

/// <summary>Immutable unit definition from <see cref="UnitDatabase"/>.</summary>
public readonly struct UnitData
{
    public UnitName Name { get; init; }
    public MovementType MovementType { get; init; }
    public int Movement { get; init; }
    public int BaseHP { get; init; }
    public int BaseMP { get; init; }
    public int BaseAttack { get; init; }
    public int BaseDefense { get; init; }
    public int BaseSpeed { get; init; }

    public bool Friendly { get; init; }

    // Force members gain levels; monsters use a pre-set level and do not gain exp.
    public int Level { get; init; }

    // Monsters use default (no job); force members use a concrete job.
    public Job DefaultJob { get; init; }

    public override string ToString()
    {
        return $"{Name.GetDisplayName()} | Move: {MovementType}/{Movement} | " +
               $"HP {BaseHP} MP {BaseMP} ATK {BaseAttack} DEF {BaseDefense} SPD {BaseSpeed} | " +
               $"Friendly: {Friendly} Lvl {Level} | Job: {DefaultJob}";
    }
}
