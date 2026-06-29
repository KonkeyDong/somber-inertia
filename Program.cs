using Raylib_cs;
using SomberInertia.Core;
using SomberInertia.Enums;
using SomberInertia.Graphics;
using SomberInertia.State;
using SomberInertia.Core.Units;
using SomberInertia.Core.Combat;
using SomberInertia.Core.Combat.Spells;
using SomberInertia.Core.Combat.StatusEffect;
using SomberInertia.Core.Combat.Weapon;

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

        MagicManager.Initialize();
        WeaponManager.Initialize();
        var game = new Game(new Grid(11, 10));

        var max = new ForceMember(UnitName.Max, MovementType.Warrior, 4);
        max.Attack = 10;
        max.HP.Current = 2;
        max.EquipWeapon(WeaponManager.Create(WeaponName.WoodenArrow));
        max.LearnSpell(MagicManager.Create(MagicName.Egress1));
        max.LearnSpell(MagicManager.Create(MagicName.Blaze1));
        max.LearnSpell(MagicManager.Create(MagicName.Blaze2));
        max.LearnSpell(MagicManager.Create(MagicName.Blaze3));
        max.LearnSpell(MagicManager.Create(MagicName.Heal1));
        max.LearnSpell(MagicManager.Create(MagicName.Heal2));
        max.LearnSpell(MagicManager.Create(MagicName.Heal3));
        max.LearnSpell(MagicManager.Create(MagicName.Heal4));
        max.LearnSpell(MagicManager.Create(MagicName.Bolt3));

        var anri = new ForceMember(UnitName.Anri, MovementType.Warrior, 4);
        anri.Friendly = true;
        anri.Attack = 3;
        anri.EquipWeapon(WeaponManager.Create(WeaponName.WoodenStaff));

        var goblin1 = new Monster(UnitName.Goblin, MovementType.Warrior, 5);
        goblin1.Friendly = false;
        goblin1.Defense = 5;

        var goblin2 = new Monster(UnitName.Goblin, MovementType.Warrior, 5);
        goblin2.Friendly = false;
        goblin2.Defense = 5;

        var goblin3 = new Monster(UnitName.Goblin, MovementType.Warrior, 5);
        goblin2.Friendly = false;
        goblin2.Defense = 5;

        game.AddUnit(max, 0, 0);
        game.AddUnit(anri, 1, 1);
        game.AddUnit(goblin1, 3, 1);
        game.AddUnit(goblin2, 4, 2);
        game.AddUnit(goblin3, 4, 3);

        // max.ApplyStatus<PoisonEffect>(new PoisonEffect());

        GameStateManager.InitializeGameState(GameStateType.CalculateUnitMovementRange, game);
        CommandIcons.Load();
        MagicIcons.Load();
        DeathSprites.Load();
        BattleBackgrounds.Load();

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