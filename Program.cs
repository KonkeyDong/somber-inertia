using Raylib_cs;
using System.CommandLine;
using SomberInertia.Core;
using SomberInertia.Enums;
using SomberInertia.Graphics;
using SomberInertia.State;
using SomberInertia.Core.Units;
using SomberInertia.Core.Combat;
using SomberInertia.Core.Combat.Spells;
using SomberInertia.Core.Combat.StatusEffect;
using SomberInertia.Core.Combat.Item;

namespace SomberInertia;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var loggerOption = new Option<LogLevel>(
            name: "--logger",
            description: "Set the logger level (debug, info, warning, error)",
            getDefaultValue: () => LogLevel.Info);

        loggerOption.AddAlias("-l");
        loggerOption.AddAlias("-d");

        var rootCommand = new RootCommand("Somber Inertia");
        rootCommand.AddOption(loggerOption);

        rootCommand.SetHandler((LogLevel logLevel) =>
        {
            Logger.MinimumLevel = logLevel;
            Logger.Info($"Logger level set to: {Logger.MinimumLevel}");

            RunGame();
        }, loggerOption);

        return await rootCommand.InvokeAsync(args);
    }

    static void RunGame()
    {
        var scale = GameConstants.Window.Scale;
        var width = (int)(GameConstants.Window.Width * scale);
        var height = (int)(GameConstants.Window.Height * scale);

        Raylib.InitWindow(width, height, "Somber Inertia");
        Raylib.SetTargetFPS(60);

        MagicDatabase.Initialize();
        ItemDatabase.Initialize();
        var game = new Game(new Grid(11, 10));

        var max = new ForceMember(UnitName.Max, MovementType.Warrior, 4);
        max.Attack = 15;
        max.HP.Current = 1;
        max.MP.Max = 99;
        max.MP.Current = 99;
        // max.EquipWeapon(WeaponManager.Create(ItemName.ShortSword));
        max.AddItem(ItemName.ShortSword, autoEquipWeapon: true);
        max.LearnSpell(MagicName.Egress1);
        max.LearnSpell(MagicName.Blaze1);
        max.LearnSpell(MagicName.Blaze2);
        max.LearnSpell(MagicName.Blaze3);
        max.LearnSpell(MagicName.Heal1);
        max.LearnSpell(MagicName.Heal2);
        max.LearnSpell(MagicName.Heal3);
        max.LearnSpell(MagicName.Heal4);
        max.LearnSpell(MagicName.Bolt3);
        max.AddItem(ItemName.MedicalHerb);
        max.AddItem(ItemName.Antidote);

        Logger.Info(max.GetEquippedWeaponName().ToString());

        var anri = new ForceMember(UnitName.Anri, MovementType.Warrior, 4);
        anri.Attack = 15;
        anri.AddItem(ItemName.Unarmed, autoEquipWeapon: true);

        var goblin1 = new Monster(UnitName.Goblin, MovementType.Warrior, 5);
        goblin1.Defense = 5;

        var runeKnight = new Monster(UnitName.RuneKnight, MovementType.Warrior, 5);
        runeKnight.HP.Current = 1;

        var dwarf = new Monster(UnitName.DarkDwarf, MovementType.Warrior, 5);
        dwarf.HP.Current = 1;

        game.AddUnit(max, 0, 0);
        game.AddUnit(anri, 1, 1);
        game.AddUnit(dwarf, 3, 1);
        game.AddUnit(runeKnight, 4, 1);
        game.AddUnit(goblin1, 3, 2);

        // max.ApplyStatus<PoisonEffect>(new PoisonEffect());

        GameStateManager.InitializeGameState(GameStateType.CalculateUnitMovementRange, game);
        CommandIcons.Load();
        MagicIcons.Load();
        ItemIcons.Load();
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