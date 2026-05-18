stateDiagram-v2
    [*] --> CalculateUnitMovementRange

    CalculateUnitMovementRange --> UnitMoving : Movement range ready

    UnitMoving --> CalculateWeaponAttackRange : Choose Attack

    CalculateWeaponAttackRange --> BattleActionMenu : Attack range calculated

    BattleActionMenu --> SelectEnemyForPhysicalAttack : Attack
    BattleActionMenu --> EndTurn : Stay / End Turn

    SelectEnemyForPhysicalAttack --> EndTurn : Attack executed

    EndTurn --> CalculateUnitMovementRange : Next unit's turn