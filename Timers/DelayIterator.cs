namespace SomberInertia.Timers;

public class DelayIterator : ITimers
{
    private readonly int _delayFrames;
    private int _currentTick;
    private int _currentIndex;

    public int CurrentIndex => _currentIndex;

    public DelayIterator(int delayFrames)
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