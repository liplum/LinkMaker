using static MakeLinkLib.LinkModeHelper;

namespace MakeLinkLib;

public class MkLink
{
    public LinkMode Mode { get; init; }
    public string Link { get; init; }
    public string Target { get; init; }

    public void Run()
    {
        CommandLine.Run($"mklink {Mode.GetParameter()} \"{Link}\" \"{Target}\"");
    }
}