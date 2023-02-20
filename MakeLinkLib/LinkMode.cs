using System;
using System.Reflection;

namespace MakeLinkLib;

public enum LinkMode
{
    [Parameter(@"/d")] DirectorySymbolicLink,
    [Parameter("")] FileSymbolicLink,
    [Parameter(@"/h")] HardLink,
    [Parameter(@"/j")] JunctionLink
}

public class Parameter : Attribute
{
    public string Param { get; }

    public Parameter(string par)
    {
        Param = par;
    }
}

public static class LinkHelper
{
    private static string GetParameter(this LinkMode type)
    {
        var fi = type.GetType().GetField(type.ToString());
        var attributes = fi!.GetCustomAttributes(typeof(Parameter), false);
        if (attributes.Length > 0)
        {
            return ((Parameter)attributes[0]).Param;
        }

        throw new Exception($"Parameter is undefended in {type}");
    }

    public static void MakeLink(LinkMode mode, string linkPath, string targetPath)
    {
        CommandLine.Run($"mklink {mode.GetParameter()} \"{linkPath}\" \"{targetPath}\"");
    }
}