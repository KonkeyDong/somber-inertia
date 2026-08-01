using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Enums;
using SomberInertia.Graphics;
using SomberInertia.Graphics.UI;
using SomberInertia.Core.Units;

namespace SomberInertia.State;

public class BattleActionMenu : IGameState
{
    private readonly Game _game;
    private Unit _currentUnit;

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
        _centerPosition = RadialMenuLayout.GetCenterPosition();
    }

    public void Exit()
    {
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
                GameStateManager.ShowMessageNotice(
                    GameConstants.MessageNotice.NoMagic,
                    GameStateType.BattleActionMenu);
            }
        }
        else // Item
        {
            if (_currentUnit.HasGiveableItem())
            {
                GameStateManager.ChangeStateType(GameStateType.BattleItemMenu);
            }
            else
            {
                GameStateManager.ShowMessageNotice(
                    GameConstants.MessageNotice.NoItem,
                    GameStateType.BattleActionMenu);
            }
        }
    }

    private void CancelSelection()
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

        // Refresh center each draw so Ctrl+/- scale stays correct.
        _centerPosition = RadialMenuLayout.GetCenterPosition();
        RadialMenuLayout.DrawCommandIcons(_game.Renderer, scale, _centerPosition, _commandByDirection);
        _game.Renderer.DrawBattleMenuMessage(
            scale,
            _selectedCommand.GetBaseName(),
            RadialMenuLayout.GetMenuMessagePosition(_centerPosition));
    }
}
