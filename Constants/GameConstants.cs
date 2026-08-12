using SomberInertia.Core.Units;
using SomberInertia.Enums;

using System.Numerics;
using Raylib_cs;

namespace SomberInertia;

public static class GameConstants
{
    // in pixels
    public const int TileSize = 24;
    public const int WorldMapSpriteSize = 24;
    
    // can only have 4 items or 4 spell families
    public const int MaxBucketSize = 4;

    public static class Debug
    {
        public static readonly Color Color = Color.Yellow;
        public const int FontSize = 16;
        public const int Spacing = 1;

        /// <summary>Logical-pixel spacing for battle debug grid (screen step = value * scale).</summary>
        public const int BattleGridLogicalSpacing = 20;
        public static readonly Color BattleGridColor = new Color(180, 40, 40, 255);
    }

    public static class Window
    {
        public const int Width = 256; // pixels
        public const int Height = 224; // pixels
        public const float Scale = 3.0f;
    }

    /// <summary>Fixed file names used under asset folders.</summary>
    public static class Files
    {
        public const string FrameData = "FrameData.json";
        public const string IdleJson = "Idle.json";
        public const string IdlePng = "Idle.png";
        public const string AttackJson = "Attack.json";
        public const string AttackPng = "Attack.png";
        public const string GrassTile = "grass_tile.png";
        public const string ForestTile = "forest_tile.png";
        public const string PngExtension = ".png";
        public const string JsonExtension = ".json";
    }

    /// <summary>
    /// Asset roots and path segments. Full roots for fixed folders; segments for dynamic Path.Combine.
    /// </summary>
    public static class Paths
    {
        public const string Root = "Assets";

        public const string Backgrounds = "Assets/Backgrounds";
        public const string Foregrounds = "Assets/Foregrounds";

        public const string Sprites = "Assets/Sprites";
        public const string Characters = "Assets/Sprites/Characters";
        public const string Monsters = "Assets/Sprites/Monsters";
        public const string Shared = "Assets/Sprites/Shared";

        public const string CommandIcons = "Assets/Sprites/Shared/CommandIcons";
        public const string ItemIcons = "Assets/Sprites/Shared/ItemIcons";
        public const string MagicIcons = "Assets/Sprites/Shared/MagicIcons";

        public const string BattleFieldDeath = "Assets/Sprites/Shared/BattleFieldDeath";

        public const string GrassTile = "Assets/grass_tile.png";
        public const string ForestTile = "Assets/forest_tile.png";

        // Folder name segments (dynamic paths)
        public const string Overworld = "Overworld";
        public const string Battle = "Battle";
        public const string Promoted = "Promoted";
        public const string Unpromoted = "Unpromoted";

        /// <summary>Folder segment under a force member: Promoted or Unpromoted.</summary>
        public static string PromotionFolder(bool promoted) =>
            promoted ? Promoted : Unpromoted;
    }

    public static class Animations
    {
        public const float HighlightTransitionSpeed = 200f; // lower number represents slower speed

        public const int RangeTintFrameDelay = 6;
        public const int CountdownTimerDelay = 60;
        public const float MovementDuration = 0.20f; // 0.25 = quarter second (15 frames)
        public const int FlipFlopDelay = 30;
        public const int BlinkDelay = 7;
        public const int IdleDelay = 10;
        public const int AttackDelay = 10; // frames
        public const int JitterOffset = 3; // pixels
        public const int SwitchStateCountdownTimer = 180;

        public static readonly Color[] RangeTintLevels =
        {
            new Color(255, 255, 255, 255), // 0: full bright
            new Color(200, 220, 255, 200), // 1: light blue
            new Color(140, 180, 255, 180), // 2: medium blue
            new Color(80,  120, 255, 160)  // 3: strong blue
        };

        public static class Dissolve
        {
            public const int GroupSize = 6;
            public const int NumberOfFrameCopies = 3;
        }
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
        public const int PoisonDamageDenominator = 8; // max HP / PoisonDamageDenominator
        public const int SleepDuration = 3; // 1 to 3 turns (turn 0 shows wake message and skips turn)
    }

    public static class Items
    {
        public const int BreakChance = 8;
    }

    public static class WorldMap
    {
        public static class Positions
        {
            public static readonly Vector2 NoTargetMessageBox = new Vector2(100, 100);
        }

