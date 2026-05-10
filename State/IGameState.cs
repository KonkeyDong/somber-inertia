namespace SomberInertia.State;

public interface IGameState
{
    void Enter(); // Called when entering this state
    void Exit();  // Called when leaving this state
    
    void HandleInput();
    void Update();
    void Draw(float scale);
}