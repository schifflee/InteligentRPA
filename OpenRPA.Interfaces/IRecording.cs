﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRPA.Interfaces
{
    public interface IRecording
    {
        string Name { get; }
        void Initialize();
        void Start();
        void Stop();
        event Action<IRecording, IRecordEvent> OnUserAction;
        bool parseUserAction(ref IRecordEvent e);
        Selector.treeelement[] GetRootEelements();
    }
}
