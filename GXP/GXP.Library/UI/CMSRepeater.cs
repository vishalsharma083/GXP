using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web.UI;

namespace GXP.Library.UI
{
    public class CMSRepeater : Repeater
    {
        public void Init()
        {
            if (SourceData is DataTable)
            {
                this.ItemDataBound += new RepeaterItemEventHandler(rptData1_ItemDataBound);
            }
            else if (SourceData is IEnumerable<XmlNode>)
            {
                this.ItemDataBound += new RepeaterItemEventHandler(rptData2_ItemDataBound);
            }
        }

        public void Bind()
        {
            this.DataSource = SourceData;
            this.DataBind();
        }

        public void Render()
        {
            GeneratedContent = string.Empty; // TODO;
        }

        protected void rptData1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                (e.Item.FindControl("ltlHeaderTemplate") as Literal).Text = CMSHeaderTemplate;
            }
            else if (e.Item.ItemType == ListItemType.Item)
            {
                (e.Item.FindControl("ltlItemTemplate") as Literal).Text = ReplaceKeywords(CMSItemTemplate, e.Item.ItemIndex);
            }
            else if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                (e.Item.FindControl("ltlAlternateItemTemplate") as Literal).Text = ReplaceKeywords(string.IsNullOrEmpty(CMSAlternateItemTemplate) ? CMSItemTemplate : CMSAlternateItemTemplate, e.Item.ItemIndex);
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                (e.Item.FindControl("ltlFooterTemplate") as Literal).Text = CMSFooterTemplate;
            }
        }

        protected void rptData2_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                (e.Item.FindControl("ltlHeaderTemplate") as Literal).Text = CMSHeaderTemplate;
            }
            else if (e.Item.ItemType == ListItemType.Item)
            {
                (e.Item.FindControl("ltlItemTemplate") as Literal).Text = ReplaceKeywordsFromXMlNode(CMSItemTemplate, e.Item.DataItem);
            }
            else if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                (e.Item.FindControl("ltlAlternateItemTemplate") as Literal).Text = ReplaceKeywordsFromXMlNode(string.IsNullOrEmpty(CMSAlternateItemTemplate) ? CMSItemTemplate : CMSAlternateItemTemplate, e.Item.DataItem);
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                (e.Item.FindControl("ltlFooterTemplate") as Literal).Text = CMSFooterTemplate;
            }
        }

        private string ReplaceKeywords(string content_, int index_)
        {
            DataTable table = (DataTable)SourceData;
            //content_ = GlobalCMS.Framework.DKIController.DoDKI(content_, table.Rows[index_]); // TODO:
            return content_;
        }
        private string ReplaceKeywordsFromXMlNode(string content_, object item)
        {
            StringBuilder sb = new StringBuilder(content_);
            sb.Replace("[#Title#]", XPathBinder.Eval(item, @"title").ToString());
            sb.Replace("[#Link#]", XPathBinder.Eval(item, @"link").ToString());
            sb.Replace("[#pubDate#]", XPathBinder.Eval(item, @"pubDate").ToString());
            sb.Replace("[#Description#]", XPathBinder.Eval(item, @"description").ToString());
            return content_.ToString();
        }

        public string CMSHeaderTemplate { get; set; }
        public string CMSItemTemplate { get; set; }
        public string CMSAlternateItemTemplate { get; set; }
        public string CMSFooterTemplate { get; set; }
        public object SourceData { get; set; }
        public string GeneratedContent { get; set; }  
    }
}
