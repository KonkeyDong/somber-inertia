namespace SomberInertia.Enums;

public static class CommandIconTypeExtensions
{
    /// <summary>
    /// Returns the lowercase base name used for loading command icon sprites.
    /// </summary>
    public static string GetBaseName(this CommandIconType commandIconType, bool capitalize = false)
    {
        var result = commandIconType switch
        {
            CommandIconType.Yes     => "yes",
            CommandIconType.No      => "no",
            CommandIconType.Talk    => "talk",
            CommandIconType.Magic   => "magic",
            CommandIconType.Item    => "item",
            CommandIconType.Search  => "search",
            CommandIconType.Attack  => "attack",
            CommandIconType.Stay    => "stay",
            CommandIconType.Use     => "use",
            CommandIconType.Give    => "give",
            CommandIconType.Equip   => "equip",
            CommandIconType.Drop    => "drop",
            CommandIconType.Map     => "map",
            CommandIconType.Speed   => "speed",
            CommandIconType.Message => "message",
            CommandIconType.Quit    => "quit",
            CommandIconType.Save    => "save",
            CommandIconType.Cure    => "cure",
            CommandIconType.Raise   => "raise",
            CommandIconType.Promote => "promote",
            CommandIconType.Buy     => "buy",
            CommandIconType.Deals   => "deals",
            CommandIconType.Sell    => "sell",
            CommandIconType.Repair  => "repair",

            // Fallback (should rarely be needed)
            _ => commandIconType.ToString().ToLowerInvariant()
        };

        return capitalize ? result.ToUpperInvariant() : result;
    }
}