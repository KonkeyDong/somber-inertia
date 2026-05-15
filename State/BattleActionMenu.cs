using SomberInertia.Enums;
using SomberInertia.Graphics;
using Raylib_cs;
using System.Numerics;

using SomberInertia.Core;

namespace SomberInertia.State;

public class BattleActionMenu : IGameState
{
    private Game _game { get; set; }
    private Unit _currentUnit { get; set; }

    // positions: up, left, right, down
    private CommandIcon[] _icons = new CommandIcon[4]
    {
        new CommandIcon(CommandIconType.Attack),
        new CommandIcon(CommandIconType.Magic),
        new CommandIcon(CommandIconType.Item),
        new CommandIcon(CommandIconType.Stay)
    };
    private Vector2[] _positions = new Vector2[4]
    {
        new Vector2((GameStateManager.CurrentWidth / 2) + 24, ((3 * GameStateManager.CurrentHeight) / 4) - (24 * (int)GameStateManager.CurrentScale)),
        new Vector2((GameStateManager.CurrentWidth / 2) - (16 * (int)GameStateManager.CurrentScale), ((3 * GameStateManager.CurrentHeight) / 4)),
        new Vector2((GameStateManager.CurrentWidth / 2) + (32 * (int)GameStateManager.CurrentScale), ((3 * GameStateManager.CurrentHeight) / 4)),
        new Vector2((GameStateManager.CurrentWidth / 2) + 24, ((3 * GameStateManager.CurrentHeight) / 4) + (24 * (int)GameStateManager.CurrentScale)),
    };
    private enum _selectedCommandEnum
    {
        Attack = 0,
        Magic,
        Item,
        Stay
    }
    private int _selectedCommand { get; set; } = (int)_selectedCommandEnum.Attack;

    public BattleActionMenu(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
    }

    public void Enter()
    {
        // _game.Grid.RangeTint.Reset();
    }
    
    public void Exit()
    {
        
    }

    public void HandleInput()
    {
        // Arrow keys
        if (Raylib.IsKeyPressed(KeyboardKey.Up))
            SetSelectedCommand((int)_selectedCommandEnum.Attack);

        if (Raylib.IsKeyPressed(KeyboardKey.Down))
            SetSelectedCommand((int)_selectedCommandEnum.Stay);

        if (Raylib.IsKeyPressed(KeyboardKey.Left))
            SetSelectedCommand((int)_selectedCommandEnum.Magic);

        if (Raylib.IsKeyPressed(KeyboardKey.Right))
            SetSelectedCommand((int)_selectedCommandEnum.Item);
    }

    private void SetSelectedCommand(int command)
    {
        if (_selectedCommand != command)
        {
            _icons[_selectedCommand].Reset();
            _selectedCommand = command;
        }
    }

    public void Update()
    {
        _icons[_selectedCommand].Update();
        _game.Grid.RangeTint.Tick();
    }

    public void Draw(float scale)
    {
        _game.Grid.DrawBackground(scale);
        _game.Grid.DrawWeaponAttackRange(scale);
        _game.Grid.DrawUnits(_game.Units, scale);

        for (int i = 0; i < _icons.Count(); i++)
        {
            _icons[i].Draw(_positions[i], scale);
        }
    }
}