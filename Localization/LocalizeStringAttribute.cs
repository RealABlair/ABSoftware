using System;
using System.Reflection;

namespace ABSoftware.Localization
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class LocalizeStringAttribute : Attribute
    {
        public LocalizeStringAttribute(string Key)
        {
            this.Key = Key;
        }

        public string Key;
    }

    public static class LocalizeStringAttributeUtils
    {
        public static LocalizeStringAttribute GetLocalizeStringAttribute(this FieldInfo field)
        {
            LocalizeStringAttribute[] attributes = (LocalizeStringAttribute[])field.GetCustomAttributes(typeof(LocalizeStringAttribute), false);
            if (attributes.Length > 0)
                return attributes[0];

            return null;
        }

        public static LocalizeStringAttribute[] GetLocalizeStringAttributes(this FieldInfo field)
        {
            LocalizeStringAttribute[] attributes = (LocalizeStringAttribute[])field.GetCustomAttributes(typeof(LocalizeStringAttribute), false);
            
            return attributes;
        }
    }
}