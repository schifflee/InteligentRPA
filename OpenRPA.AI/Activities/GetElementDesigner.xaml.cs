using Microsoft.VisualBasic.Activities;
using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OpenRPA.AI
{
    public partial class GetElementDesigner : INotifyPropertyChanged
    {
        public GetElementDesigner()
        {
            InitializeComponent();
            //HighlightImage = IEExtensions.GetImageSourceFromResource("search.png");
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public BitmapFrame HighlightImage { get; set; }
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void Open_Selector(object sender, RoutedEventArgs e)
        {
           
        }
        private void Highlight_Click(object sender, RoutedEventArgs e)
        {
            
        }
        public string ImageString
        {
            get
            {
                return null;
            }
        }
        public BitmapImage Image
        {
            get
            {
                var image = ImageString;
                System.Drawing.Bitmap b = Task.Run(() => {
                    return Interfaces.Image.Util.LoadBitmap(image);
                }).Result;
                using (b)
                {
                    if (b == null) return null;
                    return Interfaces.Image.Util.BitmapToImageSource(b, Interfaces.Image.Util.ActivityPreviewImageWidth, Interfaces.Image.Util.ActivityPreviewImageHeight);
                }
            }
        }
    }
}