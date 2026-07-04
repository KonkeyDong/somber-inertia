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
    public int X { get; set; }
    public int Y { get; set; }
    public int W { get; set; }
    public int H { get; set; }
    
    public int OffsetX { get; set; } = 0;
    public int OffsetY { get; set; } = 0;
}