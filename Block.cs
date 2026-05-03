using Raylib_cs;

namespace SomberInertia;

public enum BlockType
{
    Road,
    Grass,
    Forest,
    Hill,
    Mountain,
    Impassable,
    Water,
    Floor,
}

// A Block is a 3 X 3 arrangement of Tiles.
// Units will move and ineract on Blocks.
class Block
{
    public Texture2D Texture { get; set; }
    public BlockType BlockType { get; private set; }

    public Block(string texturePath, BlockType blockType)
    {
        Texture = Raylib.LoadTexture(texturePath);
        BlockType = blockType;
    }
}