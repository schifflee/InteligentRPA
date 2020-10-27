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

namespace OpenRPA.AI
{
    [System.Drawing.ToolboxBitmap(typeof(GetIdText), "Resources.toolbox.getimage.png")]
    [System.ComponentModel.Designer(typeof(GetIdTextDesigner), typeof(System.ComponentModel.Design.IDesigner))]
    [System.Windows.Markup.ContentProperty("Body")]
    [LocalizedToolboxTooltip("activity_getidtext_tooltip", typeof(Resources.strings))]
    [LocalizedDisplayName("activity_getidtext", typeof(Resources.strings))]
    public class GetIdText : NativeActivity, System.Activities.Presentation.IActivityTemplateFactory
    {
        public GetIdText()
        {
            Element = new InArgument<IElement>()
            {
                Expression = new Microsoft.VisualBasic.Activities.VisualBasicValue<IElement>("item")
            };
        }
        public InArgument<string> WordLimit { get; set; }
        [RequiredArgument]
        public InArgument<bool> IsFrontSide { get; set; } = true;
        [RequiredArgument]
        public InArgument<IElement> Element { get; set; }
        [RequiredArgument]
        public InArgument<bool> CaseSensitive { get; set; } = false;
        public OutArgument<Newtonsoft.Json.Linq.JObject> Result { get; set; }
        [System.ComponentModel.Browsable(false)]
        public ActivityAction<ImageElement> Body { get; set; }
        private Variable<ImageElement[]> elements = new Variable<ImageElement[]>("elements");
        private Variable<IEnumerator<ImageElement>> _elements = new Variable<IEnumerator<ImageElement>>("_elements");
        public static Newtonsoft.Json.Linq.JObject  Execute(IElement ele, System.Activities.Presentation.Model.ModelItem model)
        {
            var wordlimit = model.GetValue<string>("WordLimit");
            var casesensitive = model.GetValue<bool>("CaseSensitive");
            var isfrontside = model.GetValue<bool>("IsFrontSide");
            var lang = Config.local.ocrlanguage;


            string basepath = Interfaces.Extensions.DataDirectory;
            

            ImageElement[] result = null;
            var _ocr = new Baidu.Aip.Ocr.Ocr(ocr.API_KEY, ocr.SECRET_KEY);
            _ocr.Timeout = 60000;//设置超时时间
            

            // OpenRPA.Interfaces.Image.Util.SaveImageStamped(ele.element, "OCR");
            Bitmap sourceimg = null;
            if (ele is ImageElement)
            {
                sourceimg = ((ImageElement)ele).element;
            }
            else
            {
                sourceimg = Interfaces.Image.Util.Screenshot(ele.Rectangle.X, ele.Rectangle.Y, ele.Rectangle.Width, ele.Rectangle.Height);
            }

            String idCardSide;
            if (isfrontside)
            {
                idCardSide = "front";
            } else
            {
                idCardSide = "back";
            }
            MemoryStream ms = new MemoryStream();
            sourceimg.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] imageBytes = ms.GetBuffer();  
            ms.Close();

            var word_results = _ocr.Idcard(imageBytes, idCardSide);
            
            Log.Debug("adding element cords to results: " + word_results);
            
            return word_results;
        }
        protected override void Execute(NativeActivityContext context)
        {
            // var match = Element.Get(context);
            var wordlimit = WordLimit.Get(context);
            var lang = Config.local.ocrlanguage;
            var casesensitive = CaseSensitive.Get(context);
            var isfrontside = IsFrontSide.Get(context);
            string basepath = Interfaces.Extensions.DataDirectory;
            string path = System.IO.Path.Combine(basepath, "tessdata");
            //ocr.TesseractDownloadLangFile(path, Config.local.ocrlanguage);
            //ocr.TesseractDownloadLangFile(path, "osd");
            var ele = Element.Get(context);
            // ele.element.Save(@"c:\temp\dump.png", System.Drawing.Imaging.ImageFormat.Png);

            // var result = ocr.GetIdTextcomponents(path, Config.local.ocrlanguage, ele.element);
            // var result = ocr.GetIdTextcomponents(path, Config.local.ocrlanguage, @"c:\temp\dump.png");

            ImageElement[] result;
            // 百度OCR
            var _ocr = new Baidu.Aip.Ocr.Ocr(ocr.API_KEY, ocr.SECRET_KEY);
            _ocr.Timeout = 60000;//设置超时时间
            //_ocr.Init(path, lang.ToString(), Emgu.CV.OCR.OcrEngineMode.TesseractLstmCombined);
            //_ocr.PageSegMode = Emgu.CV.OCR.PageSegMode.SparseText;

            // OpenRPA.Interfaces.Image.Util.SaveImageStamped(ele.element, "OCR");
            Bitmap sourceimg = null;
            if(ele is ImageElement)
            {
                // 传入的是图片
                sourceimg = ((ImageElement)ele).element;
            } else
            {
                // 传入非图片，开始截图
                sourceimg = Interfaces.Image.Util.Screenshot(ele.Rectangle.X, ele.Rectangle.Y, ele.Rectangle.Width, ele.Rectangle.Height);
            }
            String idCardSide;
            if (isfrontside)
            {
                idCardSide = "front";
            }
            else
            {
                idCardSide = "back";
            }
            MemoryStream ms = new MemoryStream();
            sourceimg.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] imageBytes = ms.GetBuffer();
            ms.Close();

            var word_results = _ocr.Idcard(imageBytes, idCardSide);
            context.SetValue(Result, word_results);

            //IEnumerator<ImageElement> _enum = result.ToList().GetEnumerator();
            //context.SetValue(_elements, _enum);
            //bool more = _enum.MoveNext();
            //if (more)
            //{
            //    context.ScheduleAction(Body, _enum.Current, OnBodyComplete);
            //}
        }
        private void OnBodyComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {
            IEnumerator<ImageElement> _enum = _elements.Get(context);
            bool more = _enum.MoveNext();
            if (more)
            {
                context.ScheduleAction<ImageElement>(Body, _enum.Current, OnBodyComplete);
            }
        }
        private void LoopActionComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {
            Execute(context);
        }
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            metadata.AddDelegate(Body);

            Interfaces.Extensions.AddCacheArgument(metadata, "Element", Element);
            Interfaces.Extensions.AddCacheArgument(metadata, "Result", Result);

            metadata.AddImplementationVariable(_elements);
            base.CacheMetadata(metadata);
        }
        public Activity Create(System.Windows.DependencyObject target)
        {
            var fef = new GetIdText();
            var aa = new ActivityAction<ImageElement>();
            var da = new DelegateInArgument<ImageElement>();
            da.Name = "item";
            fef.Body = aa;
            aa.Argument = da;
            return fef;
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