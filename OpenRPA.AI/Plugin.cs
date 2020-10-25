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

namespace OpenRPA.Java
{
    public class Plugin : ObservableObject, IRecordPlugin
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "IDE1006")]
        public static treeelement[] _GetRootElements(Selector anchor)
        {
            var result = new List<treeelement>();
            
            
            return result.ToArray();
        }
        public treeelement[] GetRootElements(Selector anchor)
        {
            return Plugin._GetRootElements(anchor);
        }
        public Interfaces.Selector.Selector GetSelector(Selector anchor, Interfaces.Selector.treeelement item)
        {
           return null ;
        }
        public event Action<IRecordPlugin, IRecordEvent> OnUserAction;
        public event Action<IRecordPlugin, IRecordEvent> OnMouseMove
        {
            add { }
            remove { }
        }
        public string Name { get => "Java"; }
        // public string Status => (hook!=null && hook.jvms.Count>0 ? "online":"offline");
        private string _Status = "";
        public string Status { get => _Status; }
        private void SetStatus(string status)
        {
            _Status = status;
            NotifyPropertyChanged("Status");
        }
        private Views.RecordPluginView view;
        public System.Windows.Controls.UserControl editor
        {
            get
            {
                if (view == null)
                {
                    view = new Views.RecordPluginView();
                }
                return view;
            }
        }
        public void Start()
        {
           
        }
        public void Stop()
        {
            
        }
        private void Hook_OnJavaShutDown(int vmID)
        {
            Log.Information("JavaShutDown: " + vmID);
            NotifyPropertyChanged("Status");
        }
        //public JavaElement LastElement { get; set; }
        
        public bool ParseUserAction(ref IRecordEvent e)
        {
           
            return true;
        }
        public void Initialize(IOpenRPAClient client)
        {
            // Javahook.Instance.init();
            //try
            //{
            //    Javahook.Instance.init();
            //}
            //catch (Exception ex)
            //{
            //    Log.Error(ex.ToString());
            //}
            try
            {
                
                //Task.Run(() =>
                //{
                //});
                

                //GenericTools.RunUI(() =>
                //{
                //});

            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
            }

        }
       
        public IElement[] GetElementsWithSelector(Selector selector, IElement fromElement = null, int maxresults = 1)
        {
            
            return null;
        }
        public IElement LaunchBySelector(Selector selector, bool CheckRunning, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }
        public void CloseBySelector(Selector selector, TimeSpan timeout, bool Force)
        {
            throw new NotImplementedException();
        }
        public bool Match(SelectorItem item, IElement m)
        {
            return true;
        }
        public bool ParseMouseMoveAction(ref IRecordEvent e)
        {
            
            return true;
        }
    }
    public class GetElementResult : IBodyActivity
    {
        public GetElementResult()
        {
            
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
        public UIElement UIElement { get; set; }
        public IElement Element { get; set; }
        public Selector Selector { get; set; }
        public IBodyActivity a { get; set; }
        public bool SupportInput { get; set; }
        public bool SupportSelect { get; set; }
        public bool ClickHandled { get; set; }
        public bool SupportVirtualClick { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public MouseButton Button { get; set; }
    }

}
