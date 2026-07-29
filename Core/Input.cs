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
}
