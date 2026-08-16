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

        UnitDatabase.Initialize();
        MagicDatabase.Initialize();
        ItemDatabase.Initialize();
        var game = new Game(new Grid(11, 10));

        var max = new Unit(UnitName.Max);
        max.Job = Job.Hero;
        max.Promote();
        max.Attack = 15;
        max.HP.Current = 7;
        max.MP.Max = 99;
        max.MP.Current = 99;
        // max.EquipWeapon(WeaponManager.Create(ItemName.ShortSword));
        max.AddItem(ItemName.ChaosBreaker, autoEquipWeapon: true);
        max.AddItem(ItemName.SwordOfDarkness, autoEquipWeapon: false);
        // Smoke: spell-item Use (Bolt2). Max is Swordsman — temporarily allow equip/use.
        max.Job = Job.Hero;
        max.AddItem(ItemName.SwordOfLight);
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
        max.AddItem(ItemName.ShowerOfCure); // party-wide full heal smoke

        Logger.Info(max.GetEquippedWeaponName().ToString());

        var anri = new Unit(UnitName.Anri);
        anri.Attack = 15;
        anri.HP.Current = 7;
        // anri.ApplyStatus(StatusEffectType.Poison);
        // Unarmed equip is default (index -1); do not put Unarmed in inventory.
        anri.AddItem(ItemName.WoodenStaff, autoEquipWeapon: false);
        anri.AddItem(ItemName.PowerStaff, autoEquipWeapon: true);
        anri.AddItem(ItemName.MedicalHerb);

        var tao = new Unit(UnitName.Tao);
        tao.HP.Current = 7;
        tao.AddItem(ItemName.WoodenStaff, autoEquipWeapon: false);
        tao.AddItem(ItemName.PowerStaff, autoEquipWeapon: false);

        var goblin1 = new Unit(UnitName.Goblin);
        goblin1.Defense = 5;
        goblin1.HP.Current = 1;

        var runeKnight = new Unit(UnitName.RuneKnight);
        runeKnight.HP.Current = 1;

        var dwarf = new Unit(UnitName.DarkDwarf);
        dwarf.HP.Current = 1;

        game.AddUnit(max, 0, 0);
        // game.AddUnit(anri, 1, 0); // adjacent to Max for give/trade smoke tests
        // game.AddUnit(tao, 1, 1);
        game.AddUnit(dwarf, 3, 1);
        game.AddUnit(runeKnight, 4, 1);
        game.AddUnit(goblin1, 3, 2);

        // max.ApplyStatus(StatusEffectType.Poison);

        GameStateManager.InitializeGameState(GameStateType.CalculateUnitMovementRange, game);
        CommandIcons.Load();
        MagicIcons.Load();
        ItemIcons.Load();
        DeathSprites.Load();
        BattleBackgrounds.Load();
        BattleForegrounds.Load();

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