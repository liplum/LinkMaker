using System;
using System.Reflection;

namespace MakeLinkLib
{
    public static class MkLinkEumn
    {
        public enum LinkMode
        {
            [Parameter(@"/d")]
            Ddir,
            [Parameter("")]
            Dfile,
            [Parameter(@"/h")]
            H,
            [Parameter(@"/j")]
            J
        }

        public class Parameter : Attribute
        {
            public string Param { set; get; }
            public Parameter(string par)
            {
                Param = par;
            }
        }

        public static string GetParameter(this LinkMode type)
        {
            FieldInfo fi = type.GetType().GetField(type.ToString());
            object[] attributes = fi.GetCustomAttributes(typeof(Parameter), false);
            if (attributes.Length > 0)
            {
                return ((Parameter)attributes[0]).Param;
            }
            throw new NotDefineException($"Parameter is undefine in {type}");
        }
        public class NotDefineException : Exception
        {
            public NotDefineException(string message) : base(message) { }
        }
    }
}
