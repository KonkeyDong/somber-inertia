namespace SomberInertia.Timers;

public class CommandIcon : ITimers
{
    private int _maxNumberOfFrames;
    private int _frameCounter;
    private int _currentIndex = 0;

    public CommandIcon(int frameCounter)
    {
        _maxNumberOfFrames = frameCounter;
        _frameCounter = frameCounter;
    }

    public void Tick()
    {
        _frameCounter--;

        if (_frameCounter <= 0)
        {
            _frameCounter = _maxNumberOfFrames;
            _currentIndex = (_currentIndex + 1) % 2;
        }
    }

    public int GetCurrentFrameIndex() => _currentIndex;

    public void Reset() => _frameCounter = _maxNumberOfFrames;
}