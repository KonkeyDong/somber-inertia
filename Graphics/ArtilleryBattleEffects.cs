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
    private static readonly Vector2[] ExplosionAnchors =
    {
        // Under force sprite
        new Vector2(190, 135),
        new Vector2(160, 130),
        new Vector2(140, 140),
        // Over force sprite
        new Vector2(130, 145),
        new Vector2(150, 150),
        new Vector2(170, 155),
        new Vector2(190, 155),
    };

    private static readonly int[] StartDelayFrames = { 3, 7, 11, 15, 19, 23, 27 };

    public static SequenceTimerSlot[] CreateSlots(BattleUnitSpriteSet forceSet)
    {
        var tickDelayAmount = GameConstants.Animations.ArtilleryTickDelay;
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
            var position = ExplosionAnchors[i];

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
}
