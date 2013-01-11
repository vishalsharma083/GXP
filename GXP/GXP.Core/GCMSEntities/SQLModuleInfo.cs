using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Data;

namespace GXP.Core.GCMSEntities
{
    public class SQLModuleInfo : ProcedureInfo
    {

        public int MaxRecords { get; set; }
        public string SortOrder { get; set; }
        /// <summary>
        /// List of parameters values going as input in the procedure
        /// </summary>
        private object[] _parameterValues;
        [XmlIgnore]
        [System.Web.Script.Serialization.ScriptIgnore]
        public object[] ParameterValues
        {
            get { return _parameterValues; }
            set { _parameterValues = value; }
        }

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

    }

    public class NameSqlDbTypePair
    {
        public NameSqlDbTypePair()
        {

        }
        public NameSqlDbTypePair(string name_, SqlDbType type_)
        {
            Name = name_;
            Type = type_;
        }
        public string Name { get; set; }
        public SqlDbType Type { get; set; }
        public string KeyName { get; set; }
        public Boolean Internal { get; set; }
    }

    public class NameTypePair
    {
        public NameTypePair()
        {

        }
        public NameTypePair(string name_, string type_)
        {
            Name = name_;
            Type = type_;
        }
        public string Name { get; set; }
        public string Type { get; set; }
    }


    public class ProcedureSetting
    {
        public ProcedureSetting()
        {

        }
        public List<ProcedureInfo> ListProcedureInfo { get; set; }
    }

    [Serializable]
    public class ProcedureInfo : CMSEntityBase
    {
        public ProcedureInfo()
        {
            InputParams = new List<NameSqlDbTypePair>();
            OutputParams = new List<NameTypePair>();

        }

        public string ProcedureName { get; set; }
        public string AliasName { get; set; }
        public string ConnectionString { get; set; }
        [System.Web.Script.Serialization.ScriptIgnore]
        public List<NameSqlDbTypePair> InputParams { get; set; }
        [System.Web.Script.Serialization.ScriptIgnore]
        public List<NameTypePair> OutputParams { get; set; }
    }
}
