using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Core.Combat;
using SomberInertia.Core.Combat.Item;
using SomberInertia.Core.Combat.Spells;
using SomberInertia.Core.Units;
using SomberInertia.Enums;
using SomberInertia.Graphics.UI;

namespace SomberInertia.State;

/// <summary>
/// After UseWhichItem: pick a target in range.
/// Consumable Heal/RemovePoison → item battle (friendlies).
/// Spell item → magic range/targets, Cast(fromItem), durability → AnimateUnitDeaths.
/// </summary>
public class UseItemOnWhom : IGameState
{
    private enum UseMode
    {
        Consumable,
        SpellItem
    }

    private readonly Game _game;
    private Unit _currentUnit = null!;
    private List<Unit> _targets = new();
    private int _currentIndex;
    private int _itemSlotIndex = -1;
    private ItemData _itemData;
    private MagicData _magicData;
    private UseMode _mode;
    private List<Block> _areaOfEffect = new();

    public UseItemOnWhom(Game game)
    {
        _game = game;
    }

    public void Enter()
    {
        _currentUnit = _game.GetCurrentUnit();
        _itemSlotIndex = _game.Prompt.ItemSlotIndex;
        _areaOfEffect = new();

        if (_itemSlotIndex < 0 || _itemSlotIndex >= _currentUnit.Items.Length)
        {
            Logger.Error("UseItemOnWhom: invalid ItemSlotIndex. Returning to UseWhichItem.");
            GameStateManager.ChangeStateType(GameStateType.UseWhichItem);
            return;
        }

        var itemSlot = _currentUnit.GetItemAtIndex(_itemSlotIndex);
        _itemData = ItemDatabase.Get(itemSlot.Name);

        if (IsSupportedConsumable(_itemData))
        {
            _mode = UseMode.Consumable;
            EnterConsumable();
            return;
        }

        if (_itemData.SpellName != MagicName.NoSpell)
        {
            // Job gate already applied in UseWhichItem UsableFilter (AllowedJobs).
            if (!_currentUnit.GetJob().IsAllowedBy(_itemData.AllowedJobs))
            {
                Logger.Warning(
                    $"UseItemOnWhom: job [{_currentUnit.GetJob()}] cannot use spell item [{_itemData.Name}].");
                GameStateManager.ChangeStateType(GameStateType.UseWhichItem);
                return;
            }

            _mode = UseMode.SpellItem;
            _magicData = MagicDatabase.Get(_itemData.SpellName);
            EnterSpellItem();
            return;
        }

        Logger.Warning(
            $"UseItemOnWhom: item [{_itemData.Name}] is not a supported use target " +
            $"({_itemData.Type}, effect={_itemData.EffectType}, spell={_itemData.SpellName}).");
        GameStateManager.ChangeStateType(GameStateType.UseWhichItem);
    }

    private static bool IsSupportedConsumable(ItemData data) =>
        data.Type == ItemType.Consumable &&
        (data.EffectType == ItemEffectType.Heal || data.EffectType == ItemEffectType.RemovePoison);

    private void EnterConsumable()
    {
        _game.Grid.CalculateItemUseRange(_currentUnit, _itemData);
        BuildFriendlyTargetsIncludingSelf();
        if (!TryFinishEnter())
        {
            return;
        }
    }

    private void EnterSpellItem()
    {
        // Spell cast range from MagicData, not the weapon's melee DistanceRange.
        _game.Grid.CalculateMagicAttackRange(_currentUnit, _magicData);

        var unitsInRange = _game.Grid.BuildListOfUnitsInRange(_currentUnit);
        if (_magicData.Offensive)
        {
            _targets = unitsInRange
                .Where(u => u.Friendly != _currentUnit.Friendly)
                .ToList();
        }
        else
        {
            _targets = unitsInRange
                .Where(u => u.Friendly == _currentUnit.Friendly)
                .ToList();
            EnsureSelfInTargetsIfInRange();
        }

        if (!TryFinishEnter())
        {
            return;
        }

        RefreshSpellAoe();
    }

    private void BuildFriendlyTargetsIncludingSelf()
    {
        var unitsInRange = _game.Grid.BuildListOfUnitsInRange(_currentUnit);
        _targets = unitsInRange
            .Where(u => u.Friendly == _currentUnit.Friendly)
            .ToList();
        EnsureSelfInTargetsIfInRange();
    }

    private void EnsureSelfInTargetsIfInRange()
    {
        if (_currentUnit.Block == null)
        {
            return;
        }

        var selfCoord = (_currentUnit.Block.X, _currentUnit.Block.Y);
        if (_game.Grid.RangeSet.Contains(selfCoord) && !_targets.Contains(_currentUnit))
        {
            _targets.Insert(0, _currentUnit);
        }
    }

    /// <returns>False if no targets (already transitioned to notice).</returns>
    private bool TryFinishEnter()
    {
        if (_targets.Count == 0)
        {
            GameStateManager.ShowMessageNotice(
                GameConstants.MessageNotice.NoTarget,
                GameStateType.UseWhichItem);
            return false;
        }

        // Prefer self first for friendly-target uses.
        var preferSelf = _mode == UseMode.Consumable
            || (_mode == UseMode.SpellItem && !_magicData.Offensive);
        if (preferSelf)
        {
            var selfIndex = _targets.IndexOf(_currentUnit);
            _currentIndex = selfIndex >= 0 ? selfIndex : 0;
        }
        else
        {
            _currentIndex = 0;
        }

        _game.InitializeHighlight();
        _game.SetHighlightTarget(_targets[_currentIndex]);
        return true;
    }

