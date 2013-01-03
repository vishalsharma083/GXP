using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXP.Core.Interfaces
{
    public interface IModuleParser
    {
        bool CanParse(string moduleXml_);
        string GenerateContent(string moduleXml_);
    }
}
