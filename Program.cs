using SomberInertia.Managers;
using Raylib_cs;

namespace SomberInertia;

class Program
{
    static void Main()
    {
        Logger.MinimumLevel = LogLevel.Info;

        var scale = GameConstants.BASE_WINDOW_SCALE;
        var width = (int)(GameConstants.BASE_WINDOW_WIDTH * scale);
        var height = (int)(GameConstants.BASE_WINDOW_HEIGHT * scale);

        Raylib.InitWindow(width, height, "Somber Inertia");
        Raylib.SetTargetFPS(60);

        var game = new Game();

        while (!Raylib.WindowShouldClose())
        {
            game.Update();
            game.Draw();
        }

        TextureManager.UnloadAll();
        Raylib.CloseWindow();
    }
}