namespace SomberInertia.Timers;

/// <summary>
/// Advances <see cref="CurrentIndex"/> by one every N frames.
/// </summary>
public class Delay : ITimer
{
    private readonly int _delayFrames;
    private int _currentTick;
    private int _currentIndex;

    public int CurrentIndex => _currentIndex;

    public Delay(int delayFrames)
    {
        _delayFrames = delayFrames;
        Reset();
    }

    public void Tick()
    {
        _currentTick++;

        if (_currentTick >= _delayFrames)
        {
            _currentIndex++;
            _currentTick = 0;
        }
    }

    public void Reset()
    {
        _currentTick = 0;
        _currentIndex = 0;
    }

    public void ResetTimerOnly()
    {
        _currentTick = 0;
    }
}
