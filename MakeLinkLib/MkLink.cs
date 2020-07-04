using static MakeLinkLib.MkLinkEumn;

namespace MakeLinkLib
{
    public class MkLink
    {
        private LinkMode Mode { get; set; }
        private string Link { get; set; }
        private string Target { get; set; }
        public MkLink(LinkMode mode, string link, string target)
        {
            Mode = mode;
            Link = link;
            Target = target;
        }
        public void Run()
        {
            CMD.Run($"mklink {Mode.GetParameter()} \"{Link}\" \"{Target}\"");
        }
    }
}
