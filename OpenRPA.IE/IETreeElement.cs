﻿using FlaUI.Core.Definitions;
using OpenRPA.Interfaces.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRPA.IE
{
    public class IETreeElement : treeelement
    {
        public IEElement IEElement { get; set; }
        public ControlType ControlType
        {
            get
            {
                if(IEElement!=null)
                {
                    switch (IEElement.TagName)
                    {
                        case "input":
                            if (IEElement.Type == "hidden") return ControlType.Text;
                            if (IEElement.Type == "text" || IEElement.Type == "password") return ControlType.Edit;
                            if (IEElement.Type == "button" || IEElement.Type == "submit") return ControlType.Button;
                            break;
                        case "textarea":
                            return ControlType.Text;
                        case "a":
                            return ControlType.Hyperlink;
                        case "img":
                            return ControlType.Image;
                        case "div": case "body":
                            return ControlType.Pane;
                        case "style":
                            return ControlType.ToolTip;
                        case "html": case "head":
                            return ControlType.Hyperlink;
                        case "script":
                            return ControlType.DataItem;
                        case "form":
                            return ControlType.DataGrid;
                        case "center":
                            return ControlType.Text;
                        default:
                            break;
                    }
                }
                return ControlType.Hyperlink;
            }
        }

        public IETreeElement(treeelement parent, bool expanded, IEElement element) : base(parent)
        {
            IEElement = element;
            IsExpanded = expanded;
            Element = element;
            string controltype = "";
            string name = element.ToString();
            string automationid = "";
            //if (element.Properties.ControlType.IsSupported) ControlType = element.Properties.ControlType.Value;
            //if (element.Properties.ControlType.IsSupported) controltype = element.Properties.ControlType.Value.ToString();
            //if (element.Properties.Name.IsSupported) name = element.Properties.Name.Value;
            //if (element.Properties.AutomationId.IsSupported) automationid = element.Properties.AutomationId.Value;
            Name = (controltype + " " + name + " " + automationid).Trim();
        }

        public override void AddSubElements()
        {
            MSHTML.IHTMLElementCollection children = (MSHTML.IHTMLElementCollection)IEElement.RawElement.children;
            foreach (MSHTML.IHTMLElement elementNode in children) {
                var ele = new IEElement(IEElement.Browser, elementNode);
                var exists = Children.Where(x => ((IEElement)x.Element).UniqueID == ele.UniqueID).FirstOrDefault();
                if(exists==null)
                {
                    Interfaces.Log.Debug("Adding " + ele.ToString());
                    Children.Add(new IETreeElement(this, false, ele));
                }
            }
            int frameoffsetx = 0;
            int frameoffsety = 0;
            if (IESelector.frameTags.Contains(IEElement.TagName.ToUpper()))
            {
                frameoffsetx += IEElement.RawElement.offsetLeft;
                frameoffsety += IEElement.RawElement.offsetTop;
                var web = IEElement.RawElement as SHDocVw.IWebBrowser2;
                var _doc = (MSHTML.HTMLDocument)web.Document;
                Children.Add(new IETreeElement(this, false, new IEElement(IEElement.Browser, _doc.documentElement)));
            }
        }
    }

}
