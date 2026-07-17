using SomberInertia.Core.Units;
using SomberInertia.Enums;

using System.Numerics;
using Raylib_cs;

namespace SomberInertia;

public static class GameConstants
{
    public static class Window
    {
        public const int BASE_WINDOW_WIDTH = 256; // pixels
        public const int BASE_WINDOW_HEIGHT = 224; // pixels
        public const float BASE_WINDOW_SCALE = 3.0f;
    }
    
    public static readonly Vector2 BASE_UNFRIENDLY_POSITION = new Vector2(50, 80);   // Enemy (left side)
    public static readonly Vector2 BASE_FRIENDLY_POSITION   = new Vector2(165, 100); // Player (right side)

    // in pixels
    public const int TILE_SIZE = 24;
    public const int WORLD_MAP_SPRITE_SIZE = 24;

    public static class Files
    {
        public const string FRAME_DATA_FILE_NAME = "FrameData.json";
        public const string BATTLE_BACKGROUND_FRAME_DATA_FILE_NAME = "BattleBackgroundData.json";
    }

    public static class Folders
    {
        public const string OVERWORLD_FOLDER_NAME = "Overworld";
    }

    // can only have 4 items or 4 spell families
    public const int MAX_BUCKET_SIZE = 4;

    public static class Animations
    {
        public const float HighlightTransitionSpeed = 1000f; // lower number represents slower speed

        public const int RangeTintFrameDelay = 6;
        public const int CountdownTimerDelay = 60;
        public const float MovementDuration = 0.20f; // 0.25 = quarter second (15 frames)
        public const int FrameFlipperDelay = 30;
        public const int BlinkDelay = 7;
        public const int IdleDelay = 10;
        public const int AttackDelay = 10; // frames
        public const int JitterOffset = 3; // pixels
        public const int SwitchStateCountdownTimer = 180;
    }

    public static class Textures
    {
        public static readonly Vector2 BaseOrigin = new Vector2(0, 0);
        public const float BaseRotation = 0.0f;
        public static readonly Color ClearColor = new Color(255, 255, 255, 255);
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

    public static class WorldMap
    {
        public static class Positions
        {
            public static readonly Vector2 NoTargetMessageBox = new Vector2(100, 100);
        }

        public const int MAX_MOVEMENT_COST = 255;
    }

    public static class Battle
    {
        public static class Positions
        {
            public static readonly Vector2 BASE_BACKGROUND_POSITION = new Vector2(0, 64);
            public static readonly Vector2 BASE_FOREGROUND_POSITION = new Vector2(127, 150);

            public static readonly Vector2 UnfriendlyStats = new Vector2(15, 180);
            public static readonly Vector2 FriendlyStats   = new Vector2(200, 15);
        }

        private static readonly Dictionary<string, Vector2> SpritePositions = new()
        {
            { $"{UnitName.Max.GetBaseName()}_{WeaponName.ShortSword.GetBaseName()}", new Vector2(165, 100) },
            { $"{UnitName.Anri.GetBaseName()}_{WeaponName.Unarmed.GetBaseName()}", new Vector2(150, 85) },
            // { "Anri_Staff", new Vector2(300, 90) },
            // Add more as needed
        };

        public static Vector2 GetSpritePosition(Unit unit)
        {
            Logger.Warning("Will need to add other characters/monsters base positions and adjust in battle states.");
            var key = BuildKey(unit);

            if (SpritePositions.TryGetValue(key, out var position))
            {
                return position;
            }
            else
            {
                Logger.Warning($"No base position defined for {key}. Using default.");
                return new Vector2(100, 100); // fallback
            }
        }

        public static string BuildKey(Unit unit)
        {
            var weaponName = unit.Weapon.Name.GetBaseName() ?? "None";

            return $"{unit.Name.GetBaseName()}_{weaponName}";
        }
    }
}