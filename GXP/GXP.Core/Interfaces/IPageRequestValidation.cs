using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.Framework;

namespace GXP.Core.Interfaces
{
    public interface IPageRequestValidation
    {
        bool IsValid(PagePublisherInput input_);
        decimal SortOrder { get; }
    }
}
