using BepInEx.Logging;
using HarmonyLib;
using Reflektor.Logs;

namespace Reflektor.Patches;

public static class BepInExLogPatch
{
    [HarmonyPatch(typeof(ManualLogSource), nameof(ManualLogSource.Log))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    public static void ManualLogListener(LogLevel level, object data, ManualLogSource __instance)
    {
        var flag = level switch
        {
            LogLevel.Fatal => VisibilityFlag.Fatal,
            LogLevel.Error => VisibilityFlag.Error,
            LogLevel.Warning => VisibilityFlag.Warning,
            LogLevel.Message => VisibilityFlag.Message,
            LogLevel.Info => VisibilityFlag.Info,
            LogLevel.Debug => VisibilityFlag.Debug,
            LogLevel.All => VisibilityFlag.All,
            _ => VisibilityFlag.None
        };

        LogListener.AddLog(new ReflektorLog(flag, data == null ? "null" : data.ToString(), __instance.SourceName));
    }
}