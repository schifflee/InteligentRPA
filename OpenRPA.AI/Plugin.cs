using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using OpenRPA.Input;
using OpenRPA.Interfaces;
using OpenRPA.Interfaces.Selector;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OpenRPA.AI
{
    class Plugin : ObservableObject, IRecordPlugin
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "IDE1006")]
        public static treeelement[] _GetRootElements(Selector anchor)
        {
            return null;
        }
        public treeelement[] GetRootElements(Selector anchor)
        {
            return Plugin._GetRootElements(anchor);
        }
        public Interfaces.Selector.Selector GetSelector(Selector anchor, Interfaces.Selector.treeelement item)
        {
            return null;
        }
        public event Action<IRecordPlugin, IRecordEvent> OnUserAction;
        public event Action<IRecordPlugin, IRecordEvent> OnMouseMove
        {
            add { }
            remove { }
        }
        public string Name { get => "IE"; }
        public string Status => "";
        
        public System.Windows.Controls.UserControl editor
        {
            get
            {
                return null;
            }
        }
        public void Start()
        {
            OpenRPA.Input.InputDriver.Instance.CallNext = false;
            InputDriver.Instance.OnMouseUp += OnMouseUp;
        }
        public void Stop()
        {
            OpenRPA.Input.InputDriver.Instance.CallNext = true;
            InputDriver.Instance.OnMouseUp -= OnMouseUp;
        }
        private void ParseMouseUp(InputEventArgs e)
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
        private void OnMouseUp(InputEventArgs e)
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                ParseMouseUp(e);
            }));
            thread.IsBackground = true;
            thread.Start();
        }
        public bool ParseUserAction(ref IRecordEvent e) {
            
            return true;
        }
        public void Initialize(IOpenRPAClient client)
        {
            _ = PluginConfig.enable_xpath_support;
        }
        public IElement[] GetElementsWithSelector(Selector selector, IElement fromElement = null, int maxresults = 1)
        {
            return null;
        }
        public IElement LaunchBySelector(Selector selector, bool CheckRunning, TimeSpan timeout)
        {
            
            return null;
        }
        public void CloseBySelector(Selector selector, TimeSpan timeout, bool Force)
        {
            
        }
        public bool Match(SelectorItem item, IElement m)
        {
            return false;
        }
        public bool ParseMouseMoveAction(ref IRecordEvent e)
        {
            if (e.UIElement == null) return false;
            if (e.UIElement.ProcessId < 1) return false;
            var p = System.Diagnostics.Process.GetProcessById(e.UIElement.ProcessId);
            if (p.ProcessName != "iexplore" && p.ProcessName != "iexplore.exe") return false;
            return true;
        }
    }
    public class GetElementResult : IBodyActivity
    {
        public GetElementResult(GetElement activity)
        {
            Activity = activity;
        }
        public Activity Activity { get; set; }
        public void AddActivity(Activity a, string Name)
        {
            
        }
        public void AddInput(string value, IElement element)
        {
            try
            {
                AddActivity(new System.Activities.Statements.Assign<string>
                {
                    To = new Microsoft.VisualBasic.Activities.VisualBasicReference<string>("item.value"),
                    Value = value
                }, "item");
                element.Value = value;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
    public class RecordEvent : IRecordEvent
    {
        public RecordEvent() { SupportVirtualClick = true; }
        // public AutomationElement Element { get; set; }
        public UIElement UIElement { get; set; }
        public IElement Element { get; set; }
        public Interfaces.Selector.Selector Selector { get; set; }
        public IBodyActivity a { get; set; }
        public bool SupportInput { get; set; }
        public bool SupportSelect { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public bool ClickHandled { get; set; }
        public bool SupportVirtualClick { get; set; }
        public MouseButton Button { get; set; }
    }

}
