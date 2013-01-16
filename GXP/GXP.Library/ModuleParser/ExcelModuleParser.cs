using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.Interfaces;
using GXP.Core.Framework;
using System.Data;
using GXP.Core.GCMSEntities;
using System.IO;
using GXP.Core.Utilities;
using GXP.Library.UI;
using System.Web;

namespace GXP.Dep.ModuleParsers
{
    public class ExcelModuleParser : BaseModuleParser
    {

        public override bool CanParse()
        {
            return ModuleXml.Contains("CMSExcelInfo");
        }

        public override string GenerateContent()
        {
            ExcelPublisherInfo moduleInfo = PagePublisherUtility.DeserializeObject<ExcelPublisherInfo>(ModuleXml);

            DataTable dt = ReadExcel(moduleInfo);
            if (dt != null && dt.Rows.Count > 0)
            {
                CMSRepeater repeater = new CMSRepeater();
                repeater.SourceData = dt;
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

        private DataTable ReadExcel(ExcelPublisherInfo excelPublisherInfo_)
        {
            DataTable result = null;
            try
            {
                // TODO : Implement Caching.
                string existingFile = this.PublisherInput.ApplicationBasePath + excelPublisherInfo_.FileName;
                if (!string.IsNullOrEmpty(excelPublisherInfo_.FileName) && File.Exists(existingFile))
                {
                    result = Utility.ReadExcelFile(existingFile, "Data", excelPublisherInfo_.MaxRecords, excelPublisherInfo_.PortalId);
                }
            }
            catch (IOException ex)
            {
                throw new ModuleContentLoadException("Excel Publisher Module , System.IOException Occured-- " + excelPublisherInfo_.ModuleId + ex.Message, ex);
            }
            return result;
        }
    }
}
