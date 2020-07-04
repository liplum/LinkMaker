using System.Diagnostics;

namespace MakeLinkLib
{
    public static class CMD
    {
        public static void Run(string param)
        {
            using var p = new Process();
            p.StartInfo.FileName = @"cmd.exe";
            p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
            p.StartInfo.CreateNoWindow = false;          //不显示程序窗口

            p.Start();//启动程序

            p.StandardInput.AutoFlush = true;

            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine(param + "&exit");

            //等待完成
            p.WaitForExit();
/*
            //获取cmd窗口的输出信息
            string output = p.StandardOutput.ReadToEnd();
            return output;*/
        }
    }
}
