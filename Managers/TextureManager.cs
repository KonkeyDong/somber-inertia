using Raylib_cs;
using System.Collections.Generic;

namespace SomberInertia.Managers;

public static class TextureManager
{
    private static readonly Dictionary<string, Texture2D> _textures = new();

    public static Texture2D Load(string filePath)
    {
        if (_textures.TryGetValue(filePath, out Texture2D existing))
            return existing;

        Logger.Debug($"Loading texture: {filePath}");
        Texture2D texture = Raylib.LoadTexture(filePath);

        if (texture.Id == 0)
        {
            Logger.Error($"Failed to load texture: {filePath}");
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

        Logger.Info("All textures unloaded.");
    }
}