using System.Numerics;
using SomberInertia.Core.Units;
using SomberInertia.Enums;
using SomberInertia.Core.Combat.Item;

namespace SomberInertia.Graphics.UI;

public class ItemUI : RadialSlotUI
{
    public record ItemIconData(Vector2 Position, ItemName ItemName);

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

        if (slot.Name != ItemName.NoItem)
        {
            _selectedIndex = index;
            _selectedItemName = slot.Name;

            ItemIcons.SetSelectedItem(_selectedItemName);

            Logger.Debug($"Selected item index: [{index}], name: [{_selectedItemName}].");
        }
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

            yield return new ItemIconData(position, slot.Name);
        }
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