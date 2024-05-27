using KSP.Messages;

namespace Reflektor.Message;

public class ReflektorMessage
{
    public readonly MessageCenterMessage Message;

    private readonly string _messageType;
    private readonly DateTime _time = DateTime.Now;

    public ReflektorMessage(string messageType, MessageCenterMessage message)
    {
        _messageType = messageType;
        Message = (MessageCenterMessage)Activator.CreateInstance(message.GetType());
        CopyInfo(message);
    }

    public string GetDisplayText()
    {
        return $"<{_time.ToLongTimeString()}> {_messageType}";
    }

    private void CopyInfo(MessageCenterMessage message)
    {
        foreach (var property in message.GetType().GetProperties().Where(p => p.CanWrite))
        {
            var value = property.GetValue(message);
            property.SetValue(Message, value);
        }
    }
}