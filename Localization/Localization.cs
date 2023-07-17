using System;

namespace ABSoftware.Localization
{
    public class Localization
    {
        public string localizationFilename;
        public string name;
        public KLIN data;

        public Localization(string fileName)
        {
            this.localizationFilename = fileName;
        }

        public void Load(string klinData)
        {
            data = new KLIN(klinData);
            name = data["LocalizationName"].PropertyObject.ToString();
        }

        public string GetLocalization(string key)
        {
            object obj = data.GetProperty(key);
            if (obj == null)
                return null;
            return obj.ToString();
        }
    }
}
