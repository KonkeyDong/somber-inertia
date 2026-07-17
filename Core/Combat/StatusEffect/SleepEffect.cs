using SomberInertia.Core.Units;

namespace SomberInertia.Core.Combat.StatusEffect;

public class SleepEffect : StatusEffect
{
    private static readonly Random _random = new Random();

    public SleepEffect()
    {
        Duration = _random.Next(GameConstants.StatusEffects.SLEEP_DURATION); // 1 - 3 turns (turn 0 shows the awak message and skips turn)
    }

    public override void Process(Unit unit)
    {
        Duration--;
    }
}