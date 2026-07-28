using SomberInertia.Core.Units;

namespace SomberInertia.Core;

public class GiveContext
{
    public int GiverSlotIndex { get; set; } = -1; // index of item to trade in giver's item array
    public Unit? Recipient { get; set; }
    public int RecipientSlotIndex { get; set; } = -1; // See above; -1 = free-slot give

    public void Reset()
    {
        GiverSlotIndex = -1;
        Recipient = null;
        RecipientSlotIndex = -1;
    }
}
