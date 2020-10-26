using OpenRPA.Interfaces;
using OpenRPA.Interfaces.Resources;
using System;
using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRPA.AI

{
    [System.ComponentModel.Designer(typeof(GetElementDesigner), typeof(System.ComponentModel.Design.IDesigner))]
    [System.Drawing.ToolboxBitmap(typeof(GetElement), "Resources.toolbox.gethtmlelement.png")]
    [System.Windows.Markup.ContentProperty("Body")]
    [LocalizedToolboxTooltip("activity_getelement_tooltip", typeof(strings))]
    
    [LocalizedDisplayName("activity_getelement", typeof(strings))]
    public class GetElement : NativeActivity, System.Activities.Presentation.IActivityTemplateFactory
    {
        //[RequiredArgument]
        //public InArgument<string> XPath { get; set; }

        [Browsable(false)]
        
        public InArgument<TimeSpan> Timeout { get; set; }
        public InArgument<int> MaxResults { get; set; }
        public InArgument<int> MinResults { get; set; }
        [RequiredArgument]
        public InArgument<string> Selector { get; set; }
        
        [LocalizedDisplayName("activity_waitforready", typeof(strings)), LocalizedDescription("activity_waitforready_help", typeof(strings))]
        public InArgument<bool> WaitForReady { get; set; }
        [Browsable(false)]
        public string Image { get; set; }
        
        public Activity LoopAction { get; set; }
        public GetElement()
        {
            MaxResults = 1;
            Timeout = new InArgument<TimeSpan>()
            {
                Expression = new Microsoft.VisualBasic.Activities.VisualBasicValue<TimeSpan>("TimeSpan.FromSeconds(3)")
            };
        }
        protected override void Execute(NativeActivityContext context)
        {
            var selector = Selector.Get(context);
            selector = OpenRPA.Interfaces.Selector.Selector.ReplaceVariables(selector, context.DataContext);
            
       }
        private void OnBodyComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {
            
        }
        private void LoopActionComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {
            Execute(context);
        }
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            
        }
        public Activity Create(System.Windows.DependencyObject target)
        {
            return null;
        }
        public new string DisplayName
        {
            get
            {
                var displayName = base.DisplayName;
                if (displayName == this.GetType().Name)
                {
                    var displayNameAttribute = this.GetType().GetCustomAttributes(typeof(DisplayNameAttribute), true).FirstOrDefault() as DisplayNameAttribute;
                    if (displayNameAttribute != null) displayName = displayNameAttribute.DisplayName;
                }
                return displayName;
            }
            set
            {
                base.DisplayName = value;
            }
        }
    }
}