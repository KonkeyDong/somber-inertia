namespace SomberInertia.Timers;

public class CountdownTimer : ITimers
{
    private int _maxNumberOfFrames { get; set; }
    private int _frameCounter { get; set; }
    private bool _isActive { get; set; }

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

    public bool GetIsActive() => _isActive;
    public void Start() => _isActive = true;
    public void Reset()
    {
        _frameCounter = _maxNumberOfFrames;
        _isActive = true;
    }
}