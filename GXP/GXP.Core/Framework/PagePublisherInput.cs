using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using GXP.Core.DNNEntities;
namespace GXP.Core.Framework
{
    public class PagePublisherInput
    {
        public HttpContextBase CurrentContext { get; set; }
        public string ApplicationBasePath
        {
            get
            {
                return CurrentContext.Request.PhysicalApplicationPath;
            }
        }
        public Tabs ActiveTab { get; set; }
        public bool CanProcessRequest { get; set; }
        public string  ErrorMessage { get; set; }
        public PortalAlias CurrentPortalAlias { get; set; }
    }
}