        public const int MaxMovementCost = 255;
    }

    public static class MessageNotice
    {
        public const string NoMagic = "No magic";
        public const string NoItem = "No Item";
        public const string NoTarget = "No target";
    }

    public static class Give
    {
        public static class Positions
        {
            // Bottom-right unit info when selecting a give recipient
            public static readonly Vector2 RecipientInfoBox = new Vector2(200, 160);

            // Recipient inventory radial sits above the info box
            public static readonly Vector2 RecipientInventoryCenter = new Vector2(200, 110);

            // Trade UI uses lower-center radial for the neighbor's inventory
            public static readonly Vector2 TradeInventoryCenter = new Vector2(
                Window.Width / 2f,
                Window.Height * 0.75f
            );

            // Give/swap summary panel on PromptYesNo (logical coords)
            public static readonly Vector2 TradePromptBox = new Vector2(72, 70);
        }

        /// <summary>Yes/No vertical center when trade summary panel is shown.</summary>
        public const float TradePromptYesNoYFactor = 0.82f;

        public const float TradePromptColumnGap = 24f;
        public const float TradePromptNameToIconGap = 6f;
    }

    public static class Battle
    {
        public static class Positions
        {
            public static readonly Vector2 Background = new Vector2(0, 64);
            public static readonly Vector2 Foreground = new Vector2(127, 150);

            public static readonly Vector2 UnfriendlyStats = new Vector2(15, 180);
            public static readonly Vector2 FriendlyStats   = new Vector2(200, 15);
        }

        private static readonly Dictionary<string, Vector2> _spritePositions = new()
        {
            // Force Members
            // Max
            { $"{UnitName.Max.GetBaseName()}_unpromoted_{ItemName.ShortSword.GetBaseName()}", new Vector2(165, 100) },
            { $"{UnitName.Max.GetBaseName()}_unpromoted_{ItemName.Unarmed.GetBaseName()}", new Vector2(145, 90) },
            { $"{UnitName.Max.GetBaseName()}_promoted_{ItemName.LongSword.GetBaseName()}", new Vector2(105, 80) },

            // Anri
            { $"{UnitName.Anri.GetBaseName()}_unpromoted_{ItemName.Unarmed.GetBaseName()}", new Vector2(150, 85) },
            { $"{UnitName.Anri.GetBaseName()}_unpromoted_{ItemName.WoodenStaff.GetBaseName()}", new Vector2(144, 72) },
            { $"{UnitName.Anri.GetBaseName()}_unpromoted_{ItemName.PowerStaff.GetBaseName()}", new Vector2(134, 65) },

            // Tao
            { $"{UnitName.Tao.GetBaseName()}_unpromoted_{ItemName.Unarmed.GetBaseName()}", new Vector2(150, 85) },
            { $"{UnitName.Tao.GetBaseName()}_unpromoted_{ItemName.WoodenStaff.GetBaseName()}", new Vector2(144, 72) },
            { $"{UnitName.Tao.GetBaseName()}_unpromoted_{ItemName.PowerStaff.GetBaseName()}", new Vector2(134, 65) },

            // Monsters
            { $"{UnitName.Goblin.GetBaseName()}_unpromoted_{ItemName.Unarmed.GetBaseName()}", new Vector2(50, 75)},
            { $"{UnitName.DarkDwarf.GetBaseName()}_unpromoted_{ItemName.Unarmed.GetBaseName()}", new Vector2(40, 50)},
            { $"{UnitName.RuneKnight.GetBaseName()}_unpromoted_{ItemName.Unarmed.GetBaseName()}", new Vector2(50, 75)}
        };

        public static Vector2 GetSpritePosition(Unit unit)
        {
            var key = BuildKey(unit);

            if (_spritePositions.TryGetValue(key, out var position))
            {
                return position;
            }

            Logger.Error($"No base position defined for {key}. Aborting...");
            return new Vector2(100, 100);
        }

        public static string BuildKey(Unit unit)
        {
            var weaponName = unit.GetEquippedWeaponName();
            var promoted = unit.Promoted ? "promoted" : "unpromoted";
            Logger.Debug(unit.Name.GetBaseName());
            Logger.Debug(weaponName.ToString());

            return $"{unit.Name.GetBaseName()}_{promoted}_{weaponName.GetBaseName()}";
        }
    }
}