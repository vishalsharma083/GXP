using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXP.Core.Interfaces
{
    public interface IModuleParser
    {
        string ModuleXml { get; set; }
        bool CanParse();
        string GenerateContent();
    }
}
