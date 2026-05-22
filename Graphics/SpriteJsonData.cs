using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SomberInertia.Graphics;

public class AsepriteSheet
{
    public List<AsepriteFrameEntry> frames { get; set; } = new();
}

public class AsepriteFrameEntry
{
    [JsonPropertyName("frame")]
    public FrameRect frame { get; set; } = new();
}

public class FrameRect
{
    public int x { get; set; }
    public int y { get; set; }
    public int w { get; set; }
    public int h { get; set; }
}