using SomberInertia.Core;
using SomberInertia.Core.Combat;
using SomberInertia.Graphics;
using System.Numerics;
using Raylib_cs;

namespace SomberInertia.State;

public class EnterBattleScreen : IGameState
{
    private Game _game;
    private BattleSpriteSet _forceMemberSpriteSet = new();
    private BattleSpriteSet _monsterSpriteSet = new();

    // rough positions
    private Vector2 _battleScreenPosition = new Vector2(0, 64 * 3);
    private Vector2 _unfriendlyPosition = new Vector2(160, (64 * 3) + 50);
    private Vector2 _friendlyPosition = new Vector2(478, 300);
    private Vector2 _foregroundPosition = new Vector2(378, 450);
    private Sprite sprite;

    private FrameFlipper Flipper = new FrameFlipper(GameConfig.Animations.IdleDelay);

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
        Flipper.Tick();
    }

    public void Draw(float scale)
    {
        Raylib.ClearBackground(Color.Black);

        var background = BattleBackgrounds.Frames[0];
        _game.Renderer.Draw(scale, background, _battleScreenPosition);

        var frameIndex = Flipper.IsOn ? 1 : 0;

        _game.Renderer.Draw(scale, _monsterSpriteSet.GetIdleFrame(frameIndex), _unfriendlyPosition);
        _game.Renderer.Draw(scale, sprite, _foregroundPosition);
        _game.Renderer.Draw(scale, _forceMemberSpriteSet.GetIdleFrame(frameIndex), _friendlyPosition);
    }
}