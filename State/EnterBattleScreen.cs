using SomberInertia.Core;
using SomberInertia.Core.Combat;
using SomberInertia.Graphics;
using System.Numerics;
using Raylib_cs;

namespace SomberInertia.State;

public class EnterBattleScreen : IGameState
{
    private Game _game;
    private List<Sprite> _forceMemberSprites = new();
    private List<Sprite> _monsterSprites = new();

    // rough positions
    private Vector2 _battleScreenPosition = new Vector2(0, 64 * 3);
    private Vector2 _unfriendlyPosition = new Vector2(160, (64 * 3) + 50);
    // private Vector2 _friendlyPosition = new Vector2();
    private Vector2 _foregroundPosition = new Vector2(378, 450);
    private Sprite sprite;

    public EnterBattleScreen(Game game)
    {
        _game = game;

        // _attackerSprites = BattleSpriteManager.Get(_game.AttackContext.Attacker);
        Logger.Warning("Need to separate attacker and defender sprites into monster and force positions.");
        var defenderSprites = BattleSpriteManager.Get(_game.AttackContext.Defender);
        // var attackerSprites = BattleSpriteManager.Get(_game.AttackContext.Attacker);

        sprite = new Sprite("Assets/Foregrounds/Rock.png", new FrameRect
            {
                x = 0,
                y = 0,
                w = 96,
                h = 32
            });

        Logger.Warning("Need to add Max's sprites.");
        if (_game.AttackContext.Defender.Friendly)
        {
            _forceMemberSprites = defenderSprites;
        }
        else
        {
            _monsterSprites = defenderSprites;
        }
    }

    public void Enter()
    {
        
    }

    public void Exit()
    {

    }

    public void HandleInput()
    {

    }

    public void Update()
    {

    }

    public void Draw(float scale)
    {
        Raylib.ClearBackground(Color.Black);

        var background = BattleBackgrounds.Frames[0];
        _game.Renderer.Draw(scale, background, _battleScreenPosition);

        _game.Renderer.Draw(scale, _monsterSprites[0], _unfriendlyPosition);
        _game.Renderer.Draw(scale, sprite, _foregroundPosition);
    }
}