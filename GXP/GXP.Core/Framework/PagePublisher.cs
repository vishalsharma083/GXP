using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXP.Core.Framework
{
    public class PagePublisher
    {
        private PagePublisherInput _input = null;
        public PageRequestValidationResult IsValidRequest()
        {
            // TODO : validate request against each validation logic.
            throw new NotImplementedException("IsValidRequest");
        }

        public PagePublisherResult Publish(PagePublisherInput input_)
        {
            _input = input_;
            PagePublisherResult result = new PagePublisherResult();
            if (IsValidRequest().IsValid)
            {
                string xslt = PrepareXSLT();
                input_.CurrentContext.Response.Write(DoTransformation(xslt));
                DoHttpCacheSettings(input_);
                result.Status = PublishStatus.SUCCESS;
            }
            else
            {
                throw new NotImplementedException("Publish.if.else");
            }
            throw new NotImplementedException("Publish");
        }

        private void DoHttpCacheSettings(PagePublisherInput input_)
        {
            throw new NotImplementedException("DoHttpCacheSettings");
        }

        private string DoTransformation(string xslt_)
        {
            CMSXsltUtility cmsXsltUtility = new CMSXsltUtility();
            cmsXsltUtility.PublishingDetail = this._input;
            return cmsXsltUtility.PerformTransformation(xslt_, "<root/>", true);
        }

        private string PrepareXSLT()
        {
            // Load view mode module content to be inject into skeleton.
            throw new NotImplementedException("PrepareXSLT");
        }
    }
}


// 1.Take the request.
// 2.Load Input 
// 3.Validate it.
// 4.Prepare XSLT To Transform
// 5.Generate html