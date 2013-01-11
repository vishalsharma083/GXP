using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.Interfaces;

namespace GXP.Dep
{
    public class ASPNetCachingService : ICachingService
    {
        public object Get(string cacheKey_)
        {
            //throw new NotImplementedException();
            return null;
        }

        public void Insert(string cacheKey_, object o_, DateTime duration_)
        {
            //throw new NotImplementedException();
        }
    }
}
