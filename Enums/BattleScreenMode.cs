namespace SomberInertia.Enums;

/// <summary>What Enter/Exit battle screens present and where they hop next.</summary>
public enum BattleScreenMode
{
    /// <summary>Attacker vs defender; exit → AnimateUnitDeaths.</summary>
    Combat,

    /// <summary>Consumable use: force side only, no enemy; exit → EndTurn.</summary>
    ItemConsumable,
}
