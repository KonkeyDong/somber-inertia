using Raylib_cs;

namespace SomberInertia;

class Program
{
    static void Main()
    {
        Logger.MinimumLevel = LogLevel.Info;

        Raylib.InitWindow(800, 800, "Somber Inertia");
        Raylib.SetTargetFPS(60);

        var game = new Game();

        while (!Raylib.WindowShouldClose())
        {
            game.Update();
            game.Draw();
        }

        Raylib.CloseWindow();
    }
}