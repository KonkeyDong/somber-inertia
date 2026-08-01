# Battle state machine

Flows are split so each menu stays readable. **Two-way arrows** (`A <--> B`) mean confirm (or entry) one way and cancel/dismiss the other. **One-way arrows** are pipelines with no simple reverse.

**Input:** confirm = Z/C, cancel = X (`Core/Input.cs`).

**MessageNotice:** shows a primed string and returns to a primed state (not a single fixed parent).

**Instant states** (transition in `Enter`, no lasting UI): `CalculateWeaponAttackRange`, `PrepareMagicTargets`, `EndTurn`, and often `CalculateUnitMovementRange` / `AnimateUnitDeaths` when they only hop.

---

## 1. Core turn loop

Shared hub. Action menu is the branch point for Attack / Magic / Item / Stay.

```mermaid
stateDiagram-v2
    [*] --> CalculateUnitMovementRange

    CalculateUnitMovementRange --> UnitMoving : range ready
    CalculateUnitMovementRange --> AnimateUnitDeaths : died from poison
    CalculateUnitMovementRange --> EndTurn : still asleep

    UnitMoving <--> BattleActionMenu : confirm tile / cancel menu

    BattleActionMenu --> TransitionSelectorToNextUnit : Stay
    TransitionSelectorToNextUnit --> EndTurn

    AnimateUnitDeaths --> EndTurn
    EndTurn --> CalculateUnitMovementRange
```

From **BattleActionMenu** (see sections below):

| Command | Enters |
|---------|--------|
| Attack | Attack flow |
| Magic | Magic flow (or MessageNotice if no spells) |
| Item | Item flow (or MessageNotice if no items) |
| Stay | `TransitionSelectorToNextUnit` → `EndTurn` |

---

## 2. Attack flow

```mermaid
stateDiagram-v2
    BattleActionMenu <--> SelectEnemyForPhysicalAttack : Attack* / cancel
    BattleActionMenu <--> MessageNotice : no target / dismiss

    SelectEnemyForPhysicalAttack --> EnterBattleScreen : confirm

    EnterBattleScreen --> BattleResolution : normal
    EnterBattleScreen --> BattleResolutionDebug : debug (F1)

    BattleResolution --> ExitBattleScreen
    BattleResolutionDebug --> ExitBattleScreen : confirm (apply dmg) or cancel

    ExitBattleScreen --> AnimateUnitDeaths
    AnimateUnitDeaths --> EndTurn
```

\* **Instant hop:** `CalculateWeaponAttackRange` runs between menu and target select (or MessageNotice if no enemies).

**MessageNotice** return for no attack target: `BattleActionMenu`.

**Debug:** F1 before confirming the attack so `AttackContext` builds one pose per frame; Left/Right scrub in `BattleResolutionDebug`.

---

## 3. Magic flow

```mermaid
stateDiagram-v2
    BattleActionMenu <--> SelectMagic : Magic / cancel
    BattleActionMenu <--> MessageNotice : no spells / dismiss

    SelectMagic <--> SelectMagicLevel : confirm / cancel

    SelectMagicLevel <--> SelectMagicTargets : confirm* / cancel
    SelectMagicLevel <--> MessageNotice : no target / dismiss

    SelectMagicTargets --> AnimateUnitDeaths : cast
    AnimateUnitDeaths --> EndTurn
```

\* **Instant hop:** `PrepareMagicTargets` between level confirm and target select (or MessageNotice if no valid targets).

**MessageNotice** returns:

| Reason | Return state |
|--------|----------------|
| No spells | `BattleActionMenu` |
| No magic targets | `SelectMagicLevel` |

---

## 4. Item flow

```mermaid
stateDiagram-v2
    BattleActionMenu <--> BattleItemMenu : Item / cancel
    BattleActionMenu <--> MessageNotice : no items / dismiss

    BattleItemMenu <--> DropItem : Drop / cancel
    BattleItemMenu <--> EquipItem : Equip / done or cancel
    BattleItemMenu <--> GiveWhichItem : Give / cancel
    BattleItemMenu --> BattleItemMenu : Use (stub)

    DropItem <--> PromptYesNo : confirm / No
    PromptYesNo --> BattleItemMenu : Yes (dropped) or No (back via Drop cancel path)

    GiveWhichItem <--> GiveItemToWhom : confirm item / cancel
    GiveWhichItem <--> MessageNotice : no adjacent friend / dismiss

    GiveItemToWhom <--> TradeWhichItemFromAdjacentNeighbor : full inventory / cancel
    GiveItemToWhom <--> PromptYesNo : free-slot give / No
    TradeWhichItemFromAdjacentNeighbor <--> PromptYesNo : swap / No

    PromptYesNo --> EndTurn : Yes (give or trade)
```

**MessageNotice** (no give target) returns to `BattleItemMenu`.

**PromptYesNo** is primed per action (`ReturnStateOnYes` / `ReturnStateOnNo`):

| Action | Yes | No |
|--------|-----|-----|
| Drop | `BattleItemMenu` | `DropItem` |
| Give (free slot) | `EndTurn` | `GiveItemToWhom` |
| Trade (swap) | `EndTurn` | `TradeWhichItemFromAdjacentNeighbor` |

**Equip** always returns to `BattleItemMenu` (confirm equip or cancel).

---

## 5. Shared notes

| Topic | Detail |
|-------|--------|
| Confirm / cancel | Z or C / X |
| MessageNotice | Primed message + return state; may still draw range tints |
| Instant states | No lasting UI; hop in `Enter` |
| End of successful combat actions | Usually `AnimateUnitDeaths` → `EndTurn` (physical after battle exit; magic after cast) |
| Give/trade success | `PromptYesNo` Yes → `EndTurn` (skips death anim unless HP already 0) |
