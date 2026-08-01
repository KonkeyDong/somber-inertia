using Raylib_cs;

namespace SomberInertia.Core;

/// <summary>
/// Shared action-key bindings for menus and prompts.
/// Confirm = Z or C; Cancel = X (see README controls).
/// </summary>
public static class Input
{
    public static bool IsConfirmPressed() =>
        Raylib.IsKeyPressed(KeyboardKey.Z) || Raylib.IsKeyPressed(KeyboardKey.C);

    public static bool IsCancelPressed() =>
        Raylib.IsKeyPressed(KeyboardKey.X);

    /// <summary>Confirm or cancel — used to dismiss notices.</summary>
    public static bool IsDismissPressed() =>
        IsConfirmPressed() || IsCancelPressed();

    /// <summary>
    /// Cycle a list index with Left (next) / Right (previous). No-op if count ≤ 1.
    /// Returns true when the index changed this frame.
    /// </summary>
    public static bool TryCycleIndex(ref int index, int count)
    {
        if (count <= 1)
        {
            return false;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Left))
        {
            index = (index + 1) % count;
            return true;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Right))
        {
            index = (index - 1 + count) % count;
            return true;
        }

        return false;
    }
}
