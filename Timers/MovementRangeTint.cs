namespace SomberInertia.Timers;

using Raylib_cs;

public class MovementRangeTint : ITimers
{
    private readonly int _maxNumberOfFrames;
    private int _frameCounter { get; set; }

    private int _currentIndex = 0;
    private int _direction = 1; // 1 = forward, -1 = backward

    private static readonly Color[] TintLevels =
    {
        new Color(255, 255, 255, 255),   // 0: Full bright (no tint)
        new Color(200, 220, 255, 200),   // 1: Light blue
        new Color(140, 180, 255, 180),   // 2: Medium blue
        new Color(80,  120, 255, 160)    // 3: Strong blue (~50% darken)
    };

    public MovementRangeTint(int framesPerTint = 8)
    {
        _maxNumberOfFrames = framesPerTint;
        _frameCounter = framesPerTint;
    }

    public void Tick()
    {
        _frameCounter--;

        if (_frameCounter <= 0)
        {
            _frameCounter = _maxNumberOfFrames;

            // Move index in current direction
            _currentIndex += _direction;

            // Reverse direction at the ends
            if (_currentIndex >= TintLevels.Length - 1)
            {
                _currentIndex = TintLevels.Length - 1;
                _direction = -1;        // Start going backwards
            }
            else if (_currentIndex <= 0)
            {
                _currentIndex = 0;
                _direction = 1;         // Start going forwards
            }
        }
    }

    public Color GetCurrentColor() => TintLevels[_currentIndex];

    // Optional: Reset to start
    public void Reset()
    {
        _currentIndex = 0;
        _direction = 1;
        _frameCounter = _maxNumberOfFrames;
    }
}