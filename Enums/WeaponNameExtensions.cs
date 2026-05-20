namespace SomberInertia.Enums;

public static class WeaponNameExtensions
{
    public static string GetDisplayName(this WeaponName weaponName)
    {
        return weaponName switch
        {
            // Unarmed
            WeaponName.Unarmed => "Unarmed",

            // Swords
            WeaponName.ShortSword => "Short Sword",
            WeaponName.MiddleSword => "Middle Sword",
            WeaponName.LongSword => "Long Sword",
            WeaponName.SteelSword => "Steel Sword",
            WeaponName.BroadSword => "Broad Sword",
            WeaponName.DoomBlade => "Doom Blade",
            WeaponName.Katana => "Katana",
            WeaponName.SwordOfLight => "Sword of Light",
            WeaponName.SwordOfDarkness => "Sword of Darkness",
            WeaponName.ChaosBreaker => "Chaos Breaker",

            // Axes
            WeaponName.HandAxe => "Hand Axe",
            WeaponName.MiddleAxe => "Middle Axe",
            WeaponName.BattleAxe => "Battle Axe",
            WeaponName.HeatAxe => "Heat Axe",
            WeaponName.GreatAxe => "Great Axe",
            WeaponName.Atlas => "Atlas",

            // Staves
            WeaponName.WoodenStaff => "Wooden Staff",
            WeaponName.PowerStaff => "Power Staff",
            WeaponName.GuardianStaff => "Guardian Staff",
            WeaponName.HolyStaff => "Holy Staff",
            WeaponName.DemonRod => "Demon Rod",

            // Arrows
            WeaponName.WoodenArrow => "Wooden Arrow",
            WeaponName.SteelArrow => "Steel Arrow",
            WeaponName.ElvenArrow => "Elven Arrow",
            WeaponName.AssaultShell => "Assault Shell",
            WeaponName.BusterShot => "Buster Shot",

            // Spears
            WeaponName.Spear => "Spear",
            WeaponName.PowerSpear => "Power Spear",

            // Lances
            WeaponName.BronzeLance => "Bronze Lance",
            WeaponName.SteelLance => "Steel Lance",
            WeaponName.ChromeLance => "Chrome Lance",
            WeaponName.Halberd => "Halberd",
            WeaponName.DevilLance => "Devil Lance",
            WeaponName.Valkyrie => "Valkyrie",

            _ => weaponName.ToString()   // fallback
        };
    }
}