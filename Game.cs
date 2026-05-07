using SomberInertia.Enums;
using SomberInertia.Timers;
using System.Numerics;
using Raylib_cs;

namespace SomberInertia;

public class Game
{
    public Grid Grid { get; private set; }
    public Unit Player { get; private set; }
    public MovementRangeTint MovementRangeTint { get; private set; }
    public int FrameCounter { get; private set; }

    private int _currentWidth { get; set; }
    private int _currentHeight { get; set; }
    private float _currentScale { get; set; }

    public Game()
    {
        _currentWidth = (int)(GameConstants.BASE_WINDOW_WIDTH * GameConstants.BASE_WINDOW_SCALE);
        _currentHeight = (int)(GameConstants.BASE_WINDOW_HEIGHT * GameConstants.BASE_WINDOW_SCALE);
        _currentScale = GameConstants.BASE_WINDOW_SCALE;

        // Initialize core systems
        // map dimensions (in blocks)
        // Height: 10 (240 px)
        // Width: 11 (264 px)
        Grid = new Grid(11, 10);

        Player = new Unit("assets/max.png", "Max", MovementType.Warrior, 4);
        Player.Friendly = true;
        var goblin = new Unit("assets/goblin.png", "Goblin", MovementType.Warrior, 5);
        goblin.Friendly = false;
        Grid.AddUnit(Player, 0, 0);
        Grid.AddUnit(goblin, 2, 1);
        Grid.CalculateUnitMovementRange(Player);

        MovementRangeTint = new MovementRangeTint(6);   // 6 frames per tint step
    }

    public void Update()
    {
        FrameCounter++;

        HandleInput();
        MovementRangeTint.Tick();
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
                _currentScale += 1.0f;
                ResizeWindow();
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Minus))      // Ctrl + "-"
            {
                _currentScale = Math.Max(0.5f, _currentScale - 1.0f);
                ResizeWindow();
            }
        }
    }

    private void ResizeWindow()
    {
        _currentScale = Math.Clamp(_currentScale, 1.0f, 5.0f);

        _currentWidth = (int)(GameConstants.BASE_WINDOW_WIDTH * _currentScale);
        _currentHeight = (int)(GameConstants.BASE_WINDOW_HEIGHT * _currentScale);

        Raylib.SetWindowSize(_currentWidth, _currentHeight);
        Grid.BlockSize = (int)(GameConstants.TILE_SIZE * _currentScale);

        Logger.Debug($"ResizeWindow() Window resized to {_currentWidth} x {_currentHeight} (Scale: {_currentScale:F2}x); BlockSize: {Grid.BlockSize}");
    }

    public void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.RayWhite);

        Grid.DrawBackground(_currentScale);
        Grid.DrawMovementRange(_currentScale, MovementRangeTint.GetCurrentColor());
        Grid.DrawUnits(_currentScale);

        Raylib.EndDrawing();
    }
}