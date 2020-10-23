﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenRPA.NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace OpenRPA.SAP
{
    [Serializable]
    public class SAPEvent : PipeMessage
    {
        public string action { get; set; }
        public SAPEvent() : base()
        {
        }
        public SAPEvent(string action) : base()
        {
            this.action = action;
        }
        public string data { get; set; }
        public T Get<T>()
        {
            if (string.IsNullOrEmpty(data)) return default(T);
            return JsonConvert.DeserializeObject<T>(data);
        }
        public void Set(object o)
        {
            if (o == null) data = null; else data = JsonConvert.SerializeObject(o);
        }
    }
    [Serializable]
    public class SAPToogleRecordingEvent
    {
        public bool overlay { get; set; }
        public bool mousemove { get; set; }
    }
    [Serializable]
    public class SAPInvokeMethod
    {
        public SAPInvokeMethod() { }
        public SAPInvokeMethod(string Id, string ActionName)
        {
            this.Id = Id;
            this.ActionName = ActionName;
        }
        public SAPInvokeMethod(string SystemName, string Id, string ActionName, object[] Parameters)
        {
            this.SystemName = SystemName;
            this.Id = Id;
            this.ActionName = ActionName;
            //var _params = new List<SAPEventParameter>();
            //foreach(var p in Parameters)
            //{
            //    if(p!=null)
            //    {
            //        _params.Add(new SAPEventParameter() { Value = p, ValueType = p.GetType().FullName });
            //    } else
            //    {
            //        _params.Add(new SAPEventParameter() { Value = p, ValueType = typeof(object).FullName });
            //    }
            //}
            //this.Parameters = _params.ToArray();
            this.Parameters = JsonConvert.SerializeObject(Parameters);
        }
        // public SAPEventParameter[] Parameters { get; set; }
        public string Parameters { get; set; }
        public string SystemName { get; set; }
        public string ActionName { get; set; }
        public string Id { get; set; }
        public object Result { get; set; }
    }
    //[Serializable]
    //public class SAPEventParameter
    //{
    //    public string ValueType { get; set; }
    //    public object Value { get; set; }
    //}
    [Serializable]
    public class SAPRecordingEvent
    {
        //public SAPEventParameter[] Parameters { get; set; }
        public string Parameters { get; set; }
        public string Action { get; set; }
        public string ActionName { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string SystemName { get; set; }
        public int TypeAsNumber { get; set; }
        public bool ContainerType { get; set; }
        public string Id { get; set; }
    }
    [Serializable]
    public class SAPLoginEvent
    {
        public SAPLoginEvent(string Host, string Username, string Password, string Client, string Language, string SystemName) : base()
        {
            this.Host = Host;
            this.Username = Username;
            this.Password = Password;
            this.Client = Client;
            this.Language = Language;
            this.SystemName = SystemName;
        }
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Client { get; set; }
        public string Language { get; set; }
        public string SystemName { get; set; }
    }
    [Serializable]
    public class SAPGetSessions
    {
        public SAPConnection[] Connections;
    }
    [Serializable]
    public class SAPSessionInfo
    {
        public string ApplicationServer { get; set; }
        public string Client { get; set; }
        public int Codepage { get; set; }
        public int Flushes { get; set; }
        public string Group { get; set; }
        public int GuiCodepage { get; set; }
        public bool I18NMode { get; set; }
        public int InterpretationTime { get; set; }
        public bool IsLowSpeedConnection { get; set; }
        public string Language { get; set; }
        public string MessageServer { get; set; }
        public string Program { get; set; }
        public int ResponseTime { get; set; }
        public int RoundTrips { get; set; }
        public int ScreenNumber { get; set; }
        public bool ScriptingModeReadOnly { get; set; }
        public bool ScriptingModeRecordingDisabled { get; set; }
        public int SessionNumber { get; set; }
        public string SystemName { get; set; }
        public int SystemNumber { get; set; }
        public string SystemSessionId { get; set; }
        public string Transaction { get; set; }
        public string User { get; set; }
    }
    [Serializable]
    public class SAPWindow
    {
        public bool Changeable { get; set; }
        public int Handle { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public int ScreenLeft { get; set; }
        public int ScreenTop { get; set; }
        public bool Iconic { get; set; }
        public string IconName { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Tooltip { get; set; }
        public int WorkingPaneHeight { get; set; }
        public int WorkingPaneWidth { get; set; }
    }
    [Serializable]
    public class SAPSession
    {
        public bool Busy { get; set; }
        public string Id { get; set; }
        public SAPSessionInfo Info { get; set; }
        public bool IsActive { get; set; }
        public bool IsListBoxActive { get; set; }
        public string Name { get; set; }
        public int ProgressPercent { get; set; }
        public string ProgressText { get; set; }
        public bool Record { get; set; }
        public string RecordFile { get; set; }
        public bool SaveAsUnicode { get; set; }
        public bool ShowDropdownKeys { get; set; }
        public bool SuppressBackendPopups { get; set; }
        public int TestToolMode { get; set; }
        public SAPWindow ActiveWindow { get; set; }
    }
    [Serializable]
    public class SAPConnection
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string Description { get; set; }
        public bool DisabledByServer { get; set; }
        public SAPSession[] sessions { get; set; }
    }
    [Serializable]
    public class SAPElementProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsReadOnly { get; set; }
        public override string ToString()
        {
            if(IsReadOnly) return "*" + Name + " " + Value;
            return Name + " " + Value;
        }
    }
    [Serializable]
    public class SAPEventElement
    {
        public bool GetAllProperties { get; set; }
        public int MaxItem { get; set; }
        public int Skip { get; set; }
        public bool Flat { get; set; }
        public bool VisibleOnly { get; set; }
        public string Id { get; set; }
        public string Path { get; set; }
        public string Cell { get; set; }
        public string Name { get; set; }
        public string Parent { get; set; }
        public string SystemName { get; set; }
        public bool ContainerType { get; set; }
        public string type { get; set; }
        public SAPEventElement[] Children { get; set; }
        public SAPEventElement[] Items { get; set; }
        public SAPElementProperty[] Properties { get; set; }
        public override string ToString()
        {
            return Name + " " + Id;
        }
    }
}