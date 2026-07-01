using SomberInertia.Core.Units;
using SomberInertia.Enums;

namespace SomberInertia.Graphics;

public class BattleSpriteManager
{
    // Key will be made of unit name and equipped weapon since Force Members can equip
    // different weapons.
    private static readonly Dictionary<string, List<Sprite>> _spriteMap = new();

    public static List<Sprite> Get(Unit unit)
    {
        var key = BuildDictionaryKey(unit);
        if (_spriteMap.TryGetValue(key, out var sprites))
        {
            return sprites;
        }

        var spriteList = ExtractBattleSpriteData(unit);
        _spriteMap[key] = spriteList;

        Logger.Info($"Battle sprite extracted for {unit.GetDisplayName()}: [{spriteList.Count}].");
        return spriteList;
    }

    private static List<Sprite> ExtractBattleSpriteData(Unit unit)
    {
        var baseDirPath = BuildAssetDirPath(unit);
        Logger.Info($"base dir path: [{baseDirPath}].");

        var jsonPath = Path.Combine(baseDirPath, "Battle.json");
        var pngPath = Path.Combine(baseDirPath, "Battle.png");

        var sprites = new List<Sprite>();
        foreach (var frame in SpriteManager.ExtractFrameData(jsonPath))
        {
            sprites.Add(new Sprite(pngPath, frame));
        }

        return sprites;
    }

    private static string BuildDictionaryKey(Unit unit)
    {
        return $"{unit.Name.GetBaseName()}-{unit.Weapon.Name}";
    }

    private static string BuildAssetDirPath(Unit unit)
    {
        if (unit.Friendly)
        {
            var promoted = unit.Promoted ? "Promoted" : "Unpromoted";
            return Path.Combine("Assets", "Sprites", "Characters", unit.Name.GetBaseName(), promoted, "Battle");
        }

        // enemies
        return Path.Combine("Assets", "Sprites", "Monsters", unit.Name.GetBaseName(), "Battle");
    }
}