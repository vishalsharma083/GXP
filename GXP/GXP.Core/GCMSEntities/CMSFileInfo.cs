using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXP.Core.GCMSEntities
{

    [Serializable]
    public class CMSFileInfo : CMSEntityBase
    {
        private string _filePathWithName;
        public string FilePathWithName
        {
            get { return _filePathWithName; }
            set { _filePathWithName = value; }
        }

        private string _fileContent;
        public string FileContent
        {
            get { return _fileContent; }
            set { _fileContent = value; }
        }
    }
}
