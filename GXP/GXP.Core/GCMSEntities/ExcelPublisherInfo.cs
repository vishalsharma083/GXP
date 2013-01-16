using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GXP.Core.GCMSEntities
{
    public class ExcelPublisherInfo : CMSEntityBase
    {
        private XmlCDataSection _headerTemplate;
        [System.Web.Script.Serialization.ScriptIgnore]
        public XmlCDataSection HeaderTemplate
        {
            get { return _headerTemplate; }
            set { _headerTemplate = value; }
        }
        public string HeaderTemplateData
        {
            get { return _headerTemplate != null ? Microsoft.JScript.GlobalObject.escape(_headerTemplate.Data) : string.Empty; }
        }


        private XmlCDataSection _itemTemplate;
        [System.Web.Script.Serialization.ScriptIgnore]
        public XmlCDataSection ItemTemplate
        {
            get { return _itemTemplate; }
            set { _itemTemplate = value; }
        }
        public string ItemTemplateData
        {
            get { return _itemTemplate != null ? Microsoft.JScript.GlobalObject.escape(_itemTemplate.Data) : string.Empty; }
        }

        private XmlCDataSection _alternateItemTemplate;
        [System.Web.Script.Serialization.ScriptIgnore]
        public XmlCDataSection AlternateItemTemplate
        {
            get { return _alternateItemTemplate; }
            set { _alternateItemTemplate = value; }
        }
        public string AlternateItemTemplateData
        {
            get { return _alternateItemTemplate != null ? Microsoft.JScript.GlobalObject.escape(_alternateItemTemplate.Data) : string.Empty; }
        }


        private XmlCDataSection _footerTemplate;
        [System.Web.Script.Serialization.ScriptIgnore]
        public XmlCDataSection FooterTemplate
        {
            get { return _footerTemplate; }
            set { _footerTemplate = value; }
        }
        public string FooterTemplateData
        {
            get { return _footerTemplate != null ? Microsoft.JScript.GlobalObject.escape(_footerTemplate.Data) : string.Empty; }
        }

        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public int MaxRecords { get; set; }

    }
}
