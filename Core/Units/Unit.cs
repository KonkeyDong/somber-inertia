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

public class Unit
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

    /// <summary>
    /// Overworld sprite root: force → Characters/…/Promoted|Unpromoted; monsters → Monsters/….
    /// </summary>
    private string AssetRoot =>
        Friendly
            ? Path.Combine(
                GameConstants.Paths.Characters,
                Name.GetBaseName(),
                GameConstants.Paths.PromotionFolder(Promoted))
            : Path.Combine(
                GameConstants.Paths.Monsters,
                Name.GetBaseName());

    public UnitName Name { get; protected set; }
    public MovementType MovementType { get; protected set; }

    /// <summary>Force promotion state. Always false for monsters (cannot promote).</summary>
    private bool _promoted;
    public bool Promoted
    {
        get => Friendly && _promoted;
        set
        {
            if (!Friendly)
            {
                _promoted = false;
                return;
            }

            _promoted = value;
        }
    }

    /// <summary>Force job. Monsters have no job (default / zero flags).</summary>
    public Job Job { get; set; }

    public int Level { get; set; }
    public int Exp { get; private set; }

    public Dictionary<MagicFamily, List<MagicName>> KnownSpells { get; } = new();
    public MagicFamily?[] MagicFamilyBuckets = new MagicFamily?[GameConstants.MaxBucketSize];
    public bool HasSpells => KnownSpells.Count > 0;

    public ItemSlot[] Items = new ItemSlot[GameConstants.MaxBucketSize];
    public int EquippedWeaponIndex { get; set; } = -1; // -1 = Unarmed

    public Direction FacingDirection { get; set; } = Direction.Down;
    private Dictionary<Direction, List<Sprite>> _walkAnimations = new();

    /// <summary>Map tile where this unit's turn movement range was rooted (set at turn start).</summary>
    public readonly struct MovementOriginCoord
    {
        public int X { get; init; }
        public int Y { get; init; }

        public static MovementOriginCoord Invalid => new() { X = -1, Y = -1 };

        public bool IsValid => X >= 0 && Y >= 0;
    }

    public MovementOriginCoord MovementOrigin { get; private set; } = MovementOriginCoord.Invalid;

    public void SetMovementOrigin(int x, int y) =>
        MovementOrigin = new MovementOriginCoord { X = x, Y = y };

    public void ClearMovementOrigin() =>
        MovementOrigin = MovementOriginCoord.Invalid;

    protected Block? _block;
    public Block? Block
    {
        get => _block;
        set
        {
            if (_block == value)
            {
                return;
            }

            _block = value;
        }
    }

    public Vector2 WorldPosition { get; private set; }
    public Vector2 TargetWorldPosition { get; private set; }

    private Vector2 _startWorldPosition;
    private float _movementTimer;

    private readonly FlipFlop _movementFlipFlop = new FlipFlop(GameConstants.Animations.FlipFlopDelay / 7);
    public const float MovementDuration = GameConstants.Animations.MovementDuration;

    private bool _isAnimating;
    public bool IsAnimating => _isAnimating;

    public Stat HP { get; set; }
    public Stat MP { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Speed { get; set; }
    public int Movement { get; protected set; }
    public List<StatusEffectSlot> StatusEffects { get; protected set; } = new();

    public bool Friendly { get; set; }

    public Unit(UnitName name)
    {
        var data = UnitDatabase.Get(name);

        Name = data.Name;
        MovementType = data.MovementType;
        Movement = data.Movement;
        HP = new Stat(data.BaseHP);
        MP = new Stat(data.BaseMP);
        Attack = data.BaseAttack;
        Defense = data.BaseDefense;
        Speed = data.BaseSpeed;

        Friendly = data.Friendly;
        Level = data.Level;
        Exp = 0;
        Job = data.Friendly ? data.DefaultJob : default;
        Promoted = false;

        StatusEffects = new();
        InitializeItemSlots();
        EquipUnarmed();
        LoadWalkAnimations();

        Logger.Info(
            $"Unit created → {Name.GetDisplayName()} ({MovementType}), " +
            $"Friendly: {Friendly}, Lvl {Level}, Job: {Job}, Movement: {Movement}");
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

    /// <summary>Monsters cannot equip weapons; force members must match job flags.</summary>
    public bool CanEquipWeapon(ItemData data)
    {
        if (!Friendly)
        {
            return false;
        }

        if (!data.Type.IsWeapon())
        {
            return false;
        }

        return Job.IsAllowedBy(data.AllowedJobs);
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
        if (!CanEquipWeapon(data))
        {
            Logger.Warning(
                $"EquipWeaponAtIndex(): [{Name.GetDisplayName()}] cannot equip [{slot.Name}] " +
                $"(Friendly={Friendly}, Job={Job}).");
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

    public bool RemoveItemAtIndex(int index)
    {
        if (index < 0 || index >= Items.Length)
        {
            Logger.Error($"RemoveItemAtIndex(): index [{index}] out of range.");
            return false;
        }

        if (Items[index].IsEmpty)
        {
            Logger.Warning($"RemoveItemAtIndex(): slot [{index}] is already empty.");
            return false;
        }

        // If we removed the equipped weapon, unequip
        if (EquippedWeaponIndex == index)
        {
            EquippedWeaponIndex = -1;
        }
        else if (EquippedWeaponIndex > index)
        {
            // Equipped item shifted left by one
            EquippedWeaponIndex--;
        }

        // Shift everything after index one slot left
        for (var i = index; i < Items.Length - 1; i++)
        {
            Items[i] = Items[i + 1];
        }

        // Last slot becomes empty
        Items[Items.Length - 1] = ItemSlot.Empty;

        return true;
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

    public override string ToString()
    {
        return $"{Name.GetDisplayName()} ({MovementType}) HP = [{HP.Current} / {HP.Max}] at {Block?.PrintGridCoordinates() ?? "[null]"}";
    }

    public string GetDisplayName() => Name.GetDisplayName();

    public string CombatToString()
    {
        var weaponData = GetEquippedWeaponData();
        var weaponSlot = GetEquippedWeaponSlot();

        var sb = new StringBuilder();
        sb.AppendLine($"   {GetDisplayName()}:");
        sb.AppendLine($"   HP            = [{HP}]");
        sb.AppendLine($"   MP            = [{MP}]");
        sb.AppendLine($"   Eq. Weapon    = [{weaponData.Name}] (Damaged: {weaponSlot.Damaged})");
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
        for (var i = 0; i < GameConstants.MaxBucketSize; i++)
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

    public MagicName GetHighestMagicLevelInBucket(MagicFamily magicFamily)
    {
        var list = GetMagicListInBucket(magicFamily);
        if (list.Count == 0)
        {
            return MagicName.NoSpell;
        }

        return list[list.Count - 1];
    }

    public MagicData GetHighestMagicDataInBucket(MagicFamily magicFamily)
    {
        return MagicDatabase.Get(GetHighestMagicLevelInBucket(magicFamily));
    }

    public bool AddItem(ItemName itemName, bool damaged = false, bool autoEquipWeapon = false)
    {
        if (itemName == ItemName.Unarmed)
        {
            Logger.Warning($"Unit [{Name.GetDisplayName()}] cannot add unarmed to item list because being unarmed is without an item.");
            return false;
        }

        for (var i = 0; i < Items.Length; i++)
        {
            if (Items[i].IsEmpty)
            {
                Items[i] = new ItemSlot
                {
                    Name = itemName,
                    Damaged = damaged
                };

                if (autoEquipWeapon && EquippedWeaponIndex < 0)
                {
                    var data = ItemDatabase.Get(itemName);
                    if (CanEquipWeapon(data))
                    {
                        EquippedWeaponIndex = i;
                    }
                }

                return true;
            }
        }

        return false;
    }

    // True if the item can be given or traded. Empty and Unarmed are never transferable.
    public static bool IsGiveableItem(ItemName name)
    {
        return name != ItemName.NoItem && name != ItemName.Unarmed;
    }

    public static bool IsGiveableItemSlot(ItemSlot itemSlot) => IsGiveableItem(itemSlot.Name);

    public bool HasGiveableItem()
    {
        for (var i = 0; i < Items.Length; i++)
        {
            if (IsGiveableItemSlot(Items[i]))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Force job; monsters have no job flags (default).</summary>
    public Job GetJob() => Job;

    /// <summary>
    /// True if the item can be selected under Use: job is allowed and the item has a
    /// use effect (consumable effect or castable spell). Consumables use Job.Any so any job matches.
    /// </summary>
    public static bool IsUsableItem(ItemName name, Job unitJob)
    {
        if (name is ItemName.NoItem or ItemName.Unarmed)
        {
            return false;
        }

        var data = ItemDatabase.Get(name);
        if (!unitJob.IsAllowedBy(data.AllowedJobs))
        {
            return false;
        }

        return data.EffectType != ItemEffectType.None || data.SpellName != MagicName.NoSpell;
    }

    public static bool IsUsableItemSlot(ItemSlot itemSlot, Job unitJob) =>
        IsUsableItem(itemSlot.Name, unitJob);

    public bool HasUsableItem()
    {
        var job = GetJob();
        for (var i = 0; i < Items.Length; i++)
        {
            if (IsUsableItemSlot(Items[i], job))
            {
                return true;
            }
        }

        return false;
    }

    public bool HasEmptyItemSlot()
    {
        for (var i = 0; i < Items.Length; i++)
        {
            if (Items[i].IsEmpty)
            {
                return true;
            }
        }

        return false;
    }

    public int FindFirstEmptyItemSlot()
    {
        for (var i = 0; i < Items.Length; i++)
        {
            if (Items[i].IsEmpty)
            {
                return i;
            }
        }

        return -1;
    }

    public int FindFirstGiveableItemSlot()
    {
        for (var i = 0; i < Items.Length; i++)
        {
            if (IsGiveableItemSlot(Items[i]))
            {
                return i;
            }
        }

        return -1;
    }

    /// Free-slot transfer: copy giver slot into recipient inventory, then remove from giver.
    public bool GiveItemTo(Unit recipient, int giverSlotIndex)
    {
        if (recipient == null)
        {
            Logger.Error("GiveItemTo(): recipient is null.");
            return false;
        }

        if (giverSlotIndex < 0 || giverSlotIndex >= Items.Length)
        {
            Logger.Error($"GiveItemTo(): giver slot [{giverSlotIndex}] out of range.");
            return false;
        }

        var itemSlot = Items[giverSlotIndex];
        if (!IsGiveableItemSlot(itemSlot))
        {
            Logger.Warning($"GiveItemTo(): slot [{giverSlotIndex}] is not giveable ({itemSlot.Name}).");
            return false;
        }

        if (!recipient.HasEmptyItemSlot())
        {
            Logger.Warning("GiveItemTo(): recipient inventory is full.");
            return false;
        }

        if (!recipient.AddItem(itemSlot.Name, itemSlot.Damaged, autoEquipWeapon: false))
        {
            Logger.Error("GiveItemTo(): failed to add item to recipient.");
            return false;
        }

        RemoveItemAtIndex(giverSlotIndex);
        Logger.Info($"{GetDisplayName()} gave [{itemSlot.Name}] to {recipient.GetDisplayName()}.");

        return true;
    }

    /// Swap giveable items between this unit and another. Unequips either side if the swapped slot was equipped.
    public bool SwapItemWith(Unit other, int myIndex, int otherIndex)
    {
        if (other == null)
        {
            Logger.Error("SwapItemWith(): other unit is null.");
            return false;
        }

        if (myIndex < 0 || myIndex >= Items.Length || otherIndex < 0 || otherIndex >= other.Items.Length)
        {
            Logger.Error($"SwapItemWith(): index out of range (mine={myIndex}, other={otherIndex}).");
            return false;
        }

        var myItemSlot = Items[myIndex];
        var otherItemSlot = other.Items[otherIndex];

        if (!IsGiveableItemSlot(myItemSlot) || !IsGiveableItemSlot(otherItemSlot))
        {
            Logger.Warning($"SwapItemWith(): one or both slots are not giveable ({myItemSlot.Name} / {otherItemSlot.Name}).");
            return false;
        }

        // Unequip before mutating so equip indices stay stable.
        if (EquippedWeaponIndex == myIndex)
        {
            UnequipWeapon();
        }

        if (other.EquippedWeaponIndex == otherIndex)
        {
            other.UnequipWeapon();
        }

        Items[myIndex] = otherItemSlot;
        other.Items[otherIndex] = myItemSlot;

        Logger.Info($"{GetDisplayName()} swapped [{myItemSlot.Name}] with {other.GetDisplayName()}'s [{otherItemSlot.Name}].");

        return true;
    }

    /// Icon display name: Unarmed is shown as the empty (NoItem) icon.
    public static ItemName GetDisplayItemName(ItemName name)
    {
        return name == ItemName.Unarmed ? ItemName.NoItem : name;
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
                Damaged = false
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

        return job.IsAllowedBy(data.AllowedJobs);
    }

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

        _startWorldPosition = WorldPosition;
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

        _movementFlipFlop.Tick();

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

    public Sprite GetFacingDirectionTexture(bool globalFlipFlopIsOn)
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
            frameIndex = _movementFlipFlop.IsOn ? 1 : 0;
        }
        else
        {
            frameIndex = globalFlipFlopIsOn ? 1 : 0;
        }

        return animations[frameIndex];
    }

    public void LoadWalkAnimations()
    {
        _walkAnimations.Clear();

        var totalFramesLoaded = 0;

        var basePath = Path.Combine(AssetRoot, GameConstants.Paths.Overworld);
        var jsonPath = Path.Combine(basePath, GameConstants.Files.FrameData);
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