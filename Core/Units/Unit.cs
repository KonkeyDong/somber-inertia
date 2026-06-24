using SomberInertia.Enums;
using SomberInertia.Timers;
using SomberInertia.Graphics;
using SomberInertia.Core.Combat;

using System.Numerics;
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

    public UnitName Name { get; protected set; }
    public MovementType MovementType { get; protected set; }
    public virtual bool Promoted { get; set; } =  false;

    public Dictionary<MagicFamily, List<Magic>> KnownSpells { get; } = new();
    public MagicFamily?[] MagicFamilyBuckets = new MagicFamily?[GameConstants.MAX_BUCKET_SIZE];
    public bool HasSpells => KnownSpells.Count > 0;

    public Direction FacingDirection { get; set; } = Direction.Down;
    private Dictionary<Direction, List<Sprite>> _walkAnimations = new();

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

    private readonly FrameFlipper _movementFlipper = new FrameFlipper(GameConfig.Animations.FrameFlipperDelay / 7);
    public const float MovementDuration = GameConfig.Animations.MovementDuration;

    private bool _isAnimating;
    public bool IsAnimating => _isAnimating;


    // Stats
    public Stat HP { get; set; }
    public Stat MP { get; set; }
    public int Attack { get; set; }
    public Weapon Weapon { get; set; } = null!;
    public int Defense { get; set; }
    public int Speed { get; set; }
    public int Movement { get; protected set; }

    public bool Friendly { get; set; }

    public Unit(UnitName name, MovementType movementType, int movement)
    {
        Name = name;
        MovementType = movementType;

        Movement = movement;
        HP = new Stat(10);
        MP = new Stat(10);

        // default for now
        EquipWeapon(WeaponManager.Create(WeaponName.Unarmed));

        LoadWalkAnimations();

        Logger.Info($"Unit created → {Name.GetDisplayName()} ({movementType}), Movement: {movement}");
    }

    public void EquipWeapon(Weapon weapon)
    {
        Logger.Warning("Unit::EquipWeapon(): will need to redesign when items are more incorporated.");

        Weapon = weapon;
    }

    public void TakeDamage(int amount)
    {
        Logger.Debug($"Unit::TakeDamage({amount})");
        Logger.Info($"Unit [{Name.GetDisplayName()}] has been damaged for {amount}.");

        HP.Current = HP.Current - amount;
        if (HP.Current < 0)
        {
            HP.Current = 0;
        }
        Logger.Info($"\tUnit's current health: {HP.Current} / {HP.Max}.");
    }

    public override string ToString() => $"{Name.GetDisplayName()} ({MovementType}) HP = [{HP.Current} / {HP.Max}] at {Block?.PrintGridCoordinates() ?? "[null]"}";
    public string GetDisplayName() => Name.GetDisplayName();

    public void LearnSpell(Magic spell)
    {
        Logger.Debug("LearnSpell():");
        var family = spell.Name.ToFamily();

        if (!MagicFamilyBuckets.Contains(family))
        {
            Logger.Debug($"  Creating new bucket for magic family [{family}].");
            FillFirstAvailableBucket(family);
        }

        if (!KnownSpells.ContainsKey(family))
        {
            Logger.Debug("  Initializing new List<Magic>().");
            KnownSpells[family] = new List<Magic>();
        }

        if (!KnownSpells[family].Any(s => s.Name == spell.Name))
        {
            Logger.Debug($"  Adding spell [{spell.Name.GetDisplayName()}].");
            KnownSpells[family].Add(spell);
        }
    }

    private void FillFirstAvailableBucket(MagicFamily family)
    {
        for (var i = 0; i < GameConstants.MAX_BUCKET_SIZE; i++)
        {
            if (MagicFamilyBuckets[i] == null)
            {
                MagicFamilyBuckets[i] = family;
                return;
            }
        }

        Logger.Error($"magic family [{family}] could not be added to bucket as bucket as reached capacity.");
    }

    public List<Magic> GetMagicListInBucket(MagicFamily magicFamily)
    {
        if (KnownSpells.TryGetValue(magicFamily, out var spells))
        {
            return spells;
        }

        Logger.Error($"Magic family [{magicFamily}] could not be found in KnownSpells dictionary.");
        return new List<Magic>() { MagicManager.Create(MagicName.NoSpell) };
    }

    // This assumes that the last spell is the strongest. Spells should only
    // be added in ascending level order upon level requirement met.
    public Magic GetHighestMagicLevelInBucket(MagicFamily magicFamily)
    {
        var spell = GetMagicListInBucket(magicFamily).LastOrDefault();
        if (spell == null)
        {
            Logger.Error("Last spell in dictionary KnownSpells is null.");
        }

        return spell;
    }


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

        _movementFlipper.Tick();

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

    public Sprite GetFacingDirectionTexture(Direction direction)
    {
        if (!_walkAnimations.Any())
        {
            Logger.Error("No walk animations loaded.");
            return null!;
        }

        return _walkAnimations[direction][0];
    }

    public Sprite GetFacingDirectionTexture(bool globalFrameFlipperFlag)
    {
        if (!_walkAnimations.Any())
        {
            Logger.Error("No walk animations loaded.");
            return null!;
        }

        var animations = _walkAnimations[FacingDirection];

        int frameIndex;
        if (_isAnimating)
        {
            // Use the fast movement flipper while sliding
            frameIndex = _movementFlipper.IsOn ? 1 : 0;
        }
        else
        {
            // Use the global slow idle flipper when standing still
            frameIndex = globalFrameFlipperFlag ? 1 : 0;
        }

        return animations[frameIndex];
    }

    // ---------------------------
    // Read Spritesheet Frame Data
    // ---------------------------
    public void LoadWalkAnimations()
    {
        _walkAnimations.Clear();

        var totalFramesLoaded = 0;

        var basePath = Path.Combine(AssetRoot, GameConstants.OVERWORLD_FOLDER_NAME);
        var jsonPath = Path.Combine(basePath, GameConstants.FRAME_DATA_FILE_NAME);
        var frames = SpriteManager.ExtractFrameData(jsonPath);

        foreach (var direction in Enum.GetValues<Direction>())
        {
            _walkAnimations[direction] = new List<Sprite>();

            var pngPath = Path.Combine(basePath, direction.WalkImage());

            foreach (var frame in frames)
            {
                _walkAnimations[direction].Add(new Sprite(pngPath, frame));
                totalFramesLoaded++;
            }
        }

        Logger.Info($"LoadWalkAnimations completed. Loaded {totalFramesLoaded} frames across 4 directions.");
    }   
}