using System.Numerics;
using Raylib_cs;

namespace SomberInertia.Core.Graphics;

public readonly struct DrawData
{
    public readonly Texture2D Texture;
    public readonly Rectangle Source;
    public readonly Rectangle Destination;
    public readonly Vector2 Origin;
    public readonly float Rotation;
    public readonly Color Tint;

    public DrawData(
        Texture2D texture,
        Rectangle source,
        Rectangle destination,
        Vector2 origin,
        float rotation = 0f,
        Color? tint = null)
    {
        Texture = texture;
        Source = source;
        Destination = destination;
        Origin = origin;
        Rotation = rotation;
        Tint = tint ?? Color.White;
    }
}