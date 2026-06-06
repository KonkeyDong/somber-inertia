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
            CommandIconType.Yes     => "Yes",
            CommandIconType.No      => "No",
            CommandIconType.Talk    => "Talk",
            CommandIconType.Magic   => "Magic",
            CommandIconType.Item    => "Item",
            CommandIconType.Search  => "Search",
            CommandIconType.Attack  => "Attack",
            CommandIconType.Stay    => "Stay",
            CommandIconType.Use     => "Use",
            CommandIconType.Give    => "Give",
            CommandIconType.Equip   => "Equip",
            CommandIconType.Drop    => "Drop",
            CommandIconType.Map     => "Map",
            CommandIconType.Speed   => "Speed",
            CommandIconType.Message => "Message",
            CommandIconType.Quit    => "Quit",
            CommandIconType.Save    => "Save",
            CommandIconType.Cure    => "Cure",
            CommandIconType.Raise   => "Raise",
            CommandIconType.Promote => "Promote",
            CommandIconType.Buy     => "Buy",
            CommandIconType.Deals   => "Deals",
            CommandIconType.Sell    => "Sell",
            CommandIconType.Repair  => "Repair",

            // Fallback (should rarely be needed)
            _ => commandIconType.ToString().ToLowerInvariant()
        };

        return capitalize ? result.ToUpperInvariant() : result;
    }
}