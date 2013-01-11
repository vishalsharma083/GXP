using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.Framework;
namespace GXP.Core.Interfaces
{
    public interface IModuleParser
    {
        string ModuleXml { get; set; }
        bool CanParse();
        string GenerateContent();
        PagePublisherInput PublisherInput { get; set; }
    }
}
