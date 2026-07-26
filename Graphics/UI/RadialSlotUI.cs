using System.Numerics;
using SomberInertia.Enums;

namespace SomberInertia.Graphics.UI;

public abstract class RadialSlotUI
{
    protected int SelectedIndex = -1;
    protected Vector2 CenterPosition;
    protected Vector2 InfoBoxPosition;

    protected RadialSlotUI()
    {
        CenterPosition = RadialMenuLayout.GetCenterPosition();
        InfoBoxPosition = RadialMenuLayout.GetInfoBoxPosition(CenterPosition);
    }

    public bool HasSelection()
    {
        return SelectedIndex != -1;
    }

    public int GetSelectedIndex()
    {
        return SelectedIndex;
    }

    public Vector2 GetInformationBoxCoordinates()
    {
        return InfoBoxPosition;
    }

    public virtual void Reset()
    {
        SelectedIndex = -1;
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