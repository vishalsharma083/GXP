using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXP.Core.Interfaces
{
    public interface ILoggingService
    {
        void WriteLog(string message_, int portalId_, int tabId_, int moduleId_);

        void WriteLog(string p);

        void WriteLog(string p, string p_2, System.Web.HttpContext httpContext, string p_3, string p_4);
    }
}
