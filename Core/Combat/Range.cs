namespace SomberInertia.Core.Combat;

public readonly struct Range
{
    public int Min { get; init; }
    public int Max { get; init; }

    public Range(int min, int max)
    {
        if (min < 0 || max < min)
        {
            Logger.Error("Range(): Min cannot be less than zero; Max cannot be less than Min.");
        }

        Min = min;
        Max = max;
    }

    public override string ToString() => $"[<Range> Min: {Min}; Max: {Max}]]";
}