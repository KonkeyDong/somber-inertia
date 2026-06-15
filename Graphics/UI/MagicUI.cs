using System.Numerics;
using SomberInertia.Core.Units;
using SomberInertia.Enums;
using SomberInertia.State;

namespace SomberInertia.Graphics.UI;

public class MagicUI
{
    public record MagicIconData(Vector2 Position, MagicFamily Family);

    private Vector2 _centerPosition;
    private int _selectedMagicIndex;
    private MagicFamily _selectedMagicFamily;

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

        Reset();
    }

    public void Reset()
    {
        _selectedMagicIndex = -1;
        _selectedMagicFamily = MagicFamily.NoSpell;
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

        var bucket = currentUnit.MagicFamilyBuckets[index];

        if (bucket != null)
        {
            _selectedMagicIndex = index;
            _selectedMagicFamily = (MagicFamily)bucket;

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