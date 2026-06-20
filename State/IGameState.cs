namespace SomberInertia.State;

public interface IGameState
{
    void Enter();
    void Exit();

    void HandleInput();
    void Update();
    void Draw(float scale);
}