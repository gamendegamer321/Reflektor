using Reflektor.Windows;

namespace Reflektor.Logs;

public static class LogListener
{
    private static readonly List<ReflektorLog> Caught = new();

    public static void AddLog(ReflektorLog log)
    {
        Caught.Add(log);
        LoggerWindow.AutoRefresh();
        
        var maxLogCount = Reflektor.Instance.MaxLogs.Value;
        while (Caught.Count > maxLogCount)
        {
            Caught.RemoveAt(0);
        }
    }

    public static List<ReflektorLog> CaughtLogs()
    {
        return Caught;
    }

    public static void Clear()
    {
        Caught.Clear();
    }
}