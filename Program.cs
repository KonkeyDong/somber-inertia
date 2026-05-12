using SomberInertia.Core;
using SomberInertia.State;
using SomberInertia.Enums;
using SomberInertia.Graphics;
using Raylib_cs;

namespace SomberInertia;

class Program
{
    static void Main()
    {
        Logger.MinimumLevel = LogLevel.Info;

        var scale = GameConstants.BASE_WINDOW_SCALE;
        var width = (int)(GameConstants.BASE_WINDOW_WIDTH * scale);
        var height = (int)(GameConstants.BASE_WINDOW_HEIGHT * scale);

        Raylib.InitWindow(width, height, "Somber Inertia");
        Raylib.SetTargetFPS(60);

        var Grid = new Grid(11, 10);

        var max = new Unit("Assets/max.png", "Max", MovementType.Warrior, 4);
        max.Friendly = true;
        max.EquipWeapon(new Weapon("Sword", 6, WeaponType.Sword, new WeaponRange(2, 2)));
        var goblin = new Unit("Assets/goblin.png", "Goblin", MovementType.Warrior, 5);
        goblin.Friendly = false;
        var anri = new Unit("Assets/anri.png", "Anri", MovementType.Warrior, 4);
        anri.Friendly = true;

        Grid.AddUnit(max, 0, 0);
        Grid.AddUnit(anri, 1, 1);
        Grid.AddUnit(goblin, 2, 1);


        GameStateManager.InitializeGameState(GameStateType.CalculateUnitMovementRange, Grid);

        while (!Raylib.WindowShouldClose())
        {
            GameStateManager.HandleInput();
            GameStateManager.Update();
            GameStateManager.Draw();
        }

        SpriteManager.UnloadAll();
        Raylib.CloseWindow();
    }
}