using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GXP.Core.Framework
{
    public enum PublishStatus
    { 
        NONE,
        FAILED,
        SUCCESS
    }

    [Serializable()]
    public enum ContentViewMode
    {
        [XmlEnum("0")]
        Live = 0,
        [XmlEnum("1")]
        PreviewOnly = 1,
        [XmlEnum("2")]
        PublishOnly = 2
    }

    public enum ModuleContentErrorType
    {
        NONE = 0,
        MODULE_CONTENT_NOT_FOUND = 1,
        MODULE_CONTENT_NOT_SET_TO_LOAD_ASYNCHRONOUSLY = 2
    }
    public enum XSLTExceptionType
    {
        DBException,
        DMS_Processing_Exception,
        DMS_TimeOut_Exception,
        DMS_Service_Unavailable_Exception,
        ExcelException,
        RSSException,
        EclipseException
    }

}
