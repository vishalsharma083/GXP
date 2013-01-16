using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GXP.Core.Framework;
using GXP.Core.Utilities;

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
            PagePublisherInput input =  PagePublisherUtility.ConstructPagePublisherInput(new HttpContextWrapper(context));
            RequestValidator.Validate(input);
            if (input.CanProcessRequest == false)
            {
                context.Response.Write("can not process this request because of below \r\n\r\n");
                context.Response.Write(input.ErrorMessage);
            }
            else
            {
                PagePublisherResult result = publisher.Publish(input);
                context.Response.Write(result.ResponseText);
            }
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