﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenRPA.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenRPA.NM
{
    public class NMElement : IElement
    {
        public NativeMessagingMessage message { get; set; }
        public string Name { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public NMElement[] Children {
            get {
                var result = new List<NMElement>();
                if (chromeelement.ContainsKey("content"))
                {
                    if (!(chromeelement["content"] is JArray content)) return result.ToArray();
                    foreach (var c in content)
                    {
                        try
                        {
                            if (c.HasValues == true)
                            {
                                //var _c = c.ToObject<Dictionary<string, object>>();
                                result.Add(new NMElement(message, c.ToString()));
                            }
                        }
                        catch (Exception)
                        {
                            //var b = true;
                            //chromeelement["innerText"] = c;
                        }

                    }
                }
                return result.ToArray();
            }
        }
        public System.Drawing.Rectangle Rectangle
        {
            get
            {
                return new System.Drawing.Rectangle(X, Y, Width, Height);
            }
            set { }
        }
        public NMElement Parent { get; set; }
        public bool SupportInput {
            get
            {
                if (tagname.ToLower() != "input" && tagname.ToLower() != "select") return false;
                if(tagname.ToLower() == "input")
                {
                    if(type==null) return true;
                    if (type.ToLower() == "text" || type.ToLower() == "password") return true;
                    return false;
                } else
                {
                    return true;
                }
                
            }
        }
        private Dictionary<string, object> chromeelement { get; set; }
        public string xpath { get; set; }
        public string cssselector { get; set; }
        public string tagname { get; set; }
        public string classname { get; set; }
        public bool IsVisible { get; set; }
        public string Display { get; set; }
        public bool isVisibleOnScreen { get; set; }
        public bool Disabled { get; set; }
        public long zn_id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        private void parseChromeString(string _chromeelement)
        {
            if (string.IsNullOrEmpty(_chromeelement)) return;
            {
                JObject c = null;
                try
                {
                    c = JObject.Parse(_chromeelement);
                }
                catch (Exception)
                {
                }
                chromeelement = new Dictionary<string, object>();
                if(c!=null)
                    foreach (var kp in c )
                    {
                        if(chromeelement.ContainsKey(kp.Key.ToLower()))
                        {
                            chromeelement[kp.Key.ToLower()] = kp.Value;
                        } else
                        {
                            chromeelement.Add(kp.Key.ToLower(), kp.Value);
                        }
                        
                    }
                //if (chromeelement.ContainsKey("attributes"))
                //{
                //    JObject _c = (JObject)chromeelement["attributes"];
                //    var attributes = _c.ToObject<Dictionary<string, object>>();
                //    foreach (var a in attributes)
                //    {
                //        chromeelement[a.Key] = a.Value;
                //    }
                //}
                if (chromeelement.ContainsKey("name")) Name = chromeelement["name"].ToString();
                if (chromeelement.ContainsKey("id")) id = chromeelement["id"].ToString();
                if (chromeelement.ContainsKey("tagname")) tagname = chromeelement["tagname"].ToString();
                if (chromeelement.ContainsKey("classname")) classname = chromeelement["classname"].ToString();
                if (chromeelement.ContainsKey("type")) type = chromeelement["type"].ToString();
                if (chromeelement.ContainsKey("xpath")) xpath = chromeelement["xpath"].ToString();
                if (chromeelement.ContainsKey("cssselector")) cssselector = chromeelement["cssselector"].ToString();
                if (chromeelement.ContainsKey("cssPath")) cssselector = chromeelement["cssPath"].ToString();
                if (chromeelement.ContainsKey("csspath")) cssselector = chromeelement["csspath"].ToString();
                if (chromeelement.ContainsKey("zn_id")) zn_id = int.Parse(chromeelement["zn_id"].ToString());

                if (chromeelement.ContainsKey("isvisible")) IsVisible = bool.Parse(chromeelement["isvisible"].ToString());
                if (chromeelement.ContainsKey("display")) Display = chromeelement["display"].ToString();
                if (chromeelement.ContainsKey("isvisibleonscreen")) isVisibleOnScreen = bool.Parse(chromeelement["isvisibleonscreen"].ToString());
                if (chromeelement.ContainsKey("disabled")) Disabled = bool.Parse(chromeelement["disabled"].ToString());
                if (string.IsNullOrEmpty(Name) && tagname == "OPTION" && chromeelement.ContainsKey("content"))
                {
                    Name = Text;
                }
            }
        }
        public NMElement(NativeMessagingMessage message)
        {
            parseChromeString(message.result.ToString());
            _browser = message.browser;
            zn_id = message.zn_id;
            this.message = message;
            if(string.IsNullOrEmpty(xpath)) xpath = message.xPath;
            if (string.IsNullOrEmpty(cssselector)) cssselector = message.cssPath;
            X = message.uix;
            Y = message.uiy;
            Width = message.uiwidth;
            Height = message.uiheight;
        }
        public NMElement(NativeMessagingMessage message, string _chromeelement)
        {
            this.message = message;
            _browser = message.browser;
            parseChromeString(_chromeelement);
        }
        [Newtonsoft.Json.JsonIgnore]
        private readonly string _browser = null;
        public string browser {
            get {
                return _browser;
            }
        }
        object IElement.RawElement { get => message; set => message = value as NativeMessagingMessage; }
        public string Text
        {
            get
            {
                if(!string.IsNullOrEmpty(tagname) && tagname.ToLower() == "select")
                {
                    if (chromeelement.ContainsKey("text")) return chromeelement["text"].ToString();
                    if (chromeelement.ContainsKey("innertext")) return chromeelement["innertext"].ToString();
                }
                if (chromeelement.ContainsKey("text")) return chromeelement["text"].ToString();
                if (chromeelement.ContainsKey("innertext")) return chromeelement["innertext"].ToString();
                if(!hasRefreshed)
                {
                    Log.Output("Refresh!");
                    Refresh();
                    if (chromeelement.ContainsKey("text")) return chromeelement["text"].ToString();
                    if (chromeelement.ContainsKey("innertext")) return chromeelement["innertext"].ToString();
                }
                return null;
            }
            set
            {
                if (NMHook.connected)
                {

                    if (tagname.ToLower() == "select")
                    {
                        foreach (var item in Children)
                        {
                            if ((!string.IsNullOrEmpty(item.Text) && item.Text.ToLower() == value) || (string.IsNullOrEmpty(item.Text) && string.IsNullOrEmpty(value)))
                            {
                                Value = item.Value;
                                return;
                            }
                        }
                    }
                }
            }
        }
        public string[] Values
        {
            get
            {
                string[] result = new string[] { };
                if (chromeelement.ContainsKey("values"))
                {
                    var json = chromeelement["values"].ToString();
                    result = JsonConvert.DeserializeObject<string[]>(json);
                }
                if(result==null|| result.Length==0)
                {
                    if(!string.IsNullOrEmpty(Value))
                    {
                        result = new string[] { Value };
                    }
                }
                return result;
            }
            set
            {
                if (NMHook.connected)
                {
                    var tab = NMHook.tabs.Where(x => x.id == message.tabid).FirstOrDefault();
                    if (tab == null) throw new ElementNotFoundException("Unknown tabid " + message.tabid);
                    // NMHook.HighlightTab(tab);

                    var updateelement = new NativeMessagingMessage("updateelementvalues", PluginConfig.debug_console_output, PluginConfig.unique_xpath_ids)
                    {
                        browser = message.browser,
                        //cssPath = cssselector,
                        //xPath = xpath,
                        //tabid = message.tabid,
                        //frameId = message.frameId,
                        //data = value
                        zn_id = zn_id,
                        tabid = message.tabid,
                        frameId = message.frameId,
                        data = Interfaces.Extensions.Base64Encode(JsonConvert.SerializeObject(value))
                    };
                    var subsubresult = NMHook.sendMessageResult(updateelement, true, PluginConfig.protocol_timeout);
                    if (subsubresult == null) throw new Exception("Failed setting html element value");
                    //System.Threading.Thread.Sleep(500);
                    if (PluginConfig.wait_for_tab_after_set_value)
                    {
                        NMHook.WaitForTab(updateelement.tabid, updateelement.browser, PluginConfig.protocol_timeout);
                    }
                    return;
                }
            }
        }
        public string innerHTML
        {
            get
            {
                string result = null;
                if (!chromeelement.ContainsKey("innerhtml"))
                {
                    var getelement2 = new NativeMessagingMessage("getelement", PluginConfig.debug_console_output, PluginConfig.unique_xpath_ids)
                    {
                        browser = message.browser,
                        zn_id = zn_id,
                        tabid = message.tabid,
                        frameId = message.frameId, 
                        data = "innerhtml"
                    };
                    NativeMessagingMessage subsubresult = NMHook.sendMessageResult(getelement2, true, PluginConfig.protocol_timeout);
                    if (subsubresult == null) throw new Exception("Failed clicking html element");
                    parseChromeString(subsubresult.result.ToString());
                    if (chromeelement.ContainsKey("innerhtml"))
                    {
                        result = chromeelement["innerhtml"].ToString();
                        return result;
                    }

                }
                if (chromeelement.ContainsKey("innerhtml") && string.IsNullOrEmpty(result)) result = chromeelement["innerhtml"].ToString();
                if (chromeelement.ContainsKey("value")) result = chromeelement["value"].ToString();
                if (chromeelement.ContainsKey("innertext") && string.IsNullOrEmpty(result)) result = chromeelement["innertext"].ToString();
                if (string.IsNullOrEmpty(result)) result = Text;
                return result;
            }
            set
            {
                if (NMHook.connected)
                {
                    var tab = NMHook.tabs.Where(x => x.id == message.tabid).FirstOrDefault();
                    if (tab == null) throw new ElementNotFoundException("Unknown tabid " + message.tabid);
                    // NMHook.HighlightTab(tab);

                    var updateelement = new NativeMessagingMessage("updateelementvalue", PluginConfig.debug_console_output, PluginConfig.unique_xpath_ids)
                    {
                        browser = message.browser,
                        //cssPath = cssselector,
                        //xPath = xpath,
                        //tabid = message.tabid,
                        //frameId = message.frameId,
                        //data = value
                        zn_id = zn_id,
                        tabid = message.tabid,
                        frameId = message.frameId,
                        data = Interfaces.Extensions.Base64Encode(value), result = "innerhtml"
                    };
                    var temp = Interfaces.Extensions.Base64Decode(updateelement.data);
                    if (value == null) updateelement.data = null;
                    var subsubresult = NMHook.sendMessageResult(updateelement, true, PluginConfig.protocol_timeout);
                    if (subsubresult == null) throw new Exception("Failed setting html element value");
                    //System.Threading.Thread.Sleep(500);
                    if (PluginConfig.wait_for_tab_after_set_value)
                    {
                        NMHook.WaitForTab(updateelement.tabid, updateelement.browser, TimeSpan.FromSeconds(5));
                    }
                    return;
                }
            }
        }
        public string Value
        {
            get
            {
                string result = null;
                if (chromeelement.ContainsKey("value")) result = chromeelement["value"].ToString();
                if (chromeelement.ContainsKey("innertext") && string.IsNullOrEmpty(result)) result = chromeelement["innertext"].ToString();
                if (string.IsNullOrEmpty(result)) result = Text;
                return result;
            }
            set
            {
                if (NMHook.connected)
                {
                    var tab = NMHook.tabs.Where(x => x.id == message.tabid).FirstOrDefault();
                    if (tab == null) throw new ElementNotFoundException("Unknown tabid " + message.tabid);
                    // NMHook.HighlightTab(tab);

                    var updateelement = new NativeMessagingMessage("updateelementvalue", PluginConfig.debug_console_output, PluginConfig.unique_xpath_ids)
                    {
                        browser = message.browser,
                        //cssPath = cssselector,
                        //xPath = xpath,
                        //tabid = message.tabid,
                        //frameId = message.frameId,
                        //data = value
                        zn_id = zn_id,
                        tabid = message.tabid,
                        frameId = message.frameId,
                        data = Interfaces.Extensions.Base64Encode(value),
                        result = "value"
                    };
                    var temp = Interfaces.Extensions.Base64Decode(updateelement.data);
                    if (value == null) updateelement.data = null;
                    var subsubresult = NMHook.sendMessageResult(updateelement, true, PluginConfig.protocol_timeout);
                    if (subsubresult == null) throw new Exception("Failed setting html element value");
                    //System.Threading.Thread.Sleep(500);
                    if (PluginConfig.wait_for_tab_after_set_value)
                    {
                        NMHook.WaitForTab(updateelement.tabid, updateelement.browser, TimeSpan.FromSeconds(5));
                    }
                    return;
                }
            }
        }
        public void Click(bool VirtualClick, Input.MouseButton Button, int OffsetX, int OffsetY, bool DoubleClick, bool AnimateMouse)
        {
            if (Button != Input.MouseButton.Left) { VirtualClick = false; }
            if(type== "file") { VirtualClick = false; }
            if (!VirtualClick)
            {
                if (AnimateMouse)
                {
                    FlaUI.Core.Input.Mouse.MoveTo(new System.Drawing.Point(Rectangle.X + OffsetX, Rectangle.Y + OffsetY));
                }
                else
                {
                    NativeMethods.SetCursorPos(Rectangle.X + OffsetX, Rectangle.Y + OffsetY);
                }
                Log.Debug("Click at  " + OffsetX + "," + OffsetY + " in area " + Rectangle.ToString());
                Input.InputDriver.Click(Button);
                if (DoubleClick) Input.InputDriver.Click(Button);
                return;
            }
            bool virtualClick = true;
            NMHook.checkForPipes(true, true, true);
            if (NMHook.connected)
            {
                if (virtualClick)
                {
                    var getelement2 = new NativeMessagingMessage("clickelement", PluginConfig.debug_console_output, PluginConfig.unique_xpath_ids)
                    {
                        browser = message.browser,
                        //cssPath = cssselector,
                        //xPath = xpath,
                        zn_id = zn_id,
                        tabid = message.tabid,
                        frameId = message.frameId
                    };
                    NativeMessagingMessage subsubresult = NMHook.sendMessageResult(getelement2, true, PluginConfig.protocol_timeout);
                    if (subsubresult == null) throw new Exception("Failed clicking html element");
                    //System.Threading.Thread.Sleep(500);
                    if (PluginConfig.wait_for_tab_click)
                    {
                        NMHook.WaitForTab(getelement2.tabid, getelement2.browser, TimeSpan.FromSeconds(5));
                    }
                    return;
                }
                NativeMessagingMessage subresult = null;
                var getelement = new NativeMessagingMessage("getelement", PluginConfig.debug_console_output, PluginConfig.unique_xpath_ids)
                {
                    browser = message.browser,
                    zn_id = zn_id,
                    //cssPath = cssselector,
                    //xPath = xpath,
                    tabid = message.tabid,
                    frameId = message.frameId
                };
                if (NMHook.connected) subresult = NMHook.sendMessageResult(getelement, true, PluginConfig.protocol_timeout);
                if (subresult == null) throw new Exception("Failed clicking html element, element not found");
                int hitx = subresult.uix;
                int hity = subresult.uiy;
                int width = subresult.uiwidth;
                int height = subresult.uiheight;
                hitx += OffsetX;
                hity += OffsetY;
                if ((OffsetX == 0 && OffsetY == 0) || hitx < 0 || hity < 0)
                {
                    hitx += (width / 2);
                    hity += (height / 2);
                }
                if (AnimateMouse)
                {
                    FlaUI.Core.Input.Mouse.MoveTo(new Point(hitx, hity));
                }
                else
                {
                    NativeMethods.SetCursorPos(hitx, hity);
                }
                Input.InputDriver.Click(Button);
                if (DoubleClick) Input.InputDriver.Click(Button);
                //System.Threading.Thread.Sleep(500);
                if (PluginConfig.wait_for_tab_click)
                {
                    NMHook.WaitForTab(getelement.tabid, getelement.browser, TimeSpan.FromSeconds(5));
                }

            }
        }
        private bool hasRefreshed = false;
        public bool Refresh()
        {
            try
            {
                var getelement = new NativeMessagingMessage("getelement", PluginConfig.debug_console_output, PluginConfig.unique_xpath_ids)
                {
                    browser = browser,
                    cssPath = cssselector,
                    xPath = xpath,
                    frameId = this.message.frameId,
                    windowId = this.message.windowId,
                    zn_id = this.zn_id
                };
                NativeMessagingMessage message = null;
                // getelement.data = "getdom";
                if (NMHook.connected) message = NMHook.sendMessageResult(getelement, true, PluginConfig.protocol_timeout);
                if (message == null)
                {
                    Log.Error("Failed getting html element");
                    return false;
                }
                if (message.result == null)
                {
                    Log.Error("Failed getting html element");
                    return false;
                }

                parseChromeString(message.result.ToString());
                zn_id = message.zn_id;
                this.message = message;
                if (!string.IsNullOrEmpty(message.xPath)) xpath = message.xPath;
                if (!string.IsNullOrEmpty(message.cssPath)) cssselector = message.cssPath;
                X = message.uix;
                Y = message.uiy;
                Width = message.uiwidth;
                Height = message.uiheight;
                hasRefreshed = true;
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return false;
            }
        }
        public void Focus()
        {
            var tab = NMHook.tabs.Where(x => x.id == message.tabid).FirstOrDefault();
            if (tab == null) throw new ElementNotFoundException("Unknown tabid " + message.tabid);
            var updateelement = new NativeMessagingMessage("focuselement", PluginConfig.debug_console_output, PluginConfig.unique_xpath_ids)
            {
                browser = message.browser,
                //cssPath = cssselector,
                //xPath = xpath,
                //tabid = message.tabid,
                //frameId = message.frameId,
                //data = value
                zn_id = zn_id,
                tabid = message.tabid,
                frameId = message.frameId
            };
            var subsubresult = NMHook.sendMessageResult(updateelement, true, PluginConfig.protocol_timeout);
            if (subsubresult == null) throw new Exception("Failed setting html element value");
            //System.Threading.Thread.Sleep(500);
            if (PluginConfig.wait_for_tab_after_set_value)
            {
                NMHook.WaitForTab(updateelement.tabid, updateelement.browser, TimeSpan.FromSeconds(5));
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "IDE1006")]
        public Task _Highlight(System.Drawing.Color Color, TimeSpan Duration)
        {
            using (Interfaces.Overlay.OverlayWindow _overlayWindow = new Interfaces.Overlay.OverlayWindow(true))
            {
                _overlayWindow.BackColor = Color;
                _overlayWindow.Visible = true;
                _overlayWindow.SetTimeout(Duration);
                _overlayWindow.Bounds = Rectangle;
                Log.Debug("Highlight area " + Rectangle.ToString());
                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                do
                {
                    System.Threading.Thread.Sleep(10);
                    _overlayWindow.TopMost = true;
                } while (_overlayWindow.Visible && sw.Elapsed < Duration);
                return Task.CompletedTask;
            }
        }
        public Task Highlight(bool Blocking, Color Color, TimeSpan Duration)
        {
            if (!Blocking)
            {
                Task.Run(() => _Highlight(Color, Duration));
                return Task.CompletedTask;
            }
            return _Highlight(Color, Duration);

        }
        public override string ToString()
        {
            if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(Name)) return tagname;
            if (!string.IsNullOrEmpty(id) && string.IsNullOrEmpty(Name)) return tagname + " " + id;
            if (string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(Name)) return tagname + " " + Name;
            return "id:" + id + " TagName:" + tagname + " Name: " + Name;
        }
        public string ImageString()
        {
            var AddedWidth = 10;
            var AddedHeight = 10;
            var ScreenImageWidth = Rectangle.Width + AddedWidth;
            var ScreenImageHeight = Rectangle.Height + AddedHeight;
            var ScreenImagex = Rectangle.X - (AddedWidth / 2);
            var ScreenImagey = Rectangle.Y - (AddedHeight / 2);
            if (ScreenImagex < 0) ScreenImagex = 0; if (ScreenImagey < 0) ScreenImagey = 0;
            using (var image = Interfaces.Image.Util.Screenshot(ScreenImagex, ScreenImagey, ScreenImageWidth, ScreenImageHeight, Interfaces.Image.Util.ActivityPreviewImageWidth, Interfaces.Image.Util.ActivityPreviewImageHeight))
            {
                return Interfaces.Image.Util.Bitmap2Base64(image);
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public string href
        {
            get
            {
                if (chromeelement != null)
                {
                    if (chromeelement.ContainsKey("href")) return chromeelement["href"].ToString();
                    return null;
                }
                return null;
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public string src
        {
            get
            {
                if (chromeelement != null)
                {
                    if (chromeelement.ContainsKey("src")) return chromeelement["src"].ToString();
                    return null;
                }
                return null;
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public string alt
        {
            get
            {
                if (chromeelement != null)
                {
                    if (chromeelement.ContainsKey("alt")) return (string)chromeelement["alt"];
                    return null;
                }
                return null;
            }
        }
        public IElement[] Items
        {
            get
            {
                var result = new List<IElement>();
                if (tagname.ToLower() == "select")
                {
                    foreach(var item in Children)
                    {
                        item.Refresh();
                        result.Add(item);
                    }

                }
                return result.ToArray();
            }
        }
        public bool WaitForVanish(TimeSpan Timeout, bool IsVisible = true, bool isVisibleOnScreen = false)
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            do
            {
                System.Threading.Thread.Sleep(100);
                if (!Refresh()) return true;
                if(IsVisible && !this.IsVisible) return true;
                if(isVisibleOnScreen && !this.isVisibleOnScreen) return true;
            } while (sw.Elapsed < Timeout);
            return false;
        }
        public override bool Equals(object obj)
        {
            //if (obj is NMElement nm)
            //{
            //    var eq = new Activities.NMEqualityComparer();
            //    return eq.Equals(this, nm);
            //}
            //return base.Equals(obj);
            return hashCode == obj.GetHashCode();
        }
        private int hashCode = 0;
        public override int GetHashCode()
        {
            if (hashCode > 0) return hashCode;
            int hCode = 0;

            if (!string.IsNullOrEmpty(xpath)) hCode += xpath.GetHashCode();
            if (!string.IsNullOrEmpty(id)) hCode += id.GetHashCode();
            if (!string.IsNullOrEmpty(cssselector)) hCode += cssselector.GetHashCode();
            if (!string.IsNullOrEmpty(classname)) hCode += classname.GetHashCode();
            if (!string.IsNullOrEmpty(Text)) hCode += Text.GetHashCode();
            if (zn_id > 0) hCode += Convert.ToInt32(zn_id);
            hashCode = hCode.GetHashCode();
            return hashCode;
        }
        public bool IsChecked
        {
            get
            {
                if (chromeelement.ContainsKey("checked"))
                {
                    if (chromeelement["checked"].ToString() == "true") return true;
                    if (chromeelement["checked"].ToString() == "True") return true;
                }
                return false;
            }
            set
            {
                if (NMHook.connected)
                {
                    var tab = NMHook.tabs.Where(x => x.id == message.tabid).FirstOrDefault();
                    if (tab == null) throw new ElementNotFoundException("Unknown tabid " + message.tabid);
                    // NMHook.HighlightTab(tab);

                    var updateelement = new NativeMessagingMessage("updateelementvalue", PluginConfig.debug_console_output, PluginConfig.unique_xpath_ids)
                    {
                        browser = message.browser,
                        zn_id = zn_id,
                        tabid = message.tabid,
                        frameId = message.frameId,
                        data = Interfaces.Extensions.Base64Encode(value.ToString()),
                        result = "value"
                    };
                    var subsubresult = NMHook.sendMessageResult(updateelement, true, PluginConfig.protocol_timeout);
                    if (subsubresult == null) throw new Exception("Failed setting html element value");
                    //System.Threading.Thread.Sleep(500);
                    if (PluginConfig.wait_for_tab_after_set_value)
                    {
                        NMHook.WaitForTab(updateelement.tabid, updateelement.browser, TimeSpan.FromSeconds(5));
                    }
                    return;
                }
            }
        }

    }
}
