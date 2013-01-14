using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.DNNEntities;
using System.Data;

namespace GXP.Core.Interfaces
{
    public interface IDBService
    {
        List<Tabs> GetAllTabsByPortalId(int portalId_);
        List<TabModules> GetAllTabModules(int tabId_);
        List<Portals> GetAllPortals();
        List<PortalAlias> GetAllPortalAlias();
        List<PortalSettings> GetAllPortalSettings();
        DataSet LoadBaseData();
    }
}
