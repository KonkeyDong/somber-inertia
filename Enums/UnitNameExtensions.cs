namespace SomberInertia.Enums;

public static class UnitNameExtensions
{
    /// <summary>
    /// Returns a human-readable display name for UI and logs.
    /// </summary>
    public static string GetDisplayName(this UnitName unitName)
    {
        return unitName switch
        {
            // === The Force (Heroes) ===
            UnitName.Adam     => "Adam",
            UnitName.Alef     => "Alef",
            UnitName.Amon     => "Amon",
            UnitName.Anri     => "Anri",
            UnitName.Arthur   => "Arthur",
            UnitName.Balbaroy => "Balbaroy",
            UnitName.Bleu     => "Bleu",
            UnitName.Diane    => "Diane",
            UnitName.Domingo  => "Domingo",
            UnitName.Earnest  => "Earnest",
            UnitName.Gong     => "Gong",
            UnitName.Gort     => "Gort",
            UnitName.Guntz    => "Guntz",
            UnitName.Hanz     => "Hanz",
            UnitName.Hanzou   => "Hanzou",
            UnitName.Jogurt   => "Jogurt",
            UnitName.Ken      => "Ken",
            UnitName.Khris    => "Khris",
            UnitName.Kokichi  => "Kokichi",
            UnitName.Lowe     => "Lowe",
            UnitName.Luke     => "Luke",
            UnitName.Lyle     => "Lyle",
            UnitName.Mae      => "Mae",
            UnitName.Max      => "Max",
            UnitName.Musashi  => "Musashi",
            UnitName.Pelle    => "Pelle",
            UnitName.Tao      => "Tao",
            UnitName.Torasu   => "Torasu",
            UnitName.Vankar   => "Vankar",
            UnitName.Zylo     => "Zylo",

            // === The Bad (Enemies & Bosses) ===
            UnitName.ArmedSkeleton => "Armed Skeleton",
            UnitName.Artillery     => "Artillery",
            UnitName.Balbazak      => "Balbazak",
            UnitName.Belial        => "Belial",
            UnitName.BlueDragon    => "Blue Dragon",
            UnitName.Bowrider      => "Bowrider",
            UnitName.Cerberus      => "Cerberus",
            UnitName.Chaos         => "Chaos",
            UnitName.Chimaera      => "Chimaera",
            UnitName.Colossus      => "Colossus",
            UnitName.Conch         => "Conch",
            UnitName.DarkDragon    => "Dark Dragon",
            UnitName.DarkDwarf     => "Dark Dwarf",
            UnitName.DarkElf       => "Dark Elf",
            UnitName.Darkmage      => "Dark Mage",
            UnitName.DarkPriest    => "Dark Priest",
            UnitName.Darksol       => "Darksol",
            UnitName.DemonMaster   => "Demon Master",
            UnitName.DireClown     => "Dire Clown",
            UnitName.Durahan       => "Durahan",
            UnitName.EvilPuppet    => "Evil Puppet",
            UnitName.Gargoyle      => "Gargoyle",
            UnitName.GeneralElliot => "General Elliot",
            UnitName.Ghoul         => "Ghoul",
            UnitName.GiantBat      => "Giant Bat",
            UnitName.Goblin        => "Goblin",
            UnitName.Golem         => "Golem",
            UnitName.Hellhound     => "Hellhound",
            UnitName.HighPriest    => "High Priest",
            UnitName.Horseman      => "Horseman",
            UnitName.IceWorm       => "Ice Worm",
            UnitName.Jet           => "Jet",
            UnitName.Kane          => "Kane",
            UnitName.LaserEye      => "Laser Eye",
            UnitName.Lizardman     => "Lizardman",
            UnitName.Mannequin     => "Mannequin",
            UnitName.Marionette    => "Marionette",
            UnitName.MasterMage    => "Master Mage",
            UnitName.Minotaur      => "Minotaur",
            UnitName.Mishaela      => "Mishaela",
            UnitName.PegasusKnight => "Pegasus Knight",
            UnitName.Ramladu       => "Ramladu",
            UnitName.RuneKnight    => "Rune Knight",
            UnitName.Seabat        => "Seabat",
            UnitName.Shellfish     => "Shellfish",
            UnitName.SilverKnight  => "Silver Knight",
            UnitName.Skeleton      => "Skeleton",
            UnitName.Sniper        => "Sniper",
            UnitName.SteelClaw     => "Steel Claw",
            UnitName.TorchEye      => "Torch Eye",
            UnitName.Worm          => "Worm",
            UnitName.Wyvern        => "Wyvern",
            UnitName.Zombie        => "Zombie",

            _ => unitName.ToString()
        };
    }

    /// <summary>
    /// Returns the PascalCase name used for loading assets from folders and files.
    /// Example: DarkElf → "DarkElf"
    /// </summary>
    public static string GetBaseName(this UnitName unitName) => unitName.ToString();
}