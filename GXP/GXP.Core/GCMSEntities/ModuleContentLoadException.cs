using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXP.Core.GCMSEntities
{
    public class ModuleContentLoadException : Exception
    {
        public ModuleContentLoadException(String reason, Exception inner)
            : base(reason, inner)
        {
        }
    }
}
