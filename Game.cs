using SomberInertia.Enums;
using SomberInertia.Timers;
using System.Numerics;
using Raylib_cs;

namespace SomberInertia;

public class Game
{
    public Grid Grid { get; private set; }
    public Unit Player { get; private set; }
    public MovementRangeTint RangeTint { get; private set; }
    public int FrameCounter { get; private set; }

    private int _width { get; set; } = 264 * 3;
    private int _height { get; set; } = 240 * 3;
    private float _scale = 3.0f;

    public Game()
    {
        // Initialize core systems
        Grid = new Grid(11, 10);


        Player = new Unit("assets/max.png", "Max", MovementType.Warrior, 4);
        Player.Friendly = true;
        var goblin = new Unit("assets/goblin.png", "Goblin", MovementType.Warrior, 5);
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
        // Arrow keys
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

        // Window Resize with Ctrl + +/- 
        if (Raylib.IsKeyDown(KeyboardKey.LeftControl) || Raylib.IsKeyDown(KeyboardKey.RightControl))
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Equal))      // Ctrl + "+"
            {
                _scale += 1.0f;
                ResizeWindow();
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Minus))      // Ctrl + "-"
            {
                _scale = Math.Max(0.5f, _scale - 1.0f);
                ResizeWindow();
            }
        }
    }

    private void ResizeWindow()
    {
        _scale = Math.Clamp(_scale, 1.0f, 5.0f);

        _width = (int)(264 * _scale);
        _height = (int)(240 * _scale);

        Raylib.SetWindowSize(_width, _height);
        Grid.BlockSize = (int)(Grid.TileSize * _scale);

        Logger.Debug($"ResizeWindow() Window resized to {_width} x {_height} (Scale: {_scale:F2}x); BlockSize: {Grid.BlockSize}");
    }

    public void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.RayWhite);

        Grid.DrawBackground(RangeTint, _scale);
        Grid.DrawUnits(_scale);

        Raylib.EndDrawing();
    }
}