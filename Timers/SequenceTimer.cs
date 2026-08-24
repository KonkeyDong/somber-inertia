namespace SomberInertia.Timers;

/// <summary>
/// Plays a finite frame sequence: optional start delay, then advances
/// <see cref="CurrentIndex"/> every N frames, then stops.
/// </summary>
public class SequenceTimer : ITimer
{
    private readonly int _frameCount;
    private readonly int _framesPerStep;
    private readonly int _originalStartDelayFrames;

    private int _startDelayFramesRemaining;
    private int _currentTick;
    private int _currentIndex;
    private bool _isComplete;

    public int CurrentIndex => _currentIndex;

    /// <summary>True after the start delay has finished.</summary>
    public bool HasStarted => _startDelayFramesRemaining <= 0;

    /// <summary>True after the last frame has been passed; stop drawing.</summary>
    public bool IsComplete => _isComplete;

    /// <summary>True while started and not complete (safe to draw).</summary>
    public bool IsPlaying => HasStarted && !IsComplete;

    public int TotalDurationFrames => _originalStartDelayFrames + _frameCount * _framesPerStep;

    /// <param name="frameCount">Number of frames in the animation (must be &gt; 0).</param>
    /// <param name="framesPerStep">Ticks to wait on each frame before advancing.</param>
    /// <param name="startDelayFrames">Ticks to wait before the sequence starts.</param>
    public SequenceTimer(int frameCount, int framesPerStep, int startDelayFrames = 0)
    {
        if (frameCount <= 0)
        {
            Logger.Error("SequenceTimer: frameCount must be greater than zero.");
        }

        if (framesPerStep <= 0)
        {
            Logger.Error("SequenceTimer: framesPerStep must be greater than zero.");
        }

        if (startDelayFrames < 0)
        {
            Logger.Error("SequenceTimer: startDelayFrames cannot be less than zero.");
        }

        _frameCount = frameCount;
        _framesPerStep = framesPerStep;
        _originalStartDelayFrames = startDelayFrames;

        Reset();
    }

    public void Tick()
    {
        if (_isComplete)
        {
            return;
        }

        // Hold before the animation starts
        if (_startDelayFramesRemaining > 0)
        {
            _startDelayFramesRemaining--;
            return;
        }

        _currentTick++;

        if (_currentTick < _framesPerStep)
        {
            return;
        }

        _currentTick = 0;
        _currentIndex++;

        // Finished after leaving the last valid frame (0 .. frameCount-1)
        if (_currentIndex >= _frameCount)
        {
            _isComplete = true;
            _currentIndex = _frameCount - 1; // clamp; draw uses IsPlaying
        }
    }

    public void Reset()
    {
        _currentTick = 0;
        _currentIndex = 0;
        _startDelayFramesRemaining = _originalStartDelayFrames;
        _isComplete = false;
    }

    public void ResetTimerOnly()
    {
        _currentTick = 0;
        _startDelayFramesRemaining = _originalStartDelayFrames;
        _isComplete = false;
        // keeps CurrentIndex — only use if you intentionally want that
    }

    /// <summary>
    /// Jump to the state after <paramref name="absoluteFrame"/> ticks from reset.
    /// Used by battle debug scrubbing so FX match a scrubbed battle frame.
    /// </summary>
    public void Seek(int absoluteFrame)
    {
        Reset();
        var frames = Math.Max(0, absoluteFrame);
        for (var i = 0; i < frames; i++)
        {
            Tick();
        }
    }
}