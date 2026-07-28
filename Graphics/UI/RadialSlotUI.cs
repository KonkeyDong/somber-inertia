using System.Numerics;
using SomberInertia.Enums;

namespace SomberInertia.Graphics.UI;

public abstract class RadialSlotUI
{
    protected int _selectedIndex = -1;
    protected Vector2 _centerPosition;
    protected Vector2 _infoBoxPosition;

    protected RadialSlotUI()
    {
        _centerPosition = RadialMenuLayout.GetCenterPosition();
        _infoBoxPosition = RadialMenuLayout.GetInfoBoxPosition(_centerPosition);
    }

    public bool HasSelection()
    {
        return _selectedIndex != -1;
    }

    public int GetSelectedIndex()
    {
        return _selectedIndex;
    }

    public Vector2 GetInformationBoxCoordinates()
    {
        return _infoBoxPosition;
    }

    public virtual void Reset()
    {
        _selectedIndex = -1;
    }

    protected bool TryGetIndex(Direction direction, out int index)
    {
        if (!RadialMenuLayout.IndexByDirection.TryGetValue(direction, out index))
        {
            Logger.Error($"Direction [{direction}] not found in radial menu layout.");
            return false;
        }

        return true;
    }
}
