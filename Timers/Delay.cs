namespace SomberInertia.Timers;

/// <summary>
/// Advances <see cref="CurrentIndex"/> by one every N frames.
/// </summary>
public class Delay : ITimer
{
    private int _tickDelayAmount;
    private readonly int _originalTickDelayAmount;
    private int _startDelayFrames;
    private int _currentTick;
    private int _currentIndex;

    public int CurrentIndex => _currentIndex;

    public Delay(int tickDelayAmount)
    {
        _tickDelayAmount = tickDelayAmount;
        _startDelayFrames = 0;
        _originalTickDelayAmount = 0;
        Reset();
    }

    // Wait N number of frames before starting the timer. 
    // Useful when you have an array of Delay timers that all
    // need to start their timers at different times.
    public Delay(int tickDelayAmount, int startDelayFrames)
    {
        if (startDelayFrames < 0)
        {
            Logger.Error("Delay timer cannot have its StartDelayFrames amount be less than zero.");
        }

        _tickDelayAmount = tickDelayAmount;
        _startDelayFrames = startDelayFrames;
        _originalTickDelayAmount = startDelayFrames;

        Reset();
    }

    public void Tick()
    {
        // do not start the timer until the initial
        // _startDelayFrames have reached zero.
        if (_startDelayFrames > 0)
        {
            _startDelayFrames--;
            return;
        }

        _currentTick++;

        if (_currentTick >= _tickDelayAmount)
        {
            _currentIndex++;
            _currentTick = 0;
        }
    }

    public void Reset()
    {
        _currentTick = 0;
        _currentIndex = 0;
        _startDelayFrames = _originalTickDelayAmount;
    }

    public void ResetTimerOnly()
    {
        _currentTick = 0;
        _startDelayFrames = _originalTickDelayAmount;
    }
}
