using SomberInertia.Enums;
using SomberInertia.Core;
using SomberInertia.Core.Units;
using SomberInertia.Graphics;
using System.Numerics;

using Raylib_cs;

namespace SomberInertia.State;

public class SelectMagic : IGameState
{
    private readonly Game _game;
    private readonly Unit _currentUnit;
    private int _selectedMagicIndex = -1;
    private Vector2 _centerPosition;

    private static readonly Dictionary<Direction, int> _spellIndexByDirection = new()
    {
        { Direction.Up,    0 },   // First spell in the list
        { Direction.Left,  1 },
        { Direction.Right, 2 },
        { Direction.Down,  3 }
    };

    public SelectMagic(Game game)
    {
        _game = game;
        _currentUnit = _game.GetCurrentUnit();
    }

    public void Enter()
    {
        if (!_currentUnit.HasSpells)
        {
            Logger.Info($"Unit [{_currentUnit.Name.GetDisplayName()}] has no spells learned.");
            Logger.Warning("Need to implement 'No Magic Available' message if no spells are learned.");
            GameStateManager.ChangeStateType(GameStateType.BattleActionMenu);
        }

        SetSelectedMagic(Direction.Up);
        UpdateCenterPosition();
    }

    public void Exit()
    {

    }

    private void UpdateCenterPosition()
    {
        _centerPosition = new Vector2(
            GameStateManager.CurrentWidth / 2f,
            GameStateManager.CurrentHeight * 0.75f
        );
    }

    public void HandleInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Up))
        {
            SetSelectedMagic(Direction.Up);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Left))
        {
            SetSelectedMagic(Direction.Left);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Right))
        {
            SetSelectedMagic(Direction.Right);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Down))
        {
            SetSelectedMagic(Direction.Down);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Z) || Raylib.IsKeyPressed(KeyboardKey.C))
        {
            var bucket = _currentUnit.MagicFamilyBuckets[_selectedMagicIndex];
            if (bucket != null)
            {
                var spell = _currentUnit.GetHighestMagicLevelInBucket((MagicFamily)bucket);
                Logger.Info(spell.ToString());
            }
        }

        if (Raylib.IsKeyPressed(KeyboardKey.X))
        {
            CancelMenu();
        }
    }

    private void SetSelectedMagic(Direction direction)
    {
        if (_spellIndexByDirection.TryGetValue(direction, out var index))
        {
            if (_selectedMagicIndex == index)
            {
                return;
            }

            var bucket = _currentUnit.MagicFamilyBuckets[index];
            if (bucket != null)
            {
                // var sprite = MagicIcons.GetSprite((MagicFamily)bucket);
                // _game.Renderer.Draw(scale, sprite, position);
                _selectedMagicIndex = index;
                Logger.Debug($"Selected magic index: [{index}].");
                MagicIcons.SetSelectedSpell((MagicFamily)bucket);
            }

        }
    }

    private void CancelMenu()
    {
        Logger.Debug("SelectMagic(): Cancelled - returning to BattleActionMenu.");
        GameStateManager.ChangeStateType(GameStateType.BattleActionMenu);
    }

    public void Update()
    {
        _game.FrameFlipper.Tick();
        MagicIcons.Update();
    }

    public void Draw(float scale)
    {
        _game.Renderer.DrawBackground(scale, _game.Grid);
        _game.Renderer.DrawUnits(scale, _game.Grid, _game.Units, _game.FrameFlipper.IsOn);

        foreach (var (direction, i) in _spellIndexByDirection)
        {
            var offset = direction.GetMenuOffset();
            var position = _centerPosition + offset * (GameConstants.TILE_SIZE * scale);


            var bucket = _currentUnit.MagicFamilyBuckets[i];
            if (bucket != null)
            {
                var sprite = MagicIcons.GetSprite((MagicFamily)bucket);
                _game.Renderer.Draw(scale, sprite, position);
            }
            else
            {
                var sprite = MagicIcons.GetSprite(MagicFamily.NoSpell);
                _game.Renderer.Draw(scale, sprite, position);
            }
        }
    }
}