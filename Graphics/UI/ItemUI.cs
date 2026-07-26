using System.Numerics;
using SomberInertia.Core.Units;
using SomberInertia.Enums;
using SomberInertia.State;
using SomberInertia.Core.Combat.Item;

namespace SomberInertia.Graphics.UI;

public class ItemUI
{
    public record ItemIconData(Vector2 Position, ItemName ItemName);

    private Vector2 _centerPosition;
    private Vector2 _itemInformationBoxCoordinates;
    private int _selectedItemIconIndex;
    private ItemName _selectedItemName;

    private readonly Dictionary<Direction, int> _itemIndexByDirection = new()
    {
        { Direction.Up,    0 },
        { Direction.Left,  1 },
        { Direction.Right, 2 },
        { Direction.Down,  3 }
    };

    public ItemUI()
    {
        _centerPosition = new Vector2(
            GameStateManager.CurrentWidth / 2f,
            GameStateManager.CurrentHeight * 0.75f
        ) / GameStateManager.CurrentScale;

        _itemInformationBoxCoordinates = new Vector2(_centerPosition.X + 65, _centerPosition.Y);
        _selectedItemName = ItemName.NoItem;

        Reset();
    }

    public void Reset()
    {
        _selectedItemIconIndex = -1;
        _selectedItemName = ItemName.NoItem;
    }

    public void SetSelected(Direction direction, Unit currentUnit)
    {
        if (!_itemIndexByDirection.TryGetValue(direction, out var index))
        {
            Logger.Error($"Direction [{direction}] not found in dictionary.");
            return;
        }

        if (_selectedItemIconIndex == index)
        {
            return;
        }

        var slot = currentUnit.Items[index];

        if (slot.Name != ItemName.NoItem)
        {
            _selectedItemIconIndex = index;
            _selectedItemName = slot.Name;

            ItemIcons.SetSelectedItem(_selectedItemName);

            Logger.Debug($"Selected item index: [{index}], name: [{_selectedItemName}].");
        }
    }

    public int GetSelectedIndex()
    {
        return _selectedItemIconIndex;
    }

    public bool HasSelection()
    {
        return _selectedItemIconIndex != -1;
    }

    public ItemName GetSelectedItemName()
    {
        return _selectedItemName;
    }

    public ItemSlot GetSelectedSlot(Unit currentUnit)
    {
        if (_selectedItemIconIndex < 0 || _selectedItemIconIndex >= currentUnit.Items.Length)
        {
            return ItemSlot.Empty;
        }

        return currentUnit.Items[_selectedItemIconIndex];
    }

    public ItemData GetSelectedItemData()
    {
        return ItemDatabase.Get(_selectedItemName);
    }

    public Vector2 GetItemInformationBoxCoordinates()
    {
        return _itemInformationBoxCoordinates;
    }

    public IEnumerable<ItemIconData> GetItemIconsToDraw(float scale, Unit currentUnit)
    {
        foreach (var (direction, index) in _itemIndexByDirection)
        {
            var offset = direction.GetMenuOffset();
            var position = _centerPosition + offset * GameConstants.TILE_SIZE;

            var slot = currentUnit.GetItemAtIndex(index);

            yield return new ItemIconData(position, slot.Name);
        }
    }
}