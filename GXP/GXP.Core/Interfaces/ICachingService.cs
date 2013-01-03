using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXP.Core.Interfaces
{
    public interface ICachingService
    {
        object Get(string cacheKey_);
        void Insert(string cacheKey_, object o_, DateTime duration_);
    }
}
