using System;
using System.Reflection;
using System.IO;

namespace ABSoftware.Localization
{
    public class LocalizationManager
    {
        public static ArrayList<Localization> localizations = new ArrayList<Localization>();
        private static int currentLocalizationIndex = -1;

        public static Localization currentLocalization
        {
            get
            {
                if (currentLocalizationIndex < 0 || currentLocalizationIndex >= localizations.Size)
                    return null;

                return localizations[currentLocalizationIndex];
            }
        }

        public static bool SetLocalization(string localizationName)
        {
            currentLocalizationIndex = localizations.FindIndex(l => l.name.Equals(localizationName));

            return !(currentLocalizationIndex < 0);
        }
        
        public static bool SetLocalization(Localization localization)
        {
            if (localization == null)
                return false;

            if(!SetLocalization(localization.name))
            {
                localizations.Add(localization);
                currentLocalizationIndex = localizations.Size - 1;
            }

            return true;
        }

        public static void LoadLocalizations(string directory, string extension = ".klinloc")
        {
            string[] files = Directory.GetFiles(directory);

            for(int i = 0; i < files.Length; i++)
            {
                if (Path.GetExtension(files[i]).Equals(extension))
                    LoadLocalization(files[i]);
            }
        }

        public static Localization LoadLocalization(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            if (localizations.FindIndex(l => l.localizationFilename.Equals(fileName)) == -1)
            {
                Localization localization = new Localization(fileName);
                localization.Load(File.ReadAllText(path));
                localizations.Add(localization);

                return localization;
            }

            return null;
        }

        public static Localization LoadLocalization(string localizationFilename, string name, KLIN data)
        {
            if (localizations.FindIndex(l => l.localizationFilename.Equals(localizationFilename)) == -1)
            {
                Localization localization = new Localization(localizationFilename);
                localization.data = data;
                localization.name = name;
                data["LocalizationName"].PropertyObject = name;
                localizations.Add(localization);

                return localization;
            }

            return null;
        }

        public static KLIN CreateLocalizationSnippet(string localizationName)
        {
            KLIN klin = new KLIN();
            klin["LocalizationName"].PropertyObject = localizationName;

            FieldInfo[] fields = GetLocalizableFields();

            for (int i = 0; i < fields.Length; i++)
            {
                if(fields[i].FieldType.IsArray)
                {
                    LocalizeStringAttribute[] attributes = fields[i].GetLocalizeStringAttributes();

                    string[] array = (string[])fields[i].GetValue(null);

                    if (array == null || (attributes.Length > array.Length))
                        array = new string[attributes.Length];

                    for (int j = 0; j < attributes.Length; j++)
                    {
                        klin[attributes[j].Key].PropertyObject = string.IsNullOrEmpty(array[j]) ? string.Empty : array[j];
                    }
                }
                else
                {
                    LocalizeStringAttribute attribute = fields[i].GetLocalizeStringAttribute();

                    klin[attribute.Key].PropertyObject = fields[i].GetValue(null);
                }
            }

            return klin;
        }

        public static void LocalizeAllFields()
        {
            if (currentLocalization == null)
                return;

            FieldInfo[] fields = GetLocalizableFields();

            for (int i = 0; i < fields.Length; i++)
            {
                if(fields[i].FieldType.IsArray)
                {
                    LocalizeStringAttribute[] attributes = fields[i].GetLocalizeStringAttributes();

                    string[] array = (string[])fields[i].GetValue(null);

                    if (array == null || (attributes.Length > array.Length))
                        array = new string[attributes.Length];

                    for(int a = 0; a < attributes.Length; a++)
                    {
                        string s = currentLocalization.GetLocalization(attributes[a].Key);

                        if (s == null)
                            continue;

                        array[a] = s;
                    }

                    fields[i].SetValue(null, array);
                }
                else
                {
                    LocalizeStringAttribute attribute = fields[i].GetLocalizeStringAttribute();

                    string s = currentLocalization.GetLocalization(attribute.Key);

                    if (s == null)
                        continue;

                    fields[i].SetValue(null, s);
                }
            }
        }

        private static FieldInfo[] GetLocalizableFields()
        {
            ArrayList<FieldInfo> fieldInfos = new ArrayList<FieldInfo>();

            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();

            for (int t = 0; t < types.Length; t++)
            {
                FieldInfo[] fields = types[t].GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
                
                for (int f = 0; f < fields.Length; f++)
                {
                    if (fields[f].FieldType.IsArray)
                    {
                        LocalizeStringAttribute[] attributes = fields[f].GetLocalizeStringAttributes();
                        if (attributes != null && attributes.Length > 0)
                            fieldInfos.Add(fields[f]);
                    }
                    else
                    {
                        LocalizeStringAttribute attribute = fields[f].GetLocalizeStringAttribute();
                        if (attribute != null)
                            fieldInfos.Add(fields[f]);
                    }
                }
            }

            return fieldInfos.GetElements();
        }

        public void Clear()
        {
            localizations.Clear();
        }
    }
}
