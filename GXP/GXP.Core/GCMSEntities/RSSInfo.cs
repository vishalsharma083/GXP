using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXP.Core.GCMSEntities
{
    public class RSSInfo : CMSEntityBase
    {
        private bool isPrimaryHeader = true;
        public bool IsPrimaryHeader
        {
            get { return isPrimaryHeader; }
            set { isPrimaryHeader = value; }
        }

        private int _headerLength;
        public int HeaderLength
        {
            get { return _headerLength; }
            set { _headerLength = value; }
        }

        private string _feedURL;
        public string FeedURL
        {
            get { return _feedURL; }
            set { _feedURL = value; }
        }

        private string _secondaryURL;
        public string SecondaryURL
        {
            get { return _secondaryURL; }
            set { _secondaryURL = value; }
        }

        private DateTime _publishedDate;
        public DateTime PublishedDate
        {
            get { return _publishedDate; }
            set { _publishedDate = value; }
        }

        private int _descriptionLength;
        public int DescriptionLength
        {
            get { return _descriptionLength; }
            set { _descriptionLength = value; }
        }
        private int _maxRecords;
        public int MaxRecords
        {
            get { return _maxRecords; }
            set { _maxRecords = value; }
        }
    }
}
