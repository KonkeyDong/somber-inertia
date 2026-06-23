using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Enums;
using SomberInertia.Graphics;
using SomberInertia.Core.Units;

namespace SomberInertia.State;

public class BattleActionMenu : IGameState
{
    private readonly Game _game;
    private Unit _currentUnit;

    // Command layout: type + relative offset from center (in tile units)
    private static readonly Dictionary<Direction, CommandIconType> _commandByDirection = new()
    {
        { Direction.Up,    CommandIconType.Attack },
        { Direction.Left,  CommandIconType.Magic  },
        { Direction.Right, CommandIconType.Item   },
        { Direction.Down,  CommandIconType.Stay   }
    };
    private CommandIconType _selectedCommand = CommandIconType.Attack;

    private Vector2 _centerPosition;

    public BattleActionMenu(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
    }

    public void Enter()
    {
        _currentUnit = _game.GetCurrentUnit();
        _selectedCommand = CommandIconType.Attack;
        CommandIcons.SetSelectedIcon(_selectedCommand);

        UpdateCenterPosition();
    }

    public void Exit()
    {
    }

    private void UpdateCenterPosition()
    {
        _centerPosition = new Vector2(
            GameStateManager.CurrentWidth / 2f,
            GameStateManager.CurrentHeight * 0.75f
        );
    }

    public void HandleInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Up))
        {
            SetSelectedCommand(CommandIconType.Attack);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Down))
        {
            SetSelectedCommand(CommandIconType.Stay);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Left))
        {
            SetSelectedCommand(CommandIconType.Magic);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Right))
        {
            SetSelectedCommand(CommandIconType.Item);
        }

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

        _selectedCommand = newCommand;
        CommandIcons.SetSelectedIcon(newCommand);
    }

    private void ConfirmSelection()
    {
        Logger.Debug($"BattleActionMenu: Confirmed command {_selectedCommand}");

        if (_selectedCommand == CommandIconType.Attack)
        {
            GameStateManager.ChangeStateType(GameStateType.CalculateWeaponAttackRange);

        }
        else if (_selectedCommand == CommandIconType.Stay)
        {
            GameStateManager.ChangeStateType(GameStateType.TransitionSelectorToNextUnit);
        }
        else if (_selectedCommand == CommandIconType.Magic)
        {
            if (_currentUnit.HasSpells)
            {
                GameStateManager.ChangeStateType(GameStateType.SelectMagic);
            }
            else
            {
                GameStateManager.ChangeStateType(GameStateType.NoMagicAvailable);
            }
        }
        else
        {
            Logger.Warning($"BattleActionMenu: Command {_selectedCommand} not yet implemented.");
        }
    }

    private void CancelMenu()
    {
        Logger.Debug("BattleActionMenu: Cancelled - returning to UnitMoving");
        GameStateManager.ChangeStateType(GameStateType.UnitMoving);
    }

    public void Update()
    {
        _game.Grid.RangeTint.Tick();
        _game.FrameFlipper.Tick();
        CommandIcons.Tick();
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawMovementRange(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FrameFlipper.IsOn);

        foreach (var (direction, commandType) in _commandByDirection)
        {
            var offset = direction.GetMenuOffset();
            var position = _centerPosition + offset * (GameConstants.TILE_SIZE * scale);

            var sprite = CommandIcons.GetSprite(commandType);
            _game.Renderer.Draw(scale, sprite, position);
        }

        _game.Renderer.DrawBattleMenuMessage(scale, _selectedCommand.GetBaseName(), new Vector2(_centerPosition.X + 200, _centerPosition.Y - 50));
    }

    public void OnResize() => UpdateCenterPosition();
}