using System.Numerics;
using SomberInertia.Core.Graphics;
using SomberInertia.Timers;

namespace SomberInertia.Graphics;

public readonly struct SequenceTimerSlot
{
    public readonly SequenceTimer SequenceTimer;
    public readonly Vector2 Position;

    public SequenceTimerSlot(SequenceTimer sequenceTimer, Vector2 position)
    {
        SequenceTimer = sequenceTimer;
        Position = position;
    }
}

/// <summary>
/// Shared Artillery explosion timeline for live battle and debug scrubbing.
/// Positions = force <see cref="BattleUnitSpriteSet.BasePosition"/> +
/// (idle frame W × xFrac, idle frame H × yFrac) for the unit on screen.
/// </summary>
public static class ArtilleryBattleEffects
{
    // Rough anchors as fractions of the current force idle frame (top-left = BasePosition).
    // Values > 1 sit slightly past the right/bottom edge of the frame.
    private static readonly Vector2[] ExplosionAnchors =
    {
        // Under force sprite
        new Vector2(0.90f, 0.60f),
        new Vector2(0.83f, 0.65f),
        new Vector2(0.61f, 0.71f),
        // Over force sprite
        new Vector2(0.36f, 0.92f),
        new Vector2(0.68f, 1.07f),
        new Vector2(0.85f, 1.07f),
        new Vector2(0.95f, 0.95f),
    };

    private static readonly int[] StartDelayFrames = { 3, 7, 11, 15, 19, 23, 27 };

    public static SequenceTimerSlot[] CreateSlots(BattleUnitSpriteSet forceSet)
    {
        var tickDelayAmount = 3;
        var numFrames = ArtilleryExplosion.Frames.Count;
        if (numFrames <= 0)
        {
            Logger.Error("ArtilleryBattleEffects.CreateSlots: no explosion frames loaded.");
            return Array.Empty<SequenceTimerSlot>();
        }

        if (forceSet == null)
        {
            Logger.Error("ArtilleryBattleEffects.CreateSlots: forceSet is null.");
            return Array.Empty<SequenceTimerSlot>();
        }

        var basePos = forceSet.BasePosition;
        GetForceFrameSize(forceSet, out var frameW, out var frameH);

        var slots = new SequenceTimerSlot[ExplosionAnchors.Length];
        for (var i = 0; i < ExplosionAnchors.Length; i++)
        {
            var anchor = ExplosionAnchors[i];
            var position = new Vector2(
                basePos.X + frameW * anchor.X,
                basePos.Y + frameH * anchor.Y);

            slots[i] = new SequenceTimerSlot(
                new SequenceTimer(numFrames, tickDelayAmount, StartDelayFrames[i]),
                position);
        }

        Logger.Debug(
            $"ArtilleryBattleEffects: force frame {frameW}x{frameH} at {basePos}, " +
            $"{slots.Length} explosion anchors.");

        return slots;
    }

    private static void GetForceFrameSize(BattleUnitSpriteSet forceSet, out int width, out int height)
    {
        // Prefer the on-screen unit's idle frame from assets; attack frame as fallback.
        if (forceSet.Idle != null && forceSet.Idle.Count > 0)
        {
            var rect = forceSet.GetIdleFrame(0).FrameRect;
            width = Math.Max(1, rect.W);
            height = Math.Max(1, rect.H);
            return;
        }

        if (forceSet.Attack != null && forceSet.Attack.Count > 0)
        {
            var rect = forceSet.GetAttackFrame(0).FrameRect;
            width = Math.Max(1, rect.W);
            height = Math.Max(1, rect.H);
            return;
        }

        Logger.Warning("ArtilleryBattleEffects: no idle/attack frames; using 1x1 fallback size.");
        width = 1;
        height = 1;
    }

    public static int MaxDuration(SequenceTimerSlot[] slots)
    {
        if (slots == null || slots.Length == 0)
        {
            return 0;
        }

        return slots.Max(slot => slot.SequenceTimer.TotalDurationFrames);
    }

    public static void SeekAll(SequenceTimerSlot[] slots, int battleFrame)
    {
        if (slots == null)
        {
            return;
        }

        for (var i = 0; i < slots.Length; i++)
        {
            slots[i].SequenceTimer.Seek(battleFrame);
        }
    }

    public static void DrawRange(
        float scale,
        Renderer renderer,
        SequenceTimerSlot[] slots,
        List<Sprite> frames,
        int startIndex,
        int endIndex)
    {
        if (slots == null || frames == null || frames.Count == 0)
        {
            return;
        }

        endIndex = Math.Min(endIndex, slots.Length);

        for (var i = startIndex; i < endIndex; i++)
        {
            var slot = slots[i];
            var timer = slot.SequenceTimer;

            if (!timer.IsPlaying)
            {
                continue;
            }

            var frameIndex = Math.Clamp(timer.CurrentIndex, 0, frames.Count - 1);
            renderer.Draw(scale, frames[frameIndex], slot.Position);
        }
    }
}
