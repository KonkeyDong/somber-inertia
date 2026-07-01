using SomberInertia.Core;
using SomberInertia.Core.Combat;
using SomberInertia.Graphics;
using System.Numerics;
using Raylib_cs;

namespace SomberInertia.State;

public class EnterBattleScreen : IGameState
{
    private Game _game;
    private List<Sprite> _attackerSprites = new();
    private List<Sprite> _defenderSprites = new();

    private Vector2 _battleScreenPosition = new Vector2(0, 64 * 3);

    public EnterBattleScreen(Game game)
    {
        _game = game;

        // _attackerSprites = BattleSpriteManager.Get(_game.AttackContext.Attacker);
        _defenderSprites = BattleSpriteManager.Get(_game.AttackContext.Defender);
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
    }
}