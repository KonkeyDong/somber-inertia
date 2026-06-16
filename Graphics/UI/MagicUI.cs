using System.Numerics;
using SomberInertia.Core.Units;
using SomberInertia.Enums;
using SomberInertia.State;
using SomberInertia.Core.Combat;

namespace SomberInertia.Graphics.UI;

public class MagicUI
{
    public record MagicIconData(Vector2 Position, MagicFamily Family);

    private Vector2 _centerPosition;
    private Vector2 _magicInformationBoxCoordinates;
    private int _selectedMagicIndex;
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
        _selectedMagicIndex = -1;
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

        if (_selectedMagicIndex == index)
        {
            return;
        }

        var family = currentUnit.MagicFamilyBuckets[index];

        if (family != null)
        {
            _selectedMagicIndex = index;
            _selectedMagicFamily = (MagicFamily)family;
            _selectedMagicList = currentUnit.GetMagicListInBucket(_selectedMagicFamily);
            _selectedMagic = currentUnit.GetHighestMagicLevelInBucket(_selectedMagicFamily);

            // for setting the red border
            MagicIcons.SetSelectedSpell(_selectedMagicFamily);

            Logger.Debug($"Selected magic index: [{index}].");
        }
    }

    public int GetSelectedIndex()
    {
        return _selectedMagicIndex;
    }

    public MagicFamily GetSelectedFamily()
    {
        return _selectedMagicFamily;
    }

    public bool HasSelection()
    {
        return _selectedMagicIndex != -1;
    }

    public Magic GetSelectedMagic()
    {
        return _selectedMagic;
    }

    public Vector2 GetMagicInformationBoxCoordinates()
    {
        return _magicInformationBoxCoordinates;
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