using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using GXP.Core.Framework;
using System.IO;

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
            return File.ReadAllText(filePath_);
        }

        public static string GetAllFileContent(string filePath_)
        {
            return File.ReadAllText(filePath_);
        }
    }
}
