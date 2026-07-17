using SomberInertia.Core.Units;
using SomberInertia.Enums;
using System.Numerics;
using System.Text;

using Raylib_cs;

namespace SomberInertia.Graphics;

public class BattleSpriteSet
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
        Idle.Clear();
        Attack.Clear();
        BattleSequence.Clear();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("BattleSpriteSet:");
        sb.AppendLine("   Idle count          : " + Idle.Count);
        sb.AppendLine("   Attack count        : " + Attack.Count);
        sb.AppendLine("   BattleSequence count: " + BattleSequence.Count);

        return sb.ToString();
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
        }

        return BattleSequence[frameIndex % BattleSequence.Count];
    }

    public void BuildBattleSequence(Sprite sprite, int numberOfCopies, bool invert = false)
    {
        if (numberOfCopies <= 0)
        {
            Logger.Error("Int numberOfCopies cannot be less than or equal to zero.");
        }


        Logger.Debug("  About to build battle frames");
        for (var i = 0; i < numberOfCopies; i++)
        {
            Sprite finalSprite;

            if (invert)
            {
                finalSprite = sprite.Invert().Jitter();
            }
            else
            {
                finalSprite = sprite;
            }

            BattleSequence.Add(finalSprite);
        }

        Logger.Debug("BattleSequnce count: " + BattleSequence.Count);
    }
}

public class BattleSpriteManager
{
    // Key will be made of unit name and equipped weapon since Force Members can equip
    // different weapons.
    private static readonly Dictionary<string, BattleSpriteSet> _spriteMap = new();

    public static BattleSpriteSet Get(Unit unit)
    {
        var key = BuildDictionaryKey(unit);
        Logger.Warning("BattleSpriteManager::Get() need to fix dictionary lookup.");
        // if (_spriteMap.TryGetValue(key, out var sprites))
        // {
        //     return sprites;
        // }

        var spriteSet = LoadBattleSpriteSet(unit);
        _spriteMap[key] = spriteSet;

        return spriteSet;
    }

    private static BattleSpriteSet LoadBattleSpriteSet(Unit unit)
    {
        var baseDirPath = BuildAssetDirPath(unit);
        Logger.Info("baseDirPath: " + baseDirPath);

        var spriteSet = new BattleSpriteSet();

        // Load Idle
        var idleJson = Path.Combine(baseDirPath, "Idle.json");
        var idlePng = Path.Combine(baseDirPath, "Idle.png");
        if (File.Exists(idleJson) && File.Exists(idlePng))
        {
            spriteSet.Idle = LoadSpritesFromJson(idlePng, idleJson);
        }

        // Load Attack
        var attackJson = Path.Combine(baseDirPath, "Attack.json");
        var attackPng = Path.Combine(baseDirPath, "Attack.png");
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
        return $"{unit.Name.GetBaseName()}{unit.Weapon.Name}";
    }

    private static string BuildAssetDirPath(Unit unit)
    {
        if (unit.Friendly)
        {
            var promoted = unit.Promoted ? "Promoted" : "Unpromoted";
            return Path.Combine("Assets", "Sprites", "Characters", unit.Name.GetBaseName(), promoted, "Battle", unit.Weapon.Name.GetBaseName());
        }

        // enemies
        return Path.Combine("Assets", "Sprites", "Monsters", unit.Name.GetBaseName(), "Battle");
    }
}