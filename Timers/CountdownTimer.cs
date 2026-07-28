namespace SomberInertia.Timers;

public class CountdownTimer : ITimers
{
    private int _maxNumberOfFrames;
    private int _frameCounter;
    private bool _isActive;

    public bool IsActive => _isActive;

    public CountdownTimer(int frameCounter)
    {
        _maxNumberOfFrames = frameCounter;
        _frameCounter = frameCounter;
        _isActive = true;
    }

    public void Tick()
    {
        if (_frameCounter == 0)
        {
            return; // no op
        }

        _frameCounter--;

        if (_frameCounter == 0)
        {
            _isActive = false;
        }
    }

    public void Stop()
    {
        _frameCounter = 0;
        _isActive = false;
    }

    public void Start() => _isActive = true;
    public void Reset()
    {
        _frameCounter = _maxNumberOfFrames;
        _isActive = true;
    }
}
