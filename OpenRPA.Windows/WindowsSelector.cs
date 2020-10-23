﻿using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.AutomationElements.Infrastructure;
using OpenRPA.Interfaces;
using OpenRPA.Interfaces.Selector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRPA.Windows
{
    public class WindowsSelector : Selector
    {
        UIElement element { get; set; }
        public WindowsSelector(string json) : base(json) { }
        public WindowsSelector(AutomationElement element, WindowsSelector anchor, bool doEnum)
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            Log.Selector(string.Format("windowsselector::begin {0:mm\\:ss\\.fff}", sw.Elapsed));

            AutomationElement root = null;
            AutomationElement baseElement = null;
            var pathToRoot = new List<AutomationElement>();
            while (element != null)
            {
                // Break on circular relationship (should not happen?)
                //if (pathToRoot.Contains(element) || element.Equals(_rootElement)) { break; }
                // if (pathToRoot.Contains(element)) { break; }
                try
                {
                    if (element.Parent != null) pathToRoot.Add(element);
                    if (element.Parent == null) root = element;
                }
                catch (Exception)
                {
                    root = element;
                }
                try
                {
                    //element = _treeWalker.GetParent(element);
                    element = element.Parent;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "");
                    return;
                }
            }
            Log.Selector(string.Format("windowsselector::create pathToRoot::end {0:mm\\:ss\\.fff}", sw.Elapsed));
            pathToRoot.Reverse();
            if (anchor != null)
            {
                bool SearchDescendants = false;
                var p = anchor.First().Properties.Where(x => x.Name == "SearchDescendants").FirstOrDefault();
                if (p.Value != null && p.Value == "true") SearchDescendants = true;
                if(SearchDescendants)
                {
                    var a = anchor.Last();
                    var idx = -1;
                    for (var i = 0; i < pathToRoot.Count(); i++)
                    {
                        if (WindowsSelectorItem.Match(a, pathToRoot[i]))
                        {
                            idx = i;
                            // break;
                        }
                    }
                    pathToRoot.RemoveRange(0, idx);

                }
                else
                {
                    var anchorlist = anchor.Where(x => x.Enabled && x.Selector == null).ToList();
                    for (var i = 0; i < anchorlist.Count(); i++)
                    {
                        if (WindowsSelectorItem.Match(anchorlist[i], pathToRoot[0]))
                        //if (((WindowsSelectorItem)anchorlist[i]).Match(pathToRoot[0]))
                        {
                            pathToRoot.Remove(pathToRoot[0]);
                        }
                        else
                        {
                            Log.Selector("Element does not match the anchor path");
                            return;
                        }
                    }

                }

            }
            WindowsSelectorItem item;

            if(PluginConfig.traverse_selector_both_ways)
            {
                Log.Selector(string.Format("windowsselector::create traverse_selector_both_ways::begin {0:mm\\:ss\\.fff}", sw.Elapsed));
                var temppathToRoot = new List<AutomationElement>();
                var newpathToRoot = new List<AutomationElement>();
                foreach (var e in pathToRoot) temppathToRoot.Add(e);
                Log.Selector(string.Format("windowsselector::traverse back to element from root::begin {0:mm\\:ss\\.fff}", sw.Elapsed));
                using (var automation = AutomationUtil.getAutomation())
                {
                    bool isDesktop = true;
                    AutomationElement parent = null;
                    if (anchor != null) { parent = temppathToRoot[0].Parent; isDesktop = false; }
                    else { parent = automation.GetDesktop(); }
                    int count = temppathToRoot.Count;
                    while (temppathToRoot.Count > 0)
                    {
                        count--;
                        var i = temppathToRoot.First();
                        temppathToRoot.Remove(i);
                        item = new WindowsSelectorItem(i, false);
                        if(parent!=null)
                        {
                            var m = item.matches(root, count, parent, 2, isDesktop, false);
                            if (m.Length > 0)
                            {
                                newpathToRoot.Add(i);
                                parent = i;
                                isDesktop = false;
                            }
                            if (m.Length == 0 && Config.local.log_selector)
                            {
                                //var message = "needed to find " + Environment.NewLine + item.ToString() + Environment.NewLine + "but found only: " + Environment.NewLine;
                                //var children = parent.FindAllChildren();
                                //foreach (var c in children)
                                //{
                                //    try
                                //    {
                                //        message += new UIElement(c).ToString() + Environment.NewLine;
                                //    }
                                //    catch (Exception)
                                //    {
                                //    }
                                //}
                                //Log.Debug(message);
                            }
                        }
                    }
                }
                if (newpathToRoot.Count != pathToRoot.Count)
                {
                    Log.Information("Selector had " + pathToRoot.Count + " items to root, but traversing children only matched " + newpathToRoot.Count);
                    pathToRoot = newpathToRoot;
                }
                Log.Selector(string.Format("windowsselector::create traverse_selector_both_ways::end {0:mm\\:ss\\.fff}", sw.Elapsed));
            }
            if (pathToRoot.Count == 0)
            {
                Log.Error("Element has not parent, or is same as annchor");
                return;
            }
            baseElement = pathToRoot.First();
            element = pathToRoot.Last();
            Clear();
            Log.Selector(string.Format("windowsselector::remove anchor if needed::end {0:mm\\:ss\\.fff}", sw.Elapsed));
            if (anchor == null)
            {
                Log.Selector(string.Format("windowsselector::create root element::begin {0:mm\\:ss\\.fff}", sw.Elapsed));
                item = new WindowsSelectorItem(baseElement, true);
                item.Enabled = true;
                //item.canDisable = false;
                Items.Add(item);
                Log.Selector(string.Format("windowsselector::create root element::end {0:mm\\:ss\\.fff}", sw.Elapsed));
            }

            if (PluginConfig.search_descendants)
            {
                Log.Selector(string.Format("windowsselector::search_descendants::begin {0:mm\\:ss\\.fff}", sw.Elapsed));
                if (anchor == null)
                {
                    // Add window, we NEED to search from a window
                    item = new WindowsSelectorItem(pathToRoot[0], false, -1);
                    if (doEnum) item.EnumNeededProperties(pathToRoot[pathToRoot.Count - 1], pathToRoot[pathToRoot.Count - 1].Parent);
                    item.canDisable = false;
                    Items.Add(item);


                    var FrameworkId = item.Properties.Where(x => x.Name == "FrameworkId").FirstOrDefault();
                    if (FrameworkId != null && (FrameworkId.Value == "XAML" || FrameworkId.Value == "WinForm"))
                    {
                        var itemname = item.Properties.Where(x => x.Name == "Name").FirstOrDefault();
                        if (itemname != null) itemname.Enabled = false;
                    }
                }
                if (pathToRoot.Count > 2)
                {
                    item = new WindowsSelectorItem(pathToRoot[pathToRoot.Count - 2], false, -1);
                    if (doEnum) item.EnumNeededProperties(pathToRoot[pathToRoot.Count - 2], pathToRoot[pathToRoot.Count - 2].Parent);
                    Items.Add(item);
                }
                if (pathToRoot.Count > 1)
                {
                    int IndexInParent = -1;
                    if (pathToRoot[pathToRoot.Count - 1].Parent != null)
                    {
                        var c = pathToRoot[pathToRoot.Count - 1].Parent.FindAllChildren();
                        for (var x = 0; x < c.Count(); x++)
                        {
                            if (pathToRoot[pathToRoot.Count - 1].Equals(c[x])) IndexInParent = x;
                        }
                    }
                    item = new WindowsSelectorItem(pathToRoot[pathToRoot.Count - 1], false, IndexInParent);
                    if (doEnum) item.EnumNeededProperties(pathToRoot[pathToRoot.Count - 1], pathToRoot[pathToRoot.Count - 1].Parent);
                    Items.Add(item);
                }
                Log.Selector(string.Format("windowsselector::search_descendants::end {0:mm\\:ss\\.fff}", sw.Elapsed));
            }
            else
            {
                bool isStartmenu = false;
                for (var i = 0; i < pathToRoot.Count(); i++)
                {
                    Log.Selector(string.Format("windowsselector::search_descendants::loop element " + i + ":begin {0:mm\\:ss\\.fff}", sw.Elapsed));
                    var o = pathToRoot[i];
                    int IndexInParent = -1;
                    if (o.Parent != null && i > 0)
                    {
                        var c = o.Parent.FindAllChildren();
                        for (var x = 0; x < c.Count(); x++)
                        {
                            if (o.Equals(c[x])) IndexInParent = x;
                        }
                    }

                    item = new WindowsSelectorItem(o, false, IndexInParent);
                    var _IndexInParent = item.Properties.Where(x => x.Name == "IndexInParent").FirstOrDefault();
                    if (_IndexInParent != null) _IndexInParent.Enabled = false;
                    if (i == 0 || i == (pathToRoot.Count() - 1)) item.canDisable = false;
                    foreach (var p in item.Properties)
                    {
                        int idx = p.Value.IndexOf(".");
                        if (p.Name == "ClassName" && idx > -1)
                        {
                            var FrameworkId = item.Properties.Where(x => x.Name == "FrameworkId").FirstOrDefault();
                            //if (FrameworkId!=null && (FrameworkId.Value == "XAML" || FrameworkId.Value == "WinForm") && _IndexInParent != null)
                            //{
                            //    item.Properties.ForEach(x => x.Enabled = false);
                            //    _IndexInParent.Enabled = true;
                            //    p.Enabled = true;
                            //}
                            int idx2 = p.Value.IndexOf(".", idx + 1);
                            // if (idx2 > idx) p.Value = p.Value.Substring(0, idx2 + 1) + "*";
                            if (idx2 > idx && item.Properties.Count > 1)
                            {
                                p.Enabled = false;
                            }
                                
                        }
                        //if (p.Name == "ClassName" && p.Value.StartsWith("WindowsForms10")) p.Value = "WindowsForms10*";
                        if (p.Name == "ClassName" && p.Value.ToLower() == "shelldll_defview")
                        {
                            item.Enabled = false;
                        }
                        if (p.Name == "ClassName" && (p.Value.ToLower() == "dv2vontrolhost" || p.Value.ToLower() == "desktopprogramsmfu"))
                        {
                            isStartmenu = true;
                        }
                        //if (p.Name == "ClassName" && p.Value == "#32770")
                        //{
                        //    item.Enabled = false;
                        //}
                        if (p.Name == "ControlType" && p.Value == "ListItem" && isStartmenu)
                        {
                            p.Enabled = false;
                        }
                    }
                    var hassyslistview32 = item.Properties.Where(p => p.Name == "ClassName" && p.Value.ToLower() == "syslistview32").ToList();
                    if (hassyslistview32.Count > 0)
                    {
                        var hasControlType = item.Properties.Where(p => p.Name == "ControlType").ToList();
                        if (hasControlType.Count > 0) { hasControlType[0].Enabled = false; }
                    }

                    if (doEnum) item.EnumNeededProperties(o, o.Parent);
                    Items.Add(item);
                    Log.Selector(string.Format("windowsselector::search_descendants::loop element " + i + ":end {0:mm\\:ss\\.fff}", sw.Elapsed));
                }
            }
            pathToRoot.Reverse();
            if(anchor!=null)
            {
                var p = Items[0].Properties.Where(x => x.Name == "SearchDescendants").FirstOrDefault();
                if(p==null)
                {
                    Items[0].Properties.Add(new SelectorItemProperty("SearchDescendants", PluginConfig.search_descendants.ToString()));
                }                
            }
            Log.Selector(string.Format("windowsselector::end {0:mm\\:ss\\.fff}", sw.Elapsed));
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }
        public override IElement[] GetElements(IElement fromElement = null, int maxresults = 1)
        {
            return WindowsSelector.GetElementsWithuiSelector(this, fromElement, maxresults);
        }
        public static UIElement[] GetElementsWithuiSelector(WindowsSelector selector, IElement fromElement = null, int maxresults = 1)
        {
            TimeSpan timeout = TimeSpan.FromMilliseconds(5000);
            timeout = TimeSpan.FromMilliseconds(20000);
            //var midcounter = 1;
            //if (PluginConfig.allow_multiple_hits_mid_selector)
            //{
            //    midcounter = 10;
            //}
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            Log.Selector(string.Format("GetElementsWithuiSelector::begin {0:mm\\:ss\\.fff}", sw.Elapsed));

            UIElement _fromElement = fromElement as UIElement;
            var selectors = selector.Where(x => x.Enabled == true && x.Selector == null).ToList();

            var current = new List<UIElement>();
            var automation = AutomationUtil.getAutomation();

            var search_descendants = false;
            var p = selector[0].Properties.Where(x => x.Name == "SearchDescendants").FirstOrDefault();
            if (p != null) search_descendants = bool.Parse(p.Value);


            UIElement[] result = null;
            using (automation)
            {
                var _treeWalker = automation.TreeWalkerFactory.GetControlViewWalker();
                AutomationElement startfrom = null;
                if (_fromElement != null) startfrom = _fromElement.RawElement;
                Log.SelectorVerbose("automation.GetDesktop");
                bool isDesktop = false;
                if (startfrom == null)
                {
                    startfrom = automation.GetDesktop();
                    isDesktop = true;
                }
                current.Add(new UIElement(startfrom));
                for (var i = 0; i < selectors.Count; i++)
                {
                    var s = new WindowsSelectorItem(selectors[i]);
                    var elements = new List<UIElement>();
                    elements.AddRange(current);
                    current.Clear();
                    int count = 0;
                    foreach (var _element in elements)
                    {
                        count = maxresults;
                        //if (i == 0) count = midcounter;
                        //// if (i < selectors.Count) count = 500;
                        //if ((i + 1) < selectors.Count) count = 1;
                        if (i < (selectors.Count-1)) count = 500;
                        var matches = (s).matches(startfrom, i, _element.RawElement, count, isDesktop, search_descendants); // (i == 0 ? 1: maxresults)
                        var uimatches = new List<UIElement>();
                        foreach (var m in matches)
                        {
                            var ui = new UIElement(m);
                            uimatches.Add(ui);
                        }
                        current.AddRange(uimatches.ToArray());
                        if(sw.Elapsed > timeout)
                        {
                            Log.Selector(string.Format("GetElementsWithuiSelector::timed out {0:mm\\:ss\\.fff}", sw.Elapsed));
                            return new UIElement[] { };
                        }
                    }
                    if (i == (selectors.Count - 1)) result = current.ToArray();
                    Log.Selector(string.Format("Found " + current.Count + " hits for selector # " + i + " {0:mm\\:ss\\.fff}", sw.Elapsed));
                    if(i > 0 && elements.Count > 0 && current.Count == 0)
                    {
                        var message = "needed to find " + Environment.NewLine + selectors[i].ToString() + Environment.NewLine + "but found only: " + Environment.NewLine;
                        var children = elements[0].RawElement.FindAllChildren();
                        foreach (var c in children)
                        {
                            try
                            {
                                message += new UIElement(c).ToString() + Environment.NewLine;
                            }
                            catch (Exception)
                            {
                            }
                        }
                        Log.Selector(message);
                    }
                    if (i==0 && isDesktop && current.Count > 0)
                    {
                        if (current[0].RawElement.Patterns.Window.TryGetPattern(out var winPattern))
                        {
                            if (winPattern.WindowVisualState.Value == FlaUI.Core.Definitions.WindowVisualState.Minimized)
                            {
                                IntPtr handle = current[0].RawElement.Properties.NativeWindowHandle.Value;
                                winPattern.SetWindowVisualState(FlaUI.Core.Definitions.WindowVisualState.Normal);
                            }
                        }
                    }
                    isDesktop = false;
                }
            }
            if (result == null)
            {
                Log.Selector(string.Format("GetElementsWithuiSelector::ended with 0 results after {0:mm\\:ss\\.fff}", sw.Elapsed));
                return new UIElement[] { };
            }
            if (result.Count() > maxresults) result = result.Take(maxresults).ToArray();
            Log.Selector(string.Format("GetElementsWithuiSelector::ended with " + result.Length + " results after {0:mm\\:ss\\.fff}", sw.Elapsed));
            return result;
        }

    }
}
