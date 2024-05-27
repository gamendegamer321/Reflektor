using Reflektor.Windows;

namespace Reflektor.Logs;

public static class LogListener
{
    private static readonly List<ReflektorLog> Caught = new();

    public static void AddLog(ReflektorLog log)
    {
        Caught.Add(log);
        LoggerWindow.AutoRefresh();
    }

    public static IEnumerable<ReflektorLog> CaughtLogs()
    {
        return Caught;
    }

    public static void Clear()
    {
        Caught.Clear();
    }
}