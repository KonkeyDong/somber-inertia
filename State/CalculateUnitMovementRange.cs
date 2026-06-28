using SomberInertia.Core;
using SomberInertia.Enums;
using SomberInertia.Core.Units;
using SomberInertia.Timers;
using SomberInertia.Core.Combat.StatusEffect;
using System.Numerics;

using Raylib_cs;

namespace SomberInertia.State;

public class CalculateUnitMovementRange : IGameState
{
    private readonly Game _game;
    private readonly Unit _currentUnit;
    private readonly CountdownTimer _countdownTimer;

    private bool _isPoisoned;
    private bool _isSleeping;

    private readonly Vector2 _messageBoxPosition = new Vector2(300, 300);
    private readonly string _poisonMessage;
    private readonly string _sleepMessage;
    private readonly string _awakeMessage;

    public CalculateUnitMovementRange(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
        _countdownTimer = new CountdownTimer(GameConfig.Animations.SwitchStateCountdownTimer);

        _poisonMessage = $"{_currentUnit.GetDisplayName()} suffers poison damage.";
        _sleepMessage = $"{_currentUnit.GetDisplayName()} is sleeping.";
        _awakeMessage = $"{_currentUnit.GetDisplayName()} has awoken.";
    }

    public void Enter()
    {
        Logger.Debug("CalculateUnitMovementRange::Enter() called.");

        _isPoisoned = _currentUnit.HasStatus<PoisonEffect>();
        _isSleeping = _currentUnit.HasStatus<SleepEffect>();

        if (_isPoisoned || _isSleeping)
        {
            _countdownTimer.Reset();
            _countdownTimer.Start();
        }
        else
        {
            ProceedToMovement();
        }
    }

    public void Exit() => Logger.Debug("CalculateUnitMovementRange::Exit() called.");

    public void HandleInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Z) || Raylib.IsKeyPressed(KeyboardKey.C))
        {
            _countdownTimer.Stop();
        }
    }

    public void Update()
    {
        _countdownTimer.Tick();
        _game.FrameFlipper.Tick();

        if ((_isPoisoned || _isSleeping) && !_countdownTimer.GetIsActive())
        {
            if (_isPoisoned)
            {
                _isPoisoned = false;
                _currentUnit.ProcessPoisonStatus();

                if (_currentUnit.IsDead())
                {
                    _game.SetFirstUnitDiedFromPoison();
                    GameStateManager.ChangeStateType(GameStateType.AnimateUnitDeaths);
                    return;
                }
            }

            if (_isSleeping)
            {
                _isSleeping = false;
                _currentUnit.ProcessSleepStatus();
                GameStateManager.ChangeStateType(GameStateType.EndTurn);
                return;
            }

            ProceedToMovement();
        }
    }

    private void ProceedToMovement()
    {
        _game.Grid.CalculateUnitMovementRange(_currentUnit);
        GameStateManager.ChangeStateType(GameStateType.UnitMoving);
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawMovementRange(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FrameFlipper.IsOn);

        if (_isPoisoned)
        {
            _game.Renderer.DrawBattleMenuMessage(scale, _poisonMessage, _messageBoxPosition);
        }

        if (_isSleeping)
        {
            var message = _currentUnit.GetStatusDuration<SleepEffect>() > 0 
                ? _sleepMessage 
                : _awakeMessage;

            _game.Renderer.DrawBattleMenuMessage(scale, message, _messageBoxPosition);
        }
    }
}