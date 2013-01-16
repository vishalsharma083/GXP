using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GXP.Core.DNNEntities;
using System.Configuration;
using System.Net;
using Fareportal.GlobalCMS.DataStore;
using System.Web;
using GXP.Core.Utilities;

namespace GXP.Core.Framework
{
    public class PagePublisher
    {
        private static Regex RemoveRunAtServerVisibleAndControlAttribute = new Regex("(?<runat>(runat=\"server\"))|(?<visible>(visible=\"false\"))|(?<controltag><%.*%>)",RegexOptions.Compiled);
        private const string FindPaneRegex = "<td.*id=\"{0}\"([^>]*)>";
        private PagePublisherInput _input = null;
        private static Store _store = new Store();

        public PageRequestValidationResult IsValidRequest()
        {
            return RequestValidator.IsValidRequest(_input);
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
            return result;
        }

        private void DoDKIParsing(PagePublisherResult result)
        {
            // Parse Request DKI first.
            result.ResponseText = DKIController.DoDKI(result.ResponseText, _input.CurrentContext, _input);

            // Parse BaseData DKI...
            _store.Data = DependencyManager.DBService.LoadBaseData();
            result.ResponseText = _store.EvaluateAndParseExpressions(result.ResponseText);
        }

        private void DoHttpCacheSettings(PagePublisherInput input_)
        {
            return; // TODO:
        }

        private string DoTransformation(string xslt_)
        {
            CMSXsltUtility cmsXsltUtility = new CMSXsltUtility();
            cmsXsltUtility.PublishingDetail = this._input;
            return cmsXsltUtility.PerformTransformation(xslt_, "<root/>", true);
        }


        private string PrepareXSLT()
        {
            string skin = PagePublisherUtility.GetAllFileContent(_input.ApplicationBasePath + "\\portals\\" + _input.ActiveTab.PortalID + _input.ActiveTab.SkinSrc);

            skin = RemoveRunAtServerVisibleAndControlAttribute.Replace(skin, string.Empty);

            List<TabModules> tabModules = DependencyManager.DBService.GetAllTabModules(_input.ActiveTab.TabID);

            var groupData = tabModules.GroupBy(x => x.PaneName).ToDictionary(x => x.Key, y => y);

            foreach (var item in groupData)
            {
                foreach (var item2 in item.Value)
                {
                    Console.Out.WriteLine(item2.TabModuleID + " : " + item2.PaneName);
                }
            }

            string lineWithPaneId = string.Empty;
            StringBuilder temp = new StringBuilder();
            Regex regex = null;
            Match match = null;
            foreach (var item in groupData)
            {
                regex = new Regex(string.Format(FindPaneRegex, item.Key));
                match = regex.Match(skin);
                if (match.Success)
                {
                    lineWithPaneId = match.Value.ToString();
                    temp.Append(lineWithPaneId);
                    foreach (var module in item.Value)
                    {
                        temp.Append("<xsl:value-of select=\"CMSXsltUtility:GetContent('" + module.TabID + "','" + module.ModuleID + "')/Content\"></xsl:value-of>");
                    }
                }
                skin = skin.Replace(lineWithPaneId, temp.ToString());
                temp.Length = 0;
            }

            // http://msdn.microsoft.com/en-us/library/ee388354(v=vs.100).aspx
            return WebUtility.HtmlDecode(skin);
        }
    }
}


// 1.Take the request.
// 2.Load Input 
// 3.Validate it.
// 4.Prepare XSLT To Transform
// 5.Generate html