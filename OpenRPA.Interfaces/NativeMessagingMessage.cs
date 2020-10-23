﻿using OpenRPA.NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRPA.Interfaces
{
    [Serializable]
    public class NativeMessagingMessage : PipeMessage
    {

        private  NativeMessagingMessage() : base()
        {
        }
        public NativeMessagingMessage(string functionName, bool debug, string[] unique_xpath_ids) : base()
        {
            this.functionName = functionName;
            this.debug = debug;
            this.uniquexpathids = unique_xpath_ids;
        }
        public NativeMessagingMessage(NativeMessagingMessage e) : base(e)
        {
            functionName = e.functionName;
            browser = e.browser;
            windowId = e.windowId;
            tabid = e.tabid;
        }
        public string browser { get; set; }
        public bool debug { get; set; }
        public string[] uniquexpathids { get; set; }
        public int windowId { get; set; } = -1;
        public string functionName { get; set; } = "ping";
        public string script { get; set; }
        public object result { get; set; }
        public NativeMessagingMessage[] results { get; set; }
        public int tabid { get; set; } = -1;
        public long frameId { get; set; } = -1;
        public long zn_id { get; set; } = -1;
        public string key { get; set; }
        public string frame { get; set; }
        public NativeMessagingMessageTab tab { get; set; }
        //public string selector { get; set; }
        public string cssPath { get; set; }
        public string xPath { get; set; }
        public string[] xpaths { get; set; }
        public string fromcssPath { get; set; }
        public string fromxPath { get; set; }
        public int x { get; set; } = -1;
        public int y { get; set; } = -1;
        public int width { get; set; } = -1;
        public int height { get; set; } = -1;
        public int uix { get; set; } = -1;
        public int uiy { get; set; } = -1;
        public int uiwidth { get; set; } = -1;
        public int uiheight { get; set; } = -1;
        public string data { get; set; }
        public int c { get; set; }
        
    }
}
