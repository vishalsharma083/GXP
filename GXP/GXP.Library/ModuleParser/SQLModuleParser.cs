using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.Interfaces;
using System.Xml.Linq;
using GXP.Core.GCMSEntities;
using GXP.Core.Utility;
using System.IO;
using GXP.Core.Framework;
using System.Web;
using System.Data;
using System.Web.Configuration;
using Microsoft.ApplicationBlocks.Data;
using GXP.Core;
using System.Web.UI.WebControls;
using GXP.Library.UI;
namespace GXP.Dep.ModuleParsers
{
    public class SQLModuleParser : BaseModuleParser
    {
        private static ProcedureSetting _procSettings = null;
        static SQLModuleParser()
        {
            _procSettings = PagePublisherUtility.DeserializeObject<ProcedureSetting>(File.ReadAllText(@"/config/sqlmodule.config"));
        }
        public override bool CanParse()
        {
            return ModuleXml.Contains("SQLModuleInfo");
        }

        /// <summary>
        /// generate html based on module. return empty string in any negative scenario.
        /// </summary>
        /// <returns></returns>
        public override string GenerateContent()
        {
            SQLModuleInfo moduleInfo = PagePublisherUtility.DeserializeObject<SQLModuleInfo>(ModuleXml);

            if (moduleInfo == null)
            {
                return string.Empty;
            }

            DataTable dt = GetResultSet(moduleInfo);
            if (dt != null && dt.Rows.Count > 0)
            {
                CMSRepeater repeater = new CMSRepeater();
                repeater.CMSItemTemplate = moduleInfo.ItemTemplate.Data;
                repeater.CMSAlternateItemTemplate = moduleInfo.AlternateItemTemplate.Data;
                repeater.CMSHeaderTemplate = moduleInfo.HeaderTemplate.Data;
                repeater.CMSFooterTemplate = moduleInfo.FooterTemplate.Data;
                repeater.Init();
                repeater.Bind();
                repeater.Render();
                return repeater.GeneratedContent;
            }
            else
            {
                return string.Empty;
            }
        }

        private object[] GetinputParams(List<NameSqlDbTypePair> objProcInputParams_, List<NameSqlDbTypePair> objSQLModuleInputParams_, int maxRecords_)
        {
            object[] values = null;
            int valCount = 0;
            values = new object[objProcInputParams_.Count];
            foreach (NameSqlDbTypePair inputParams in objProcInputParams_)
            {
                switch (inputParams.Name)
                {
                    case "@MaxRecord":
                        values[valCount] = maxRecords_;
                        break;
                    case "@PortalID":
                        values[valCount] = base.PublisherInput.ActiveTab.PortalID.ToString();
                        break;
                    default:
                        foreach (NameSqlDbTypePair SQLModuleInputParam in objSQLModuleInputParams_)
                        {
                            if (SQLModuleInputParam.Name == inputParams.Name)
                                inputParams.KeyName = SQLModuleInputParam.KeyName;
                        }
                        if (inputParams.KeyName != null && inputParams.KeyName.ToLower().Trim() != "null")
                        {
                            values[valCount] = base.PublisherInput.CurrentContext.Request.QueryString[inputParams.KeyName] != null ? base.PublisherInput.CurrentContext.Request.QueryString[inputParams.KeyName].Trim().Replace("-", " ") : /*DKIController.DoDKI(inputParams.KeyName.ToString().Trim(), ref cmsPageContext)*/ string.Empty; // TODO;
                        }
                        else
                        {
                            values[valCount] = null;
                        }
                        break;
                }
                valCount++;
            }

            return values;
        }

        /// <summary>
        /// Gets the result for the particular module
        /// </summary>
        /// <param name="sqlModuleInfo_"></param>
        /// <returns></returns>
        public DataTable GetResultSet(SQLModuleInfo sqlModuleInfo_)
        {

            DataTable dt = new DataTable();
            {
                ProcedureInfo procInfo = _procSettings.ListProcedureInfo.Where(x => x.AliasName == sqlModuleInfo_.AliasName).FirstOrDefault<ProcedureInfo>();

                if (procInfo != null)
                {
                    string cacheKey = procInfo.AliasName;

                    foreach (var param in sqlModuleInfo_.ParameterValues)
                    {
                        cacheKey = cacheKey + "__" + param;
                    }

                    try
                    {
                        if (((ContentViewMode)sqlModuleInfo_.IsPublish) == ContentViewMode.Live)
                        {
                            object obj = DependencyManager.CachingService.Get(cacheKey);
                            if (obj != null)
                            {
                                return obj as DataTable;
                            }
                        }
                    }
                    catch { /* Do Nothing */ }
                    string str = WebConfigurationManager.ConnectionStrings[procInfo.ConnectionString].ConnectionString;
                    dt = SqlHelper.ExecuteDataset(str, procInfo.ProcedureName, sqlModuleInfo_.ParameterValues).Tables[0];

                    try
                    {
                        if (((ContentViewMode)sqlModuleInfo_.IsPublish) == ContentViewMode.Live)
                        {
                            DependencyManager.CachingService.Insert(cacheKey, dt, DateTime.Now.AddHours(12));
                        }
                    }
                    catch { /* Do Nothing */ }
                }
                else
                {
                    dt = null;
                }
                return dt;
            }
        }
    }
}
