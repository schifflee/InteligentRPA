﻿using OpenRPA.Interfaces;
using System;
using System.Activities;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenRPA
{
    public class WorkflowTrackingParticipant : TrackingParticipant
    {
        public delegate void VisualTrackingHandler(WorkflowInstance Instance, string ActivityId, string ChildActivityId, string State);
        public event VisualTrackingHandler OnVisualTracking;
        public string runactivityid { get; set; }
        public WorkflowTrackingParticipant()
        {
            TrackingProfile = new TrackingProfile()
            {
                Name = "CustomTrackingProfile",
                Queries =
                        {
                            new CustomTrackingQuery()
                            {
                                Name = "*",
                                ActivityName = "*"
                            },
                            new WorkflowInstanceQuery()
                            {
                                // Limit workflow instance tracking records for started and completed workflow states
                                // States = { WorkflowInstanceStates.Started, WorkflowInstanceStates.Completed },
                                // States = { "*" }
                            },
                            new ActivityStateQuery()
                            {
                                // Subscribe for track records from all activities for all states
                                ActivityName = "*",
                                States = { "*" },
                                // Extract workflow variables and arguments as a part of the activity tracking record
                                // VariableName = "*" allows for extraction of all variables in the scope
                                // of the activity
                                Variables = {"item" },
                                Arguments = { "*" }
                            },
                            new ActivityScheduledQuery()
                            {
                                ActivityName = "*",
                                ChildActivityName = "*",
                            }
                        }
            };
        }
        protected override void Track(TrackingRecord trackRecord, TimeSpan timeStamp)
        {
            try
            {
                string State = "unknown";
                Guid InstanceId = trackRecord.InstanceId;
                ActivityStateRecord activityStateRecord = trackRecord as ActivityStateRecord;
                ActivityScheduledRecord activityScheduledRecord = trackRecord as ActivityScheduledRecord;
                //if (activityStateRecord != null || activityScheduledRecord != null)
                if (activityStateRecord != null)
                {
                    var Instance = WorkflowInstance.Instances.Where(x => x.InstanceId == InstanceId.ToString()).FirstOrDefault();
                    foreach (var v in activityStateRecord.Variables)
                    {
                        if (Instance.Variables.ContainsKey(v.Key))
                        {
                            Instance.Variables[v.Key].value = v.Value;
                        }
                        else
                        {
                            if (v.Value != null)
                            {
                                Instance.Variables.Add(v.Key, new WorkflowInstanceValueType(v.Value.GetType(), v.Value));
                            }

                        }
                        //wfi.variables.Add(v.Key, v.Value);
                    }
                    // Log.Activity(activityStateRecord.State + " " + activityStateRecord.Activity.Id + " " + activityStateRecord.Activity.Name);
                }
                if (activityScheduledRecord != null)
                {
                    var Instance = WorkflowInstance.Instances.Where(x => x.InstanceId == InstanceId.ToString()).FirstOrDefault();
                    if (Instance == null || Instance.wfApp == null) return;
                    var wfApp = Instance.wfApp;
                    var executor = typeof(System.Activities.Hosting.WorkflowInstance).GetField("executor", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(wfApp);
                    var scheduler = executor.GetType().GetField("scheduler", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(executor);

                    string ActivityId = null;
                    string ChildActivityId = null;
                    if (activityStateRecord != null)
                    {
                        ActivityId = activityStateRecord.Activity.Id;
                        State = activityStateRecord.State.ToLower();
                    }
                    if (activityScheduledRecord != null)
                    {
                        State = "Scheduled";
                        if (activityScheduledRecord.Activity != null) ActivityId = activityScheduledRecord.Activity.Id;
                        if (activityScheduledRecord.Child != null) ChildActivityId = activityScheduledRecord.Child.Id;
                    }
                    if(activityScheduledRecord.Activity == null && activityScheduledRecord.Child!=null)
                    {
                        // this will make "1" be handles twice, but "1" is always sendt AFTER being scheduled, but we can catch it here ?
                        ActivityId = activityScheduledRecord.Child.Id;
                        ChildActivityId = activityScheduledRecord.Child.Id;
                    }
                    if (string.IsNullOrEmpty(ActivityId)) return;

                    if(activityScheduledRecord.Child.Id == "1.11")
                    {
                        // scheduler.GetType().GetMethod("ClearAllWorkItems", BindingFlags.Public | BindingFlags.Instance).Invoke(scheduler, new object[] { executor });
                        // scheduler.GetType().GetMethod("ScheduleWork", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(scheduler, new object[] { false });
                        //var firstWorkItem = scheduler.GetType().GetField("firstWorkItem", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(scheduler);
                        //firstWorkItem.GetType().GetMethod("Release", BindingFlags.Public | BindingFlags.Instance).Invoke(firstWorkItem, new object[] { executor });
                        //firstWorkItem.GetType().GetMethod("Dispose", BindingFlags.Public | BindingFlags.Instance).Invoke(firstWorkItem, new object[] { executor });

                        //scheduler.GetType().GetMethod("NotifyWorkCompletion", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(scheduler, new object[] { });
                    }
                    if (activityScheduledRecord.Activity != null)
                    {
                        // Log.Activity("Scheduled " + activityScheduledRecord.Activity.Id + " " + activityScheduledRecord.Activity.Name + " -> " + activityScheduledRecord.Child.Id + " " + activityScheduledRecord.Child.Name);
                    }
                    else if (activityScheduledRecord.Child != null)
                    {
                        // Log.Activity("Scheduled " + activityScheduledRecord.Child.Id + " " + activityScheduledRecord.Child.Name);
                    }

                    if (Instance.Variables == null) Instance.Variables = new Dictionary<string, WorkflowInstanceValueType>();
                    if (activityStateRecord != null)
                    {
                        foreach (var v in Instance.Variables.ToList())
                        {
                            if (!activityStateRecord.Variables.ContainsKey(v.Key)) Instance.Variables.Remove(v.Key);
                        }
                        foreach (var v in activityStateRecord.Variables)
                        {
                            if (Instance.Variables.ContainsKey(v.Key))
                            {
                                Instance.Variables[v.Key].value = v.Value;
                            }
                        }
                    }
                    var instanceMapField = executor.GetType().GetField("instanceMap", BindingFlags.NonPublic | BindingFlags.Instance);

                    // get SerializedProgramMapping to have InstanceMap get filled, needed by SerializedProgramMapping
                    var SerializedProgramMapping = executor.GetType().GetProperty("SerializedProgramMapping", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(executor);
                    ActivityInstance activityInstance = executor.GetType().GetField("rootInstance", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(executor) as ActivityInstance;

                    // Sometimes we can find the ActivityInstance in rootInstance
                    ActivityInstance result = findActivityInstance(executor, activityInstance, ActivityId);

                    // But more often, we find it in InstanceMapping
                    var instanceMap = instanceMapField.GetValue(executor);
                    if (instanceMap != null && result == null)
                    {
                        var _list = SerializedProgramMapping.GetType().GetProperty("InstanceMapping", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(SerializedProgramMapping);
                        foreach (System.Collections.DictionaryEntry kvp in (System.Collections.IDictionary)_list)
                        {
                            var a = kvp.Key as Activity;
                            if (a == null) continue;
                            if (result == null && a.Id == ActivityId)
                            {
                                result = findActivityInstance(kvp.Value, ActivityId);
                            }
                        }
                    }
                    if (result != null)
                    {
                        WorkflowDataContext context = null;
                        var cs = typeof(WorkflowDataContext).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
                        ConstructorInfo c = cs.First();

                        try
                        {
                            object o = c.Invoke(new Object[] { executor, result, true });
                            context = o as WorkflowDataContext;
                            var vars = context.GetProperties();
                            foreach (dynamic v in vars)
                            {
                                var value = v.GetValue(context);
                                if (Instance.Variables.ContainsKey(v.DisplayName))
                                {
                                    Instance.Variables[v.DisplayName] = new WorkflowInstanceValueType(v.PropertyType, value);
                                }
                                else
                                {
                                    Instance.Variables.Add(v.DisplayName, new WorkflowInstanceValueType(v.PropertyType, value));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Debug(ex.Message);
                        }
                    }

                    OnVisualTracking?.Invoke(Instance, ActivityId, ChildActivityId, State);
                }
                else
                {
                    // Log.Debug(trackRecord.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
        private ActivityInstance findActivityInstance(object executor, ActivityInstance activityInstance, string ActivityId)
        {
            object list = null;
            int list_count = 0;
            try
            {
                if (activityInstance.Activity.Id == ActivityId) return activityInstance;
                list = typeof(ActivityInstance).GetField("childList", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(activityInstance);
                if (list == null)
                {
                    return null;
                }
                list_count = (int)list.GetType().GetProperties().Where(x => x.Name == "Count").First().GetValue(list);
                if (list_count == 0)
                {
                    return null;
                }
                for (var i = 0; i < list_count; i++)
                {
                    var e = list.GetType().GetProperties().Where(x => x.Name == "Item").First().GetValue(list, new Object[] { i }) as ActivityInstance;
                    if (e.Activity.Id == ActivityId) return e;
                    var res = findActivityInstance(e, ActivityId);
                    if (res != null) return res;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
        private ActivityInstance findActivityInstance(object list, string ActivityId)
        {
            //int list_count = 0;
            try
            {
                //log(String.Format("found: {0}", activityInstance.Id));
                if (list == null)
                {
                    return null;
                }
                int list_count = 0;
                var pcount = list.GetType().GetProperties().Where(x => x.Name == "Count").FirstOrDefault();
                if (pcount != null)
                {
                    list_count = (int)pcount.GetValue(list);
                }
                if (list_count == 0)
                {
                    return null;
                }
                for (var i = 0; i < list_count; i++)
                {
                    var e = list.GetType().GetProperties().Where(x => x.Name == "Item").First().GetValue(list, new Object[] { i }) as ActivityInstance;
                    if (e != null)
                    {
                        if (e.Activity.Id == ActivityId) return e;
                        var res = findActivityInstance(e, ActivityId);
                        if (res != null) return res;
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
    }
}