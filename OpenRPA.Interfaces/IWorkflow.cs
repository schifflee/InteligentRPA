﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRPA.Interfaces
{
    public delegate void idleOrComplete(IWorkflowInstance sender, EventArgs e);
    public delegate void VisualTrackingHandler(IWorkflowInstance Instance, string ActivityId, string ChildActivityId, string State);
    public interface IWorkflow : INotifyPropertyChanged, IBase
    {
        long current_version { get; set;  }
        string queue { get; set; }
        string Xaml { get; set; }
        string projectid { get; set; }
        string RelativeFilename { get; }
        string IDOrRelativeFilename { get; }
        string FilePath { get; }
        string Filename { get; set; }
        bool Serializable { get; set; }
        IProject Project { get; set; }
        string ProjectAndName { get; set; }
        List<workflowparameter> Parameters { get; set; }
        IWorkflowInstance CreateInstance(Dictionary<string, object> Parameters, string queuename, string correlationId, idleOrComplete idleOrComplete, VisualTrackingHandler VisualTracking);
        Task Delete();
        string UniqueFilename();
        Task Save(bool UpdateImages);
        void SaveFile(string overridepath = null, bool exportImages = false);
        void RunPendingInstances();
        void ParseParameters();
    }
    public enum workflowparameterdirection
    {
        @in = 0,
        @out = 1,
        inout = 2,
    }
    public class workflowparameter
    {
        public string name { get; set; }
        public string type { get; set; }
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public workflowparameterdirection direction { get; set; }
    }

}
