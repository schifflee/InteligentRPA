﻿using OpenRPA.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpenRPA.Views
{
    /// <summary>
    /// Interaction logic for OpenProject.xaml
    /// </summary>
    public partial class OpenProject : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
        public DelegateCommand DockAsDocumentCommand = new DelegateCommand((e) => { }, (e) => false);
        public DelegateCommand AutoHideCommand { get; set; } = new DelegateCommand((e) => { }, (e) => false);
        public bool CanClose { get; set; } = false;
        public bool CanHide { get; set; } = false;
        public event Action<Workflow> onOpenWorkflow;
        public event Action<Project> onOpenProject;
        public event Action onSelectedItemChanged;
        //public System.Collections.ObjectModel.ObservableCollection<Project> Projects { get; set; }
        private MainWindow main = null;
        public ICommand PlayCommand { get { return new RelayCommand<object>(MainWindow.instance.OnPlay, MainWindow.instance.CanPlay); } }
        public ICommand ExportCommand { get { return new RelayCommand<object>(MainWindow.instance.OnExport, MainWindow.instance.CanExport); } }
        public ICommand RenameCommand { get { return new RelayCommand<object>(MainWindow.instance.OnRename, MainWindow.instance.CanRename); } }
        public ICommand DeleteCommand { get { return new RelayCommand<object>(MainWindow.instance.OnDelete2, MainWindow.instance.CanDelete); } }
        // public ICommand DeleteCommand { get { return new RelayCommand<object>(MainWindow.instance.OnDelete, MainWindow.instance.CanDelete); } }
        public ICommand CopyIDCommand { get { return new RelayCommand<object>(MainWindow.instance.OnCopyID, MainWindow.instance.CanCopyID); } }
        public ICommand CopyRelativeFilenameCommand { get { return new RelayCommand<object>(MainWindow.instance.OnCopyRelativeFilename, MainWindow.instance.CanCopyID); } }
        public ICommand DisableCachingCommand { get { return new RelayCommand<object>(OnDisableCaching, CanDisableCaching); } }
        internal bool CanDisableCaching(object _item)
        {
            try
            {
                if (!MainWindow.instance.IsConnected) return false;
                if (MainWindow.instance.SelectedContent is Views.OpenProject view)
                {
                    var val = view.listWorkflows.SelectedValue;
                    if (val == null) return false;
                    if (view.listWorkflows.SelectedValue is Project p) return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return false;
            }
        }
        internal async void OnDisableCaching(object _item)
        {
            if (MainWindow.instance.SelectedContent is Views.OpenProject view)
            {
                var val = view.listWorkflows.SelectedValue;
                if (val == null) return;
                if (view.listWorkflows.SelectedValue is Project project)
                {
                    await project.Save(false);
                }
            }
        }
        public System.Collections.ObjectModel.ObservableCollection<Project> Projects
        {
            get
            {
                return RobotInstance.instance.Projects;
            }
        }
        public OpenProject(MainWindow main)
        {
            Log.FunctionIndent("OpenProject", "OpenProject");
            try
            {
                InitializeComponent();
                this.main = main;
                DataContext = this;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            Log.FunctionOutdent("OpenProject", "OpenProject");
        }
        private string _FilterText = "";
        public string FilterText
        {
            get
            {
                return _FilterText;
            }
            set
            {
                _FilterText = value;
                var workflows = new List<string>();
                if (string.IsNullOrEmpty(_FilterText))
                {
                    foreach (var designer in RobotInstance.instance.Designers)
                    {
                        if (string.IsNullOrEmpty(designer.Workflow._id) && !string.IsNullOrEmpty(designer.Workflow.Filename))
                        {
                            workflows.Add(designer.Workflow.RelativeFilename);
                        }
                        else if (!string.IsNullOrEmpty(designer.Workflow._id))
                        {
                            workflows.Add(designer.Workflow._id);

                        }
                    }
                }
                foreach (var p in Projects)
                {
                    bool expand = false;
                    foreach(var _wf in p.Workflows)
                    {
                        if(_wf is Workflow wf)
                        {
                            if (string.IsNullOrEmpty(_FilterText))
                            {
                                wf.IsVisible = true;
                                if(workflows.Contains(wf.IDOrRelativeFilename)) expand = true;
                            }
                            else
                            {
                                wf.IsVisible = wf.name.ToLower().Contains(_FilterText);
                                if (wf.IsVisible) expand = true;
                            }
                        }
                    }
                    p.IsExpanded = expand;
                }
                NotifyPropertyChanged("FilterText");
            }
        }
        private void ListWorkflows_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Log.FunctionIndent("OpenProject", "ListWorkflows_MouseDoubleClick");
            try
            {
                if (listWorkflows.SelectedItem is Workflow f)
                {
                    onOpenWorkflow?.Invoke(f);
                    return;
                }
                //var p = (Project)listWorkflows.SelectedItem;
                //if (p == null) return;
                //onOpenProject?.Invoke(p);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            Log.FunctionOutdent("OpenProject", "ListWorkflows_MouseDoubleClick");
        }
        private async void ButtonEditXAML(object sender, RoutedEventArgs e)
        {
            Log.FunctionIndent("OpenProject", "ButtonEditXAML");
            try
            {
                if (listWorkflows.SelectedItem is Workflow workflow)
                {
                    try
                    {
                        var f = new EditXAML();
                        f.XAML = workflow.Xaml;
                        f.ShowDialog();
                        workflow.Xaml = f.XAML;
                        await workflow.Save(false);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.ToString());
                        System.Windows.MessageBox.Show("ButtonEditXAML: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            Log.FunctionOutdent("OpenProject", "ButtonEditXAML");
        }
        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            Log.FunctionIndent("OpenProject", "UserControl_KeyUp");
            try
            {
                if (e.Key == Key.F2)
                {
                    MainWindow.instance.OnRename(null);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            Log.FunctionOutdent("OpenProject", "UserControl_KeyUp");
        }
        public Workflow Workflow
        {
            get
            {
                if (listWorkflows.SelectedItem == null) return null;
                if (listWorkflows.SelectedItem is Project) return null;
                if (listWorkflows.SelectedItem is Workflow) return listWorkflows.SelectedItem as Workflow;
                return null;

            }
            set
            {
            }
        }
        public Project Project
        {
            get
            {
                if (listWorkflows.SelectedItem == null) return null;
                if (listWorkflows.SelectedItem is Project) return listWorkflows.SelectedItem as Project;
                if (listWorkflows.SelectedItem is Workflow wf) return wf.Project as Project;
                return null;
            }
            set
            {
            }
        }
        private void listWorkflows_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            NotifyPropertyChanged("Workflow");
            NotifyPropertyChanged("Project");
            NotifyPropertyChanged("IsWorkflowSelected");
            NotifyPropertyChanged("IsWorkflowOrProjectSelected");
            onSelectedItemChanged?.Invoke();
        }
        public bool IsWorkflowSelected
        {
            get
            {
                if (listWorkflows.SelectedItem == null) return false;
                if (listWorkflows.SelectedItem is Workflow wf) return true;
                return false;
            }
            set { }
        }
        public bool IsWorkflowOrProjectSelected
        {
            get
            {
                if (listWorkflows.SelectedItem == null) return false;
                if (listWorkflows.SelectedItem is Workflow wf) return true;
                if (listWorkflows.SelectedItem is Project p) return true;
                return false;
            }
            set { }
        }
        public bool IncludePrerelease { get; set; }
        private async void ButtonOpenPackageManager(object sender, RoutedEventArgs e)
        {
            Log.FunctionIndent("OpenProject", "ButtonOpenPackageManager");
            try
            {
                if (listWorkflows.SelectedItem is Project project)
                {
                    try
                    {
                        var f = new PackageManager(project);
                        if (RobotInstance.instance.Window is MainWindow main) f.Owner = main;
                        f.ShowDialog();
                        if (f.NeedsReload)
                        {
                            await project.InstallDependencies(true);
                            WFToolbox.Instance.InitializeActivitiesToolbox();
                        }                        
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.ToString());
                        System.Windows.MessageBox.Show("ButtonOpenPackageManager: " + ex.Message);
                    }
                }
                if (listWorkflows.SelectedItem is Workflow workflow)
                {
                    try
                    {
                        Project p = workflow.Project as Project;
                        var f = new PackageManager(p);
                        if(RobotInstance.instance.Window is MainWindow main) f.Owner = main;
                        f.ShowDialog();
                        if(f.NeedsReload)
                        {
                            await p.InstallDependencies(true);
                            WFToolbox.Instance.InitializeActivitiesToolbox();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.ToString());
                        System.Windows.MessageBox.Show("ButtonOpenPackageManager: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            Log.FunctionOutdent("OpenProject", "ButtonEditXAML");
        }
    }
}
