using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Security.AccessControl;
    using System.Security.Principal;

namespace Infomat.InfomatTools
{
    

    
    public class Config : IConfig
    {
        
        private XDocument document;

        public Config(string configFile)
        {
            if (!File.Exists(configFile)) File.Create(configFile).Close();
            //Permissions.GrantAccess(configFile);
            try
            {
                XDocument.Load(configFile);
            }
            catch (Exception)
            {
                new XDocument(new XElement("Settings")).Save(configFile);
            }
            document = XDocument.Load(configFile);
        }

        public string GetValue(string section, string key)
        {
            if (section == null)
                throw new ArgumentNullException(nameof(section));
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            string value = "";
            try
            {
                //var xElement = document.Root?.Element(section);
                //var element = xElement?.Element(key);
                //if (element != null) value = element.Value;
                value = document.Root?.Element(section)
                    .Element(key).Value.ToString();
            }
            catch
            {
                throw new NullReferenceException();
            }
            return value;
            
            
        }

        public void SetValue(string section, string key, string value)
        {
            if (section == null)
                throw new ArgumentNullException(nameof(section));
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            //if (document.Root != null && document.Root.IsEmpty) document.Root.SetValue("");

            //if (document.Root != null && document.Root.Element(section) == null) document.Root?.SetElementValue(section, "");

            if (document.Root.IsEmpty) document.Root.SetValue("");

            if (document.Root.Element(section) == null) document.Root?.SetElementValue(section, "");

            var element = document.Root?.Element(section);
                
            element?.SetElementValue(key, value);
   
            document.Save("settings.xml");
            
        }

    }
}
