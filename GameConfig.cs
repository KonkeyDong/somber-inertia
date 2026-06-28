using System.Numerics;

using Raylib_cs;

namespace SomberInertia;

public static class GameConfig
{
    public static class Animations
    {
        public const float HighlightTransitionSpeed = 1000f; // lower number represents slower speed
        public const int RangeTintFrameDelay = 6;
        public const int CountdownTimerDelay = 60;
        public const float MovementDuration = 0.20f; // 0.25 = quarter second (15 frames)
        public const int FrameFlipperDelay = 30;
        public const int BlinkDelay = 7;
        public const int SwitchStateCountdownTimer = 180;
    }

    public static class Textures
    {
        public static readonly Vector2 BaseOrigin = new Vector2(0, 0);
        public const float BaseRotation = 0.0f;
        public static readonly Color ClearColor = Color.White;
        public static readonly Color Blue = new Color(38, 74, 220, 255);
        public static readonly Color DarkOrange = new Color(177, 82, 24, 255);
        public static readonly Color LightOrange = new Color(255, 203, 94, 255);
        public static readonly Color OffWhite = new Color(248, 235, 244, 255);
        public static readonly Color DarkRed = new Color(180, 40, 40, 255);
    }

    public static class StatusEffects
    {
        public const int POISON_DAMAGE_DENOMINATOR = 8; // max HP / POISON_DAMAGE_DENOMINATOR
        public const int SLEEP_DURATION = 3; // _random.Next(GameConstants.SLEEP_DURATION); // 1 to 3 (technically 0 to 2) turns (turn 0 shows the awak message and skips turn)
    }
}