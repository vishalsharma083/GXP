using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.Interfaces;
using System.Reflection;
using System.IO;

namespace GXP.Core.Framework
{
    public class ModuleParsingManager
    {
        public static List<IModuleParser> _moduleParsers = null;
        static ModuleParsingManager()
        {
            string cacheKey = "AllParsers";
            object oCache = DependencyManager.CachingService.Get(cacheKey);
            if (oCache != null)
            {
                _moduleParsers = oCache as List<IModuleParser>;
            }
            else
            {
                var type = typeof(IModuleParser);

                _moduleParsers = new List<IModuleParser>();

                Assembly assem = null;
                assem = Assembly.Load("GXP.Library");
                foreach (Type t in assem.GetTypes())
                {
                    if (type.IsAssignableFrom(t) && type.Name != t.Name)
                    {
                        _moduleParsers.Add(Activator.CreateInstance("GXP.Library", t.FullName).Unwrap() as IModuleParser);
                    }
                }
                DependencyManager.CachingService.Insert(cacheKey, _moduleParsers, DateTime.Now.AddMinutes(60));
            }
        }

        internal static string GenerateContent(string p)
        {
            string generatedHTML = string.Empty;
            foreach (var item in _moduleParsers)
            {
                item.ModuleXml = p;
                if (item.CanParse())
                {
                    generatedHTML  = item.GenerateContent();
                    break;
                }
            }
            return generatedHTML;
        }
    }
}
