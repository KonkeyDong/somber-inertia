using SomberInertia.Core;
using SomberInertia.Enums;
using Raylib_cs;

namespace SomberInertia.State;

public static class GameStateManager
{
    public static GameStateType CurrentStateType { get; private set; }
    public static IGameState _gameState { get; private set; }
    public static Game Game { get; private set; }

    public static int CurrentWidth = (int)(GameConstants.BASE_WINDOW_WIDTH * GameConstants.BASE_WINDOW_SCALE);
    public static int CurrentHeight = (int)(GameConstants.BASE_WINDOW_HEIGHT * GameConstants.BASE_WINDOW_SCALE);
    public static float CurrentScale = GameConstants.BASE_WINDOW_SCALE;

    public static void InitializeGameState(GameStateType gameStateType, Game game)
    {
        CurrentStateType = gameStateType;
        Game = game;

        ChangeStateType(gameStateType);
    }

    public static void ChangeStateType(GameStateType gameStateType)
    {
        Logger.Info($"ChangeGameState() updating game state from [{CurrentStateType}] to [{gameStateType}].");

        CurrentStateType = gameStateType;
        BuildGameState();
    }

    private static void BuildGameState()
    {
        IGameState newGameState = null;
        switch(CurrentStateType)
        {
            case GameStateType.UnitMoving:
                newGameState = new UnitMoving(Game);
                break;

            case GameStateType.CalculateUnitMovementRange:
                newGameState = new CalculateUnitMovementRange(Game);
                break;

            case GameStateType.CalculateWeaponAttackRange:
                newGameState = new CalculateWeaponAttackRange(Game);
                break;

            case GameStateType.BattleActionMenu:
                newGameState = new BattleActionMenu(Game);
                break;

            default:
                Logger.Error($"BuildGameState() unidentified game state {CurrentStateType}.");
                break;
        }

        if (_gameState != null)
        {
            _gameState.Exit();
        }

        _gameState = newGameState;
        _gameState.Enter();
    }

    private static void ResizeWindow()
    {
        CurrentScale = Math.Clamp(CurrentScale, 1.0f, 5.0f);

        CurrentWidth = (int)(GameConstants.BASE_WINDOW_WIDTH * CurrentScale);
        CurrentHeight = (int)(GameConstants.BASE_WINDOW_HEIGHT * CurrentScale);

        Raylib.SetWindowSize(CurrentWidth, CurrentHeight);
        Game.Grid.BlockSize = (int)(GameConstants.TILE_SIZE * CurrentScale);

        Logger.Info($"ResizeWindow() Window resized to {CurrentWidth} x {CurrentHeight} (Scale: {CurrentScale:F2}x); BlockSize: {Game.Grid.BlockSize}");
    }

    private static void HandleResizingWindow()
    {
        // Window Resize with Ctrl + +/- 
        if (Raylib.IsKeyDown(KeyboardKey.LeftControl) || Raylib.IsKeyDown(KeyboardKey.RightControl))
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Equal))      // Ctrl + "+"
            {
                CurrentScale += 1.0f;
                ResizeWindow();
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Minus))      // Ctrl + "-"
            {
                CurrentScale = Math.Max(0.5f, CurrentScale - 1.0f);
                ResizeWindow();
            }
        }
    }

    private static void HandleLoggingToggle()
    {
        // Logging toggle
        if (Raylib.IsKeyPressed(KeyboardKey.F1))
        {
            Logger.MinimumLevel = Logger.MinimumLevel == LogLevel.Debug 
                ? LogLevel.Info 
                : LogLevel.Debug;

            Logger.Info($"Logging level changed to: {Logger.MinimumLevel}");
        }
    }

    public static void HandleInput()
    {
        // generic input (happens regardless of state)
        HandleResizingWindow();
        HandleLoggingToggle();

        // game state specific input
        _gameState.HandleInput();
    }

    public static void Update()
    {
        _gameState.Update();
    }

    public static void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.RayWhite);

        _gameState.Draw(CurrentScale);

        Raylib.EndDrawing();
    }
}




