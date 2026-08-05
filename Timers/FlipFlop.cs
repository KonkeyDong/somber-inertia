namespace SomberInertia.Timers;

/// <summary>
/// Toggles a two-phase bool every N frames (walk frames, icon blink, etc.).
/// </summary>
public class FlipFlop : ITimer
{
    private readonly int _framesPerPhase;
    private int _counter;
    private bool _state;

    public bool IsOn => _state;

    public FlipFlop(int framesPerPhase)
    {
        _framesPerPhase = framesPerPhase;
        _counter = 0;
        _state = false;
    }

    public void Tick()
    {
        _counter++;

        if (_counter >= _framesPerPhase)
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
