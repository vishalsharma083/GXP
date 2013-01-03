using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.Interfaces;
using System.Reflection;

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
                System.Reflection.Assembly assem = Assembly.Load("GXP.Library"); // TODO : Hard coding to be removed.
                foreach (Type t in assem.GetTypes())
                {
                    if (type.IsAssignableFrom(t) && type.Name != t.Name)
                    {
                        _moduleParsers.Add(Activator.CreateInstance("GXP.Library", t.FullName).Unwrap() as IModuleParser);
                    }
                }
                _moduleParsers.Sort();
                DependencyManager.CachingService.Insert(cacheKey, _moduleParsers, DateTime.Now.AddMinutes(60));

            }
        }

        internal static string GenerateContent(string p)
        {
            string generatedHTML = string.Empty;
            foreach (var item in _moduleParsers)
            {
                if (item.CanParse(p))
                {
                    generatedHTML  = item.GenerateContent(p);
                    break;
                }
            }
            return generatedHTML;
        }
    }
}
