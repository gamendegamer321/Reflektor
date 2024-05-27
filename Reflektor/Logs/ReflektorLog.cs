namespace Reflektor.Logs;

public class ReflektorLog
{
    private readonly VisibilityFlag _visibilityFlag;
    private readonly string _sourceName;
    private readonly string _message;
    private readonly DateTime _time;

    public ReflektorLog(VisibilityFlag visibility, string message, string sourceName)
    {
        _visibilityFlag = visibility;
        _sourceName = sourceName;
        _message = message;
        _time = DateTime.Now;
    }

    public bool Filter(VisibilityFlag visibility, string filterString)
    {
        return (visibility & _visibilityFlag) > 0
               && (filterString == null
                   || _message.Contains(filterString, StringComparison.InvariantCultureIgnoreCase));
    }

    public string GetFormattedText()
    {
        var visibilityString = _visibilityFlag switch
        {
            VisibilityFlag.Fatal => "<color=#CC6666>[Fatal]</color>",
            VisibilityFlag.Error => "<color=#CC6666>[<b>Error</b>]</color>",
            VisibilityFlag.Warning => "<color=#F0C674>[Warning]</color>",
            VisibilityFlag.Message => "<color=#8ABEB7>[Message]</color>",
            VisibilityFlag.Info => "<color=#B5BD68>[Info]</color>",
            VisibilityFlag.Debug => "<color=#B294BB>[Debug]</color>",
            _ => "<color=#81A2BE>[Unknown]</color>"
        };

        return $"<{_time.ToLongTimeString()}> {visibilityString} [{_sourceName}] {_message}";
    }
}