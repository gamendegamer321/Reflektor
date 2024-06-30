using HarmonyLib;
using KSP.Messages;
using Reflektor.Message;
using Reflektor.Windows;

namespace Reflektor.Patches;

public static class MessagesPatch
{
    [HarmonyPatch(typeof(MessageCenter), nameof(MessageCenter.Publish), typeof(Type), typeof(MessageCenterMessage))]
    [HarmonyPriority(Priority.First)]
    [HarmonyPrefix]
    public static void MessageListener(Type type, MessageCenterMessage message)
    {
        if (!Reflektor.Instance.IsActive) return;

        // If we don't have any info about a message, return
        // Otherwise try to fill the missing type/message
        switch (type)
        {
            case null when message == null:
                return;
            case null:
                type = message.GetType();
                break;
            default:
                message ??= (MessageCenterMessage)Activator.CreateInstance(type);
                break;
        }

        Messages.Add(new ReflektorMessage(type.ToString(), message));
    }
}