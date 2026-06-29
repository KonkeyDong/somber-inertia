using SomberInertia.Core;
using SomberInertia.Graphics;
using System.Numerics;
using Raylib_cs;

namespace SomberInertia.State;

public class EnterBattleScreen : IGameState
{
    private Game _game;

    public EnterBattleScreen(Game game)
    {
        _game = game;
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
        _game.Renderer.Draw(scale, background, new Vector2(0, 200));
    }
}