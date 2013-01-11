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

}
