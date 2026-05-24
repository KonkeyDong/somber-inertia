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

    public void SetPosition(Block block) => Block = block;

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

    public void ResetFacingDirection() => FacingDirection = Direction.Down;

    public override string ToString() => $"{Name} ({MovementType}) at {Block?.PrintGridCoordinates() ?? "[null]"}";

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