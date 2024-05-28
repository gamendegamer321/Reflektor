using System.Text;
using BepInEx.Logging;
using Reflektor.Logs;
using UitkForKsp2.API;
using UnityEngine.UIElements;

namespace Reflektor.Windows;

public class LoggerWindow
{
    private static VisibilityFlag _visibility = VisibilityFlag.All;

    private static readonly List<ReflektorLog> InternalLogs = new();
    private static readonly List<ReflektorLog> ShownLogs = new();

    private static readonly UIDocument Window;

    private static readonly TextField FilterInput;
    private static readonly Toggle AutoRefreshToggle;
    private static readonly ListView LogsView;

    static LoggerWindow()
    {
        Window = Utils.GetNewWindow("LoggerWindow");

        FilterInput = Window.rootVisualElement.Q<TextField>("FilterInput");
        AutoRefreshToggle = Window.rootVisualElement.Q<Toggle>("AutoRefreshToggle");

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

        LogsView = Window.rootVisualElement.Q<ListView>("LogsView");
        LogsView.SetEmptyText("No logs available");
        LogsView.itemsSource = ShownLogs;
        LogsView.makeItem = () => new Label();
        LogsView.bindItem = (element, i) =>
        {
            var log = ShownLogs[i];
            if (element is not Label label)
            {
                return;
            }

            label.text = log.GetFormattedText();
        };

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
        var newLogs = LogListener.CaughtLogs();

        if (newLogs.Count > 0)
        {
            InternalLogs.AddRange(newLogs);
            LogListener.Clear();

            var maxLogCount = Reflektor.Instance.MaxLogs.Value;
            while (InternalLogs.Count > maxLogCount)
            {
                InternalLogs.RemoveAt(0);
            }
        }

        ShownLogs.Clear();
        ShownLogs.AddRange(InternalLogs.Where(e => e.Filter(_visibility, FilterInput.value)).Reverse());

        LogsView.ClearSelection();
        LogsView.Rebuild();
        LogsView.RefreshItems();
    }

    public static void SetDisplay(bool visible)
    {
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