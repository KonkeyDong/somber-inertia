using Raylib_cs;

namespace SomberInertia;

class Program
{
    static void Main(string[] args)
    {
        // ============== LOGGING SETUP ==============
        Logger.MinimumLevel = LogLevel.Info;     // Change to Debug for more detail during development
        // Logger.MinimumLevel = LogLevel.Debug; // Uncomment for verbose logging

        const int screenWidth = 800;
        const int screenHeight = 800;

        Raylib.InitWindow(screenWidth, screenHeight, "Somber Inertia");
        Raylib.SetTargetFPS(60);

        Logger.Info("Game window initialized.");

        // ============== GAME INITIALIZATION ==============
        var grid = new Grid(4, 4);
        
        var max = new Unit("assets/max_8x.png", "Max", MovementType.Warrior);
        grid.AddUnit(max, 0, 0);

        Logger.Info("Game setup complete. Starting main loop...");

        // ============== MAIN GAME LOOP ==============
        while (!Raylib.WindowShouldClose())
        {
            // 1. Input
            HandleInput(grid, max);

            // 2. Update (add more game logic here later)

            // 3. Render
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RayWhite);

            grid.DrawBackground();
            grid.DrawUnits();

            Raylib.EndDrawing();
        }

        Logger.Info("Closing game window...");
        Raylib.CloseWindow();
    }

    public static void HandleInput(Grid grid, Unit unit)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Up))
            grid.MoveUnitInDirection(unit, Direction.Up);

        if (Raylib.IsKeyPressed(KeyboardKey.Down))
            grid.MoveUnitInDirection(unit, Direction.Down);

        if (Raylib.IsKeyPressed(KeyboardKey.Left))
            grid.MoveUnitInDirection(unit, Direction.Left);

        if (Raylib.IsKeyPressed(KeyboardKey.Right))
            grid.MoveUnitInDirection(unit, Direction.Right);
    }
}