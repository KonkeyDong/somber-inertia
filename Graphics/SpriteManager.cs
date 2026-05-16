using Raylib_cs;
using System.Collections.Generic;

namespace SomberInertia.Graphics;

public static class SpriteManager
{
    private static readonly Dictionary<string, Texture2D> _textures = new();

    public static Texture2D Load(string filePath)
    {
        if (_textures.TryGetValue(filePath, out var existing))
        {
            return existing;
        }

        Logger.Debug($"Loading sprite: {filePath}");
        var texture = Raylib.LoadTexture(filePath);

        if (texture.Id == 0)
        {
            Logger.Error($"Failed to load sprite: {filePath}");
        }

        _textures[filePath] = texture;
        return texture;
    }

    public static void UnloadAll()
    {
        foreach (var texture in _textures.Values)
        {
            Raylib.UnloadTexture(texture);
        }

        _textures.Clear();

        Logger.Info("All sprites unloaded.");
    }
}