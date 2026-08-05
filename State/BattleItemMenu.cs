using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Enums;
using SomberInertia.Graphics;
using SomberInertia.Graphics.UI;
using SomberInertia.Core.Units;

namespace SomberInertia.State;

public class BattleItemMenu : IGameState
{
    private readonly Game _game;
    private Unit _currentUnit;

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
        _centerPosition = RadialMenuLayout.GetCenterPosition();
    }

    public void Exit()
    {
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

        if (Input.IsConfirmPressed())
        {
            ConfirmSelection();
        }

        if (Input.IsCancelPressed())
        {
            CancelSelection();
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
        Logger.Debug($"BattleItemMenu: Confirmed command {_selectedCommand}");

        if (_selectedCommand == CommandIconType.Use)
        {
            if (!_currentUnit.HasUsableItem())
            {
                GameStateManager.ShowMessageNotice(
                    GameConstants.MessageNotice.NoItem,
                    GameStateType.BattleItemMenu);
                return;
            }

            GameStateManager.ChangeStateType(GameStateType.UseWhichItem);
        }
        else if (_selectedCommand == CommandIconType.Drop)
        {
            GameStateManager.ChangeStateType(GameStateType.DropItem);
        }
        else if (_selectedCommand == CommandIconType.Give)
        {
            if (!_currentUnit.HasGiveableItem())
            {
                Logger.Warning("Give: no giveable items in inventory.");
                return;
            }

            GameStateManager.ChangeStateType(GameStateType.GiveWhichItem);
        }
        else // EQUIP
        {
            GameStateManager.ChangeStateType(GameStateType.EquipItem);
        }
    }

    private void CancelSelection()
    {
        GameStateManager.ChangeStateType(GameStateType.BattleActionMenu);
    }

    public void Update()
    {
        _game.Grid.RangeTint.Tick();
        _game.FlipFlop.Tick();
        CommandIcons.Tick();
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawMovementRange(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FlipFlop.IsOn);

        _centerPosition = RadialMenuLayout.GetCenterPosition();
        RadialMenuLayout.DrawCommandIcons(_game.Renderer, scale, _centerPosition, _commandByDirection);
        _game.Renderer.DrawBattleMenuMessage(
            scale,
            _selectedCommand.GetBaseName(),
            RadialMenuLayout.GetMenuMessagePosition(_centerPosition));
    }
}
