using SomberInertia.Core;
using SomberInertia.Graphics;
using SomberInertia.Core.Units;
using SomberInertia.Enums;
using SomberInertia.Core.Combat;
using SomberInertia.Core.Combat.Spells;
using System.Numerics;

using Raylib_cs;

namespace SomberInertia.Core.Graphics;

public class Renderer
{
    public Renderer()
    {

    }

    public void Draw(float scale, Sprite sprite, Vector2 position)
    {
        var source = new Rectangle(
            x: sprite.FrameRect.X,
            y: sprite.FrameRect.Y,
            width: sprite.FrameRect.W,
            height: sprite.FrameRect.H
        );

        var dest = new Rectangle(
            x: position.X,
            y: position.Y,
            width: sprite.FrameRect.W * scale,
            height: sprite.FrameRect.H * scale
        );

        Raylib.DrawTexturePro(
            sprite.Texture,
            source,
            dest,
            GameConfig.Textures.BaseOrigin,
            GameConfig.Textures.BaseRotation,
            GameConfig.Textures.ClearColor
        );
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
                    GameConfig.Textures.BaseRotation,
                    scale,
                    GameConfig.Textures.ClearColor
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
    public void DrawMagicAttackRange(float scale, Grid grid) => DrawRangeBlockColor(scale, grid, grid.MagicAttackRangeSet);
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
                GameConfig.Textures.BaseRotation,
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

        Raylib.DrawRectangleLinesEx(highlightRect, scale, Color.White);
    }

    // New overload for AoE
    public void DrawHighlightRectangle(float scale, List<Vector2> positions)
    {
        foreach (var pos in positions)
        {
            DrawHighlightRectangle(scale, pos);
        }
    }

    public void DrawUnit(float scale, Grid grid, Unit unit, bool frameFlipperFlag)
    {
        var position = new Vector2();

        if (unit.Block == null)
        {
            Logger.Error($"Unit {unit.Name} has no Block reference!");
            return;
        }

        position = unit.WorldPosition;
        var sprite = unit.GetFacingDirectionTexture(frameFlipperFlag);

        Draw(scale, sprite, position);
    }

    public void DrawUnits(float scale, Grid grid, List<Unit> units, bool frameFlipperFlag)
    {
        // We loop in reverse to get the drawing order correct.
        // This allows current controlled unit to always be on top
        // of a block containing an occupant.
        for (var i = units.Count - 1; i >= 0; i--)
        {
            DrawUnit(scale, grid, units[i], frameFlipperFlag);
        }
    }

    public void DrawMagicIcon(float scale, MagicFamily family, Vector2 position)
    {
        var sprite = MagicIcons.GetSprite(family);
        Draw(scale, sprite, position);
    }

