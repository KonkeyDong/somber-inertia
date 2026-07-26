namespace SomberInertia.Enums;

public static class ItemNameExtensions
{
    public static string GetDisplayName(this ItemName itemName)
    {
        return itemName switch
        {
            // Unarmed
            ItemName.Unarmed => "Unarmed",

            // Swords
            ItemName.ShortSword => "Short Sword",
            ItemName.MiddleSword => "Middle Sword",
            ItemName.LongSword => "Long Sword",
            ItemName.SteelSword => "Steel Sword",
            ItemName.BroadSword => "Broad Sword",
            ItemName.DoomBlade => "Doom Blade",
            ItemName.Katana => "Katana",
            ItemName.SwordOfLight => "Sword of Light",
            ItemName.SwordOfDarkness => "Sword of Darkness",
            ItemName.ChaosBreaker => "Chaos Breaker",

            // Axes
            ItemName.HandAxe => "Hand Axe",
            ItemName.MiddleAxe => "Middle Axe",
            ItemName.BattleAxe => "Battle Axe",
            ItemName.HeatAxe => "Heat Axe",
            ItemName.GreatAxe => "Great Axe",
            ItemName.Atlas => "Atlas",

            // Staves
            ItemName.WoodenStaff => "Wooden Staff",
            ItemName.PowerStaff => "Power Staff",
            ItemName.GuardianStaff => "Guardian Staff",
            ItemName.HolyStaff => "Holy Staff",
            ItemName.DemonRod => "Demon Rod",

            // Arrows
            ItemName.WoodenArrow => "Wooden Arrow",
            ItemName.SteelArrow => "Steel Arrow",
            ItemName.ElvenArrow => "Elven Arrow",
            ItemName.AssaultShell => "Assault Shell",
            ItemName.BusterShot => "Buster Shot",

            // Spears
            ItemName.Spear => "Spear",
            ItemName.PowerSpear => "Power Spear",

            // Lances
            ItemName.BronzeLance => "Bronze Lance",
            ItemName.SteelLance => "Steel Lance",
            ItemName.ChromeLance => "Chrome Lance",
            ItemName.Halberd => "Halberd",
            ItemName.DevilLance => "Devil Lance",
            ItemName.Valkyrie => "Valkyrie",

            // Consumable Items
            ItemName.MedicalHerb => "Medical Herb",
            ItemName.HealingSeed => "Healing Seed",
            ItemName.ShowerOfCure => "Shower of Cure",
            ItemName.Antidote => "Antidote",
            ItemName.AngelWing => "Angel Wing",
            ItemName.BreadOfLife => "Bread of Life",
            ItemName.PowerPotion => "Power Potion",
            ItemName.DefensePotion => "Defense Potion",
            ItemName.LegsOfHaste => "Legs of Haste",
            ItemName.TurboPepper => "Turbo Pepper",

            // Key Items
            ItemName.OrbOfLight => "Orb of Light",
            ItemName.DomingoEgg => "Domingo Egg",
            ItemName.MoonStone => "Moon Stone",
            ItemName.LunarDew => "Lunar Dew",

            // Clothes
            ItemName.SugoiMizugi => "Sugoi Mizugi",
            ItemName.KituiHuku => "Kitui Huku",

            _ => itemName.ToString() // fallback
        };
    }

    public static string GetBaseName(this ItemName itemName) => itemName.ToString();
}