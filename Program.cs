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

        var Game = new Game(new Grid(11, 10));

        var max = new Unit("Assets/max.png", "Max", MovementType.Warrior, 4);
        max.Friendly = true;
        max.Attack = 10;
        max.EquipWeapon(new Weapon("Sword", 6, WeaponType.Sword, new WeaponRange(1, 1)));
        var goblin = new Unit("Assets/goblin.png", "Goblin", MovementType.Warrior, 5);
        goblin.Friendly = false;
        goblin.Defense = 5;
        var anri = new Unit("Assets/anri.png", "Anri", MovementType.Warrior, 4);
        anri.Friendly = true;
        anri.EquipWeapon(new Weapon("Staff", 4, WeaponType.Staff, new WeaponRange(1, 1)));

        Game.AddUnit(max, 0, 0);
        Game.AddUnit(anri, 1, 1);
        Game.AddUnit(goblin, 2, 1);


        GameStateManager.InitializeGameState(GameStateType.CalculateUnitMovementRange, Game);

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