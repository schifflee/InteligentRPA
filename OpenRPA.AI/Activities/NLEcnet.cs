using OpenRPA.Interfaces;
using OpenRPA.Image;
using System;
using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace OpenRPA.AI
{
    [System.Drawing.ToolboxBitmap(typeof(NLEcnet), "Resources.toolbox.getimage.png")]
    [System.ComponentModel.Designer(typeof(NLEcnetDesigner), typeof(System.ComponentModel.Design.IDesigner))]
    [System.Windows.Markup.ContentProperty("Body")]
    [LocalizedToolboxTooltip("activity_getidtext_tooltip", typeof(Resources.strings))]
    [LocalizedDisplayName("activity_nlecnet", typeof(Resources.strings))]
    public class NLEcnet : CodeActivity
    {
        public NLEcnet()
        {
        }
        [RequiredArgument]
        public InArgument<string> Text { get; set; }

        public OutArgument<string> CorrectText { get; set; }
        public OutArgument<double> Score { get; set; }
        
        
         protected override void Execute(CodeActivityContext context)
        {
            var text = Text.Get(context);
            
            string basepath = Interfaces.Extensions.DataDirectory;
            string path = System.IO.Path.Combine(basepath, "tessdata");

            // 百度OCR
            var _ocr = new Baidu.Aip.Nlp.Nlp("SYxNNxCZXW6TaarZS2a9oeiH", "MP8OclERWqaKfSrT77majSD9QQkOhN5z");
            _ocr.Timeout = 60000;//设置超时时间
            //_ocr.Init(path, lang.ToString(), Emgu.CV.OCR.OcrEngineMode.TesseractLstmCombined);
            //_ocr.PageSegMode = Emgu.CV.OCR.PageSegMode.SparseText;

            // OpenRPA.Interfaces.Image.Util.SaveImageStamped(ele.element, "OCR");
            Bitmap sourceimg = null;


            var result = _ocr.Ecnet(text);
            context.SetValue(CorrectText, result["item"]["correct_query"].ToString());
            context.SetValue(Score, Convert.ToDouble(result["item"]["score"]));

            //IEnumerator<ImageElement> _enum = result.ToList().GetEnumerator();
            //context.SetValue(_elements, _enum);
            //bool more = _enum.MoveNext();
            //if (more)
            //{
            //    context.ScheduleAction(Body, _enum.Current, OnBodyComplete);
            //}
        }
 
        [LocalizedDisplayName("activity_nlecnet", typeof(Resources.strings))]
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