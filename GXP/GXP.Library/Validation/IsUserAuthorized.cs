using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.Interfaces;
using GXP.Core.Framework;

namespace GXP.Library.Validation
{
    public class IsUserAuthorized : IPageRequestValidation
    {
        public bool IsValid(PagePublisherInput input_)
        {
            return true;
        }

        public decimal SortOrder
        {
            get
            {
                return 0.9M;
            }
        }
    }
}
