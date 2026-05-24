using SomberInertia.Enums;
using SomberInertia.Graphics;

using System.Numerics;
using System.Text.Json;
using Raylib_cs;

namespace SomberInertia.Core.Units;

public abstract class Unit
{
    public class Stat
    {
        public int Current { get; set; }
        public int Max { get; set; }

        public Stat(int max)
        {
            Current = max;
            Max = max;
        }
    }

    public Texture2D Texture { get; protected set; }
    protected abstract string AssetRoot { get; }

    public string Name { get; protected set; }
    public MovementType MovementType { get; protected set; }
    public virtual bool Promoted { get; set; } =  false;

    public Direction FacingDirection { get; set; } = Direction.Down;
    private Dictionary<Direction, List<SpriteV2>> _walkAnimations = new();

    // Core reference - source of truth for position
    protected Block? _block;
    public Block? Block
    {
        get => _block;
        set
        {
            if (_block == value)
            {
                return; // avoid spam on same value
            }

            _block = value;
        }
    }

    // ─────────────────────────────────────────────────────────────
    // Movement Animation (smooth sliding between tiles)
    // ─────────────────────────────────────────────────────────────
    public Vector2 WorldPosition { get; private set; }
    public Vector2 TargetWorldPosition { get; private set; }

    private Vector2 _startWorldPosition; // ← important for correct lerp
    private float _movementTimer;
    private bool _isAnimating;

    public bool IsAnimating => _isAnimating;

    public const float MovementDuration = GameConfig.Animations.MovementDuration;

    // Stats
    public Stat HP { get; set; }
    public Stat MP { get; set; }
    public int Attack { get; set; }
    public Weapon Weapon { get; set; } = null!;
    public int Defense { get; set; }
    public int Speed { get; set; }
    public int Movement { get; protected set; }

    public bool Friendly { get; set; }

    public Unit(string name, MovementType movementType, int movement)
    {
        Name = name;
        MovementType = movementType;

        // Original terrain movement costs could use values like 1.5.
        // Multiply by two to get rid of the decimal.
        Movement = (movement * 2);
        HP = new Stat(10);
        MP = new Stat(10);

        // default for now
        EquipWeapon(WeaponManager.Create(WeaponName.Unarmed));

        LoadWalkAnimations();

        Logger.Info($"Unit created → {Name} ({movementType}), Movement: {movement}");
    }

    public void EquipWeapon(Weapon weapon)
    {
        Logger.Warning("Unit::EquipWeapon(): will need to redesign when items are more incorporated.");

        Weapon = weapon;
    }

    public void TakeDamage(int amount)
    {
        Logger.Debug($"Unit::TakeDamage({amount})");
        Logger.Info($"Unit [{Name}] has been damaged for {amount}.");

        HP.Current = HP.Current - amount;
        if (HP.Current < 0)
        {
            HP.Current = 0;
        }
        Logger.Info($"\tUnit's current health: {HP.Current} / {HP.Max}.");
    }

    public override string ToString() => $"{Name} ({MovementType}) at {Block?.PrintGridCoordinates() ?? "[null]"}";

    // -----------------
    // Animation methods
    // -----------------
    public void ResetStartingWorldPosition() 
    {
        if (Block == null)
        {
            Logger.Error("Block is null.");
        }

        WorldPosition = Block.GetPixelCoordinates();
    }

    public void StartMovingTo(Block targetBlock)
    {
        if (targetBlock == null) 
        {
            return;
        }

        _startWorldPosition = WorldPosition; // save where we are now
        TargetWorldPosition = targetBlock.GetPixelCoordinates();
        _movementTimer = 0f;
        _isAnimating = true;
    }

    public void SnapToCurrentBlock()
    {
        if (Block == null)
        {
            Logger.Error("Cannot snap unit - Block is null.");
            return;
        }

        var pos = Block.GetPixelCoordinates();

        WorldPosition = pos;
        TargetWorldPosition = pos;
        _startWorldPosition = pos;
    }

    public void UpdateMovement(float deltaTime)
    {
        if (!_isAnimating) 
        {
            return;
        }

        _movementTimer += deltaTime;

        var progress = Math.Clamp(_movementTimer / MovementDuration, 0f, 1f);

        WorldPosition = Vector2.Lerp(_startWorldPosition, TargetWorldPosition, progress);

        if (progress >= 1.0f)
        {
            StopMovement();
        }
    }

    public void StopMovement()
    {
        WorldPosition = TargetWorldPosition;
        _isAnimating = false;
        _movementTimer = 0f;
    }

    public void ResetFacingDirection() => FacingDirection = Direction.Down;

    public SpriteV2 GetFacingDirectionTexture(bool frameFlipperFlag)
    {
        if (!_walkAnimations.Any())
        {
            Logger.Error("no walk animations set.");
        }

        var animations = _walkAnimations[FacingDirection];
        var index = frameFlipperFlag == false ? 0 : 1;

        return animations[index];
    }

    // ---------------------------
    // Read Spritesheet Frame Data
    // ---------------------------
    public void LoadWalkAnimations()
    {
        _walkAnimations.Clear();

        var totalFramesLoaded = 0;

        foreach (var direction in Enum.GetValues<Direction>())
        {
            _walkAnimations[direction] = new List<SpriteV2>();

            var basePath = $"{AssetRoot}/Overworld/walk_{direction.ToLower()}";
            var jsonPath = Path.Combine(basePath + ".json");
            var pngPath  = Path.Combine(basePath + ".png");

            var frames = ExtractFrameData(jsonPath);

            foreach (var frame in frames)
            {
                _walkAnimations[direction].Add(new SpriteV2(pngPath, frame));
                totalFramesLoaded++;
            }
        }

        Logger.Info($"LoadWalkAnimations completed. Loaded {totalFramesLoaded} frames across 4 directions.");
    }

    private List<FrameRect> ExtractFrameData(string jsonFilePath)
    {
        if (string.IsNullOrWhiteSpace(jsonFilePath))
        {
            Logger.Error("ExtractFrameData: jsonFilePath is empty or null.");
            return new List<FrameRect>();
        }

        try
        {
            var jsonText = File.ReadAllText(jsonFilePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var sheet = JsonSerializer.Deserialize<AsepriteSheet>(jsonText, options);

            if (sheet?.frames == null || sheet.frames.Count == 0)
            {
                Logger.Warning($"No frames found in JSON: {jsonFilePath}");
                return new List<FrameRect>();
            }

            return sheet.frames
                        .Where(entry => entry?.frame != null)
                        .Select(entry => entry.frame)
                        .ToList();
        }
        catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
        {
            Logger.Error($"JSON file not found: {jsonFilePath}");

            return new List<FrameRect>();
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to load/parse JSON {jsonFilePath}: {ex.Message}");

            return new List<FrameRect>();
        }
    }
}