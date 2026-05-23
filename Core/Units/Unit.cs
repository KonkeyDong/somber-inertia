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

    public Unit(string texturePath, string name, MovementType movementType, int movement)
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

    public override string ToString() => $"{Name} ({MovementType}) at {Block?.PrintGridCoordinates() ?? "[null]"}";

    public SpriteV2 GetFacingDirectionTexture()
    {
        if (!_walkAnimations.Any())
        {
            Logger.Error("no walk animations set.");
        }

        return _walkAnimations[FacingDirection][0];
    }

    public void LoadWalkAnimations()
    {
        var directions = new Direction[] 
        {
            Direction.Up,
            Direction.Right,
            Direction.Down,
            Direction.Left,
        };

        var count = 0;
        foreach (var direction in directions)
        {
            _walkAnimations[direction] = new List<SpriteV2>();

            var path = $"{AssetRoot}/Overworld/walk_{direction.ToLower()}";
            var json = $"{path}.json";
            var png = $"{path}.png";

            var frames = ExtractFrameData(json);

            foreach (var frame in frames)
            {
                _walkAnimations[direction].Add(new SpriteV2(png, frame));
                count++;
            }
        }

        Logger.Info($"LoadWalkAnimations finished with count [{count}].");
    }

    private List<FrameRect> ExtractFrameData(string jsonFilePath)
    {
        if (string.IsNullOrWhiteSpace(jsonFilePath))
        {
            Logger.Error($"jsonFilePath is empty.");
        }

        Logger.Debug($"jsonFilePath = [{jsonFilePath}]");

        var jsonText = File.ReadAllText(jsonFilePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true   // handles "frame" vs "Frame" etc.
        };

        var sheet = JsonSerializer.Deserialize<AsepriteSheet>(jsonText, options);
        if (sheet == null)
        {
            Logger.Error("Problem deserializing json data.");
        }

        var frameRects = new List<FrameRect>();
        foreach (var entry in sheet.frames)
        {
            if (entry?.frame != null)
            {
                frameRects.Add(entry.frame);
            }
        }

        return frameRects;
    }
}