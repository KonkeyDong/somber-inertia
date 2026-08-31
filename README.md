# Somber Inertia

A recreation of the Shining Force series for Sega Genesis.

Made for education, not selling purposes. Gameplay is implemented without Unity, Godot, or any other engine. See the state machine diagram in `/State` for how the game flows. If you are lost, start with `/State/EndTurn.cs`.

You can purchase a copy of Shining Force off of Steam or play on Nintendo Switch Online+. **Please support the original release**!

## Note

This is not a one to one copy of the original Shining Force. Some balance is different (archers are much stronger against flying units, for example), and it includes quality of life improvements from Shining Force 2.

## Tech Stack

Requires .NET 10.0 and Raylib to run. You can install Raylib via NuGet:

```bash
dotnet add package Raylib-cs
```

## Run the Program

```bash
dotnet run
```

## Controls

* Arrow keys to move/select.
* `Z`, `C` to select; `X` to cancel.
