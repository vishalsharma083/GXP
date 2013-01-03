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
        public Tabs ActiveTab { get; set; }
    }
}
