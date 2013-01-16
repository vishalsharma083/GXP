using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.Interfaces;
using GXP.Core.GCMSEntities;
using System.IO;
using GXP.Core.Framework;
using GXP.Core.Utilities;
namespace GXP.Dep.ModuleParsers
{
    public class FileModuleParser : BaseModuleParser
    {

        public override bool CanParse()
        {
            return ModuleXml.Contains("CMSFileInfo");
        }

        public override string GenerateContent()
        {
            CMSFileInfo fileInfo = PagePublisherUtility.DeserializeObject<CMSFileInfo>(ModuleXml);
            if (fileInfo != null && File.Exists(fileInfo.FilePathWithName))
            {
                return File.ReadAllText(fileInfo.FilePathWithName);
            }
            else
            {
                return string.Empty;
            }
        }

        
    }
}
