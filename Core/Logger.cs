namespace SomberInertia;

public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error,
    Fatal
}

public static class Logger
{
    public static LogLevel MinimumLevel { get; set; } = LogLevel.Info;

    private const string Reset = "\x1b[0m"; // white
    private const string Red = "\x1b[91m";
    private const string Cyan = "\x1b[36m";
    private const string Yellow = "\x1b[33m";
    private const string Green = "\x1b[92m";

    private static readonly object _lock = new object();

    public static void Log(LogLevel level, string message)
    {
        if (level < MinimumLevel) 
        {
            return;
        }

        lock (_lock)  // thread-safe
        {
            var prefix = level switch
            {
                LogLevel.Debug   => $"{Green}[DEBUG]{Reset}  ",
                LogLevel.Info    => $"{Cyan}[INFO]{Reset}   ",
                LogLevel.Warning => $"{Yellow}[WARN]{Reset}   ",
                LogLevel.Error   => $"{Red}[ERROR]{Reset}  ",
                LogLevel.Fatal   => "[FATAL]  ",
                _ => "[LOG]    "
            };

            Console.WriteLine($"{prefix}{message}");
        }
    }

    // Convenience methods
    public static void Debug(string message)   => Log(LogLevel.Debug, message);
    public static void Info(string message)    => Log(LogLevel.Info, message);
    public static void Warning(string message) => Log(LogLevel.Warning, message);
    public static void Error(string message)   => Log(LogLevel.Error, message);
    public static void Fatal(string message)   => Log(LogLevel.Fatal, message);
}