using System;
using System.Reflection;

namespace MakeLinkLib;

public static class MkLinkEnum
{
    public enum LinkMode
    {
        [Parameter(@"/d")]
        DirectorySymbolLink,
        [Parameter("")]
        FileSymbolLink,
        [Parameter(@"/h")]
        HardLink,
        [Parameter(@"/j")]
        JunctionLink
    }

    private class Parameter : Attribute
    {
        public string Param { get; }
        public Parameter(string par)
        {
            Param = par;
        }
    }

    public static string GetParameter(this LinkMode type)
    {
        var fi = type.GetType().GetField(type.ToString());
        var attributes = fi!.GetCustomAttributes(typeof(Parameter), false);
        if (attributes.Length > 0)
        {
            return ((Parameter)attributes[0]).Param;
        }
        throw new NotDefineException($"Parameter is undefended in {type}");
    }
    public class NotDefineException : Exception
    {
        public NotDefineException(string message) : base(message) { }
    }
}