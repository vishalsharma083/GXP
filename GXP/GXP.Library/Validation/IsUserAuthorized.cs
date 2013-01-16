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
            throw new NotImplementedException();
        }

        public decimal SortOrder
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
