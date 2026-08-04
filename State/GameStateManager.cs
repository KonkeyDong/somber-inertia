using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Enums;

namespace SomberInertia.State;

public static class GameStateManager
{
    public static GameStateType CurrentStateType { get; private set; }
    public static IGameState? GameState { get; private set; }
    public static Game Game { get; private set; } = null!;

    public static int CurrentWidth { get; set; } = (int)(GameConstants.Window.Width * GameConstants.Window.Scale);
    public static int CurrentHeight { get; set; } = (int)(GameConstants.Window.Height * GameConstants.Window.Scale);
    public static float CurrentScale { get; set; } = GameConstants.Window.Scale;

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

    /// Prime a temporary notice message and transition to <see cref="GameStateType.MessageNotice"/>.
    public static void ShowMessageNotice(string message, GameStateType returnState)
    {
        Game.MessageNotice.Set(message, returnState);
        ChangeStateType(GameStateType.MessageNotice);
    }

    private static void BuildGameState()
    {
        IGameState newGameState = CurrentStateType switch
        {
            GameStateType.UnitMoving => new UnitMoving(Game),
            GameStateType.CalculateUnitMovementRange => new CalculateUnitMovementRange(Game),
            GameStateType.CalculateWeaponAttackRange => new CalculateWeaponAttackRange(Game),
            GameStateType.PrepareMagicTargets => new PrepareMagicTargets(Game),
            GameStateType.BattleActionMenu => new BattleActionMenu(Game),
            GameStateType.BattleItemMenu => new BattleItemMenu(Game),
            GameStateType.EndTurn => new EndTurn(Game),
            GameStateType.SelectEnemyForPhysicalAttack => new SelectEnemyForPhysicalAttack(Game),
            GameStateType.TransitionSelectorToNextUnit => new TransitionSelectorToNextUnit(Game),
            GameStateType.AnimateUnitDeaths => new AnimateUnitDeaths(Game),
            GameStateType.SelectMagic => new SelectMagic(Game),
            GameStateType.SelectMagicLevel => new SelectMagicLevel(Game),
            GameStateType.MessageNotice => new MessageNotice(Game),
            GameStateType.SelectMagicTargets => new SelectMagicTargets(Game),
            GameStateType.EnterBattleScreen => new EnterBattleScreen(Game),
            GameStateType.BattleResolution => new BattleResolution(Game),
            GameStateType.BattleResolutionDebug => new BattleResolutionDebug(Game),
            GameStateType.ExitBattleScreen => new ExitBattleScreen(Game),
            GameStateType.DropItem => new DropItem(Game),
            GameStateType.PromptYesNo => new PromptYesNo(Game),
            GameStateType.EquipItem => new EquipItem(Game),
            GameStateType.UseWhichItem => new UseWhichItem(Game),
            GameStateType.GiveWhichItem => new GiveWhichItem(Game),
            GameStateType.GiveItemToWhom => new GiveItemToWhom(Game),
            GameStateType.TradeWhichItemFromAdjacentNeighbor => new TradeWhichItemFromAdjacentNeighbor(Game),

            _ => throw new ArgumentOutOfRangeException(nameof(CurrentStateType), CurrentStateType, "Unknown game state")
        };

        GameState?.Exit();

        GameState = newGameState;
        GameState.Enter();
    }

    private static void ResizeWindow()
    {
        CurrentScale = Math.Clamp(CurrentScale, 1.0f, 5.0f);

        CurrentWidth = (int)(GameConstants.Window.Width * CurrentScale);
        CurrentHeight = (int)(GameConstants.Window.Height * CurrentScale);

        Raylib.SetWindowSize(CurrentWidth, CurrentHeight);
        Game.Grid.BlockSize = (int)(GameConstants.TileSize * CurrentScale);

        Logger.Info($"ResizeWindow() Window resized to {CurrentWidth} x {CurrentHeight} (Scale: {CurrentScale:F2}x); BlockSize: {Game.Grid.BlockSize}");
    }

    private static void HandleResizingWindow()
    {
        // Window Resize with Ctrl + +/- 
        if (Raylib.IsKeyDown(KeyboardKey.LeftControl) || Raylib.IsKeyDown(KeyboardKey.RightControl))
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Equal)) // Ctrl + "+"
            {
                CurrentScale += 1.0f;
                ResizeWindow();
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Minus)) // Ctrl + "-"
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
        GameState?.HandleInput();
    }

    public static void Update() => GameState?.Update();

    public static void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.RayWhite);

        GameState?.Draw(CurrentScale);

        Raylib.EndDrawing();
    }
}
