﻿using OpenRPA.Input;
using OpenRPA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OpenRPA.Views
{
    /// <summary>
    /// Interaction logic for InsertText.xaml
    /// </summary>
    public partial class KeyboardSeqWindow : Window, System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
        public bool oneKeyOnly { get; set; }
        public string Text { get; set; }
        public KeyboardSeqWindow()
        {
            Log.FunctionIndent("KeyboardSeqWindow", "KeyboardSeqWindow");
            try
            {
                InitializeComponent();
                EventManager.RegisterClassHandler(typeof(Window), Window.LoadedEvent, new RoutedEventHandler(Window_Loaded));
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                DataContext = this;
                Activate();
                Focus();
                Topmost = true;
                Topmost = false;
                Focus();
                textbox.Focus();
                Topmost = true;
                typeText = new Activities.TypeText();
                InputDriver.Instance.OnKeyDown += OnKeyDown;
                InputDriver.Instance.OnKeyUp += OnKeyUp;
                InputDriver.Instance.CallNext = false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            Log.FunctionOutdent("KeyboardSeqWindow", "KeyboardSeqWindow");
        }
        private Activities.TypeText typeText = null;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Log.FunctionIndent("KeyboardSeqWindow", "Window_Loaded");
            try
            {
                var window = e.Source as Window;
                System.Threading.Thread.Sleep(100);
                window.Dispatcher.Invoke(
                new Action(() =>
                {
                    try
                    {
                        textbox.Focus();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.ToString());
                    }
                }));
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            Log.FunctionOutdent("KeyboardSeqWindow", "Window_Loaded");
        }
        private void OnKeyDown(Input.InputEventArgs e)
        {
            Log.FunctionIndent("KeyboardSeqWindow", "OnKeyDown");
            try
            {
                // if (downkeys == 0) typeText._keys.Clear();
                typeText.AddKey(new Interfaces.Input.vKey((FlaUI.Core.WindowsAPI.VirtualKeyShort)e.Key, false), null);
                Text = typeText.result;
                NotifyPropertyChanged("Text");
                if (oneKeyOnly)
                {
                    typeText.AddKey(new Interfaces.Input.vKey((FlaUI.Core.WindowsAPI.VirtualKeyShort)e.Key, true), null);
                    InputDriver.Instance.OnKeyDown -= OnKeyDown;
                    InputDriver.Instance.OnKeyUp -= OnKeyUp;
                    Text = typeText.result;
                    InputDriver.Instance.CallNext = true;
                    DialogResult = true;
                }
            }
            catch (Exception ex) 
            {
                Log.Error(ex.ToString());
            }
            Log.FunctionOutdent("KeyboardSeqWindow", "OnKeyDown");
        }
        private void OnKeyUp(Input.InputEventArgs e)
        {
            Log.FunctionIndent("KeyboardSeqWindow", "OnKeyUp");
            try
            {
                typeText.AddKey(new Interfaces.Input.vKey((FlaUI.Core.WindowsAPI.VirtualKeyShort)e.Key, true), null);
                Text = typeText.result;
                NotifyPropertyChanged("Text");
                if (typeText.keysdown == 0 && typeText._keys.Count > 0)
                {
                    InputDriver.Instance.OnKeyDown -= OnKeyDown;
                    InputDriver.Instance.OnKeyUp -= OnKeyUp;
                    InputDriver.Instance.CallNext = true;
                    DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            Log.FunctionOutdent("KeyboardSeqWindow", "OnKeyUp");
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Log.FunctionIndent("KeyboardSeqWindow", "Window_Unloaded");
            try
            {
                InputDriver.Instance.OnKeyDown -= OnKeyDown;
                InputDriver.Instance.OnKeyUp -= OnKeyUp;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            Log.FunctionOutdent("KeyboardSeqWindow", "Window_Unloaded");
        }
    }
}
