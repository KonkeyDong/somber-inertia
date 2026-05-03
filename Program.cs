using System;
using Raylib_cs;

namespace SomberInertia;
class Program
{
    static void Main(string[] args)
    {
        const int screenWidth = 800;
        const int screenHeight = 800;

        Raylib.InitWindow(screenWidth, screenHeight, "Somber Inertia");
        Raylib.SetTargetFPS(60);

        var max = new Unit("assets/max_8x.png", 10, 10);
        var grassTile = new Block("assets/grass_tile.png", BlockType.Grass);
        Block[][] background = new Block[4][]
        {
            new Block[] { grassTile, grassTile, grassTile, grassTile },
            new Block[] { grassTile, grassTile, grassTile, grassTile },
            new Block[] { grassTile, grassTile, grassTile, grassTile },
            new Block[] { grassTile, grassTile, grassTile, grassTile }
        };

        while(!Raylib.WindowShouldClose())
        {
            // 1) handle input
            HandleKeyPresses(max);

            // last) render graphics
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RayWhite);

            DrawBackground(background);

            Raylib.DrawTexture(max.Texture, max.X, max.Y, Color.White);
            Raylib.EndDrawing();   
        }
        
        Raylib.CloseWindow();
    }

    public static void DrawBackground(Block[][] background)
    {
        var x = 0;
        var y = 0;
        var width = 24 * 8;
        var height = 24 * 8;
        foreach (Block[] row in background)
        {
            x = 0;
            foreach (Block cell in row)
            {
                Raylib.DrawTexture(cell.Texture, x, y, Color.White);
                x += width;
            }

            y += height;
        }
    }

    public static void HandleKeyPresses(Unit unit)
    {
        int amount = 24 * 8;
        if (Raylib.IsKeyPressed(KeyboardKey.Up))
        {
            unit.Y -= amount;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Down))
        {
            unit.Y += amount;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Left))
        {
            unit.X -= amount;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Right))
        {
            unit.X += amount;
        }
    }
}