    public void Exit()
    {
    }

    public void HandleInput()
    {
        if (_targets.Count == 0)
        {
            return;
        }

        if (Input.TryCycleIndex(ref _currentIndex, _targets.Count))
        {
            var newTarget = _targets[_currentIndex];
            if (newTarget.Block != null)
            {
                _game.SetHighlightTarget(newTarget);
            }

            if (_mode == UseMode.SpellItem)
            {
                RefreshSpellAoe();
            }
        }

        if (Input.IsConfirmPressed())
        {
            ConfirmSelection();
        }

        if (Input.IsCancelPressed())
        {
            CancelSelection();
        }
    }

    private void RefreshSpellAoe()
    {
        var selected = _targets[_currentIndex];
        _game.Grid.CalculateSpellEffectRange(selected, _magicData);
        // Recalc cast range was overwritten by spell effect range — store AoE blocks, then restore cast range for draw.
        _areaOfEffect = _game.Grid.GetBlocksFromRangeSet();
        _game.Grid.CalculateMagicAttackRange(_currentUnit, _magicData);
    }

    private void ConfirmSelection()
    {
        if (_mode == UseMode.Consumable)
        {
            ConfirmConsumable();
            return;
        }

        ConfirmSpellItem();
    }

    private void ConfirmConsumable()
    {
        var target = _targets[_currentIndex];
        var targets = new List<Unit> { target };

        _game.ItemContext = new ItemContext(_currentUnit, targets, _game.Grid, _itemSlotIndex);
        _game.ItemContext.LoadBattleSprites();
        _game.BattleScreenMode = BattleScreenMode.ItemConsumable;

        Logger.Info(_game.ItemContext.ToString());

        _game.ItemUI.Reset();
        _game.ItemUI.ResetLayoutCenter();
        _game.Grid.ClearRangeSet();

        GameStateManager.ChangeStateType(GameStateType.EnterBattleScreen);
    }

    private void ConfirmSpellItem()
    {
        var selected = _targets[_currentIndex];

        // Build AoE target list from spell TargetRange around the selected unit.
        _game.Grid.CalculateSpellEffectRange(selected, _magicData);
        var aoeUnits = _game.Grid.BuildListOfUnitsInRange(_currentUnit);

        List<Unit> castTargets;
        if (_magicData.Offensive)
        {
            castTargets = aoeUnits
                .Where(u => u.Friendly != _currentUnit.Friendly)
                .ToList();
        }
        else
        {
            castTargets = aoeUnits
                .Where(u => u.Friendly == _currentUnit.Friendly)
                .ToList();
        }

        if (castTargets.Count == 0)
        {
            // At least the selected unit if still valid for this spell side.
            if (_magicData.Offensive
                ? selected.Friendly != _currentUnit.Friendly
                : selected.Friendly == _currentUnit.Friendly)
            {
                castTargets.Add(selected);
            }
        }

        if (castTargets.Count == 0)
        {
            Logger.Warning("UseItemOnWhom spell: no valid cast targets in AoE.");
            GameStateManager.ShowMessageNotice(
                GameConstants.MessageNotice.NoTarget,
                GameStateType.UseWhichItem);
            return;
        }

        var magicContext = new MagicContext(_currentUnit, castTargets, _game.Grid);
        Logger.Info(magicContext.ToString());
        Logger.Info($"Casting item spell [{_itemData.SpellName}] from [{_itemData.Name}] (no MP).");

        MagicDatabase.Cast(_itemData.SpellName, magicContext, fromItem: true);
        ItemDatabase.ApplySpellItemDurability(_currentUnit, _itemSlotIndex);

        _game.ItemUI.Reset();
        _game.ItemUI.ResetLayoutCenter();
        _game.Grid.ClearRangeSet();
        _game.Prompt.Reset();

        GameStateManager.ChangeStateType(GameStateType.AnimateUnitDeaths);
    }

    private void CancelSelection()
    {
        GameStateManager.ChangeStateType(GameStateType.UseWhichItem);
    }

    public void Update()
    {
        _game.Grid.RangeTint.Tick();
        _game.FlipFlop.Tick();
        _game.UpdateHighlightPosition();
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawRange(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FlipFlop.IsOn);

        if (_mode == UseMode.SpellItem && _game.IsHighlightSettled())
        {
            foreach (var block in _areaOfEffect)
            {
                _game.Renderer.DrawHighlightRectangle(scale, block.GetPixelCoordinates());
            }
        }
        else
        {
            _game.Renderer.DrawHighlightRectangle(scale, _game.GetHighlightPosition());
        }

        if (_targets.Count > 0)
        {
            var selected = _targets[_currentIndex];
            _game.Renderer.DrawUnitInfoBox(
                scale,
                selected,
                GameConstants.Give.Positions.RecipientInfoBox);
        }
    }
}
