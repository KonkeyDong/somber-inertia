using SomberInertia.Core;
using SomberInertia.Graphics;
using SomberInertia.Core.Units;
using SomberInertia.Enums;
using SomberInertia.Core.Combat;
using SomberInertia.Core.Combat.Spells;
using SomberInertia.Core.Combat.Item;
using SomberInertia.State;
using System.Numerics;

using Raylib_cs;

namespace SomberInertia.Core.Graphics;

public class Renderer
{
    private readonly record struct InfoBoxMetrics(
        int FillX,
        int FillY,
        int FillW,
        int FillH,
        int TextLeftX,
        int TextStartY,
        int FontSize,
        int LineSpacing,
        int LeftMargin
    );

    public Renderer()
    {

    }

    public void Draw(float scale, Sprite sprite, Vector2 position)
    {
        Draw(scale, sprite, position, 255);
    }

    public void Draw(float scale, Sprite sprite, Vector2 position, int alpha)
    {
        var source = new Rectangle(
            x: sprite.FrameRect.X,
            y: sprite.FrameRect.Y,
            width: sprite.FrameRect.W,
            height: sprite.FrameRect.H
        );

        var dest = new Rectangle(
            x: (int)((position.X + sprite.FrameRect.OffsetX) * scale),
            y: (int)((position.Y + sprite.FrameRect.OffsetY) * scale),
            width: sprite.FrameRect.W * scale,
            height: sprite.FrameRect.H * scale
        );

        var tint = new Color(255, 255, 255, alpha);

        Raylib.DrawTexturePro(
            sprite.Texture,
            source,
            dest,
            GameConstants.Textures.BaseOrigin,
            GameConstants.Textures.BaseRotation,
            tint
        );

        if (Logger.InDebugMode())
        {
            Raylib.DrawRectangleLinesEx(dest, GameConstants.Debug.Spacing, GameConstants.Debug.Color);

            var debugText = $"X: {position.X + sprite.FrameRect.OffsetX}, Y: {position.Y + sprite.FrameRect.OffsetY}";

            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                debugText,
                new Vector2(dest.X, dest.Y - 15), // slightly above the sprite
                GameConstants.Debug.FontSize,
                GameConstants.Debug.Spacing,
                GameConstants.Debug.Color
            );
        }
    }

    public void DrawBackground(float scale, Grid grid, int alpha = 255)
    {
        var debugFlag = Logger.InDebugMode();
        var tint = new Color(255, 255, 255, alpha);
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
                    GameConstants.Textures.BaseRotation,
                    scale,
                    tint
                );

                if (debugFlag)
                {
                    Raylib.DrawText(
                        grid.Blocks[x, y].PrintGridCoordinates(), 
                        (int)position.X, 
                        (int)position.Y + 20, 
                        16, 
                        Color.White
                    );
                }
            }
        }

        // Draw red grid lines when in debug mode
        if (debugFlag)
        {
            var gridColor = new Color(200, 50, 50, 255);
            var lineThickness = 1.0f * scale;

            // Vertical lines
            for (var x = 0; x <= grid.Width; x++)
            {
                var xPos = x * grid.BlockSize;
                Raylib.DrawLineEx(
                    new Vector2((int)(xPos), 0),
                    new Vector2((int)(xPos), (int)(grid.Height * grid.BlockSize)),
                    lineThickness,
                    gridColor
                );
            }

            // Horizontal lines
            for (var y = 0; y <= grid.Height; y++)
            {
                var yPos = y * grid.BlockSize;
                Raylib.DrawLineEx(
                    new Vector2(0, (int)(yPos)),
                    new Vector2((int)(grid.Width * grid.BlockSize), (int)(yPos)),
                    lineThickness,
                    gridColor
                );
            }
        }
    }

    public void DrawDebugLogicalGrid(float scale, int logicalSpacing, Color color)
    {
        if (logicalSpacing <= 0)
        {
            return;
        }

        var step = logicalSpacing * scale;
        var width = GameStateManager.CurrentWidth;
        var height = GameStateManager.CurrentHeight;
        var lineThickness = Math.Max(1f, scale);

        for (var x = 0f; x <= width; x += step)
        {
            Raylib.DrawLineEx(
                new Vector2(x, 0),
                new Vector2(x, height),
                lineThickness,
                color
            );
        }

        for (var y = 0f; y <= height; y += step)
        {
            Raylib.DrawLineEx(
                new Vector2(0, y),
                new Vector2(width, y),
                lineThickness,
                color
            );
        }
    }

    public void DrawMovementRange(float scale, Grid grid) => DrawRangeBlockColor(scale, grid, grid.MovementRangeSet);
    public void DrawWeaponAttackRange(float scale, Grid grid) => DrawRangeBlockColor(scale, grid, grid.WeaponAttackRangeSet);
    public void DrawMagicAttackRange(float scale, Grid grid) => DrawRangeBlockColor(scale, grid, grid.MagicAttackRangeSet);
    public void DrawGiveRange(float scale, Grid grid) => DrawRangeBlockColor(scale, grid, grid.GiveRangeSet);
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
                GameConstants.Textures.BaseRotation,
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
        var tileSize = GameConstants.TileSize * scale;

        var highlightRect = new Rectangle(
            newPosition.X * (int)scale,
            newPosition.Y * (int)scale,
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

    public void DrawUnit(float scale, Grid grid, Unit unit, bool frameFlipperFlag, int alpha = 255)
    {
        var position = new Vector2();

        if (unit.Block == null)
        {
            Logger.Error($"Unit {unit.Name} has no Block reference!");
            return;
        }

        position = unit.WorldPosition;
        var sprite = unit.GetFacingDirectionTexture(frameFlipperFlag);

        Draw(scale, sprite, position, alpha);
    }

    public void DrawUnits(float scale, Grid grid, List<Unit> units, bool frameFlipperFlag, int alpha = 255)
    {
        // We loop in reverse to get the drawing order correct.
        // This allows current controlled unit to always be on top
        // of a block containing an occupant.
        for (var i = units.Count - 1; i >= 0; i--)
        {
            DrawUnit(scale, grid, units[i], frameFlipperFlag, alpha);
        }
    }

    public void DrawMagicIcon(float scale, MagicFamily family, Vector2 position)
    {
        var sprite = MagicIcons.GetSprite(family);
        Draw(scale, sprite, position);
    }

    public void DrawItemIcon(float scale, ItemName name, Vector2 position, bool isSelected = false)
    {
        var sprite = ItemIcons.GetSprite(name, isSelected);
        Draw(scale, sprite, position);
    }

    private InfoBoxMetrics DrawInfoBoxFrame(float scale, Vector2 position, float contentWidth, float contentHeight)
    {
        var fontSize = (int)(8 * scale);
        var padding = 12;
        var lineSpacing = 4;
        var leftMargin = 8;

        var boxWidth = (int)contentWidth + padding * 2;
        var boxHeight = (int)contentHeight + padding * 2;
        var boxX = (int)(position.X * scale) - padding;
        var boxY = (int)(position.Y * scale) - padding;

        var darkOrange = GameConstants.Textures.DarkOrange;
        var lightOrange = GameConstants.Textures.LightOrange;
        var offWhite = GameConstants.Textures.OffWhite;
        var blue = GameConstants.Textures.Blue;

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

        return new InfoBoxMetrics(
            fillX,
            fillY,
            fillW,
            fillH,
            fillX + leftMargin,
            fillY + 6,
            fontSize,
            lineSpacing,
            leftMargin
        );
    }

    private static void SplitDisplayName(string displayName, out string line1, out string line2)
    {
        var words = displayName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (words.Length <= 1)
        {
            line1 = displayName;
            line2 = "";
            return;
        }

        line1 = string.Join(" ", words.Take(words.Length - 1));
        line2 = words[words.Length - 1];
    }

    private static Vector2 Measure(string text, int fontSize)
    {
        return Raylib.MeasureTextEx(Raylib.GetFontDefault(), text, fontSize, 1);
    }

    public void DrawBattleMenuMessage(float scale, string text, Vector2 textPos)
    {
        var fontSize = (int)(8 * scale);
        var textSize = Measure(text, fontSize);
        var metrics = DrawInfoBoxFrame(scale, textPos, textSize.X, textSize.Y);

        var finalTextPos = new Vector2(
            metrics.FillX + (metrics.FillW - textSize.X) / 2,
            metrics.FillY + (metrics.FillH - textSize.Y) / 2
        );

        Raylib.DrawTextEx(Raylib.GetFontDefault(), text, finalTextPos, metrics.FontSize, 1, Color.White);
    }

    public void DrawSpellInfoBox(float scale, MagicData spell, Vector2 position, bool highlightLevel = false)
    {
        var fontSize = (int)(8 * scale);
        var line1 = spell.Name.GetBaseName();
        var line2 = $"Level {spell.Level}";
        var line3Left = "MP";
        var line3Right = spell.MPCost.ToString();

        var size1 = Measure(line1, fontSize);
        var size2 = Measure(line2, fontSize);
        var sizeLeft = Measure(line3Left, fontSize);
        var sizeRight = Measure(line3Right, fontSize);

        var contentWidth = Math.Max(size1.X, Math.Max(size2.X, sizeLeft.X + sizeRight.X + 20));
        var contentHeight = size1.Y + size2.Y + sizeLeft.Y + (4 * 2);

        var m = DrawInfoBoxFrame(scale, position, contentWidth, contentHeight);
        var y = m.TextStartY;

        Raylib.DrawTextEx(Raylib.GetFontDefault(), line1, new Vector2(m.TextLeftX, y), m.FontSize, 1, Color.White);
        y += (int)(size1.Y + m.LineSpacing);

        if (highlightLevel)
        {
            var pad = 2;
            Raylib.DrawRectangle(
                (int)(m.TextLeftX - pad),
                (int)(y - pad),
                (int)(size2.X + pad * 2),
                (int)(size2.Y + pad * 2),
                GameConstants.Textures.DarkRed
            );
        }

        Raylib.DrawTextEx(Raylib.GetFontDefault(), line2, new Vector2(m.TextLeftX, y), m.FontSize, 1, Color.White);
        y += (int)(size2.Y + m.LineSpacing);

        Raylib.DrawTextEx(Raylib.GetFontDefault(), line3Left, new Vector2(m.TextLeftX, y), m.FontSize, 1, Color.White);
        Raylib.DrawTextEx(
            Raylib.GetFontDefault(),
            line3Right,
            new Vector2(m.FillX + m.FillW - sizeRight.X - m.LeftMargin, y),
            m.FontSize,
            1,
            Color.White
        );
    }

    public void DrawUnitInfoBox(float scale, Unit unit, Vector2 position, int alpha = 255)
    {
        var fontSize = (int)(8 * scale);
        var line1 = unit.GetDisplayName();
        var line2 = $"HP: {unit.HP}";
        var line3 = $"MP: {unit.MP}";

        var size1 = Measure(line1, fontSize);
        var size2 = Measure(line2, fontSize);
        var size3 = Measure(line3, fontSize);

        var contentWidth = Math.Max(size1.X, Math.Max(size2.X, size3.X));
        var contentHeight = size1.Y + size2.Y + size3.Y + (4 * 2);

        var m = DrawInfoBoxFrame(scale, position, contentWidth, contentHeight);
        var y = m.TextStartY;

        Raylib.DrawTextEx(Raylib.GetFontDefault(), line1, new Vector2(m.TextLeftX, y), m.FontSize, 1, Color.White);
        y += (int)(size1.Y + m.LineSpacing);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), line2, new Vector2(m.TextLeftX, y), m.FontSize, 1, Color.White);
        y += (int)(size2.Y + m.LineSpacing);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), line3, new Vector2(m.TextLeftX, y), m.FontSize, 1, Color.White);
    }

    public void DrawItemInfoBox(float scale, ItemData item, bool isEquipped, Vector2 position)
    {
        var fontSize = (int)(8 * scale);
        SplitDisplayName(item.Name.GetDisplayName(), out var line1, out var line2);
        var line3 = isEquipped ? "EQUIPPED" : "";

        var size1 = Measure(line1, fontSize);
        var size2 = Measure(string.IsNullOrEmpty(line2) ? "A" : line2, fontSize);
        var size3 = Measure(string.IsNullOrEmpty(line3) ? "EQUIPPED" : line3, fontSize);

        var contentWidth = Math.Max(size1.X, Math.Max(size2.X, size3.X));
        var contentHeight = size1.Y + size2.Y + size3.Y + (4 * 2);

        var m = DrawInfoBoxFrame(scale, position, contentWidth, contentHeight);
        var y = m.TextStartY;

        Raylib.DrawTextEx(Raylib.GetFontDefault(), line1, new Vector2(m.TextLeftX, y), m.FontSize, 1, Color.White);
        y += (int)(size1.Y + m.LineSpacing);

        if (!string.IsNullOrEmpty(line2))
        {
            Raylib.DrawTextEx(Raylib.GetFontDefault(), line2, new Vector2(m.TextLeftX, y), m.FontSize, 1, Color.White);
        }

        y += (int)(size2.Y + m.LineSpacing);

        if (isEquipped)
        {
            Raylib.DrawTextEx(Raylib.GetFontDefault(), line3, new Vector2(m.TextLeftX, y), m.FontSize, 1, Color.White);
        }
    }

    public void DrawEquipWeaponInfoBox(float scale, ItemData item, Vector2 position)
    {
        var fontSize = (int)(8 * scale);
        var line1 = "WEAPON";
        string line2;
        string line3;

        if (item.Name == ItemName.Unarmed)
        {
            line2 = "REMOVE";
            line3 = "";
        }
        else
        {
            SplitDisplayName(item.Name.GetDisplayName(), out line2, out line3);
        }

        var size1 = Measure(line1, fontSize);
        var size2 = Measure(line2, fontSize);
        var size3 = Measure(string.IsNullOrEmpty(line3) ? "A" : line3, fontSize);

        var contentWidth = Math.Max(size1.X, Math.Max(size2.X, size3.X));
        var contentHeight = size1.Y + size2.Y + size3.Y + (4 * 2);

        var m = DrawInfoBoxFrame(scale, position, contentWidth, contentHeight);
        var y = m.TextStartY;

        Raylib.DrawTextEx(Raylib.GetFontDefault(), line1, new Vector2(m.TextLeftX, y), m.FontSize, 1, Color.White);
        y += (int)(size1.Y + m.LineSpacing);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), line2, new Vector2(m.TextLeftX, y), m.FontSize, 1, Color.White);
        y += (int)(size2.Y + m.LineSpacing);

        if (!string.IsNullOrEmpty(line3))
        {
            Raylib.DrawTextEx(Raylib.GetFontDefault(), line3, new Vector2(m.TextLeftX, y), m.FontSize, 1, Color.White);
        }
    }

    public void DrawEquipStatsBox(float scale, int attack, int defense, int move, int agility, Vector2 position)
    {
        var fontSize = (int)(8 * scale);
        var labels = new[] { "ATTACK", "DEFENSE", "MOVE", "AGILITY" };
        var values = new[] { attack.ToString(), defense.ToString(), move.ToString(), agility.ToString() };
        var valueGap = 16;

        float maxLabelWidth = 0;
        float maxValueWidth = 0;
        float lineHeight = 0;

        for (var i = 0; i < labels.Length; i++)
        {
            var labelSize = Measure(labels[i], fontSize);
            var valueSize = Measure(values[i], fontSize);

            maxLabelWidth = Math.Max(maxLabelWidth, labelSize.X);
            maxValueWidth = Math.Max(maxValueWidth, valueSize.X);
            lineHeight = Math.Max(lineHeight, labelSize.Y);
        }

        var contentWidth = maxLabelWidth + valueGap + maxValueWidth;
        var contentHeight = (lineHeight * labels.Length) + (4 * (labels.Length - 1));

        var m = DrawInfoBoxFrame(scale, position, contentWidth, contentHeight);
        var y = (float)m.TextStartY;
        var valueRightX = m.FillX + m.FillW - m.LeftMargin;

        for (var i = 0; i < labels.Length; i++)
        {
            Raylib.DrawTextEx(Raylib.GetFontDefault(), labels[i], new Vector2(m.TextLeftX, y), m.FontSize, 1, Color.White);

            var valueSize = Measure(values[i], m.FontSize);
            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                values[i],
                new Vector2(valueRightX - valueSize.X, y),
                m.FontSize,
                1,
                Color.White
            );

            y += lineHeight + m.LineSpacing;
        }
    }

    public void DrawTradePromptBox(
        float scale,
        string actionLabel,
        string giverName,
        ItemName giverItem,
        string receiverName,
        ItemName? receiverItem,
        Vector2 position)
    {
        var fontSize = (int)(8 * scale);
        var iconLogical = GameConstants.TileSize;
        var columnGap = GameConstants.Give.TradePromptColumnGap * scale;
        var nameToIconGap = GameConstants.Give.TradePromptNameToIconGap * scale;

        var actionSize = Measure(actionLabel, fontSize);
        var giverNameSize = Measure(giverName, fontSize);
        var receiverNameSize = Measure(receiverName, fontSize);

        var iconPixel = iconLogical * scale;
        var leftColWidth = Math.Max(giverNameSize.X, iconPixel);
        var rightColWidth = Math.Max(receiverNameSize.X, iconPixel);
        var contentWidth = Math.Max(actionSize.X, leftColWidth + columnGap + rightColWidth);
        var contentHeight = actionSize.Y
            + nameToIconGap
            + Math.Max(giverNameSize.Y, receiverNameSize.Y)
            + nameToIconGap
            + iconPixel;

        var m = DrawInfoBoxFrame(scale, position, contentWidth, contentHeight);

        // Action title centered at top
        var actionX = m.FillX + (m.FillW - actionSize.X) / 2f;
        var y = (float)m.TextStartY;
        Raylib.DrawTextEx(
            Raylib.GetFontDefault(),
            actionLabel,
            new Vector2(actionX, y),
            m.FontSize,
            1,
            Color.White
        );

        y += actionSize.Y + nameToIconGap;

        // Names: left column left-justified, right column right-justified
        var leftColLeft = m.TextLeftX;
        var rightColRight = m.FillX + m.FillW - m.LeftMargin;

        Raylib.DrawTextEx(
            Raylib.GetFontDefault(),
            giverName,
            new Vector2(leftColLeft, y),
            m.FontSize,
            1,
            Color.White
        );

        Raylib.DrawTextEx(
            Raylib.GetFontDefault(),
            receiverName,
            new Vector2(rightColRight - receiverNameSize.X, y),
            m.FontSize,
            1,
            Color.White
        );

        y += Math.Max(giverNameSize.Y, receiverNameSize.Y) + nameToIconGap;

        // Icons under each column (logical coords for DrawItemIcon)
        var iconYLogical = y / scale;
        var leftIconXLogical = leftColLeft / scale;
        var rightIconXLogical = (rightColRight - iconPixel) / scale;

        DrawItemIcon(scale, giverItem, new Vector2(leftIconXLogical, iconYLogical), isSelected: false);

        if (receiverItem.HasValue)
        {
            DrawItemIcon(
                scale,
                receiverItem.Value,
                new Vector2(rightIconXLogical, iconYLogical),
                isSelected: false
            );
        }
    }

    public float EaseInOut(float t)
    {
        return t < 0.5f ? 2 * t * t : 1 - (float)Math.Pow(-2 * t + 2, 2) / 2;
    }
}