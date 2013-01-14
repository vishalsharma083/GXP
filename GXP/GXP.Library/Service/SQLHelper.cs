using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.Interfaces;
using System.Data.Linq;
using GXP.Core.DNNEntities;
using GXP.Core;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using System.Configuration;
namespace GXP.Dep
{
    public class SQLHelper : IDBService
    {
        private const string TABS_KEY = "TABS_BY_PORTALID_{0}";
        private const string TAB_MODULES_KEY = "ALL_TAB_MODULES_{0}";
        private const string PORTALS_KEY = "PORTALS";
        private const string PORTAL_ALIAS_KEY = "PORTAL_ALIAS";
        private const string PORTAL_SETTINGS_KEY = "PORTAL_SETTINGS";
        private const string BASE_DATA = "BaseData";

        public List<Tabs> GetAllTabsByPortalId(int portalId_)
        {
            List<Tabs> tabs = DependencyManager.CachingService.Get(string.Format(TABS_KEY, portalId_)) as List<Tabs>;
            if (tabs ==null)
            {
                using (DNNEntities context = new DNNEntities())
                {
                    tabs= context.Tabs.Where(x => x.PortalID == portalId_).ToList<Tabs>();
                    DependencyManager.CachingService.Insert(string.Format(TABS_KEY, portalId_), tabs, DateTime.Now.AddHours(24));
                }    
            }
            return tabs;
        }

        public List<TabModules> GetAllTabModules(int tabId_)
        {
            List<TabModules> tabModules = DependencyManager.CachingService.Get(string.Format(TAB_MODULES_KEY, tabId_)) as List<TabModules>;
            if (tabModules == null)
            {
                using (DNNEntities context = new DNNEntities())
                {
                    tabModules = context.TabModules.Where(x => x.TabID == tabId_).ToList<TabModules>();
                    DependencyManager.CachingService.Insert(string.Format(TABS_KEY, tabId_), tabModules, DateTime.Now.AddHours(24));
                }
            }
            return tabModules;
        }

        public List<Portals> GetAllPortals()
        {
            List<Portals> portals = DependencyManager.CachingService.Get(PORTALS_KEY) as List<Portals>;
            if (portals == null)
            {
                using (DNNEntities context = new DNNEntities())
                {
                    portals = context.Portals.ToList<Portals>();
                    DependencyManager.CachingService.Insert(PORTALS_KEY, portals, DateTime.Now.AddHours(24));
                }
            }
            return portals;
        }

        public List<PortalAlias> GetAllPortalAlias()
        {
            List<PortalAlias> portalAlias = DependencyManager.CachingService.Get(PORTAL_ALIAS_KEY) as List<PortalAlias>;
            if (portalAlias == null)
            {
                using (DNNEntities context = new DNNEntities())
                {
                    portalAlias = context.PortalAlias.ToList<PortalAlias>();
                    DependencyManager.CachingService.Insert(PORTAL_ALIAS_KEY, portalAlias, DateTime.Now.AddHours(24));
                }
            }
            return portalAlias;
        }

        public List<PortalSettings> GetAllPortalSettings()
        {
            List<PortalSettings> portalSettings = DependencyManager.CachingService.Get(PORTAL_SETTINGS_KEY) as List<PortalSettings>;
            if (portalSettings == null)
            {
                using (DNNEntities context = new DNNEntities())
                {
                    portalSettings = context.PortalSettings.ToList<PortalSettings>();
                    DependencyManager.CachingService.Insert(PORTAL_SETTINGS_KEY, portalSettings, DateTime.Now.AddHours(24));
                }
            }
            return portalSettings;
        }


        public System.Data.DataSet LoadBaseData()
        {
            DataSet baseData = DependencyManager.CachingService.Get(BASE_DATA) as DataSet;
            if (baseData != null)
            {
                return baseData;
            }

            baseData = SqlHelper.ExecuteDataset(ConfigurationManager.ConnectionStrings["GCMS-ConnectionString-1"].ConnectionString, "usp_GCMSBaseDataGet", null);
            baseData.Tables[0].TableName = "CityLocations";
            baseData.Tables[1].TableName = "AirportLocations";
            baseData.Tables[2].TableName = "RegionLocations";
            baseData.Tables[3].TableName = "CMSGenericPageList";
            baseData.Tables[4].TableName = "CarVendor";
            baseData.Tables[5].TableName = "AirlineDetails";
            baseData.Tables[6].TableName = "StateLocations";
            baseData.Tables[7].TableName = "CountryLocations";

            DependencyManager.CachingService.Insert(BASE_DATA, baseData, DateTime.Now.AddHours(24));

            return baseData;
        }
    }
}
