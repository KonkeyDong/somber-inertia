using System.Numerics;
using SomberInertia.Core.Graphics;
using SomberInertia.Enums;
using SomberInertia.State;

namespace SomberInertia.Graphics.UI;

public static class RadialMenuLayout
{
    public static readonly Dictionary<Direction, int> IndexByDirection = new()
    {
        { Direction.Up,    0 },
        { Direction.Left,  1 },
        { Direction.Right, 2 },
        { Direction.Down,  3 }
    };

    public static Vector2 GetCenterPosition()
    {
        return new Vector2(
            GameStateManager.CurrentWidth / 2f,
            GameStateManager.CurrentHeight * 0.75f
        ) / GameStateManager.CurrentScale;
    }

    public static Vector2 GetInfoBoxPosition(Vector2 center)
    {
        return new Vector2(center.X + 65, center.Y);
    }

    public static Vector2 GetMenuMessagePosition(Vector2 center)
    {
        return new Vector2(center.X + 65, center.Y + 18);
    }

    public static Vector2 GetIconPosition(Vector2 center, Direction direction)
    {
        var offset = direction.GetMenuOffset();
        return center + offset * GameConstants.TileSize;
    }

    /// <summary>Draw command icons in the standard radial layout.</summary>
    public static void DrawCommandIcons(
        Renderer renderer,
        float scale,
        Vector2 center,
        IReadOnlyDictionary<Direction, CommandIconType> commands)
    {
        foreach (var (direction, commandType) in commands)
        {
            var position = GetIconPosition(center, direction);
            renderer.Draw(scale, CommandIcons.GetSprite(commandType), position);
        }
    }
}
