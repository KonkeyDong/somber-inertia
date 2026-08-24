using SomberInertia.Core.Units;
using SomberInertia.Enums;
using System.Numerics;
using System.Text;

using Raylib_cs;

namespace SomberInertia.Graphics;

public class BattleUnitSpriteSet
{
    public List<Sprite> Idle = new();
    public List<Sprite> Attack = new();
    public List<Sprite> BattleSequence = new();
    public Vector2 BasePosition = new();

    public void SetBasePosition(Unit unit)
    {
        BasePosition = GameConstants.Battle.GetSpritePosition(unit);
    }

    public void Reset()
    {
        UnloadDynamicTexture();

        Idle.Clear();
        Attack.Clear();
        BattleSequence.Clear();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("BattleUnitSpriteSet:");
        sb.AppendLine("   Idle count          : " + Idle.Count);
        sb.AppendLine("   Attack count        : " + Attack.Count);
        sb.AppendLine("   BattleSequence count: " + BattleSequence.Count);

        return sb.ToString();
    }

    public int GetIndexOfLastAttackFrame()
    {
        if (Attack == null || Attack.Count == 0)
        {
            Logger.Error("Cannot get last index of empty attack List<Sprite>.");
        }

        return Attack.Count - 1;
    }

    public Sprite GetIdleFrame(int frameIndex)
    {
        if (Idle == null || Idle.Count == 0)
        {
            Logger.Error("No idle frames detected.");
        }

        // If there's only 1 frame, always return it (ignore index)
        if (Idle.Count == 1)
        {
            return Idle[0];
        }

        // Otherwise use modulo to loop normally
        return Idle[frameIndex % Idle.Count];
    }

    public Sprite GetAttackFrame(int frameIndex)
    {
        if (Attack == null || Attack.Count == 0)
        {
            Logger.Error("No attack frames detected.");
        }

        return Attack[frameIndex % Attack.Count];
    }

    public Sprite GetBattleSequenceFrame(int frameIndex)
    {
        if (BattleSequence == null || BattleSequence.Count == 0)
        {
            Logger.Error("No battle sequence frames detected.");
            return null!;
        }

        // One-shot attack/dissolve timelines must not wrap (wrapping replays hit-jitter after dissolve).
        var index = Math.Clamp(frameIndex, 0, BattleSequence.Count - 1);
        return BattleSequence[index];
    }

    public void BuildBattleSequence(Sprite sprite, int numberOfCopies, bool invert = false)
    {
        if (numberOfCopies <= 0)
        {
            Logger.Error("Int numberOfCopies cannot be less than or equal to zero.");
        }


        Logger.Debug("  About to build battle frames");
        // invert once and jitter per copy: Jitter() only offsets the frame rect,
        // it doesn't allocate a new texture, so re-invering per copy woul be redundant.
        var invertedSprite = invert ? sprite.Invert() : null;

        for (var i = 0; i < numberOfCopies; i++)
        {
            var finalSprite = invert ? invertedSprite!.Jitter() : sprite;

            BattleSequence.Add(finalSprite);
        }

        Logger.Debug("BattleSequnce count: " + BattleSequence.Count);
    }

    /// <summary>Repeat the last sequence frame until <paramref name="length"/> (keeps dissolve clear / idle held).</summary>
    public void PadBattleSequenceToLength(int length)
    {
        if (BattleSequence.Count == 0 || BattleSequence.Count >= length)
        {
            return;
        }

        var last = BattleSequence[^1];
        while (BattleSequence.Count < length)
        {
            BattleSequence.Add(last);
        }
    }

    private void UnloadDynamicTexture()
    {
        var unloadedTextureIds = new HashSet<uint>();
        foreach (var sprite in BattleSequence)
        {
            if (sprite.OwnsTexture && unloadedTextureIds.Add(sprite.Texture.Id))
            {
                sprite.Unload();
            }
        }
    }
}

public class BattleUnitSpriteManager
{
    // Key will be made of unit name and equipped weapon since Force Members can equip
    // different weapons.
    private static readonly Dictionary<string, BattleUnitSpriteSet> _spriteMap = new();

    public static BattleUnitSpriteSet Get(Unit unit)
    {
        var key = BuildDictionaryKey(unit);
        Logger.Warning("BattleUnitSpriteManager::Get() need to fix dictionary lookup.");
        // if (_spriteMap.TryGetValue(key, out var sprites))
        // {
        //     return sprites;
        // }

        var spriteSet = LoadBattleUnitSpriteSet(unit);
        _spriteMap[key] = spriteSet;

        return spriteSet;
    }

    private static BattleUnitSpriteSet LoadBattleUnitSpriteSet(Unit unit)
    {
        var baseDirPath = BuildAssetDirPath(unit);
        Logger.Info("baseDirPath: " + baseDirPath);

        var spriteSet = new BattleUnitSpriteSet();

        // Load Idle
        var idleJson = Path.Combine(baseDirPath, GameConstants.Files.IdleJson);
        var idlePng = Path.Combine(baseDirPath, GameConstants.Files.IdlePng);
        if (File.Exists(idleJson) && File.Exists(idlePng))
        {
            spriteSet.Idle = LoadSpritesFromJson(idlePng, idleJson);
        }

        // Load Attack
        var attackJson = Path.Combine(baseDirPath, GameConstants.Files.AttackJson);
        var attackPng = Path.Combine(baseDirPath, GameConstants.Files.AttackPng);
        if (File.Exists(attackJson) && File.Exists(attackPng))
        {
            spriteSet.Attack = LoadSpritesFromJson(attackPng, attackJson);
        }

        return spriteSet;
    }

    private static List<Sprite> LoadSpritesFromJson(string pngPath, string jsonPath)
    {
        var sprites = new List<Sprite>();

        foreach (var frame in SpriteManager.ExtractFrameData(jsonPath))
        {
            sprites.Add(new Sprite(pngPath, frame));
        }

        return sprites;
    }

    private static string BuildDictionaryKey(Unit unit)
    {
        return $"{unit.Name.GetBaseName()}{unit.GetEquippedWeaponName()}";
    }

    private static string BuildAssetDirPath(Unit unit)
    {
        if (unit.Friendly)
        {
            var weapon = unit.GetEquippedWeaponData();

            return Path.Combine(
                GameConstants.Paths.ForceMembers,
                unit.Name.GetBaseName(),
                GameConstants.Paths.PromotionFolder(unit.Promoted),
                GameConstants.Paths.Battle,
                weapon.Name.GetBaseName());
        }

        // enemies
        return Path.Combine(
            GameConstants.Paths.Monsters,
            unit.Name.GetBaseName(),
            GameConstants.Paths.Battle);
    }
}