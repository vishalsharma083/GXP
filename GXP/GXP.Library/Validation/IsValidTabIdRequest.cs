using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.Interfaces;
using GXP.Core.Framework;
using GXP.Core;
using GXP.Core.DNNEntities;

namespace GXP.Library.Validation
{
    public class IsValidTabIdRequest : IPageRequestValidation
    {
        public bool IsValid(PagePublisherInput input_)
        {
            int tabid = -1;
            int.TryParse(input_.CurrentContext.Request.QueryString["tabid"], out tabid);
            if (tabid > -1)
            {
                input_.ActiveTab = DependencyManager.DBService.GetAllTabsByPortalId(input_.CurrentPortalAlias.PortalID).Where(x => x.TabID == tabid).FirstOrDefault<Tabs>();
                if (input_.ActiveTab == null)
                {
                    input_.CanProcessRequest = false;
                }
            }
            return input_.CanProcessRequest;
        }

        public decimal SortOrder
        {
            get
            {
                return 3;
            }
        }
    }
}
