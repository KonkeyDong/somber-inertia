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

        if (SelectedIndex == index)
        {
            return;
        }

        var slot = currentUnit.Items[index];

        if (slot.Name != ItemName.NoItem)
        {
            SelectedIndex = index;
            _selectedItemName = slot.Name;

            ItemIcons.SetSelectedItem(_selectedItemName);

            Logger.Debug($"Selected item index: [{index}], name: [{_selectedItemName}].");
        }
    }

    public ItemName GetSelectedItemName()
    {
        return _selectedItemName;
    }

    public ItemSlot GetSelectedSlot(Unit currentUnit)
    {
        if (SelectedIndex < 0 || SelectedIndex >= currentUnit.Items.Length)
        {
            return ItemSlot.Empty;
        }

        return currentUnit.Items[SelectedIndex];
    }

    public ItemData GetSelectedItemData()
    {
        return ItemDatabase.Get(_selectedItemName);
    }

    public IEnumerable<ItemIconData> GetItemIconsToDraw(float scale, Unit currentUnit)
    {
        foreach (var (direction, index) in RadialMenuLayout.IndexByDirection)
        {
            var position = RadialMenuLayout.GetIconPosition(CenterPosition, direction);
            var slot = currentUnit.GetItemAtIndex(index);

            yield return new ItemIconData(position, slot.Name);
        }
    }
}