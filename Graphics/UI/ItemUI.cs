using System.Numerics;
using SomberInertia.Core.Units;
using SomberInertia.Enums;
using SomberInertia.Core.Combat.Item;

namespace SomberInertia.Graphics.UI;

public class ItemUI : RadialSlotUI
{
    public record ItemIconData(Vector2 Position, ItemName ItemName, bool IsSelected);

    private ItemName _selectedItemName;

    public ItemUI()
    {
        _selectedItemName = ItemName.NoItem;
        Reset();
    }

    public override void Reset()
    {
        base.Reset();
        _selectedItemName = ItemName.NoItem;
        ItemIcons.ClearSelection();
        ItemIcons.Reset();
    }

    public void SetSelected(Direction direction, Unit currentUnit)
    {
        if (!TryGetIndex(direction, out var index))
        {
            return;
        }

        if (_selectedIndex == index)
        {
            return;
        }

        var slot = currentUnit.Items[index];

        // Empty and Unarmed are not selectable for give/drop/trade flows.
        if (!Unit.IsGiveableSlot(slot))
        {
            return;
        }

        _selectedIndex = index;
        _selectedItemName = slot.Name;
        ItemIcons.Reset(); // restart blink on the newly selected slot

        Logger.Debug($"Selected item index: [{index}], name: [{_selectedItemName}].");
    }

    public ItemName GetSelectedItemName()
    {
        return _selectedItemName;
    }

    public int GetSelectedItemIndex()
    {
        return _selectedIndex;
    }

    public ItemSlot GetSelectedSlot(Unit currentUnit)
    {
        if (_selectedIndex < 0 || _selectedIndex >= currentUnit.Items.Length)
        {
            return ItemSlot.Empty;
        }

        return currentUnit.Items[_selectedIndex];
    }

    public ItemData GetSelectedItemData()
    {
        return ItemDatabase.Get(_selectedItemName);
    }

    public IEnumerable<ItemIconData> GetItemIconsToDraw(float scale, Unit currentUnit)
    {
        foreach (var (direction, index) in RadialMenuLayout.IndexByDirection)
        {
            var position = RadialMenuLayout.GetIconPosition(_centerPosition, direction);
            var slot = currentUnit.GetItemAtIndex(index);
            var isSelected = index == _selectedIndex;

            // Unarmed displays as empty (NoItem) icon
            yield return new ItemIconData(position, Unit.GetDisplayItemName(slot.Name), isSelected);
        }
    }

    // Display-only inventory (e.g. give recipient preview). Never marks a slot selected.
    public IEnumerable<ItemIconData> GetItemIconsToDrawAt(Vector2 center, Unit unit)
    {
        foreach (var (direction, index) in RadialMenuLayout.IndexByDirection)
        {
            var position = RadialMenuLayout.GetIconPosition(center, direction);
            var slot = unit.GetItemAtIndex(index);

            yield return new ItemIconData(position, Unit.GetDisplayItemName(slot.Name), IsSelected: false);
        }
    }

    public bool HasValidSelection()
    {
        return _selectedIndex >= 0 && Unit.IsGiveableItem(_selectedItemName);
    }

    // Select the first giveable inventory slot (Up→Left→Right→Down order).
    public void SelectFirstGiveableItem(Unit unit)
    {
        foreach (var direction in new[] { Direction.Up, Direction.Left, Direction.Right, Direction.Down })
        {
            if (!TryGetIndex(direction, out var index))
            {
                continue;
            }

            if (Unit.IsGiveableSlot(unit.Items[index]))
            {
                _selectedIndex = index;
                _selectedItemName = unit.Items[index].Name;
                ItemIcons.ClearSelection();
                ItemIcons.Reset();
                return;
            }
        }

        Reset();
        _selectedItemName = ItemName.NoItem;
    }

    public bool IsSelectedItemEquipped(Unit unit)
    {
        if (_selectedIndex < 0)
        {
            return false;
        }

        return _selectedIndex == unit.EquippedWeaponIndex;
    }
}
