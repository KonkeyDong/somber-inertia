using System.Numerics;

namespace SomberInertia.Enums;

public static class DirectionExtensions
{
    public static string ToLower(this Direction direction) => direction.ToString().ToLower();

    public static Vector2 GetMenuOffset(this Direction direction)
    {
        return direction switch
        {
            Direction.Up    => new Vector2( 0, -1),
            Direction.Left  => new Vector2(-1,  0),
            Direction.Right => new Vector2( 1,  0),
            Direction.Down  => new Vector2( 0,  1),
            _ => Vector2.Zero
        };
    }
}