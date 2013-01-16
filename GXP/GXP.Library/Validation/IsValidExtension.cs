using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.Interfaces;
using GXP.Core.Framework;

namespace GXP.Dep.Validations
{
    public class IsValidExtension : IPageRequestValidation
    {

        public bool IsValid(PagePublisherInput input_)
        {
            return true;
        }

        public decimal SortOrder
        {
            get
            {
                return 1;
            }
        }
    }
}
