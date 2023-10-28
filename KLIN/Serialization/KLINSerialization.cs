using System;
using System.Reflection;

namespace ABSoftware
{
    public class KLINSerialization
    {
        public static T Deserialize<T>(KLIN klin, string root = "") where T : new()
        {
            Type type = typeof(T);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
            T deserialized = new T();
            for(int i = 0; i < fields.Length; i++)
            {
                KLINProperty property = fields[i].GetKLINPropertyAttribute();
                if (property != null)
                {
                    string propertyName = CombineTrees(root, (property.Name == null) ? fields[i].Name : property.Name);
                    if(fields[i].FieldType.IsArray)
                    {
                        Type arrayType = fields[i].FieldType.GetElementType();
                        KLINArray array = new KLINArray();
                        array.Parse(SolveKLINTree(klin, propertyName));
                        Array objects = Array.CreateInstance(arrayType, array.Size);
                        for(int o = 0; o < objects.Length; o++)
                        {
                            objects.SetValue(TypeConverter(arrayType, array.Get(o).ToString()), o);
                        }
                        fields[i].SetValue(deserialized, objects);
                        array.Clear();
                    }
                    else
                    {
                        fields[i].SetValue(deserialized, TypeConverter(fields[i].FieldType, SolveKLINTree(klin, propertyName)));
                    }
                }
            }

            return deserialized;
        }

        public static void Serialize<T>(KLIN klin, T instance, string root = "")
        {
            Type type = typeof(T);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++)
            {
                KLINProperty property = fields[i].GetKLINPropertyAttribute();
                if (property != null)
                {
                    string propertyName = CombineTrees(root, (property.Name == null) ? fields[i].Name : property.Name);
                    if (fields[i].FieldType.IsArray)
                    {
                        KLINArray array = new KLINArray();
                        Array fieldArray = (Array)fields[i].GetValue(instance);
                        for (int o = 0; o < fieldArray.Length; o++)
                        {
                            array.Add(fieldArray.GetValue(o));
                        }
                        GetLastKLINTreeToken(klin, propertyName).PropertyObject = array.ToString();
                        array.Clear();
                    }
                    else
                    {
                        GetLastKLINTreeToken(klin, propertyName).PropertyObject = fields[i].GetValue(instance);
                    }
                }
            }
        }

        private static string SolveKLINTree(KLIN klin, string tree)
        {
            string[] entries = tree.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (entries.Length < 1)
                return null;
            if (entries.Length == 1)
            {
                if (klin.PropertyExists(entries[0]))
                    return klin[entries[0]].PropertyObject.ToString();
                else
                    return string.Empty;
            }
            KLINToken token = klin[entries[0]];
            for (int i = 1; i < entries.Length; i++)
            {
                if (token == null)
                    break;
                token = token[entries[i]];
            }
            if (token == null)
                return null;
            return token.PropertyObject.ToString();
        }

        private static KLINToken GetLastKLINTreeToken(KLIN klin, string tree)
        {
            string[] entries = tree.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (entries.Length < 1)
                return null;
            if (entries.Length == 1)
            {
                return klin[entries[0]];
            }
            KLINToken token = klin[entries[0]];
            for (int i = 1; i < entries.Length; i++)
            {
                if (token == null)
                    break;
                token = token[entries[i]];
            }
            if (token == null)
                return null;
            return token;
        }

        private static string CombineTrees(string a, string b)
        {
            if (a.Length < 1)
                return b;
            if (b.Length < 1)
                return a;
            string c = (a[a.Length - 1] == '.' ? a : a + ".") + b;
            return c;
        }

        private static object TypeConverter(Type fieldType, string value)
        {
            if(string.IsNullOrEmpty(value))
            {
                if (fieldType != typeof(string)) return 0;
                else return value;
            }

            if (fieldType == typeof(sbyte)) return sbyte.Parse(value);
            if (fieldType == typeof(byte)) return byte.Parse(value);
            if (fieldType == typeof(short)) return short.Parse(value);
            if (fieldType == typeof(ushort)) return ushort.Parse(value);
            if (fieldType == typeof(int)) return int.Parse(value);
            if (fieldType == typeof(uint)) return uint.Parse(value);
            if (fieldType == typeof(long)) return long.Parse(value);
            if (fieldType == typeof(ulong)) return ulong.Parse(value);

            if (fieldType == typeof(float)) return float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
            if (fieldType == typeof(double)) return double.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);

            return value;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class KLINProperty : Attribute
    {
        public string Name;

        /// <param name="Name">Property name in KLIN. Set to null by default, which makes the deserializator use the field's name. Name can use dots to define a tree. E.g. Group1.Group2.Property</param>
        public KLINProperty(string Name = null)
        {
            this.Name = Name;
        }
    }

    public static class KLINPropertyUtils
    {
        public static KLINProperty GetKLINPropertyAttribute(this FieldInfo fieldInfo)
        {
            KLINProperty[] attributes = (KLINProperty[])fieldInfo.GetCustomAttributes(typeof(KLINProperty));
            if (attributes.Length > 0)
                return attributes[0];

            return null;
        }
    }
}