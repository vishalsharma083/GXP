using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using GXP.Core.Framework;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace GXP.Core.Utility
{
    public class PagePublisherUtility
    {
        public static PagePublisherInput ConstructPagePublisherInput(HttpContextBase context_)
        {
            throw new NotImplementedException();
        }

        public static string GetViewModeModuleContent(string filePath_)
        {
            if (File.Exists(filePath_))
            {
                return File.ReadAllText(filePath_);    
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetAllFileContent(string filePath_)
        {
            return File.ReadAllText(filePath_);
        }

        public static T DeserializeObject<T>(string xml_)
        {
            T retObject;

            XmlSerializer xSerializer = new XmlSerializer(typeof(T));
            using (StringReader stringReader = new StringReader(xml_))
            {
                retObject = (T)xSerializer.Deserialize(stringReader) ;
            }
            return retObject;
        }
    }
}
