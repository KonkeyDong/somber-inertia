namespace SomberInertia.Timers;

/// <summary>
/// Steps through <paramref name="levels"/> forward then reverse on a fixed frame period.
/// </summary>
public class Oscillator<T> : ITimer
{
    private readonly T[] _levels;
    private readonly int _framesPerStep;
    private int _frameCounter;
    private int _currentIndex;
    private int _direction = 1; // 1 = forward, -1 = backward

    public T Current => _levels[_currentIndex];

    public Oscillator(T[] levels, int framesPerStep = 8)
    {
        if (levels == null || levels.Length == 0)
        {
            throw new ArgumentException("Oscillator requires at least one level.", nameof(levels));
        }

        _levels = levels;
        _framesPerStep = framesPerStep;
        _frameCounter = framesPerStep;
        _currentIndex = 0;
        _direction = 1;
    }

    public void Tick()
    {
        _frameCounter--;

        if (_frameCounter <= 0)
        {
            _frameCounter = _framesPerStep;

            _currentIndex += _direction;

            if (_currentIndex >= _levels.Length - 1)
            {
                _currentIndex = _levels.Length - 1;
                _direction = -1;
            }
            else if (_currentIndex <= 0)
            {
                _currentIndex = 0;
                _direction = 1;
            }
        }
    }

    public void Reset()
    {
        _currentIndex = 0;
        _direction = 1;
        _frameCounter = _framesPerStep;
    }
}
