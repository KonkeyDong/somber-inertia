using System.Numerics;
using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Core.Units;
using SomberInertia.Core.Combat.Item;
using SomberInertia.Enums;
using SomberInertia.Graphics;
using SomberInertia.Graphics.UI;

namespace SomberInertia.State;

public class EquipItem : IGameState
{
    private readonly Game _game;
    private readonly Unit _currentUnit;
    private readonly Job _job;

    private Vector2 _centerPosition;
    private int _selectedIndex; // 0..2 inventory, 3 = Unarmed
    private bool _isUnarmedSelected;

    private static readonly Dictionary<Direction, int> _indexByDirection = new()
    {
        { Direction.Up,    0 },
        { Direction.Left,  1 },
        { Direction.Right, 2 },
        { Direction.Down,  3 } // Unarmed
    };

    public EquipItem(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();

        _job = _currentUnit is ForceMember forceMember
            ? forceMember.Job
            : Job.Any;
    }

    public void Enter()
    {
        UpdateCenterPosition();
        _selectedIndex = 3; // default Unarmed, or pick current equipped below
        _isUnarmedSelected = true;

        // Start on currently equipped weapon if possible
        if (_currentUnit.EquippedWeaponIndex >= 0 && _currentUnit.EquippedWeaponIndex <= 2)
        {
            if (IsSlotEquippable(_currentUnit.EquippedWeaponIndex))
            {
                _selectedIndex = _currentUnit.EquippedWeaponIndex;
                _isUnarmedSelected = false;
            }
        }

        UpdateSelectedIcon();
    }

    public void Exit()
    {

    }

    private void UpdateCenterPosition()
    {
        _centerPosition = RadialMenuLayout.GetCenterPosition();
    }

    public void HandleInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Up))
        {
            TrySelect(Direction.Up);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Left))
        {
            TrySelect(Direction.Left);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Right))
        {
            TrySelect(Direction.Right);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Down))
        {
            TrySelect(Direction.Down);
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

    private void TrySelect(Direction direction)
    {
        if (!_indexByDirection.TryGetValue(direction, out var index))
        {
            return;
        }

        // Down = always Unarmed
        if (index == 3)
        {
            _selectedIndex = 3;
            _isUnarmedSelected = true;
            UpdateSelectedIcon();
            return;
        }

        // Inventory slot: only select if equippable weapon is there
        if (!IsSlotEquippable(index))
        {
            return;
        }

        _selectedIndex = index;
        _isUnarmedSelected = false;
        UpdateSelectedIcon();
    }

    private bool IsSlotEquippable(int index)
    {
        if (index < 0 || index > 2)
        {
            return false;
        }

        var slot = _currentUnit.Items[index];
        if (slot.IsEmpty)
        {
            return false;
        }

        var data = ItemDatabase.Get(slot.Name);
        if (!data.Type.IsWeapon())
        {
            return false;
        }

        return CanJobEquip(data);
    }

    private bool CanJobEquip(ItemData data) => _job.IsAllowedBy(data.AllowedJobs);

    private void UpdateSelectedIcon()
    {
        // Selection blink is driven per-slot via DrawItemIcon(..., isSelected).
        // Restart flipper when the selected slot changes.
        ItemIcons.ClearSelection();
        ItemIcons.Reset();
    }

    private void ConfirmSelection()
    {
        if (_isUnarmedSelected)
        {
            _currentUnit.EquipUnarmed();
            Logger.Info($"{_currentUnit.GetDisplayName()} equipped Unarmed.");
            GameStateManager.ChangeStateType(GameStateType.BattleItemMenu);
            return;
        }

        if (!IsSlotEquippable(_selectedIndex))
        {
            return;
        }

        _currentUnit.EquipWeaponAtIndex(_selectedIndex);
        Logger.Info($"{_currentUnit.GetDisplayName()} equipped {_currentUnit.Items[_selectedIndex].Name}.");
        GameStateManager.ChangeStateType(GameStateType.BattleItemMenu);
    }

    private void CancelSelection()
    {
        GameStateManager.ChangeStateType(GameStateType.BattleItemMenu);
    }

    private ItemName GetDisplayedItemName(int index)
    {
        if (index == 3)
        {
            return ItemName.Unarmed;
        }

        if (!IsSlotEquippable(index))
        {
            return ItemName.NoItem;
        }

        return _currentUnit.Items[index].Name;
    }

    private int GetPreviewAttack()
    {
        if (_isUnarmedSelected)
        {
            return _currentUnit.Attack;
        }

        var data = ItemDatabase.Get(_currentUnit.Items[_selectedIndex].Name);
        return _currentUnit.Attack + data.Attack;
    }

    public void Update()
    {
        _game.FrameFlipper.Tick();
        ItemIcons.Tick();
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FrameFlipper.IsOn);

        // Radial icons — highlight by slot index, not item name
        foreach (var (direction, index) in _indexByDirection)
        {
            var position = RadialMenuLayout.GetIconPosition(_centerPosition, direction);
            var itemName = GetDisplayedItemName(index);
            var isSelected = (_isUnarmedSelected && index == 3)
                || (!_isUnarmedSelected && index == _selectedIndex);
            _game.Renderer.DrawItemIcon(scale, itemName, position, isSelected);
        }

        // Right: weapon name box
        var infoPos = RadialMenuLayout.GetInfoBoxPosition(_centerPosition);
        infoPos.Y -= 20;
        var selectedName = _isUnarmedSelected
            ? ItemName.Unarmed
            : _currentUnit.Items[_selectedIndex].Name;

        _game.Renderer.DrawEquipWeaponInfoBox(
            scale,
            ItemDatabase.Get(selectedName),
            infoPos
        );

        // Left: stats preview
        var statsPos = new Vector2(_centerPosition.X - 90, _centerPosition.Y - 20);
        _game.Renderer.DrawEquipStatsBox(
            scale,
            GetPreviewAttack(),
            _currentUnit.Defense,
            _currentUnit.Movement,
            _currentUnit.Speed,
            statsPos
        );
    }
}