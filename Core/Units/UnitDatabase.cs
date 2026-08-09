using SomberInertia.Enums;

namespace SomberInertia.Core.Units;

public static class UnitDatabase
{
    private static readonly Dictionary<UnitName, UnitData> _units = new();

    public static void Initialize()
    {
        _units.Clear();

        // ─────────────────────────────────────────
        // Force members
        // ─────────────────────────────────────────
        Register(new UnitData
        {
            Name = UnitName.Max,
            MovementType = MovementType.Warrior,
            Movement = 6,
            BaseHP = 15,
            BaseMP = 8,
            BaseAttack = 10,
            BaseDefense = 5,
            BaseSpeed = 5,
            Friendly = true,
            Level = 1,
            DefaultJob = Job.Swordsman
        });

        Register(new UnitData
        {
            Name = UnitName.Anri,
            MovementType = MovementType.Warrior,
            Movement = 5,
            BaseHP = 10,
            BaseMP = 12,
            BaseAttack = 3,
            BaseDefense = 4,
            BaseSpeed = 6,
            Friendly = true,
            Level = 1,
            DefaultJob = Job.Mage
        });

        Register(new UnitData
        {
            Name = UnitName.Tao,
                        MovementType = MovementType.Warrior,
            Movement = 5,
            BaseHP = 10,
            BaseMP = 12,
            BaseAttack = 3,
            BaseDefense = 4,
            BaseSpeed = 6,
            Friendly = true,
            Level = 1,
            DefaultJob = Job.Mage
        });

        // ─────────────────────────────────────────
        // Monsters used in current test setup
        // (no job; pre-set level; cannot equip / promote / gain exp)
        // ─────────────────────────────────────────
        Register(new UnitData
        {
            Name = UnitName.Goblin,
            MovementType = MovementType.Warrior,
            Movement = 5,
            BaseHP = 12,
            BaseMP = 0,
            BaseAttack = 6,
            BaseDefense = 5,
            BaseSpeed = 4,
            Friendly = false,
            Level = 1,
            DefaultJob = Job.Monster
        });

        Register(new UnitData
        {
            Name = UnitName.RuneKnight,
            MovementType = MovementType.Warrior,
            Movement = 5,
            BaseHP = 14,
            BaseMP = 0,
            BaseAttack = 8,
            BaseDefense = 6,
            BaseSpeed = 5,
            Friendly = false,
            Level = 3,
            DefaultJob = Job.Monster
        });

        Register(new UnitData
        {
            Name = UnitName.DarkDwarf,
            MovementType = MovementType.Warrior,
            Movement = 4,
            BaseHP = 16,
            BaseMP = 0,
            BaseAttack = 7,
            BaseDefense = 8,
            BaseSpeed = 3,
            Friendly = false,
            Level = 2,
            DefaultJob = Job.Monster
        });
    }

    public static UnitData Get(UnitName name)
    {
        if (_units.TryGetValue(name, out var data))
        {
            return data;
        }

        Logger.Warning($"UnitDatabase.Get(): No data for [{name}]. Using default template.");

        return new UnitData
        {
            Name = name,
            MovementType = MovementType.Warrior,
            Movement = 5,
            BaseHP = 10,
            BaseMP = 0,
            BaseAttack = 5,
            BaseDefense = 5,
            BaseSpeed = 5,
            Friendly = false,
            Level = 1,
            DefaultJob = Job.Any
        };
    }

    private static void Register(UnitData data)
    {
        _units[data.Name] = data;
    }
}
