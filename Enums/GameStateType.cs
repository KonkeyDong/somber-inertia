namespace SomberInertia.Enums;

public enum GameStateType
{
    Exploration,
    Talking,
    Searching,
    Shopping,
    EnterTown,
    LeaveTown,

    EnterBattle,
    LeaveBattle,
    MoveCursor,
    IsBossDefeated,
    AreAllEnemiesDefeated,
    IsHeroDefeated,
    Fighting,
    UnitMoving,
    EndTurn,
    CalculateUnitMovementRange,
    CalculateWeaponAttackRange,
    PrepareMagicTargets,
    BattleActionMenu,
    SelectingAction,
    SelectEnemyForPhysicalAttack,
    TransitionSelectorToNextUnit,
    AnimateUnitDeaths,
    SelectMagic,
    SelectMagicLevel,
    NoMagicAvailable,
    NoAttackTargetAvailable,
    NoMagicTargetAvailable,
    SelectMagicTargets
}