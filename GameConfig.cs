using System.Numerics;

using Raylib_cs;

namespace SomberInertia;

public static class GameConfig
{
    public static class Animations
    {
        public const float HighlightTransitionSpeed = 1000f; // lower number represents slower speed
        public const int MovementRangeTintFrameDelay = 6;
        public const int CountdownTimerDelay = 60;
    }

    public static class Textures
    {
        public static readonly Vector2 BaseOrigin = new Vector2(0, 0);
        public const float BaseRotation = 0.0f;
        public static readonly Color ClearColor = Color.White;
    }
}