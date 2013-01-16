using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXP.Core.Interfaces;

namespace GXP.Core.Framework
{
    public abstract class BaseModuleParser : IModuleParser
    {
        private string _moduleXml;
        public string ModuleXml
        {
            get
            {
                return _moduleXml;
            }
            set
            {
                _moduleXml = value;
            }
        }
        public abstract bool CanParse();
        public abstract string GenerateContent();
        private PagePublisherInput _publisherInput;
        public PagePublisherInput PublisherInput
        {
            get
            {
                return _publisherInput;
            }
            set
            {
                _publisherInput = value;
            }
        }
    }
}
