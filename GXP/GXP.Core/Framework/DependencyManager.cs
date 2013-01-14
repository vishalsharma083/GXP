using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.Interfaces;

namespace GXP.Core
{
    public class DependencyManager
    {
        public static ICachingService CachingService { get; set; }
        public static ILoggingService LoggingService { get; set; }
        public static IDBService DBService { get; set; }
    }
}
