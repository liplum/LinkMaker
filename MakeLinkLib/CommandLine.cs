using System;
using System.Diagnostics;

namespace MakeLinkLib;

public static class CommandLine
{
    public static void Run(string param)
    {
        using var p = new Process();
        p.StartInfo.FileName = @"cmd.exe";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.CreateNoWindow = false;

        p.Start();

        p.StandardInput.AutoFlush = true;

        p.StandardInput.WriteLine(param + "&exit");

        p.WaitForExit();

        var output = p.StandardOutput.ReadToEnd();
        Console.WriteLine(output);
    }
}