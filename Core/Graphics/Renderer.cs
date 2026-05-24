using SomberInertia.Core;
using SomberInertia.Core.Units;
using System.Numerics;

using Raylib_cs;

namespace SomberInertia.Core.Graphics;

public class Renderer
{
    public Renderer()
    {

    }

    public void DrawBackground(float scale, Grid grid)
    {
        var debugFlag = Logger.MinimumLevel == LogLevel.Debug;

        var position = new Vector2();
        for (var x = 0; x < grid.Width; x++)
        {
            for (var y = 0; y < grid.Height; y++)
            {
                position.X = x * grid.BlockSize;
                position.Y = y * grid.BlockSize;

                Raylib.DrawTextureEx(
                    grid.Blocks[x, y].Texture,
                    position,
                    0.0f, // rotation
                    scale,
                    Color.White
                );

                if (debugFlag)
                {
                    Raylib.DrawText(grid.Blocks[x, y].PrintGridCoordinates(), (int)position.X, (int)position.Y + 20, 16, Color.White);
                }
            }
        }
    }

    public void DrawMovementRange(float scale, Grid grid) => DrawRangeBlockColor(scale, grid, grid.MovementRangeSet);
    public void DrawWeaponAttackRange(float scale, Grid grid) => DrawRangeBlockColor(scale, grid, grid.WeaponAttackRangeSet);
    private void DrawRangeBlockColor(float scale, Grid grid, HashSet<(int x, int y)> hashSet)
    {
        var debugFlag = Logger.MinimumLevel == LogLevel.Debug;

        var position = new Vector2();
        foreach ((var x, var y) in hashSet)
        {
            position.X = x * grid.BlockSize;
            position.Y = y * grid.BlockSize;

            Raylib.DrawTextureEx(
                grid.Blocks[x, y].Texture,
                position,
                0.0f,
                scale,
                grid.RangeTint.GetCurrentColor()
            );

            if (debugFlag)
            {
                Raylib.DrawText(grid.Blocks[x, y].PrintGridCoordinates(), (int)position.X, (int)position.Y + 20, 16, Color.White);
            }
        }
    }

    public void DrawHighlightRectangle(float scale, Vector2 newPosition)
    {
        var tileSize = GameConstants.TILE_SIZE * scale;

        var highlightRect = new Rectangle(
            newPosition.X,
            newPosition.Y,
            tileSize,
            tileSize
        );

        // Optional nice pulsing effect
        var pulse = 3f + MathF.Sin((float)Raylib.GetTime() * 10f) * 1.5f;

        Raylib.DrawRectangleLinesEx(highlightRect, scale, Color.White);
    }

    public void DrawUnits(float scale, Grid grid, List<Unit> units, bool frameFlipperFlag)
    {
        var position = new Vector2();

        // We loop in reverse to get the drawing order correct.
        // This allows current controlled unit to always be on top
        // of a block containing an occupant.
        for (var i = units.Count - 1; i >= 0; i--)
        {
            var unit = units[i];
            if (unit.Block == null)
            {
                Logger.Error($"Unit {unit.Name} has no Block reference!");
                continue;
            }

            position.X = unit.Block.X * grid.BlockSize;
            position.Y = unit.Block.Y * grid.BlockSize;

            var sprite = unit.GetFacingDirectionTexture(frameFlipperFlag);

            var source = new Rectangle(
                x: sprite.FrameRect.x,
                y: sprite.FrameRect.y,
                width: sprite.FrameRect.w,
                height: sprite.FrameRect.h
            );

            var dest = new Rectangle(
                x: position.X,
                y: position.Y,
                width: sprite.FrameRect.w * scale,
                height: sprite.FrameRect.h * scale
            );

            Raylib.DrawTexturePro(
                sprite.Texture,
                source,
                dest,
                new Vector2(0, 0),     // origin
                0.0f,                  // rotation
                Color.White
            );
        }
    }
}