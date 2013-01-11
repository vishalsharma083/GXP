﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.Interfaces;
using GXP.Core.Framework;

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
            return string.Empty;
        }
      
    }
}
