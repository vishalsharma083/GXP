using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
namespace GXP.Core.Utilities
{
    public class CMSHttpUtility
    {

        public static Stream GetXMLWebResponseAsStream(string url_)
        {
            Stream streamResponse = null;
            WebRequest httpWebRequest = null;
            WebResponse httpWebResponse = null;
            try
            {
                httpWebRequest = WebRequest.Create(url_);
                httpWebResponse = (WebResponse)httpWebRequest.GetResponse();
                streamResponse = httpWebResponse.GetResponseStream();
            }
            catch (WebException webEx)
            {
                DependencyManager.LoggingService.WriteLog(url_ + " - " + webEx.ToString());
            }
            catch (Exception ex)
            {
                DependencyManager.LoggingService.WriteLog(url_ + " - " + ex.ToString());
            }
            return streamResponse;
        }

        public static string GetXMLWebResponseAsString(string url_)
        {
            Stream streamResponse = null;
            StreamReader streamReader = null;
            try
            {
                streamResponse = GetXMLWebResponseAsStream(url_);
                if (streamResponse != null)
                {
                    streamReader = new StreamReader(streamResponse);
                    return streamReader.ReadToEnd();
                }
            }
            catch (WebException webEx)
            {
                DependencyManager.LoggingService.WriteLog(url_ + " - " + webEx.ToString());
            }
            catch (Exception ex)
            {
                DependencyManager.LoggingService.WriteLog(url_ + " - " + ex.ToString());
            }
            finally
            {
                if ((streamResponse != null))
                {
                    streamResponse.Dispose();
                }

                if ((streamReader != null))
                {
                    streamReader.Dispose();
                }
            }
            return string.Empty;
        }

        public static DataSet GetXMLWebResponseInDataTable(string url_)
        {
            // make a call to GetXMLWebResponse 
            // read stream in dataset and return
            DataSet dataSet = null;
            Stream streamResponse = null;
            try
            {
                dataSet = new DataSet();
                streamResponse = GetXMLWebResponseAsStream(url_);
                dataSet.ReadXml(streamResponse, XmlReadMode.InferSchema);
            }
            catch (Exception ex)
            {
                DependencyManager.LoggingService.WriteLog("RSS URL : " + url_ + "\\n" + ex.ToString());
            }
            finally
            {
                if (streamResponse != null)
                {
                    streamResponse.Dispose();
                    streamResponse.Close();
                    streamResponse.Flush();
                }
            }
            return dataSet;
        }

    }

}