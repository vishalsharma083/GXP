using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.Utility;
using System.Text.RegularExpressions;

namespace GXP.Core.Framework
{
    public class PagePublisher
    {
        private static Regex RemoveRunAtServerVisibleAndControlAttribute = new Regex("(?<runat>(runat=\"server\"))|(?<visible>(visible=\"false\"))|(?<controltag><%.*%>)",RegexOptions.Compiled);
        private const string FindPaneRegex = "<td.*id=\"{0}\"([^>]*)>";

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
                result.ResponseText = DoTransformation(xslt);
                DoDKIParsing(result);
                input_.CurrentContext.Response.Write(result.ResponseText);
                DoHttpCacheSettings(input_);
                result.Status = PublishStatus.SUCCESS;
            }
            else
            {
                throw new NotImplementedException("Publish.if.else");
            }
            throw new NotImplementedException("Publish");
        }

        private void DoDKIParsing(PagePublisherResult result)
        {
            throw new NotImplementedException();
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
            string skin = PagePublisherUtility.GetAllFileContent(_input.ActiveTab.SkinSrc);

            skin = RemoveRunAtServerVisibleAndControlAttribute.Replace(skin, string.Empty);
            

            throw new NotImplementedException("PrepareXSLT");
        }
    }
}


// 1.Take the request.
// 2.Load Input 
// 3.Validate it.
// 4.Prepare XSLT To Transform
// 5.Generate html