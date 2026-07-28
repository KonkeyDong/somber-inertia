using SomberInertia.Enums;

namespace SomberInertia.Core;

public class MessageNoticeContext
{
    public string Message { get; set; } = "";
    public GameStateType ReturnState { get; set; } = GameStateType.BattleActionMenu;

    public void Reset()
    {
        Message = "";
        ReturnState = GameStateType.BattleActionMenu;
    }

    public void Set(string message, GameStateType returnState)
    {
        Message = message;
        ReturnState = returnState;
    }
}
