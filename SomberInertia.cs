using System;
using Raylib_cs;

namespace ShiningForceRayLib
{
    class SomberInertia
    {
        static void Main(string[] args)
        {
            const int screenWidth = 800;
            const int screenHeight = 800;

            Raylib.InitWindow(screenWidth, screenHeight, "Somber Inertia");
            Raylib.SetTargetFPS(60);

            var max = Raylib.LoadTexture("assets/max_8x.png");
            
            while(!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.RayWhite);
                Raylib.DrawText("It works", 200, 200, 30, Color.DarkBlue);
                Raylib.DrawTexture(max, 10, 10, Color.White);
                Raylib.EndDrawing();   
            }
            
            Raylib.CloseWindow();
        }
    }
}