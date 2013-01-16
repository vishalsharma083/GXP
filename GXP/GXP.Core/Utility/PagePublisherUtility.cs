using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using GXP.Core.Framework;
using System.IO;
using System.Xml.Serialization;
using System.Xml;


namespace GXP.Core.Utilities
{
    public class PagePublisherUtility
    {
        public static PagePublisherInput ConstructPagePublisherInput(HttpContextBase context_)
        {
            PagePublisherInput input = new PagePublisherInput();
            try
            {
                input.CurrentContext = context_;
                int tabid = -1;
                int.TryParse(context_.Request.QueryString["tabid"], out tabid);
                if (tabid > -1)
                {
                    string hostName = context_.Request.UserHostName;
                    DNNEntities.PortalAlias portalAlias = DependencyManager.DBService.GetAllPortalAlias().Where(x => x.HTTPAlias == hostName).FirstOrDefault<DNNEntities.PortalAlias>();
                    if (portalAlias == null)
                    {
                        return input;
                    }
                    input.ActiveTab = DependencyManager.DBService.GetAllTabsByPortalId(portalAlias.PortalID).Where(x => x.TabID == tabid).FirstOrDefault<DNNEntities.Tabs>();
                    if (input.ActiveTab == null)
                    {
                        return input;
                    }
                }
                input.CanProcessRequest = true;
            }
            catch (Exception ex)
            {
                input.ErrorMessage = ex.ToString();
            }
            
            return input;
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
