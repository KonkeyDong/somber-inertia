using System.Numerics;

namespace SomberInertia;

public static class GameConstants
{
    public const int MAX_MOVEMENT_COST = 255;
    public const int BASE_WINDOW_WIDTH = 256; // pixels
    public const int BASE_WINDOW_HEIGHT = 224; // pixels
    public const float BASE_WINDOW_SCALE = 3.0f;

    // Battle Sprite Base Positions
    public static readonly Vector2 BASE_BACKGROUND_POSITION = new Vector2(0, 64);
    public static readonly Vector2 BASE_FOREGROUND_POSITION = new Vector2(127, 150);
    public static readonly Vector2 BASE_UNFRIENDLY_POSITION = new Vector2(50, 80);   // Enemy (left side)
    public static readonly Vector2 BASE_FRIENDLY_POSITION   = new Vector2(165, 100); // Player (right side)

    public static readonly Vector2 BASE_UNFRIENDLY_STATS_POSITION = new Vector2(15, 180);
    public static readonly Vector2 BASE_FRIENDLY_STATS_POSITION   = new Vector2(200, 15);

    public static readonly Vector2 BASE_NO_TARGET_MESSAGE_BOX_POSITION = new Vector2(100, 100);

    // in pixels
    public const int TILE_SIZE = 24;
    public const int WORLD_MAP_SPRITE_SIZE = 24;

    public const float HIGHLIGHT_TRANSITION_SPEED = 1000f; // lower number represents slower speed

    // files and folders
    public const string OVERWORLD_FOLDER_NAME = "Overworld";
    public const string FRAME_DATA_FILE_NAME = "FrameData.json";
    public const string BATTLE_BACKGROUND_FRAME_DATA_FILE_NAME = "BattleBackgroundData.json";

    // can only have 4 items or 4 spell families
    public const int MAX_BUCKET_SIZE = 4;
}