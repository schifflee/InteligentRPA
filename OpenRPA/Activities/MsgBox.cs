using System;
using System.Activities;
using System.Windows;
using OpenRPA.Interfaces;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRPA.Interfaces.Input;

namespace OpenRPA.Activities
{
    [System.ComponentModel.Designer(typeof(MsgBoxDesigner), typeof(System.ComponentModel.Design.IDesigner))]
    [System.Drawing.ToolboxBitmap(typeof(ResFinder), "Resources.toolbox.typetext.png")]
    [LocalizedToolboxTooltip("activity_typetext_tooltip", typeof(Resources.strings))]
    [LocalizedDisplayName("activity_msgbox_text", typeof(Resources.strings))]
    public class MsgBox : CodeActivity
    {
       
        [RequiredArgument]
        public InArgument<string> Text { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            var text = Text.Get(context);
            MessageBox.Show(text);

        }

        [LocalizedDisplayName("activity_msgbox_text", typeof(Resources.strings))]
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