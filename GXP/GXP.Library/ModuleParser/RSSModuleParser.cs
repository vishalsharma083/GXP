using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.Interfaces;
using GXP.Core.Framework;
using System.Web;
using System.Data;
using System.Xml.Linq;
using System.IO;
using GXP.Core.GCMSEntities;
using System.Web.Caching;
using GXP.Core;
using GXP.Core.Utilities;

namespace GXP.Dep.ModuleParsers
{
    public class RSSModuleParser : BaseModuleParser
    {

        public override bool CanParse()
        {
            return ModuleXml.Contains("RSSInfo");

        }

        public override string GenerateContent()
        {
            return string.Empty;
        }

        public DataTable GetFilteredRSSNodeList(RSSInfo rssInfo_, HttpRequest request_)
        {
            XDocument xdoc = null;
            IEnumerable<XElement> nodeList = null;
            DataTable table = null;
            try
            {

                // first do the dki replacement.
                //rssInfo_.FeedURL = ParseFeedURLWithContentIdentifier(rssInfo_.FeedURL, request_);
                //rssInfo_.SecondaryURL = ParseFeedURLWithContentIdentifier(rssInfo_.SecondaryURL, request_);

                string feedURL_ = ParseFeedURLWithContentIdentifier(rssInfo_.FeedURL, request_);
                string secondaryURL_ = ParseFeedURLWithContentIdentifier(rssInfo_.SecondaryURL, request_);

                // load response in reader
                xdoc = GetRSSXMLReader(rssInfo_, request_, true);
                if (xdoc == null)
                {
                    return null;
                }

                if (xdoc != null && xdoc.Root.Name.ToString() == "rss")
                {
                    string filterRSS = string.Empty;
                    string selectUrl = rssInfo_.IsPrimaryHeader == false ? secondaryURL_ : feedURL_;
                    System.Uri Url = new Uri(selectUrl);

                    if (!string.IsNullOrEmpty(Url.Query) && HttpUtility.ParseQueryString(Url.Query)["filterfeedfor"] != null)
                    {
                        filterRSS = HttpUtility.ParseQueryString(Url.Query).Get("filterfeedfor").ToLower();
                    }


                    nodeList = GetFilteredNodeListFromXDoc(rssInfo_, xdoc, filterRSS);

                    if (nodeList != null && nodeList.Count() > 0)
                    {
                        table = GetTableFromNodeList(rssInfo_, nodeList);
                    }
                }
                // if response is not found after filteration from first source
                if (table == null || (table != null && table.Rows.Count == 0 && !rssInfo_.IsPrimaryHeader))
                {
                    xdoc = GetRSSXMLReader(rssInfo_, request_, false);
                    if (xdoc == null)
                    {
                        return null;
                    }
                    if (xdoc != null && xdoc.Root.Name.ToString() == "rss")
                    {
                        nodeList = GetFilteredNodeListFromXDoc(rssInfo_, xdoc, string.Empty);
                        if (nodeList != null && nodeList.Count() > 0)
                        {
                            table = GetTableFromNodeList(rssInfo_, nodeList);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                throw new ModuleContentLoadException("System.IOException Occured-- " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                DependencyManager.LoggingService.WriteLog(ex.ToString(), (Int32)rssInfo_.PortalId, Convert.ToInt32(rssInfo_.TabId), Convert.ToInt32(rssInfo_.ModuleId));
            }
            return table;
        }

        /// <summary>
        /// return xml reader after making request to primary ( if specified ), if not found from primary then it will load response from secondary.
        /// </summary>
        /// <param name="rssInfo_"></param>
        /// <param name="request_"></param>
        /// <param name="isLoadPrimarySource_"></param>
        /// <returns></returns>
        private XDocument GetRSSXMLReader(RSSInfo rssInfo_, HttpRequest request_, bool isLoadPrimarySource_)
        {
            string basePath = string.Empty;
            XDocument xdoc = null;
            string rssurl = string.Empty;

            // first try to load primary feed url response.
            if (rssInfo_ == null || string.IsNullOrEmpty(rssInfo_.FeedURL))
            {
                return null;
            }

            System.Uri uri = new Uri(ParseFeedURLWithContentIdentifier(rssInfo_.FeedURL, request_));
            rssurl = uri.OriginalString;
            if (!string.IsNullOrEmpty(uri.Query))
            {
                // filterfeedfor querystring was being used when we were using yahoo pipes but now we are filtering after fetching the response.
                // we are removing "filterfeedfor" querystring only, because url might be having some another required querystring.
                if (uri.Query.Contains("filterfeedfor="))
                {
                    rssurl = uri.OriginalString.Replace("filterfeedfor=" + HttpUtility.ParseQueryString(uri.Query).Get("filterfeedfor").ToLower(), string.Empty);
                }
            }

            if (isLoadPrimarySource_)
            {
                xdoc = LoadXmlReader(rssurl.ToLower(), rssInfo_.PortalId);
            }

            // either response not loaded due to any error in connection or wrong feed url then try to load for secondary feed if url provided.
            if ((xdoc == null || (xdoc != null && xdoc.Root.Name.ToString().ToLower() != "rss")) && !string.IsNullOrEmpty(rssInfo_.SecondaryURL))
            {
                uri = new Uri(rssInfo_.SecondaryURL.ToLower());

                rssurl = uri.OriginalString;

                if (!string.IsNullOrEmpty(uri.Query))
                {
                    rssurl = uri.OriginalString.Replace(uri.Query, string.Empty);
                }

                // not found make a http request for secondary url
                xdoc = LoadXmlReader(rssurl, rssInfo_.PortalId);

                if (xdoc != null && xdoc.Root.Name.ToString().ToLower() == "rss")
                {
                    // set to use secondary header in case of response found
                    //rssInfo_.IsPrimaryHeader = false;
                    rssInfo_.IsPrimaryHeader = false;
                }
            }

            return xdoc;
        }

        /// <summary>
        /// returns reader either by making httprequest or loading data from cache.
        /// </summary>
        /// <param name="url_"></param>
        /// <param name="portalId_"></param>
        /// <returns></returns>
        private XDocument LoadXmlReader(string url_, int portalId_)
        {
            XDocument xdoc = null;
            try
            {
                if (string.IsNullOrEmpty(url_))
                {
                    return null;
                }
                xdoc = GetCachedResponseAsXmlReader(url_, portalId_);
                if (xdoc != null && xdoc.Root.Name.ToString().ToLower() == "rss")
                {
                    return xdoc;
                }
                else
                {
                    using (Stream stream = CMSHttpUtility.GetXMLWebResponseAsStream(url_))
                    {
                        xdoc = XDocument.Load(stream);
                        if (xdoc != null && xdoc.Root.Name.ToString() == "rss")
                        {
                            DoResponseCaching(url_, portalId_, new StringBuilder(xdoc.Root.ToString()));
                        }
                    }
                    return GetCachedResponseAsXmlReader(url_, portalId_);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// try to load response from disk
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="portalId_"></param>
        /// <returns></returns>
        private XDocument GetCachedResponseAsXmlReader(string basePath, int portalId_)
        {
            try
            {
                if (HttpRuntime.Cache[basePath] != null)
                {
                    using (StringReader reader = new StringReader(Convert.ToString(HttpRuntime.Cache[basePath])))
                    {
                        return XDocument.Load(reader); // TODO:
                    }
                }
            }
            catch (IOException ex)
            {
                throw new ModuleContentLoadException("System.IOException Occured-- " + ex.Message, ex);
            }

            return null;
        }

        private void DoResponseCaching(string feedurl_, int portalId_, StringBuilder rssResponse_)
        {
            try
            {
                HttpRuntime.Cache.Add(feedurl_, rssResponse_, null, DateTime.Now.AddSeconds(GXP.Core.GXPSetting.Default.RSSCacheDuration), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }
            catch (System.Exception ex)
            {
                DependencyManager.LoggingService.WriteLog(ex.ToString());
            }
        }

        /// <summary>
        /// filter response
        /// </summary>
        /// <param name="rssInfo_"></param>
        /// <param name="xdoc"></param>
        /// <param name="filterRSS"></param>
        /// <returns></returns>
        private IEnumerable<XElement> GetFilteredNodeListFromXDoc(RSSInfo rssInfo_, XDocument xdoc, string filterRSS)
        {
            IEnumerable<XElement> nodeList = null;
            if (!string.IsNullOrEmpty(filterRSS))
            {
                nodeList = from item in xdoc.Descendants(@"item")
                           where item.Value.ToLower().Contains(filterRSS)
                           select item;

                nodeList = nodeList.Take(rssInfo_.MaxRecords);
            }
            else
            {
                nodeList = from item in xdoc.Descendants(@"item")
                           select item;
                nodeList = nodeList.Take(rssInfo_.MaxRecords);
            }
            return nodeList;

        }

        private DataTable GetTableFromNodeList(RSSInfo rssInfo_, IEnumerable<XElement> nodeList)
        {
            DataTable table = GetRSSTableSchema();
            DataRow dr = null;
            string temp = string.Empty;
            DateTime pubDate = DateTime.Now;
            XElement tempElement = null;
            foreach (XElement element in nodeList)
            {
                dr = table.NewRow();
                temp = element.Element(@"title").Value;
                dr["Title"] = (temp.Length > rssInfo_.HeaderLength ? temp.Substring(0, rssInfo_.HeaderLength) + "..." : temp);
                dr["Link"] = element.Element(@"link").Value;
                temp = element.Element(@"description").Value;
                dr["Description"] = (temp.Length > rssInfo_.DescriptionLength ? temp.Substring(0, rssInfo_.DescriptionLength) + "..." : temp); ;
                dr["PubDate"] = Convert.ToDateTime(element.Element(@"pubDate").Value).ToString("ddd, dd MMM yyyy");
                tempElement = element.Element(@"thumbnail");
                if (tempElement != null)
                {
                    dr["ThumbNail"] = tempElement.Value;
                }

                table.Rows.Add(dr);
            }
            return table;
        }

        private DataTable GetRSSTableSchema()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Title", Type.GetType("System.String"));
            table.Columns.Add("Link", Type.GetType("System.String"));
            table.Columns.Add("Description", Type.GetType("System.String"));
            table.Columns.Add("PubDate", Type.GetType("System.String"));
            table.Columns.Add("ThumbNail", Type.GetType("System.String"));
            return table;
        }

        private string ParseFeedURLWithContentIdentifier(string url_, HttpRequest request_)
        {
            foreach (string key in request_.QueryString.Keys)
            {
                url_ = Microsoft.VisualBasic.Strings.Replace(url_, "[#" + key + "#]", request_.QueryString[key].Replace("-", " "), 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
            }
            return url_;
        }
    }
}
