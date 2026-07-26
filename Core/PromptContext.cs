using SomberInertia.Enums;

namespace SomberInertia.Core;

public class PromptContext
{
    public PromptAction Action { get; set; } = PromptAction.None;
    public GameStateType ReturnStateOnNo { get; set; }
    public GameStateType ReturnStateOnYes { get; set; }

    // Action-specific
    public int ItemSlotIndex { get; set; } = -1;

    public void Reset()
    {
        Action = PromptAction.None;
        ItemSlotIndex = -1;
        ReturnStateOnNo = GameStateType.BattleActionMenu;
        ReturnStateOnYes = GameStateType.BattleActionMenu;
    }
}