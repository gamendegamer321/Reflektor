using UitkForKsp2.API;
using UnityEngine.UIElements;

namespace Reflektor.Windows;

public class Navigator
{
    private static readonly UIDocument Window;

    private static readonly Toggle InspectorToggle;
    private static readonly Toggle BrowserToggle;
    private static readonly Toggle LoggerToggle;
    private static readonly Toggle MessagesToggle;

    static Navigator()
    {
        Window = Utils.GetNewWindow("Navbar");

        InspectorToggle = Window.rootVisualElement.Q<Toggle>("InspectorToggle");
        BrowserToggle = Window.rootVisualElement.Q<Toggle>("BrowserToggle");
        LoggerToggle = Window.rootVisualElement.Q<Toggle>("LoggerToggle");
        MessagesToggle = Window.rootVisualElement.Q<Toggle>("MessagesToggle");

        InspectorToggle.RegisterValueChangedCallback(evt => UpdateValue(WindowId.Inspector, evt.newValue));
        BrowserToggle.RegisterValueChangedCallback(evt => UpdateValue(WindowId.Browser, evt.newValue));
        LoggerToggle.RegisterValueChangedCallback(evt => UpdateValue(WindowId.Logger, evt.newValue));
        MessagesToggle.RegisterValueChangedCallback(evt => UpdateValue(WindowId.Messages, evt.newValue));
    }

    public static void UpdateValue(WindowId id, bool targetValue)
    {
        Toggle target;
        switch (id)
        {
            case WindowId.Inspector:
                target = InspectorToggle;
                Inspector.SetDisplay(targetValue);
                break;
            case WindowId.Browser:
                target = BrowserToggle;
                Browser.SetDisplay(targetValue);
                break;
            case WindowId.Logger:
                target = LoggerToggle;
                LoggerWindow.SetDisplay(targetValue);
                break;
            case WindowId.Messages:
                target = MessagesToggle;
                Messages.SetDisplay(targetValue);
                break;
            default:
                // TODO: log error
                return;
        }

        if (target.value != targetValue)
        {
            target.SetValueWithoutNotify(targetValue);
        }
    }

    public static void ToggleDisplay()
    {
        Window.ToggleDisplay();
    }

    public enum WindowId
    {
        Inspector,
        Browser,
        Logger,
        Messages
    }
}