    public void DrawBattleMenuMessage(float scale, string text, Vector2 textPos)
    {
        var fontSize = (int)(8 * scale);
        var textColor = Color.White;

        var textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), text, fontSize, 1);
        var padding = 14;

        var boxWidth = (int)textSize.X + padding * 2;
        var boxHeight = (int)textSize.Y + padding * 2;
        var boxX = (int)textPos.X - padding;
        var boxY = (int)textPos.Y - padding;

        // Colors from your config
        var darkOrange = GameConfig.Textures.DarkOrange;
        var lightOrange = GameConfig.Textures.LightOrange;
        var offWhite = GameConfig.Textures.OffWhite;
        var blue = GameConfig.Textures.Blue;

        // === Outer dark orange border ===
        Raylib.DrawRectangle(boxX, boxY, boxWidth, boxHeight, darkOrange);

        // === Light orange bevel (top + left) ===
        Raylib.DrawRectangle(boxX, boxY, boxWidth, 3, lightOrange);
        Raylib.DrawRectangle(boxX, boxY, 3, boxHeight, lightOrange);

        // === OffWhite inner layer ===
        var offWhiteX = boxX + 3;
        var offWhiteY = boxY + 3;
        var offWhiteW = boxWidth - 6;
        var offWhiteH = boxHeight - 6;

        Raylib.DrawRectangle(offWhiteX, offWhiteY, offWhiteW, offWhiteH, offWhite);

        // === Blue fill ===
        var blueX = offWhiteX + 3;
        var blueY = offWhiteY + 3;
        var blueW = offWhiteW - 6;
        var blueH = offWhiteH - 6;

        Raylib.DrawRectangle(blueX, blueY, blueW, blueH, blue);

        // === Draw text centered ===
        var finalTextPos = new Vector2(
            boxX + (boxWidth - textSize.X) / 2,
            boxY + (boxHeight - textSize.Y) / 2
        );

        Raylib.DrawTextEx(Raylib.GetFontDefault(), text, finalTextPos, fontSize, 1, textColor);
    }

    public void DrawSpellInfoBox(float scale, Magic spell, Vector2 position, bool highlightLevel = false)
    {
        var spellName = spell.Name.GetBaseName();
        var level = spell.Level;
        var mpCost = spell.MPCost;

        var fontSize = (int)(8 * scale);
        var textColor = Color.White;

        // Prepare lines
        var line1 = spellName;
        var line2 = $"Level {level}";
        var line3Left = "MP";
        var line3Right = mpCost.ToString();

        // Measure text
        var size1 = Raylib.MeasureTextEx(Raylib.GetFontDefault(), line1, fontSize, 1);
        var size2 = Raylib.MeasureTextEx(Raylib.GetFontDefault(), line2, fontSize, 1);
        var sizeLeft = Raylib.MeasureTextEx(Raylib.GetFontDefault(), line3Left, fontSize, 1);
        var sizeRight = Raylib.MeasureTextEx(Raylib.GetFontDefault(), line3Right, fontSize, 1);

        var padding = 12;
        var lineSpacing = 4;
        var leftMargin = 8;

        // Calculate box dimensions
        var contentWidth = Math.Max(size1.X, Math.Max(size2.X, sizeLeft.X + sizeRight.X + 20));
        var contentHeight = size1.Y + size2.Y + sizeLeft.Y + (lineSpacing * 2);

        var boxWidth = (int)contentWidth + padding * 2;
        var boxHeight = (int)contentHeight + padding * 2;

        var boxX = (int)position.X - padding;
        var boxY = (int)position.Y - padding;

        // === Border layers ===
        var darkOrange = GameConfig.Textures.DarkOrange;
        var lightOrange = GameConfig.Textures.LightOrange;
        var offWhite = GameConfig.Textures.OffWhite;
        var blue = GameConfig.Textures.Blue;

        Raylib.DrawRectangle(boxX, boxY, boxWidth, boxHeight, darkOrange);
        Raylib.DrawRectangle(boxX, boxY, boxWidth, 3, lightOrange);
        Raylib.DrawRectangle(boxX, boxY, 3, boxHeight, lightOrange);

        var innerX = boxX + 3;
        var innerY = boxY + 3;
        var innerW = boxWidth - 6;
        var innerH = boxHeight - 6;

        Raylib.DrawRectangle(innerX, innerY, innerW, innerH, offWhite);

        var fillX = innerX + 3;
        var fillY = innerY + 3;
        var fillW = innerW - 6;
        var fillH = innerH - 6;

        Raylib.DrawRectangle(fillX, fillY, fillW, fillH, blue);

        // === Text positioning ===
        var textStartY = fillY + 6;
        var textLeftX = fillX + leftMargin;

        // Line 1 - Name (left justified)
        Raylib.DrawTextEx(Raylib.GetFontDefault(), line1, new Vector2(textLeftX, textStartY), fontSize, 1, textColor);

        // Line 2 - Level (with optional red highlight)
        var text2Y = textStartY + size1.Y + lineSpacing;

        if (highlightLevel)
        {
            var highlightPadding = 2;
            var highlightX = (int)(textLeftX - highlightPadding);
            var highlightY = (int)(text2Y - highlightPadding);
            var highlightWidth = (int)(size2.X + (highlightPadding * 2));
            var highlightHeight = (int)(size2.Y + (highlightPadding * 2));

            Raylib.DrawRectangle(highlightX, highlightY, highlightWidth, highlightHeight, GameConfig.Textures.DarkRed);
        }

        Raylib.DrawTextEx(Raylib.GetFontDefault(), line2, new Vector2(textLeftX, text2Y), fontSize, 1, textColor);

        // Line 3 - MP left + Cost right
        var text3Y = text2Y + size2.Y + lineSpacing;
        var text3RightX = fillX + fillW - sizeRight.X - leftMargin;

        Raylib.DrawTextEx(Raylib.GetFontDefault(), line3Left, new Vector2(textLeftX, text3Y), fontSize, 1, textColor);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), line3Right, new Vector2(text3RightX, text3Y), fontSize, 1, textColor);
    }
}