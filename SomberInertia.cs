using System;
using Raylib_cs;

namespace ShiningForceRayLib
{
    public class Unit
    {
        public Texture2D Texture { get; set; }
        public int X { get; set; } // horizontal position (X coordinate)
        public int Y { get; set; } // vertical position (Y coordinate)

        public Unit(string texturePath, int x, int y)
        {
            Texture = Raylib.LoadTexture(texturePath);
            X = x;
            Y = y;
        }
    }

    class SomberInertia
    {
        static void Main(string[] args)
        {
            const int screenWidth = 800;
            const int screenHeight = 800;
 
            Raylib.InitWindow(screenWidth, screenHeight, "Somber Inertia");
            Raylib.SetTargetFPS(60);

            var max = new Unit("assets/max_8x.png", 10, 10);
            
            while(!Raylib.WindowShouldClose())
            {
                // 1) handle input
                HandleKeyPresses(max);

                // last) render graphics
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.RayWhite);

                Raylib.DrawTexture(max.Texture, max.X, max.Y, Color.White);
                Raylib.EndDrawing();   
            }
            
            Raylib.CloseWindow();
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
}