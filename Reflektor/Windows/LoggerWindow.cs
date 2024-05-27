using System.Text;
using BepInEx.Logging;
using Reflektor.Logs;
using UitkForKsp2.API;
using UnityEngine.UIElements;

namespace Reflektor.Windows;

public class LoggerWindow
{
    private static VisibilityFlag _visibility = VisibilityFlag.All;

    private static List<ReflektorLog> _showLogs = new();

    private static readonly UIDocument Window;

    private static readonly TextField FilterInput;
    private static readonly Toggle AutoRefreshToggle;
    private static readonly Label Logs;

    static LoggerWindow()
    {
        Window = Utils.GetNewWindow("LoggerWindow");

        FilterInput = Window.rootVisualElement.Q<TextField>("FilterInput");
        AutoRefreshToggle = Window.rootVisualElement.Q<Toggle>("AutoRefreshToggle");
        Logs = Window.rootVisualElement.Q<Label>("Logs");

        FilterInput.RegisterValueChangedCallback(_ => Refresh());

        Window.rootVisualElement.Q<Toggle>("FatalToggle")
            .RegisterValueChangedCallback(evt => UpdateVisibilityFlag(VisibilityFlag.Fatal, evt.newValue));
        Window.rootVisualElement.Q<Toggle>("ErrorToggle")
            .RegisterValueChangedCallback(evt => UpdateVisibilityFlag(VisibilityFlag.Error, evt.newValue));
        Window.rootVisualElement.Q<Toggle>("WarningToggle")
            .RegisterValueChangedCallback(evt => UpdateVisibilityFlag(VisibilityFlag.Warning, evt.newValue));
        Window.rootVisualElement.Q<Toggle>("MessageToggle")
            .RegisterValueChangedCallback(evt => UpdateVisibilityFlag(VisibilityFlag.Message, evt.newValue));
        Window.rootVisualElement.Q<Toggle>("InfoToggle")
            .RegisterValueChangedCallback(evt => UpdateVisibilityFlag(VisibilityFlag.Info, evt.newValue));
        Window.rootVisualElement.Q<Toggle>("DebugToggle")
            .RegisterValueChangedCallback(evt => UpdateVisibilityFlag(VisibilityFlag.Debug, evt.newValue));
        
        Window.rootVisualElement.Q<Button>("RefreshButton").clicked += Refresh;
    }

    public static void AutoRefresh()
    {
        if (!AutoRefreshToggle.value)
        {
            Refresh();
        }
    }
    
    private static void Refresh()
    {
        _showLogs.AddRange(LogListener.CaughtLogs());
        LogListener.Clear();

        var maxLogCount = Reflektor.Instance.MaxLogs.Value;
        while (_showLogs.Count > maxLogCount)
        {
            _showLogs.RemoveAt(0);
        }

        StringBuilder logText = new();
        foreach (var log in _showLogs.Where(e => e.Filter(_visibility, FilterInput.value)).Reverse())
        {
            logText.AppendLine(log.GetFormattedText());
        }

        Logs.Text(logText.ToString());
    }

    private static readonly ManualLogSource ManualLogSource = Logger.CreateLogSource("Logger window #1232");
    
    public static void SetDisplay(bool visible)
    {
        ManualLogSource.LogInfo("Wow this is now toggled!");
        
        if (visible)
        {
            Window.rootVisualElement.Show();
        }
        else
        {
            Window.rootVisualElement.Hide();
        }
    }

    private static void UpdateVisibilityFlag(VisibilityFlag flag, bool newValue)
    {
        _visibility = newValue ? _visibility | flag : _visibility & ~flag;
        Refresh();
    }
}