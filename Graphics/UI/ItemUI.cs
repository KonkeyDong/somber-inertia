using System.Numerics;
using SomberInertia.Core.Units;
using SomberInertia.Enums;
using SomberInertia.Core.Combat.Item;

namespace SomberInertia.Graphics.UI;

/// <summary>
/// Predicate for which inventory slots may be selected (and optionally shown) in item radials.
/// </summary>
public delegate bool ItemSlotFilter(ItemSlot itemSlot, Unit unit);

public class ItemUI : RadialSlotUI
{
    public record ItemIconData(Vector2 Position, ItemName ItemName, bool IsSelected);

    /// <summary>Give / drop / trade: any non-empty, non-Unarmed item.</summary>
    public static bool GiveableFilter(ItemSlot itemSlot, Unit unit) =>
        Unit.IsGiveableItemSlot(itemSlot);

    /// <summary>Use: job-allowed items with an effect or castable spell.</summary>
    public static bool UsableFilter(ItemSlot itemSlot, Unit unit) =>
        Unit.IsUsableItemSlot(itemSlot, unit.GetJob());

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

    public void SetSelected(Direction direction, Unit currentUnit, ItemSlotFilter canSelect)
    {
        if (!TryGetIndex(direction, out var index))
        {
            return;
        }

        if (_selectedIndex == index)
        {
            return;
        }

        var itemSlot = currentUnit.Items[index];
        if (!canSelect(itemSlot, currentUnit))
        {
            return;
        }

        _selectedIndex = index;
        _selectedItemName = itemSlot.Name;
        ItemIcons.Reset();

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

    public ItemSlot GetSelectedItemSlot(Unit currentUnit)
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

    /// <summary>
    /// Radial icons. If <paramref name="blankDisallowed"/> is true, slots failing
    /// <paramref name="canSelect"/> are drawn as empty (Use menu).
    /// </summary>
    public IEnumerable<ItemIconData> GetItemIconsToDraw(
        Unit currentUnit,
        ItemSlotFilter? canSelect = null,
        bool blankDisallowed = false)
    {
        foreach (var (direction, index) in RadialMenuLayout.IndexByDirection)
        {
            var position = RadialMenuLayout.GetIconPosition(_centerPosition, direction);
            var itemSlot = currentUnit.GetItemAtIndex(index);
            var isSelected = index == _selectedIndex;

            ItemName displayName;
            if (blankDisallowed && canSelect != null && !canSelect(itemSlot, currentUnit))
            {
                displayName = ItemName.NoItem;
            }
            else
            {
                displayName = Unit.GetDisplayItemName(itemSlot.Name);
            }

            yield return new ItemIconData(position, displayName, isSelected);
        }
    }

    /// <summary>
    /// Display-only inventory (e.g. give recipient preview). Never marks a slot selected.
    /// </summary>
    public IEnumerable<ItemIconData> GetItemIconsToDrawAt(Vector2 center, Unit unit)
    {
        foreach (var (direction, index) in RadialMenuLayout.IndexByDirection)
        {
            var position = RadialMenuLayout.GetIconPosition(center, direction);
            var itemSlot = unit.GetItemAtIndex(index);

            yield return new ItemIconData(position, Unit.GetDisplayItemName(itemSlot.Name), IsSelected: false);
        }
    }

    public bool HasValidSelection(Unit unit, ItemSlotFilter canSelect)
    {
        if (_selectedIndex < 0 || _selectedIndex >= unit.Items.Length)
        {
            return false;
        }

        return canSelect(unit.Items[_selectedIndex], unit);
    }

    /// <summary>Select the first matching inventory slot (Up→Left→Right→Down order).</summary>
    public void SelectFirstItem(Unit unit, ItemSlotFilter canSelect)
    {
        foreach (var direction in new[] { Direction.Up, Direction.Left, Direction.Right, Direction.Down })
        {
            if (!TryGetIndex(direction, out var index))
            {
                continue;
            }

            if (canSelect(unit.Items[index], unit))
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
