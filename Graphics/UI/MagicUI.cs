using System.Numerics;
using SomberInertia.Core.Units;
using SomberInertia.Enums;
using SomberInertia.State;
using SomberInertia.Core.Combat.Spells;

namespace SomberInertia.Graphics.UI;

public class MagicUI
{
    public record MagicIconData(Vector2 Position, MagicFamily Family);

    private Vector2 _centerPosition;
    private Vector2 _magicInformationBoxCoordinates;
    private int _selectedMagicIconIndex;
    private int _selectedMagicLevel;
    private MagicFamily _selectedMagicFamily;
    private List<Magic> _selectedMagicList = new();
    private Magic _selectedMagic;

    private readonly Dictionary<Direction, int> _spellIndexByDirection = new()
    {
        { Direction.Up,    0 },
        { Direction.Left,  1 },
        { Direction.Right, 2 },
        { Direction.Down,  3 }
    };

    public MagicUI()
    {
        _centerPosition = new Vector2(
            GameStateManager.CurrentWidth / 2f,
            GameStateManager.CurrentHeight * 0.75f
        );

        _magicInformationBoxCoordinates = new Vector2(_centerPosition.X + 200, _centerPosition.Y);
        _selectedMagic = MagicManager.Create(MagicName.NoSpell);

        Reset();
    }

    public void Reset()
    {
        _selectedMagicIconIndex = -1;
        _selectedMagicLevel = 0;
        _selectedMagicFamily = MagicFamily.NoSpell;
        _selectedMagicList = new();
        _selectedMagic = MagicManager.Create(MagicName.NoSpell);
    }

    public void SetSelected(Direction direction, Unit currentUnit)
    {
        if (!_spellIndexByDirection.TryGetValue(direction, out var index))
        {
            Logger.Error($"Direction [{direction}] not found in dictionary.");
            return;
        }

        if (_selectedMagicIconIndex == index)
        {
            return;
        }

        var family = currentUnit.MagicFamilyBuckets[index];

        if (family != null)
        {
            _selectedMagicIconIndex = index;
            _selectedMagicFamily = (MagicFamily)family;
            _selectedMagicList = currentUnit.GetMagicListInBucket(_selectedMagicFamily);
            _selectedMagic = currentUnit.GetHighestMagicLevelInBucket(_selectedMagicFamily);
            _selectedMagicLevel = _selectedMagicList.Count - 1;

            // for setting the red border
            MagicIcons.SetSelectedSpell(_selectedMagicFamily);

            Logger.Debug($"Selected magic index: [{index}].");
        }
    }

    public int GetSelectedIndex()
    {
        return _selectedMagicIconIndex;
    }

    public MagicFamily GetSelectedFamily()
    {
        return _selectedMagicFamily;
    }

    public bool HasSelection()
    {
        return _selectedMagicIconIndex != -1;
    }

    public Magic GetSelectedMagic()
    {
        return _selectedMagic;
    }

    public Vector2 GetMagicInformationBoxCoordinates()
    {
        return _magicInformationBoxCoordinates;
    }

    public bool IsSelectedMagicOffensive()
    {
        return _selectedMagic.Offensive;
    }

    public void NextSpellLevel()
    {
        if (_selectedMagicList.Count == 1)
        {
            return;
        }

        _selectedMagicLevel++;

        if (_selectedMagicLevel >= _selectedMagicList.Count)
        {
            _selectedMagicLevel = 0;
        }

        _selectedMagic = _selectedMagicList[_selectedMagicLevel];
    }

    public void PreviousSpellLevel()
    {
        if (_selectedMagicList.Count == 1)
        {
            return;
        }

        _selectedMagicLevel--;

        if (_selectedMagicLevel < 0)
        {
            _selectedMagicLevel = _selectedMagicList.Count - 1;
        }

        _selectedMagic = _selectedMagicList[_selectedMagicLevel];
    }

    public IEnumerable<MagicIconData> GetMagicIconsToDraw(float scale, Unit currentUnit)
    {
        foreach (var (direction, index) in _spellIndexByDirection)
        {
            var offset = direction.GetMenuOffset();
            var position = _centerPosition + offset * (GameConstants.TILE_SIZE * scale);

            var bucket = currentUnit.MagicFamilyBuckets[index];
            var family = bucket != null ? (MagicFamily)bucket : MagicFamily.NoSpell;

            yield return new MagicIconData(position, family);
        }
    }
}