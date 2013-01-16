using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Xsl;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Configuration;
using System.Net;
using System.Text.RegularExpressions;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using System.Web;
using System.Xml.Linq;
using System.Collections;
using System.Diagnostics;
using GXP.Core.ExternalServices;
using GXP.Core.Utilities;

namespace GXP.Core.Framework
{
    public class CMSXsltUtility
    {
        private List<XSLTExceptionType> _exceptionsList = new List<XSLTExceptionType>();
        public List<XSLTExceptionType> ExceptionList
        {
            get { return _exceptionsList; }
            set { _exceptionsList = value; }
        }
        public PagePublisherInput PublishingDetail { get; set; }

        public string PerformTransformation(string xsltInput, string xmlToOperateOn, bool published_)
        {
            string output = string.Empty;
            StringBuilder builder = new StringBuilder();
            string inputText = "<xsl:transform version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:CMSXsltUtility=\"urn:CMSXsltUtility\"><xsl:output method=\"html\" omit-xml-declaration=\"yes\" /><xsl:template match=\"/\">";
            XslCompiledTransform xslt = new XslCompiledTransform();
            XsltArgumentList xslArgs = new XsltArgumentList();
            try
            {

                //pass an instance of the custom object
                xslArgs.AddExtensionObject("urn:CMSXsltUtility", this);
                inputText = inputText + xsltInput.Trim() + "</xsl:template></xsl:transform>";

                using (StringReader stringReader = new StringReader(inputText))
                {
                    using (XmlReader xmlReader = XmlReader.Create(stringReader))
                    {
                        xslt.Load(xmlReader);
                    }
                }

                XPathDocument xPathDoc = null;
                using (StringWriter stringWriter = new StringWriter(builder))
                {
                    using (XmlTextWriter writer = new XmlTextWriter(stringWriter))
                    {
                        using (StringReader stringReader = new StringReader(xmlToOperateOn))
                        {
                            xPathDoc = new XPathDocument(stringReader);
                            xslt.Transform(xPathDoc, xslArgs, writer);
                            output = builder.ToString().Replace(" xmlns:CMSXsltUtility=\"urn:CMSXsltUtility\"", string.Empty);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                if (published_ == false)
                {
                    output = xsltInput + "</br> <b>Error while xslt Transformation </b>: " + ex.ToString();
                }
            }
            return WebUtility.HtmlDecode(output);
        }

        public XPathNavigator GetContent(int tabid_, int moduleId_)
        {
            string filePath = string.Format(@"{0}\{1}\{2}\publish\default.gtxt", ConfigurationManager.AppSettings["DataStorePath"], tabid_, moduleId_);
            string content = ModuleParsingManager.GenerateContent(PagePublisherUtility.GetViewModeModuleContent(filePath), PublishingDetail);
            XmlDocument xdoc = new XmlDocument();
            if (string.IsNullOrEmpty(content) == false)
            {
                xdoc.LoadXml("<Content><![CDATA[" + content + "]]></Content>");
            }
            else
            {
                xdoc.LoadXml(GetEmptyContent());
            }

            return xdoc.CreateNavigator();
        }



        private string GetEmptyContent()
        {
            return "<root><noelement></noelement></root>";
        }

        public XPathNavigator GetDataFromDB(string datasourceName_, string params_)
        {
            object[] prm = null;
            prm = Regex.Split(params_, "##");
            return GetDataFromDB(datasourceName_, "DBSource", prm);
        }

        public XPathNavigator GetDataFromDB(string datasourceName_, string tableName_, params object[] params_)
        {
            XPathNavigator output = null;
            string keyString = datasourceName_;
            object[] modParams = null;
            string procedureName = string.Empty;
            string connectionString = string.Empty;
            string parameters = string.Empty;
            try
            {
                modParams = new object[params_.Length];
                DataTable dt = null;
                foreach (string prm in params_)
                {
                    keyString = keyString + "__" + Convert.ToString(prm);
                }

                int count = 0;
                for (count = 0; count <= params_.Length - 1; count++)
                {
                    if ((params_[count].ToString() == "null"))
                    {
                        modParams[count] = DBNull.Value;
                    }
                    else if ((params_[count].ToString() == "NOPARAM"))
                    {
                        continue;
                    }
                    else
                    {
                        modParams[count] = params_[count];
                    }
                }

                dt = (DataTable)GetObjectDataFromCache(keyString);
                if (dt == null || dt.Rows.Count <= 0)
                {
                    string filePath = /*Globals.ApplicationMapPath +*/ "\\Portals\\_default\\config\\XSLTProcedures.config"; // TODO: 
                    IEnumerable query = null;

                    XDocument doc = HttpContext.Current.Cache.Get(filePath) as XDocument;
                    if (doc == null)
                    {
                        doc = XDocument.Load(filePath);
                        HttpRuntime.Cache.Insert(filePath, doc, new System.Web.Caching.CacheDependency(filePath));
                    }
                    query = from p in doc.Elements("ProcedureSetting").Elements("ListProcedureInfo").Elements("ProcedureInfo")
                            where (string)p.Element("AliasName").Value == datasourceName_
                            select p;
                    foreach (XElement row in query)
                    {
                        procedureName = row.Element("ProcedureName").Value;
                        connectionString = row.Element("ConnectionString").Value;
                    }

                    if (!string.IsNullOrEmpty(procedureName) & !string.IsNullOrEmpty(connectionString))
                    {
                        connectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings[connectionString].ConnectionString;
                        dt = SqlHelper.ExecuteDataset(connectionString, procedureName, modParams).Tables[0];
                        dt.TableName = tableName_;
                        output = ConvertDataTableToXPathNavigator(dt);
                        SetObjectDataInCache(keyString, dt);
                    }
                }
                else
                {
                    output = ConvertDataTableToXPathNavigator(dt);
                }
            }
            catch (Exception ex)
            {
                DependencyManager.LoggingService.WriteLog(string.Format("ProcedureName :{0}, InputParams :{1},IPAddress :{2},,CurrentURL :{3}", procedureName, keyString, Utility.GetClientIP(), HttpContext.Current.Request.RawUrl) + "-" + ex.ToString());
                //if (HttpContext.Current.Request != null && HttpContext.Current.Request.QueryString("preview") != null) {
                //    output = ConvertErrorMessageToXPathNavigator(ex.Message);
                //}
            }
            return output;
        }

        ///<summary>
        ///  This method will make a call to dms and based on dms response it will decide below two things : 
        ///   1. whether to cache the dms response result or not.  2. what should be the page cache duration at nginx server.
        ///</summary>
        public XPathNavigator GetDatafromDMS(string dealId_, string @params)
        {
            DealServiceClient client = null;
            string dmsOutput = null;
            Stopwatch dmsResponseWatch = null;
            string dmsResultForDebug = null;
            string cachekey = string.Empty;
            bool cacheDMSResult = false;
            try
            {
                dmsResponseWatch = new Stopwatch();
                dmsResponseWatch.Start();
                cachekey = dealId_ + "__" + @params;
                dmsOutput = GetObjectDataFromCache(cachekey) as string;
                if (string.IsNullOrEmpty(dmsOutput))
                {
                    client = new DealServiceClient();
                    dmsOutput = client.GetDealResultWithParam(dealId_, @params);
                    dmsResponseWatch.Stop();
                    string[] dmsEscapeKeywords = ConfigurationManager.AppSettings["DMSContentStatus"].Split(new char[] { ',' });
                    foreach (string keyword in dmsEscapeKeywords)
                    {
                        if (dmsOutput.ToLower().Contains(keyword))
                        {
                            ExceptionList.Add(XSLTExceptionType.DMS_Processing_Exception);
                            //#2
                            //Logger.WriteLog("DMSResponseMonitor", dealId_ + ", " + @params + ", Total Time Taken (ms) : " + dmsResponseWatch.ElapsedMilliseconds, HttpContext.Current, this.ToString(), "GetDatafromDMS-DMSProcessingMessage : " + keyword);
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }
                    if (dmsOutput.Length > 200)
                    {
                        dmsResultForDebug = dmsOutput.Substring(0, 199);
                    }
                    else
                    {
                        dmsResultForDebug = dmsOutput;
                    }

                    DependencyManager.LoggingService.WriteLog("DMSResponseMonitor", dealId_ + ", " + @params + ", Total Time Taken (ms) : " + dmsResponseWatch.ElapsedMilliseconds, HttpContext.Current, this.ToString(), "GetDatafromDMS-DMSResult : " + dmsResultForDebug);

                    if (ExceptionList.Count <= 0)
                    {
                        cacheDMSResult = true;
                    }

                }
                return GetXPathNavigator("DMSDeal", dmsOutput);
            }
            catch (System.TimeoutException timeoutEx)
            {
                ExceptionList.Add(XSLTExceptionType.DMS_TimeOut_Exception);
                //#2
                DependencyManager.LoggingService.WriteLog("DMSResponseMonitor", dealId_ + ", " + @params + ", Total Time Taken (ms) : " + dmsResponseWatch.ElapsedMilliseconds + ", " + timeoutEx.Message, HttpContext.Current, this.ToString(), "GetDatafromDMS-System.TimeoutException");
                //if (HttpContext.Current.Request != null && HttpContext.Current.Request.QueryString("preview") != null)
                //{
                return ConvertErrorMessageToXPathNavigator(timeoutEx.Message);
                //}

            }
            catch (System.Net.WebException netEx)
            {
                ExceptionList.Add(XSLTExceptionType.DMS_Service_Unavailable_Exception);
                //#2
                DependencyManager.LoggingService.WriteLog("DMSResponseMonitor", dealId_ + ", " + @params + ", Total Time Taken (ms) : " + dmsResponseWatch.ElapsedMilliseconds + ", " + netEx.Message, HttpContext.Current, this.ToString(), "GetDatafromDMS-System.Net.WebException");
                //if (HttpContext.Current.Request != null && HttpContext.Current.Request.QueryString("preview") != null)
                //{
                return ConvertErrorMessageToXPathNavigator(netEx.Message);
                //}

            }
            catch (Exception ex)
            {
                StringBuilder exBuilder = new StringBuilder();
                exBuilder.Append(ex.ToString());

                if ((ex.InnerException != null))
                {
                    exBuilder.Append(", InnerException : " + ex.InnerException.ToString());
                }
                ExceptionList.Add(XSLTExceptionType.DMS_Processing_Exception);
                //#2

                DependencyManager.LoggingService.WriteLog("DMSResponseMonitor", dealId_ + ", " + @params + ", Total Time Taken (ms) : " + dmsResponseWatch.ElapsedMilliseconds + ", " + exBuilder.ToString(), HttpContext.Current, this.ToString(), "GetDatafromDMS-System.Exception");
                //if (HttpContext.Current.Request != null && HttpContext.Current.Request.QueryString("preview") != null)
                //{
                return ConvertErrorMessageToXPathNavigator(ex.Message);
                //}
            }
            finally
            {
                if (dmsResponseWatch.IsRunning)
                {
                    dmsResponseWatch.Stop();
                }
                if ((client != null))
                {
                    client.Close();
                }
                if (cacheDMSResult)
                {
                    SetObjectDataInCache(cachekey, dmsOutput, 1);
                }
            }
        }

        private XPathNavigator GetXPathNavigator(string rootElement_, string content_)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<" + rootElement_ + ">" + content_ + "</" + rootElement_ + ">");
            return xmlDoc.CreateNavigator();
        }

        private XPathNavigator ConvertErrorMessageToXPathNavigator(string errorMessage)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<ErrorMessage>" + errorMessage + "</ErrorMessage>");
            return xmlDoc.FirstChild.CreateNavigator();
        }

        private object GetObjectDataFromCache(string key_)
        {
            //object obj = null;
            //if (object.Equals(HttpContext.Current.Request.QueryString("preview"), null) | HttpContext.Current.Request.QueryString("preview") != "1")
            //{
            throw new NotImplementedException();
            //}
            //return obj;
        }

        //' set xpathnavigator output with key.
        private bool SetObjectDataInCache(string key_, object value_)
        {
            return SetObjectDataInCache(key_, value_, 12);
        }

        //' set xpathnavigator output with key.
        private bool SetObjectDataInCache(string key_, object value_, Int32 duration)
        {
            throw new NotImplementedException();
        }

        private XPathNavigator GetDataFromCache(string key_)
        {
            throw new NotImplementedException();
        }

        //' set xpathnavigator output with key.
        private bool SetDataInCache(string key_, XPathNavigator value_)
        {
            throw new NotImplementedException();
        }

        private XPathNavigator ConvertDataTableToXPathNavigator(DataTable dt)
        {
            XPathNavigator xpath = null;
            XmlDocument xmlDoc = new XmlDocument();

            using (StringWriter sw = new StringWriter())
            {
                dt.WriteXml(sw);
                xmlDoc.LoadXml(sw.ToString());
            }

            xpath = xmlDoc.FirstChild.CreateNavigator();
            return xpath;
        }
    }


}
