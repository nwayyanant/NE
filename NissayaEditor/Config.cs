using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Configuration
{
    class Config
    {
        string configFile;
        string appConfigContent;
        bool fileExists = true;

        public Config(string fName)
        {
            configFile = Directory.GetCurrentDirectory() + "\\" + fName;
            fileExists = File.Exists(configFile);
            if (!fileExists) return;
            appConfigContent = File.ReadAllText(configFile);
        }

       
        public bool FileExists() { return fileExists; }

        public string ItemValue(string key)
        {
            string searchString = "<add key=\"" + key + "\" value=\"";
            int index = appConfigContent.IndexOf(searchString) + searchString.Length;
            string currentValue = appConfigContent.Substring(index, appConfigContent.IndexOf("\"", index) - index);
            return currentValue;
        }
    }
}
