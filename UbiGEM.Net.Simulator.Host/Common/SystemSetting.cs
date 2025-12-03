using System;
using System.IO;
using System.Xml.Linq;

namespace UbiGEM.Net.Simulator.Host.Common
{
    public class SystemSetting
    {
        private const string SETTING_KEY_LAST_EDITED_FILEPATH = "LastEditedFilepath";
        private const string DIR = @"UbiSam\UbiGEM";
        private const string FILENAME = "UbiGEM.Net.Simulator";

        public string LastEditedFilepath { get; set; }

        public SystemSetting()
        {
            LastEditedFilepath = string.Empty;
            LoadSystemSetting();
        }

        public void SaveSystemSetting()
        {
            string dirPath;
            string myDocumentPath;
            string path;
            XElement root;
            XElement element;

            myDocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dirPath = string.Format(@"{0}\{1}", myDocumentPath, DIR);

            if (Directory.Exists(dirPath) == false)
            {
                Directory.CreateDirectory(dirPath);
            }

            path = string.Format(@"{0}\{1}\{2}.ini", myDocumentPath, DIR, FILENAME);
            root = new XElement(FILENAME);

            if (string.IsNullOrEmpty(LastEditedFilepath) == false)
            {
                element = new XElement(SETTING_KEY_LAST_EDITED_FILEPATH, LastEditedFilepath);
                root.Add(element);
            }

            root.Save(path);
        }

        private void LoadSystemSetting()
        {
            string myDocumentPath;
            string path;
            XElement root;
            XElement element;

            myDocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = string.Format(@"{0}\{1}\{2}.ini", myDocumentPath, DIR, FILENAME);

            if (File.Exists(path) == true)
            {
                try
                {
                    root = XElement.Load(path);
                    if (root.Name == FILENAME)
                    {
                        element = root.Element(SETTING_KEY_LAST_EDITED_FILEPATH);
                        if (element != null)
                        {
                            LastEditedFilepath = element.Value;
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    element = null;
                    root = null;
                }
            }
        }
    }
}
