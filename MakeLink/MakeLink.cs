using System.IO;

namespace MakeLink
{
    class MakeLink
    {
        static void Main(string[] args)
        {
            var path = @"G:\Test\需要被链接的文件夹";
            var ifFile = new FileInfo(path);
            System.Console.WriteLine(ifFile.Exists);
            var ifDir = new DirectoryInfo(path);
            System.Console.WriteLine(ifDir.Exists);
        }
    }
}
