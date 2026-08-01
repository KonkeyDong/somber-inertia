# Battle state machine (current)

High-level flow of the tactical battle loop. Instant states transition immediately in `Enter` (or early `Update`).

```mermaid
stateDiagram-v2
    [*] --> CalculateUnitMovementRange

    CalculateUnitMovementRange --> UnitMoving : range ready (or skip if asleep)
    CalculateUnitMovementRange --> AnimateUnitDeaths : died from poison
    CalculateUnitMovementRange --> EndTurn : still asleep

    UnitMoving --> BattleActionMenu : confirm on free tile

    BattleActionMenu --> CalculateWeaponAttackRange : Attack
    BattleActionMenu --> SelectMagic : Magic (has spells)
    BattleActionMenu --> MessageNotice : Magic (no spells)
    BattleActionMenu --> BattleItemMenu : Item (has items)
    BattleActionMenu --> MessageNotice : Item (no items)
    BattleActionMenu --> TransitionSelectorToNextUnit : Stay
    BattleActionMenu --> UnitMoving : cancel

    CalculateWeaponAttackRange --> SelectEnemyForPhysicalAttack : enemies in range
    CalculateWeaponAttackRange --> MessageNotice : no target
    MessageNotice --> BattleActionMenu : dismiss (attack/magic/item notices)

    SelectEnemyForPhysicalAttack --> EnterBattleScreen : confirm
    SelectEnemyForPhysicalAttack --> BattleActionMenu : cancel

    EnterBattleScreen --> BattleResolution : normal
    EnterBattleScreen --> BattleResolutionDebug : Logger.InDebugMode (F1)
    BattleResolution --> ExitBattleScreen
    BattleResolutionDebug --> ExitBattleScreen : confirm (apply dmg) or cancel
    ExitBattleScreen --> AnimateUnitDeaths

    SelectMagic --> SelectMagicLevel : confirm
    SelectMagic --> BattleActionMenu : cancel
    SelectMagicLevel --> PrepareMagicTargets : confirm
    SelectMagicLevel --> SelectMagic : cancel
    PrepareMagicTargets --> SelectMagicTargets : targets exist
    PrepareMagicTargets --> MessageNotice : no target
    MessageNotice --> SelectMagicLevel : dismiss (magic no target)
    SelectMagicTargets --> AnimateUnitDeaths : cast
    SelectMagicTargets --> SelectMagicLevel : cancel

    BattleItemMenu --> DropItem : Drop
    BattleItemMenu --> EquipItem : Equip
    BattleItemMenu --> GiveWhichItem : Give
    BattleItemMenu --> BattleItemMenu : Use (stub)
    BattleItemMenu --> BattleActionMenu : cancel

    DropItem --> PromptYesNo
    DropItem --> BattleItemMenu : cancel
    EquipItem --> BattleItemMenu
    GiveWhichItem --> GiveItemToWhom : confirm item
    GiveWhichItem --> MessageNotice : no adjacent friend
    GiveWhichItem --> BattleItemMenu : cancel
    MessageNotice --> BattleItemMenu : dismiss (give no target)
    GiveItemToWhom --> PromptYesNo : free slot
    GiveItemToWhom --> TradeWhichItemFromAdjacentNeighbor : full inventory
    GiveItemToWhom --> GiveWhichItem : cancel
    TradeWhichItemFromAdjacentNeighbor --> PromptYesNo
    TradeWhichItemFromAdjacentNeighbor --> GiveItemToWhom : cancel

    PromptYesNo --> BattleItemMenu : drop yes/no
    PromptYesNo --> EndTurn : give/trade yes
    PromptYesNo --> GiveItemToWhom : give no
    PromptYesNo --> TradeWhichItemFromAdjacentNeighbor : trade no

    AnimateUnitDeaths --> EndTurn
    TransitionSelectorToNextUnit --> EndTurn
    EndTurn --> CalculateUnitMovementRange
```

## Notes

- **MessageNotice**: primed message + return state; draws any filled range tints.
- **Input**: confirm Z/C, cancel X (`Core/Input.cs`).
- **Debug battle**: F1 debug before attack → one pose per frame + Left/Right scrub.
