using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Xsl;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace GXP.Core.Framework
{
    public class CMSXsltUtility
    {
        public PagePublisherInput PublishingDetail { get; set; }

        public string PerformTransformation(string xsltInput, string xmlToOperateOn, bool published_)
        {
            string output = string.Empty;
            StringBuilder builder = new StringBuilder();
            string inputText = "<xsl:transform version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" xmlns:CMSXsltUtility=\"urn:CMSXsltUtility\" xmlns:GeoLocation=\"urn:GeoLocation\"><xsl:output method=\"html\" omit-xml-declaration=\"yes\" /><xsl:template match=\"/\">";
            XslCompiledTransform xslt = new XslCompiledTransform();
            XsltArgumentList xslArgs = new XsltArgumentList();
            try
            {
                //pass an instance of the custom object
                xslArgs.AddExtensionObject("urn:CMSXsltUtility", this);
                inputText = inputText + xsltInput.Trim() + "</xsl:template></xsl:transform>";

                using (StringReader stringReader = new StringReader(inputText))
                {
                    using (XmlReader xmlReader = XmlReader.Create(stringReader))
                    {
                        xslt.Load(xmlReader);
                    }
                }

                XPathDocument xPathDoc = null;
                using (StringWriter stringWriter = new StringWriter(builder))
                {
                    using (XmlTextWriter writer = new XmlTextWriter(stringWriter))
                    {
                        using (StringReader stringReader = new StringReader(xmlToOperateOn))
                        {
                            xPathDoc = new XPathDocument(stringReader);
                            xslt.Transform(xPathDoc, xslArgs, writer);
                            output = builder.ToString().Replace(" xmlns:CMSXsltUtility=\"urn:CMSXsltUtility\" xmlns:GeoLocation=\"urn:GeoLocation\"", string.Empty);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                if (published_ == false)
                {
                    output = xsltInput + "</br> <b>Error while xslt Transformation </b>: " + ex.ToString();
                }
            }
            return output;
        }

        public string GetContent(int tabid_, int moduleId_)
        {
            return ModuleParsingManager.GenerateContent(Utility.PagePublisherUtility.GetViewModeModuleContent(string.Empty)); // TODO : To pass actual value.
        }
    }
}
