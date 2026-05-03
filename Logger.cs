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

    private static readonly object _lock = new object();

    public static void Log(LogLevel level, string message)
    {
        if (level < MinimumLevel) return;

        lock (_lock)  // thread-safe
        {
            string prefix = level switch
            {
                LogLevel.Debug   => "[DEBUG]  ",
                LogLevel.Info    => "[INFO]   ",
                LogLevel.Warning => "[WARN]   ",
                LogLevel.Error   => "[ERROR]  ",
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