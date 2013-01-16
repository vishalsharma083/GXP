using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Data.OleDb;
using System.Web.Configuration;
using System.Web;

namespace GXP.Core.Utilities
{
    public class Utility
    {
        //' Function to read any excel file and return datatable
        public static DataTable ReadExcelFile(string excelFile_, string sheetName_, int maxRecords_, int portalId_)
        {
            DataTable dataTable = null;
            int index = 0;
            if (File.Exists(excelFile_))
            {
                string excelFilePath = WebConfigurationManager.AppSettings["XLSXConnectionString"].Replace("[#FilePath#]", excelFile_);
                using (OleDbConnection oledbConn = new OleDbConnection(excelFilePath))
                {
                    oledbConn.Open();
                    dataTable = new DataTable();
                    if (string.IsNullOrEmpty(sheetName_) == false)
                    {
                        for (index = 1; index <= GXP.Core.GXPSetting.Default.MaxAttemptToReadWriteFiles; index++)
                        {
                            try
                            {
                                using (OleDbCommand oledbCommand = new OleDbCommand("SELECT top " + maxRecords_ + " * FROM [" + sheetName_ + "$]", oledbConn))
                                {
                                    using (OleDbDataAdapter oledbAdapter = new OleDbDataAdapter())
                                    {
                                        oledbAdapter.SelectCommand = oledbCommand;
                                        oledbAdapter.Fill(dataTable);
                                    }
                                }
                                break; // TODO: might not be correct. Was : Exit For
                            }
                            catch (System.Data.OleDb.OleDbException ex)
                            {
                                System.Threading.Thread.Sleep(500);
                            }
                        }
                    }
                }
            }

            return dataTable;
        }
        public static string GetClientIP()
        {
            if (HttpContext.Current == null)
            {
                return "255.255.255.255";
            }

            if (HttpContext.Current.Request.ServerVariables["HTTP_TRUE_CLIENT_IP"] != null)
            {
                return HttpContext.Current.Request.ServerVariables["HTTP_TRUE_CLIENT_IP"];
            }

            if (HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"] != null)
            {
                return HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"];
            }

            return HttpContext.Current.Request.UserHostAddress;
        }
    }
}
