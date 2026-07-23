using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Enums;
using SomberInertia.Graphics;
using SomberInertia.Core.Units;

namespace SomberInertia.State;

public class BattleItemMenu : IGameState
{
    private readonly Game _game;
    private Unit _currentUnit;

    // Command layout: type + relative offset from center (in tile units)
    private static readonly Dictionary<Direction, CommandIconType> _commandByDirection = new()
    {
        { Direction.Up,    CommandIconType.Use   },
        { Direction.Left,  CommandIconType.Give  },
        { Direction.Right, CommandIconType.Equip },
        { Direction.Down,  CommandIconType.Drop  }
    };
    private CommandIconType _selectedCommand = CommandIconType.Use;

    private Vector2 _centerPosition;

    public BattleItemMenu(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
    }

    public void Enter()
    {
        _currentUnit = _game.GetCurrentUnit();
        _selectedCommand = CommandIconType.Use;
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
        ) / GameStateManager.CurrentScale;

    }

    public void HandleInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Up))
        {
            SetSelectedCommand(CommandIconType.Use);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Down))
        {
            SetSelectedCommand(CommandIconType.Drop);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Left))
        {
            SetSelectedCommand(CommandIconType.Give);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Right))
        {
            SetSelectedCommand(CommandIconType.Equip);
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

        if (_selectedCommand == CommandIconType.Use)
        {
            // GameStateManager.ChangeStateType(GameStateType.CalculateWeaponAttackRange);
            Logger.Warning("Item::Use not implemented.");
        }
        else if (_selectedCommand == CommandIconType.Drop)
        {
            // GameStateManager.ChangeStateType(GameStateType.TransitionSelectorToNextUnit);
            Logger.Warning("Item::Drop not implemented.");
        }
        else if (_selectedCommand == CommandIconType.Give)
        {
            Logger.Warning("Item::Give not implemented.");
        }
        else
        {
            Logger.Warning("Item::Equip not implemented.");
        }
    }

    private void CancelMenu()
    {
        GameStateManager.ChangeStateType(GameStateType.BattleActionMenu);
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
            var position = _centerPosition + offset * GameConstants.TILE_SIZE;
            var sprite = CommandIcons.GetSprite(commandType);
            
            _game.Renderer.Draw(scale, sprite, position);
        }
        
        var messagePosition = _centerPosition;
        messagePosition.X += 65;
        messagePosition.Y += 18;

        _game.Renderer.DrawBattleMenuMessage(scale, _selectedCommand.GetBaseName(), messagePosition);
    }

    public void OnResize() => UpdateCenterPosition();
}