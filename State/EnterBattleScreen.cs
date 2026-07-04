using SomberInertia.Core;
using SomberInertia.Core.Combat;
using SomberInertia.Graphics;
using SomberInertia.Timers;
using System.Numerics;
using Raylib_cs;

namespace SomberInertia.State;

public class EnterBattleScreen : IGameState
{
    private Game _game;
    private BattleSpriteSet _forceMemberSpriteSet = new();
    private BattleSpriteSet _monsterSpriteSet = new();

    private Sprite sprite;
    private DelayIterator _delayIterator = new DelayIterator(GameConfig.Animations.IdleDelay);

    public EnterBattleScreen(Game game)
    {
        _game = game;

        sprite = new Sprite("Assets/Foregrounds/Rock.png", new FrameRect
            {
                X = 0,
                Y = 0,
                W = 96,
                H = 32
            });
    }

    public void Enter()
    {
        Logger.Warning("Need to separate attacker and defender sprites into monster and force positions.");
        var defenderSprites = BattleSpriteManager.Get(_game.AttackContext.Defender);
        var attackerSprites = BattleSpriteManager.Get(_game.AttackContext.Attacker);


        Logger.Warning("Need to add Max's sprites.");
        if (_game.AttackContext.Defender.Friendly)
        {
            _forceMemberSpriteSet = defenderSprites;
            _monsterSpriteSet = attackerSprites;
        }
        else
        {
            _monsterSpriteSet = defenderSprites;
            _forceMemberSpriteSet = attackerSprites;
        }
    }

    public void Exit()
    {

    }

    public void HandleInput()
    {

    }

    public void Update()
    {
        _delayIterator.Tick();
    }

    public void Draw(float scale)
    {
        Raylib.ClearBackground(Color.Black);

        var background = BattleBackgrounds.Frames[0];

        var battleScreenPosition = GameConstants.BASE_BACKGROUND_POSITION * scale;
        var foregroundPosition   = GameConstants.BASE_FOREGROUND_POSITION * scale;
        var unfriendlyPosition   = GameConstants.BASE_UNFRIENDLY_POSITION * scale;
        var friendlyPosition     = GameConstants.BASE_FRIENDLY_POSITION   * scale;

        _game.Renderer.Draw(scale, background, battleScreenPosition);
        _game.Renderer.Draw(scale, _monsterSpriteSet.GetIdleFrame(_delayIterator.CurrentIndex), unfriendlyPosition);
        _game.Renderer.Draw(scale, sprite, foregroundPosition);
        _game.Renderer.Draw(scale, _forceMemberSpriteSet.GetIdleFrame(_delayIterator.CurrentIndex), friendlyPosition);
    }
}