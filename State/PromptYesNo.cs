using System.Numerics;
using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Enums;
using SomberInertia.Graphics;
using SomberInertia.Core.Units;

namespace SomberInertia.State;

public class PromptYesNo : IGameState
{
    private readonly Game _game;
    private readonly Unit _currentUnit;

    private Vector2 _centerPosition;
    private bool _yesSelected = true;

    private const float IconSpacingTiles = 0.75f;

    public PromptYesNo(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
    }

    public void Enter()
    {
        _yesSelected = true;
        CommandIcons.SetSelectedIcon(CommandIconType.Yes);
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
        if (Raylib.IsKeyPressed(KeyboardKey.Left))
        {
            SetSelection(yes: true);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Right))
        {
            SetSelection(yes: false);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Z) || Raylib.IsKeyPressed(KeyboardKey.C))
        {
            ConfirmSelection();
        }

        if (Raylib.IsKeyPressed(KeyboardKey.X))
        {
            OnNo();
        }
    }

    private void SetSelection(bool yes)
    {
        if (_yesSelected == yes)
        {
            return;
        }

        _yesSelected = yes;
        CommandIcons.SetSelectedIcon(yes ? CommandIconType.Yes : CommandIconType.No);
    }

    private void ConfirmSelection()
    {
        if (_yesSelected)
        {
            OnYes();
        }
        else
        {
            OnNo();
        }
    }

    private void OnYes()
    {
        Logger.Debug($"PromptYesNo: Yes — action [{_game.Prompt.Action}].");

        switch (_game.Prompt.Action)
        {
            case PromptAction.DropItem:
                _currentUnit.RemoveItemAtIndex(_game.Prompt.ItemSlotIndex);
                break;

            case PromptAction.None:
            default:
                Logger.Warning("PromptYesNo: No prompt action set.");
                break;
        }

        var nextState = _game.Prompt.ReturnStateOnYes;
        _game.Prompt.Reset();
        GameStateManager.ChangeStateType(nextState);
    }

    private void OnNo()
    {
        Logger.Debug("PromptYesNo: No selected.");

        var nextState = _game.Prompt.ReturnStateOnNo;
        _game.Prompt.Reset();
        GameStateManager.ChangeStateType(nextState);
    }

    public void Update()
    {
        _game.FrameFlipper.Tick();
        CommandIcons.Tick();
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FrameFlipper.IsOn);

        var gap = GameConstants.TILE_SIZE * IconSpacingTiles;

        var yesPosition = new Vector2(_centerPosition.X - gap, _centerPosition.Y);
        var noPosition = new Vector2(_centerPosition.X + gap, _centerPosition.Y);

        _game.Renderer.Draw(scale, CommandIcons.GetSprite(CommandIconType.Yes), yesPosition);
        _game.Renderer.Draw(scale, CommandIcons.GetSprite(CommandIconType.No), noPosition);

        var messagePosition = _centerPosition;
        messagePosition.X += 65;
        messagePosition.Y += 10;

        var label = _yesSelected ? "Yes" : "No";
        _game.Renderer.DrawBattleMenuMessage(scale, label, messagePosition);
    }
}