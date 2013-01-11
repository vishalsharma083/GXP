using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GXP.Framework;
namespace GXP.Core.GCMSEntities
{
    [Serializable]
    public class CMSTextInfo : CMSEntityBase
    {
        private CDATA _content = new CDATA();

        public CMSTextInfo()
        {
        }
        public CMSTextInfo(int portalId_)
        {
            PortalId = portalId_;
        }

        [XmlElement("Content", Type = typeof(CDATA))]
        public CDATA Content
        {
            get { return _content; }
            set { _content = value; }
        }
    }
}
