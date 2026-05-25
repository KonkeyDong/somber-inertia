using System.Collections.Generic;

using System.Text.Json;
using Raylib_cs;

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

    public static List<FrameRect> ExtractFrameData(string jsonFilePath)
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