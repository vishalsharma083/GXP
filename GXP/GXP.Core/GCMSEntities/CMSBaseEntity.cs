using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using GXP.Core.Framework;

namespace GXP.Core.GCMSEntities
{
    [Serializable()]
    public abstract class CMSEntityBase : ICloneable
    {

        public object Clone()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, this);
                ms.Position = 0;
                object obj = bf.Deserialize(ms);
                ms.Close();
                return obj;
            }
        }

        private int _createdBy;
        public int CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }

        private DateTime _createdOn;
        public DateTime CreatedOn
        {
            get { return _createdOn; }
            set { _createdOn = value; }
        }

        private int _portaldId;
        public int PortalId
        {
            get { return _portaldId; }
            set { _portaldId = value; }
        }

        private string _moduleId;
        public string ModuleId
        {
            get { return _moduleId; }
            set { _moduleId = value; }
        }

        private int _TabId;
        public int TabId
        {
            get { return _TabId; }
            set { _TabId = value; }
        }

        private int _modifiedBy;
        public int ModifiedBy
        {
            get { return _modifiedBy; }
            set { _modifiedBy = value; }
        }

        private DateTime _modifiedOn;
        public DateTime ModifiedOn
        {
            get { return _modifiedOn; }
            set { _modifiedOn = value; }
        }

        private string _language;
        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }

        private string _storagePath;
        [XmlIgnore()]
        [System.Web.Script.Serialization.ScriptIgnore()]
        public string StoragePath
        {
            get { return _storagePath; }
            set { _storagePath = value; }
        }

        private bool _isDefault;
        public bool IsDefault
        {
            get { return _isDefault; }
            set { _isDefault = value; }
        }


        private ContentViewMode _isPublish;
        public object IsPublish
        {
            get { return _isPublish; }
            set
            {
                //To make backward compatibility, As we are getting boolean values(T/F) for older files.
                // Means user is eithering opening landing page directly from web URL with No preview as QS, Or want to see Published content.
                if (value == null || value.Equals(true))
                {
                    _isPublish = ContentViewMode.PublishOnly;
                    //User want to see Previewcontent only.
                }
                else if (value.Equals(false))
                {
                    _isPublish = ContentViewMode.PreviewOnly;
                    //ContentViewMode Type confirtable object.
                }
                else if (value.GetType().Name.Equals("ContentViewMode") || value.GetType().Name.Equals("Int32"))
                {
                    _isPublish = (ContentViewMode)value;
                }
                else if (value as System.Xml.XmlNode[] != null)
                {
                    _isPublish = Convert.ToBoolean(((System.Xml.XmlNode[])value)[0].Value) ? ContentViewMode.PublishOnly : ContentViewMode.PreviewOnly;
                }
            }
        }
        private string _incomingUrl;
        public string Incomingurl
        {
            get { return _incomingUrl; }
            set { _incomingUrl = value; }
        }

        private int _repeatColumns = 1;
        public int RepeatColumns
        {
            get { return _repeatColumns; }
            set { _repeatColumns = value; }
        }

        private string _repeatDirection = "Vertical";
        public string RepeatDirection
        {
            get { return _repeatDirection; }
            set { _repeatDirection = value; }
        }


        private bool _isAsynchronous;
        public bool IsAsynchronous
        {
            get { return _isAsynchronous; }
            set { _isAsynchronous = value; }
        }



        private bool _isInitialAsyncLoad;
        public bool IsInitialAsyncLoad
        {
            get { return _isInitialAsyncLoad; }
            set { _isInitialAsyncLoad = value; }
        }



        private string _asyncLoadEvents;
        public string AsyncLoadEvents
        {
            get { return _asyncLoadEvents; }
            set { _asyncLoadEvents = value; }
        }



    }
}