using SomberInertia.Timers;

public class FrameFlipper : ITimers
{
    private readonly int _framesPerFlip;
    private int _counter;
    private bool _state;

    public bool IsOn => _state;

    public FrameFlipper(int framesPerFlip)
    {
        _framesPerFlip = framesPerFlip;
        _counter = 0;
        _state = false;
    }

    public void Tick()
    {
        _counter++;

        if (_counter >= _framesPerFlip)
        {
            _state = !_state;
            _counter = 0;
        }
    }

    public void Reset()
    {
        _state = false;
        _counter = 0;
    }
}