using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.Interfaces;
using System.Reflection;

namespace GXP.Core.Framework
{
    public class RequestValidator
    {
        private static List<IPageRequestValidation> _validators = new List<IPageRequestValidation>();
        static RequestValidator()
        {
            string cacheKey = "AllRequestValidators";
            object oCache = DependencyManager.CachingService.Get(cacheKey);
            if (oCache != null)
            {
                _validators = oCache as List<IPageRequestValidation>;
            }
            else
            {
                _validators = new List<IPageRequestValidation>();
                var type = typeof(IPageRequestValidation);
                _validators = new List<IPageRequestValidation>();
                System.Reflection.Assembly assem = Assembly.Load("GXP.Library"); // TODO : Hard coding to be removed.
                foreach (Type t in assem.GetTypes())
                {
                    if (type.IsAssignableFrom(t) && type.Name != t.Name)
                    {
                        _validators.Add(Activator.CreateInstance("GXP.Library", t.FullName).Unwrap() as IPageRequestValidation);
                    }
                }
                _validators.Sort(delegate(IPageRequestValidation x, IPageRequestValidation y)
                {
                    return x.SortOrder.CompareTo(y.SortOrder);
                });

                DependencyManager.CachingService.Insert(cacheKey, _validators, DateTime.Now.AddMinutes(60));
            }
        }

        public static void Validate(PagePublisherInput input_)
        {
             foreach (IPageRequestValidation validator in _validators)
            {
                if (validator.IsValid(input_)==false)
                {
                    input_.CanProcessRequest = false;
                    break;
                }
            }
        }
    }
}
