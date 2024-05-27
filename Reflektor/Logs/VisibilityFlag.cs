namespace Reflektor.Logs;

[Flags]
public enum VisibilityFlag
{
    None = 0,
    Fatal = 1,
    Error = 2,
    Warning = 4,
    Message = 8,
    Info = 16,
    Debug = 32,
    All = None | Fatal | Error | Warning | Message | Info | Debug
}