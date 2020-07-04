using MakeLinkLib;

namespace MakeLink
{
    class MakeLink
    {
        static void Main(string[] args)
        {
            string link = @"E:\E盘链接",
                target = @"G:\G盘源文件";

            var cmd = new MkLink(MkLinkEumn.LinkMode.D,link,target);
            cmd.Run();
        }
    }
}
