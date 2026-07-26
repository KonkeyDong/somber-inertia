using System.Numerics;
using SomberInertia.Core.Units;
using SomberInertia.Enums;
using SomberInertia.Core.Combat.Spells;

namespace SomberInertia.Graphics.UI;

public class MagicUI : RadialSlotUI
{
    public record MagicIconData(Vector2 Position, MagicFamily Family);

    private int _selectedMagicLevel;
    private MagicFamily _selectedMagicFamily;
    private List<MagicName> _selectedMagicList = new();
    private MagicName _selectedMagicName;

    public MagicUI()
    {
        _selectedMagicName = MagicName.NoSpell;
        Reset();
    }

    public override void Reset()
    {
        base.Reset();
        _selectedMagicLevel = 0;
        _selectedMagicFamily = MagicFamily.NoSpell;
        _selectedMagicList = new List<MagicName>();
        _selectedMagicName = MagicName.NoSpell;
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

        var family = currentUnit.MagicFamilyBuckets[index];

        if (family != null)
        {
            SelectedIndex = index;
            _selectedMagicFamily = (MagicFamily)family;
            _selectedMagicList = currentUnit.GetMagicListInBucket(_selectedMagicFamily);
            _selectedMagicName = currentUnit.GetHighestMagicLevelInBucket(_selectedMagicFamily);
            _selectedMagicLevel = Math.Max(0, _selectedMagicList.Count - 1);

            MagicIcons.SetSelectedSpell(_selectedMagicFamily);

            Logger.Debug($"Selected magic index: [{index}], family: [{_selectedMagicFamily}], spell: [{_selectedMagicName}].");
        }
    }

    public MagicFamily GetSelectedFamily()
    {
        return _selectedMagicFamily;
    }

    public MagicName GetSelectedMagicName()
    {
        return _selectedMagicName;
    }

    public MagicData GetSelectedMagicData()
    {
        return MagicDatabase.Get(_selectedMagicName);
    }

    public bool IsSelectedMagicOffensive()
    {
        return GetSelectedMagicData().Offensive;
    }

    public void NextSpellLevel()
    {
        if (_selectedMagicList.Count <= 1)
        {
            return;
        }

        _selectedMagicLevel++;

        if (_selectedMagicLevel >= _selectedMagicList.Count)
        {
            _selectedMagicLevel = 0;
        }

        _selectedMagicName = _selectedMagicList[_selectedMagicLevel];
    }

    public void PreviousSpellLevel()
    {
        if (_selectedMagicList.Count <= 1)
        {
            return;
        }

        _selectedMagicLevel--;

        if (_selectedMagicLevel < 0)
        {
            _selectedMagicLevel = _selectedMagicList.Count - 1;
        }

        _selectedMagicName = _selectedMagicList[_selectedMagicLevel];
    }

    public IEnumerable<MagicIconData> GetMagicIconsToDraw(float scale, Unit currentUnit)
    {
        foreach (var (direction, index) in RadialMenuLayout.IndexByDirection)
        {
            var position = RadialMenuLayout.GetIconPosition(CenterPosition, direction);

            var bucket = currentUnit.MagicFamilyBuckets[index];
            var family = bucket != null ? (MagicFamily)bucket : MagicFamily.NoSpell;

            yield return new MagicIconData(position, family);
        }
    }
}