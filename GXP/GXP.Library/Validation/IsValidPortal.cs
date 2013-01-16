using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.Interfaces;
using GXP.Core.Framework;
using GXP.Core.DNNEntities;
using GXP.Core;

namespace GXP.Library.Validation
{
    public class IsValidPortal : IPageRequestValidation
    {
        public bool IsValid(PagePublisherInput input_)
        {
            string hostName = input_.CurrentContext.Request.UserHostName;
            PortalAlias portalAlias = DependencyManager.DBService.GetAllPortalAlias().Where(x => x.HTTPAlias == hostName).FirstOrDefault<PortalAlias>();
            if (portalAlias != null)
            {
                input_.CanProcessRequest = true;
                input_.CurrentPortalAlias = portalAlias;
            }
            return input_.CanProcessRequest;
        }

        public decimal SortOrder
        {
            get
            {
                return 2;
            }
        }
    }
}
