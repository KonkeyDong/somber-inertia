using Raylib_cs;

namespace SomberInertia;

public enum MovementType
{
    Warrior,
    Flyer,
    Horse,
    Mage,
    Thief,
    Archer,
    Werewolf,
}

public class Unit
{
    public Texture2D Texture { get; set; }
    public int X { get; set; } // horizontal position (X coordinate)
    public int Y { get; set; } // vertical position (Y coordinate)
    public MovementType MovementType { get; private set; }

    public Unit(string texturePath, int x, int y)
    {
        Texture = Raylib.LoadTexture(texturePath);
        X = x;
        Y = y;
    }
}