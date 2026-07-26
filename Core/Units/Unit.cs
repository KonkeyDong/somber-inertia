using SomberInertia.Enums;
using SomberInertia.Timers;
using SomberInertia.Graphics;
using SomberInertia.Core.Combat.Spells;
using SomberInertia.Core.Combat.StatusEffect;
using SomberInertia.Core.Combat.Item;


using System.Numerics;
using System.Text;
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

        public override string ToString() 
        {
            if (Max == 0)
            {
                return "0 / 0";
            }

            return $"{Current} / {Max}";
        }
    }

    public Texture2D Texture { get; protected set; }
    protected abstract string AssetRoot { get; }

    public UnitName Name { get; protected set; }
    public MovementType MovementType { get; protected set; }
    public virtual bool Promoted { get; set; } =  false;

    public Dictionary<MagicFamily, List<MagicName>> KnownSpells { get; } = new();
    public MagicFamily?[] MagicFamilyBuckets = new MagicFamily?[GameConstants.MAX_BUCKET_SIZE];
    public bool HasSpells => KnownSpells.Count > 0;

    public ItemSlot[] Items = new ItemSlot[GameConstants.MAX_BUCKET_SIZE];
    public int EquippedWeaponIndex { get; set; } = -1; // -1 = Unarmed

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

    private readonly FrameFlipper _movementFlipper = new FrameFlipper(GameConstants.Animations.FrameFlipperDelay / 7);
    public const float MovementDuration = GameConstants.Animations.MovementDuration;

    private bool _isAnimating;
    public bool IsAnimating => _isAnimating;


    // Stats
    public Stat HP { get; set; }
    public Stat MP { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Speed { get; set; }
    public int Movement { get; protected set; }
    public List<StatusEffectSlot> StatusEffects { get; protected set; } = new();

    public bool Friendly { get; set; }

    public Unit(UnitName name, MovementType movementType, int movement)
    {
        Name = name;
        MovementType = movementType;

        Movement = movement;
        HP = new Stat(10);
        MP = new Stat(10);
        StatusEffects = new();

        InitializeItemSlots();
        EquipUnarmed();

        LoadWalkAnimations();

        Logger.Info($"Unit created → {Name.GetDisplayName()} ({movementType}), Movement: {movement}");
    }

    private void InitializeItemSlots()
    {
        for (var i = 0; i < Items.Length; i++)
        {
            Items[i] = ItemSlot.Empty;
        }
    }

    public void EquipUnarmed()
    {
        EquippedWeaponIndex = -1;
    }

    public bool EquipWeaponAtIndex(int index)
    {
        if (index < 0 || index >= Items.Length)
        {
            Logger.Error($"EquipWeaponAtIndex(): index [{index}] out of range.");
            return false;
        }

        var slot = Items[index];
        if (slot.IsEmpty)
        {
            Logger.Warning("EquipWeaponAtIndex(): slot is empty.");
            return false;
        }

        var data = ItemDatabase.Get(slot.Name);
        if (!data.Type.IsWeapon())
        {
            Logger.Warning($"EquipWeaponAtIndex(): [{slot.Name}] is not a weapon.");
            return false;
        }

        EquippedWeaponIndex = index;
        Logger.Info($"Equipped [{slot.Name}] from slot [{index}].");
        return true;
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

    public bool IsDead()
    {
        return HP.Current == 0;
    }

    public void ApplyStatus(StatusEffectType type)
    {
        if (HasStatus(type))
        {
            Logger.Info($"Unit [{GetDisplayName()}] already has [{type}]. Ignoring.");
            return;
        }

        StatusEffects.Add(StatusEffectSystem.Create(type));
        Logger.Info($"Applied [{type}] to [{GetDisplayName()}].");
    }

    public bool HasStatus(StatusEffectType type)
    {
        return FindStatusIndex(type) >= 0;
    }

    public int FindStatusIndex(StatusEffectType type)
    {
        for (var i = 0; i < StatusEffects.Count; i++)
        {
            if (StatusEffects[i].Type == type)
            {
                return i;
            }
        }

        return -1;
    }

    public void RemoveStatus(StatusEffectType type)
    {
        var index = FindStatusIndex(type);
        if (index < 0)
        {
            return;
        }

        StatusEffects.RemoveAt(index);
        Logger.Info($"Removed [{type}] from [{GetDisplayName()}].");
    }

    public void RemoveAllStatus()
    {
        StatusEffects.Clear();
    }

    public int GetStatusDuration(StatusEffectType type)
    {
        var index = FindStatusIndex(type);
        if (index < 0)
        {
            return -1;
        }

        return StatusEffects[index].Duration;
    }

    public void ProcessPoisonStatus()
    {
        StatusEffectSystem.ProcessPoison(this);
    }

    public void ProcessSleepStatus()
    {
        StatusEffectSystem.ProcessSleep(this);
    }

    public override string ToString() => $"{Name.GetDisplayName()} ({MovementType}) HP = [{HP.Current} / {HP.Max}] at {Block?.PrintGridCoordinates() ?? "[null]"}";
    public string GetDisplayName() => Name.GetDisplayName();
    public string CombatToString()
    {
        var weaponData = GetEquippedWeaponData();
        var weaponSlot = GetEquippedWeaponSlot();

        var sb = new StringBuilder();
        sb.AppendLine($"   {GetDisplayName()}:");
        sb.AppendLine($"   HP            = [{HP}]");
        sb.AppendLine($"   MP            = [{MP}]");
        sb.AppendLine($"   Eq. Weapon    = [{weaponData.Name}] ({weaponSlot.Condition})");
        sb.AppendLine($"   Offense       = [{GetTotalOffense()}]");
        sb.AppendLine($"   Defense       = [{Defense}]");
        sb.AppendLine($"   Speed         = [{Speed}]");
        sb.AppendLine($"   Movement Type = [{MovementType}]");

        return sb.ToString();
    }

    public void LearnSpell(MagicName spellName)
    {
        var data = MagicDatabase.Get(spellName);
        var family = data.Family;

        if (!MagicFamilyBuckets.Contains(family))
        {
            FillFirstAvailableBucket(family);
        }

        if (!KnownSpells.ContainsKey(family))
        {
            KnownSpells[family] = new List<MagicName>();
        }

        if (!KnownSpells[family].Contains(spellName))
        {
            KnownSpells[family].Add(spellName);
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

    public List<MagicName> GetMagicListInBucket(MagicFamily magicFamily)
    {
        if (KnownSpells.TryGetValue(magicFamily, out var spells))
        {
            return spells;
        }

        Logger.Error($"Magic family [{magicFamily}] not found.");
        return new List<MagicName> { MagicName.NoSpell };
    }

    // This assumes that the last spell is the strongest. Spells should only
    // be added in ascending level order upon level requirement met.
    public MagicName GetHighestMagicLevelInBucket(MagicFamily magicFamily)
    {
        var list = GetMagicListInBucket(magicFamily);
        if (list.Count == 0)
        {
            return MagicName.NoSpell;
        }

        // Assumes spells are learned in ascending level order
        return list[list.Count - 1];
    }

    public MagicData GetHighestMagicDataInBucket(MagicFamily magicFamily)
    {
        return MagicDatabase.Get(GetHighestMagicLevelInBucket(magicFamily));
    }

    public bool AddItem(ItemName itemName, ItemCondition condition = ItemCondition.Normal, bool autoEquipWeapon = false)
    {
        for (var i = 0; i < Items.Length; i++)
        {
            if (Items[i].IsEmpty)
            {
                Items[i] = new ItemSlot
                {
                    Name = itemName,
                    Condition = condition
                };

                if (autoEquipWeapon && EquippedWeaponIndex < 0)
                {
                    var data = ItemDatabase.Get(itemName);
                    if (data.Type.IsWeapon())
                    {
                        EquippedWeaponIndex = i;
                    }
                }

                return true;
            }
        }

        return false;
    }

    public ItemSlot GetItemAtIndex(int index)
    {
        if (index < 0 || index >= Items.Length)
        {
            return ItemSlot.Empty;
        }

        return Items[index];
    }

    public ItemName GetEquippedWeaponName()
    {
        if (EquippedWeaponIndex < 0 || EquippedWeaponIndex >= Items.Length)
        {
            return ItemName.Unarmed;
        }

        var slot = Items[EquippedWeaponIndex];
        if (slot.IsEmpty)
        {
            return ItemName.Unarmed;
        }

        return slot.Name;
    }

    public ItemSlot GetEquippedWeaponSlot()
    {
        if (EquippedWeaponIndex < 0 || EquippedWeaponIndex >= Items.Length)
        {
            return new ItemSlot
            {
                Name = ItemName.Unarmed,
                Condition = ItemCondition.Normal
            };
        }

        return Items[EquippedWeaponIndex];
    }

    public ItemData GetEquippedWeaponData()
    {
        return ItemDatabase.Get(GetEquippedWeaponName());
    }

    public int GetTotalOffense()
    {
        var weaponData = GetEquippedWeaponData();
        return Attack + weaponData.Attack;
    }

    public void UnequipWeapon()
    {
        EquippedWeaponIndex = -1;
    }

    public bool CanUseWeaponAsItem(ItemName itemName, Job job)
    {
        var data = ItemDatabase.Get(itemName);

        if (data.SpellName == MagicName.NoSpell)
        {
            return false;
        }

        if (data.AllowedJobs == Job.Any)
        {
            return true;
        }

        return (data.AllowedJobs & job) != 0;
    }

    // -----------------
    // Animation methods
    // -----------------
    #region Animations
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

        var basePath = Path.Combine(AssetRoot, GameConstants.Folders.OVERWORLD_FOLDER_NAME);
        var jsonPath = Path.Combine(basePath, GameConstants.Files.FRAME_DATA_FILE_NAME);
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

    #endregion
}