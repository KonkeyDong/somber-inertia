using SomberInertia.Enums;
using SomberInertia.Core;
using SomberInertia.Graphics;

using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

namespace SomberInertia.State;

public class BattleActionMenu : IGameState
{
    private readonly Game _game;
    private Unit _currentUnit;

    private readonly CommandIcon[] _icons;

    private CommandIconType _selectedCommand = CommandIconType.Attack;

    // Layout data — super easy to add more commands later
    private static readonly (CommandIconType Type, Vector2 Offset)[] _layout =
    [
        (CommandIconType.Attack, new Vector2( 0, -24)), // up
        (CommandIconType.Magic,  new Vector2(-24,   0)), // left
        (CommandIconType.Item,   new Vector2( 24,   0)), // right
        (CommandIconType.Stay,   new Vector2( 0,  24))  // down
    ];

    // Quick lookup: type → array index
    private static readonly Dictionary<CommandIconType, int> _typeToIndex = new()
    {
        { CommandIconType.Attack, 0 },
        { CommandIconType.Magic,  1 },
        { CommandIconType.Item,   2 },
        { CommandIconType.Stay,   3 }
    };

    private Vector2 _centerPosition;

    public BattleActionMenu(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();

        _icons = new CommandIcon[_layout.Length];
        for (var i = 0; i < _layout.Length; i++)
        {
            _icons[i] = new CommandIcon(_layout[i].Type);
        }
    }

    public void Enter()
    {
        _currentUnit = _game.GetCurrentUnit(); // refresh in case it changed
        _selectedCommand = CommandIconType.Attack;

        UpdateCenterPosition();
        
        foreach (var icon in _icons)
        {
            icon.Reset();
        }
    }

    public void Exit() { }

    private void UpdateCenterPosition()
    {
        _centerPosition = new Vector2(
            GameStateManager.CurrentWidth / 2f,
            3f * GameStateManager.CurrentHeight / 4f
        );
    }

    public void HandleInput()
    {
        // Directional selection (matches your original cross layout)
        if (Raylib.IsKeyPressed(KeyboardKey.Up))    { SetSelectedCommand(CommandIconType.Attack); }
        if (Raylib.IsKeyPressed(KeyboardKey.Down))  { SetSelectedCommand(CommandIconType.Stay); }
        if (Raylib.IsKeyPressed(KeyboardKey.Left))  { SetSelectedCommand(CommandIconType.Magic); }
        if (Raylib.IsKeyPressed(KeyboardKey.Right)) { SetSelectedCommand(CommandIconType.Item); }

        if (Raylib.IsKeyPressed(KeyboardKey.Z) || Raylib.IsKeyPressed(KeyboardKey.C))
        {
            ConfirmSelection();
        }

        if (Raylib.IsKeyPressed(KeyboardKey.X))
        {
            CancelMenu();
        }
    }

    private void SetSelectedCommand(CommandIconType newCommand)
    {
        if (_selectedCommand == newCommand) 
        {
            return;
        }

        // Reset the old icon's animation
        var oldIndex = _typeToIndex[_selectedCommand];
        _icons[oldIndex].Reset();

        _selectedCommand = newCommand;
    }

    private void ConfirmSelection()
    {
        Logger.Debug($"BattleActionMenu::ConfirmSelection() selected command: {_selectedCommand}.");
    }

    private void CancelMenu()
    {
       Logger.Debug($"BattleActionMenu::CancelMenu() called; returning to [{GameStateType.UnitMoving}] state.");
       GameStateManager.ChangeStateType(GameStateType.UnitMoving);
    }

    public void Update()
    {
        var currentIndex = _typeToIndex[_selectedCommand];
        _icons[currentIndex].Update();

        _game.Grid.RangeTint.Tick();
    }

    public void Draw(float scale)
    {
        _game.Grid.DrawBackground(scale);
        _game.Grid.DrawWeaponAttackRange(scale);
        _game.Grid.DrawUnits(_game.Units, scale);

        for (var i = 0; i < _icons.Length; i++)
        {
            var position = _centerPosition + _layout[i].Offset * scale;
            _icons[i].Draw(position, scale);
        }
    }

    // Call this from your main game loop if the window is resized
    public void OnResize() => UpdateCenterPosition();
}