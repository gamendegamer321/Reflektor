using Reflektor.Message;
using UitkForKsp2.API;
using UnityEngine.UIElements;
using Enumerable = UniLinq.Enumerable;

namespace Reflektor.Windows;

public class Messages
{
    private static readonly UIDocument Window;

    private static readonly TextField FilterInput;
    private static readonly ListView MessagesList;

    private static readonly List<ReflektorMessage> InternalMessages = new();
    private static readonly List<ReflektorMessage> PublishedMessages = new();

    static Messages()
    {
        Window = Utils.GetNewWindow("MessagesWindow");

        FilterInput = Window.rootVisualElement.Q<TextField>("FilterInput");
        FilterInput.RegisterValueChangedCallback(_ => Refresh());

        MessagesList = Window.rootVisualElement.Q<ListView>("MessageView");
        MessagesList.SetEmptyText("No messages available");
        MessagesList.itemsSource = PublishedMessages;
        MessagesList.makeItem = () => new Label();
        MessagesList.bindItem = (element, i) =>
        {
            var messageInfo = PublishedMessages[i];
            if (element is not Label label)
            {
                return;
            }

            label.text = messageInfo.GetDisplayText();
        };
        MessagesList.itemsChosen += enumerable =>
        {
            var obj = enumerable.First();
            if (obj is ReflektorMessage message)
            {
                Inspector.SwitchTab(new SelectKey(message.Message));
            }
        };
    }

    public static void Add(ReflektorMessage message)
    {
        InternalMessages.Add(message);
        while (InternalMessages.Count > Reflektor.Instance.MaxMessages.Value)
        {
            InternalMessages.RemoveAt(0);
        }

        Refresh();
    }

    private static void Refresh()
    {
        PublishedMessages.Clear();

        if (FilterInput.value is null or "")
        {
            PublishedMessages.AddRange(Enumerable.Reverse(InternalMessages));
        }
        else
        {
            var filterString = FilterInput.value;
            PublishedMessages.AddRange(InternalMessages.Where(e => e.GetDisplayText().Contains(filterString)).Reverse());
        }

        MessagesList.ClearSelection();
        MessagesList.Rebuild();
        MessagesList.RefreshItems();
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
}