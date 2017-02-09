/* Copyright 2017 Cimpress

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License. */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace VP.FF.PT.Common.WpfInfrastructure.Extensions
{
    /// <summary>
    /// Provides miscellaneous helper methods for the <see cref="System.ComponentModel.EventHandlerList" /> class.
    /// </summary>
    public static class EventHelper
    {
        private static readonly Dictionary<Type, List<FieldInfo>> dicEventFieldInfos = new Dictionary<Type, List<FieldInfo>>();

        internal static BindingFlags AllBindings
        {
            get { return BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static; }
        }

        //--------------------------------------------------------------------------------
        private static IEnumerable<FieldInfo> GetTypeEventFields(Type t)
        {
            if (dicEventFieldInfos.ContainsKey(t))
            {
                return dicEventFieldInfos[t];
            }

            var lst = new List<FieldInfo>();
            BuildEventFields(t, lst);
            dicEventFieldInfos.Add(t, lst);
            return lst;
        }

        //--------------------------------------------------------------------------------
        private static void BuildEventFields(Type t, ICollection<FieldInfo> lst)
        {
            foreach (var ei in t.GetEvents(AllBindings))
            {
                var dt = ei.DeclaringType;
                if (dt != null)
                {
                    var fi = dt.GetField(ei.Name, AllBindings);
                    if (fi != null)
                    {
                        lst.Add(fi);
                    }
                }
            }
        }

        //--------------------------------------------------------------------------------
        private static EventHandlerList GetStaticEventHandlerList(Type t, object obj)
        {
            var mi = t.GetMethod("get_Events", AllBindings);
            return (EventHandlerList)mi.Invoke(obj, new object[] { });
        }

        //--------------------------------------------------------------------------------
        // public static void RemoveAllEventHandler(object obj) { RemoveAllEventHandler( obj, "" ); }

        //--------------------------------------------------------------------------------

        /// <summary>
        /// Removes all event handler with the given name.
        /// </summary>
        /// <param name="obj">The object with the hooked event handler.</param>
        /// <param name="eventName">The name of the event. If empty all event handler will be removed.</param>
        public static void RemoveAllEventHandler(object obj, string eventName = "")
        {
            if (obj == null)
            {
                return;
            }

            var t = obj.GetType();
            var event_fields = GetTypeEventFields(t);
            EventHandlerList static_event_handlers = null;

            foreach (var fi in event_fields)
            {
                if (eventName != string.Empty && string.Compare(eventName, fi.Name, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    continue;
                }

                if (fi.IsStatic)
                {
                    if (static_event_handlers == null)
                    {
                        static_event_handlers = GetStaticEventHandlerList(t, obj);
                    }

                    var idx = fi.GetValue(obj);
                    var eh = static_event_handlers[idx];
                    if (eh == null)
                    {
                        continue;
                    }

                    var dels = eh.GetInvocationList();
                    var ei = t.GetEvent(fi.Name, AllBindings);
                    foreach (var del in dels)
                    {
                        if (ei != null)
                        {
                            ei.RemoveEventHandler(obj, del);
                        }
                    }
                }
                else
                {
                    var ei = t.GetEvent(fi.Name, AllBindings);
                    if (ei == null)
                    {
                        continue;
                    }

                    var val = fi.GetValue(obj);
                    var mdel = val as Delegate;
                    if (mdel == null)
                    {
                        continue;
                    }

                    foreach (var del in mdel.GetInvocationList())
                    {
                        ei.RemoveEventHandler(obj, del);
                    }
                }
            }
        }
    }
}
