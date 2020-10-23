﻿using OpenRPA.Interfaces;
using System.Activities.Presentation.Model;
using System.Windows;
using System.Windows.Media;

namespace OpenRPA.Office.Activities
{
    public partial class RunSlideShowDesigner
    {
        public RunSlideShowDesigner()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            openFileDialog1.Filter = "PowerPoint Presentation|*.pptx;*.ppt;*.pptm|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            ModelItem.Properties["Filename"].SetValue(
                new System.Activities.InArgument<string>()
                {
                    Expression = new Microsoft.VisualBasic.Activities.VisualBasicValue<string>("\"" + openFileDialog1.FileName.ReplaceEnvironmentVariable() + "\"")
                });
        }
    }
}