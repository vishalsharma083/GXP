using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GXP.Core.Framework;
using GXP.Core.Utility;

namespace WebApp
{
    /// <summary>
    /// Summary description for PageRequestHandler
    /// </summary>
    public class PageRequestHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            PagePublisher publisher = new PagePublisher();
            publisher.Publish(PagePublisherUtility.ConstructPagePublisherInput(new HttpContextWrapper(context)));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}