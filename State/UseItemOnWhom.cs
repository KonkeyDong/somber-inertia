using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Core.Combat.Item;
using SomberInertia.Core.Units;
using SomberInertia.Enums;
using SomberInertia.Graphics.UI;

namespace SomberInertia.State;

/// <summary>
/// After UseWhichItem: pick a friendly unit in item range (including self when range allows).
/// Heal / RemovePoison → item battle presentation (no enemy).
/// </summary>
public class UseItemOnWhom : IGameState
{
    private readonly Game _game;
    private Unit _currentUnit = null!;
    private List<Unit> _targets = new();
    private int _currentIndex;
    private int _itemSlotIndex = -1;

    public UseItemOnWhom(Game game)
    {
        _game = game;
    }

    public void Enter()
    {
        _currentUnit = _game.GetCurrentUnit();
        _itemSlotIndex = _game.Prompt.ItemSlotIndex;

        if (_itemSlotIndex < 0 || _itemSlotIndex >= _currentUnit.Items.Length)
        {
            Logger.Error("UseItemOnWhom: invalid ItemSlotIndex. Returning to UseWhichItem.");
            GameStateManager.ChangeStateType(GameStateType.UseWhichItem);
            return;
        }

        var itemSlot = _currentUnit.GetItemAtIndex(_itemSlotIndex);
        var data = ItemDatabase.Get(itemSlot.Name);

        if (data.Type != ItemType.Consumable ||
            (data.EffectType != ItemEffectType.Heal && data.EffectType != ItemEffectType.RemovePoison))
        {
            Logger.Warning(
                $"UseItemOnWhom: item [{data.Name}] is not a supported consumable yet " +
                $"({data.Type}, {data.EffectType}). Returning to UseWhichItem.");
            GameStateManager.ChangeStateType(GameStateType.UseWhichItem);
            return;
        }

        _game.Grid.CalculateItemUseRange(_currentUnit, data);

        var unitsInRange = _game.Grid.BuildListOfUnitsInRange(_currentUnit);
        _targets = unitsInRange
            .Where(u => u.Friendly == _currentUnit.Friendly)
            .ToList();

        // Ensure caster is included if their tile is in range (self-use).
        if (_currentUnit.Block != null)
        {
            var selfCoord = (_currentUnit.Block.X, _currentUnit.Block.Y);
            if (_game.Grid.RangeSet.Contains(selfCoord) && !_targets.Contains(_currentUnit))
            {
                _targets.Insert(0, _currentUnit);
            }
        }

        if (_targets.Count == 0)
        {
            GameStateManager.ShowMessageNotice(
                GameConstants.MessageNotice.NoTarget,
                GameStateType.UseWhichItem);
            return;
        }

        // Prefer self first if present.
        _currentIndex = _targets.IndexOf(_currentUnit);
        if (_currentIndex < 0)
        {
            _currentIndex = 0;
        }

        _game.InitializeHighlight();
        _game.SetHighlightTarget(_targets[_currentIndex]);
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

    private void ConfirmSelection()
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
        _game.Renderer.DrawHighlightRectangle(scale, _game.GetHighlightPosition());

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
