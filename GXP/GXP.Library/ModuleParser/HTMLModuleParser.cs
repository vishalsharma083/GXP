using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.Interfaces;
using GXP.Core.GCMSEntities;
using GXP.Core.Framework;
using GXP.Core.Utilities;

namespace GXP.Dep.ModuleParsers
{
    public class HTMLModuleParser : BaseModuleParser
    {
        public override bool CanParse()
        {
            return ModuleXml.Contains("CMSTextInfo");
        }

        public override string GenerateContent()
        {
            return PagePublisherUtility.DeserializeObject<CMSTextInfo>(ModuleXml).Content.Text;
        }

    }
}
