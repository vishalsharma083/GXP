using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
namespace GXP.Framework
{
    public class CDATA : IXmlSerializable
    {


        private string _text;
        public CDATA()
        {
        }

        public CDATA(string text)
        {
            this._text = text;
        }

        public string Text
        {
            get { return _text; }
        }

        private void ReadXml(XmlReader reader)
        {
            this._text = reader.ReadString();
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this._text = reader.ReadString();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteCData(_text);
        }
    }
}