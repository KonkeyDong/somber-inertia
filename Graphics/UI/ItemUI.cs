using System.Numerics;
using SomberInertia.Core.Units;
using SomberInertia.Enums;
using SomberInertia.State;
using SomberInertia.Core.Combat.Item;

namespace SomberInertia.Graphics.UI;

public class ItemUI
{
    public record ItemIconData(Vector2 Position, Item Item);

    private Vector2 _centerPosition;
    private Vector2 _itemInformationBoxCoordinates;
    private int _selectedItemIconIndex;
    private Item _selectedItem;

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
        _selectedItem = ItemManager.Create(ItemName.NoItem);

        Reset();
    }

    public void Reset()
    {
        _selectedItemIconIndex = -1;
        _selectedItem = ItemManager.Create(ItemName.NoItem);
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

        var family = currentUnit.Items[index];

        if (family != null)
        {
            _selectedItemIconIndex = index;
            _selectedItem = currentUnit.GetItemAtIndex(index);

            // for setting the red border
            ItemIcons.SetSelectedItem(_selectedItem.Name);

            Logger.Debug($"Selected item index: [{index}].");
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

    public Item GetSelectedItem()
    {
        return _selectedItem;
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

            var item = currentUnit.GetItemAtIndex(index);

            yield return new ItemIconData(position, item);
        }
    }
}