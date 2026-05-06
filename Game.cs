using SomberInertia.Enums;
using SomberInertia.Timers;
using Raylib_cs;

namespace SomberInertia;

public class Game
{
    public Grid Grid { get; private set; }
    public Unit Player { get; private set; }
    public MovementRangeTint RangeTint { get; private set; }
    public int FrameCounter { get; private set; }

    public Game()
    {
        // Initialize core systems
        Grid = new Grid(4, 4);

        Player = new Unit("assets/max_8x.png", "Max", MovementType.Warrior, 3);
        Player.Friendly = true;
        var goblin = new Unit("assets/goblin_8x.png", "Goblin", MovementType.Warrior, 5);
        goblin.Friendly = false;
        Grid.AddUnit(Player, 0, 0);
        Grid.AddUnit(goblin, 2, 1);
        Grid.CalculateUnitMovementRange(Player);

        RangeTint = new MovementRangeTint(6);   // 6 frames per tint step
    }

    public void Update()
    {
        FrameCounter++;

        HandleInput();
        RangeTint.Tick();
    }

    private void HandleInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Up))
            Grid.MoveUnitInDirection(Player, Direction.Up);

        if (Raylib.IsKeyPressed(KeyboardKey.Down))
            Grid.MoveUnitInDirection(Player, Direction.Down);

        if (Raylib.IsKeyPressed(KeyboardKey.Left))
            Grid.MoveUnitInDirection(Player, Direction.Left);

        if (Raylib.IsKeyPressed(KeyboardKey.Right))
            Grid.MoveUnitInDirection(Player, Direction.Right);

        // Logging toggle
        if (Raylib.IsKeyPressed(KeyboardKey.F1))
        {
            Logger.MinimumLevel = Logger.MinimumLevel == LogLevel.Debug 
                ? LogLevel.Info 
                : LogLevel.Debug;

            Logger.Info($"Logging level changed to: {Logger.MinimumLevel}");
        }
    }

    public void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.RayWhite);

        Grid.DrawBackground(RangeTint);
        Grid.DrawUnits();

        Raylib.EndDrawing();
    }
}