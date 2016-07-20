using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading;

public class EventDispatcher
{

    private static EventDispatcher sinstance;
    public static EventDispatcher Instance
    { 
        get
        {
            if (sinstance == null)
            {
                sinstance = new EventDispatcher();
            }
            return sinstance;    
        }        
    }
    public static bool isEnuming = false;
    public delegate void EventCallback(EventBase eb);
    private Dictionary<string, List<EventCallback>> registedCallbacks = new Dictionary<string, List<EventCallback>>();
    private Dictionary<string, List<EventCallback>> registedCallbacksPending = new Dictionary<string, List<EventCallback>>();
    public void RegistEventListener(string sEventName, EventCallback eventCallback)
    {
        lock (this)
        {
            if (!registedCallbacks.ContainsKey(sEventName))
            {
                registedCallbacks.Add(sEventName, new List<EventCallback>());
            }
            if (isEnuming)
            {
                if (!registedCallbacksPending.ContainsKey(sEventName))
                {
                    registedCallbacksPending.Add(sEventName, new List<EventCallback>());
                }
                registedCallbacksPending[sEventName].Add(eventCallback);
                return;
            }
            registedCallbacks[sEventName].Add(eventCallback);
        }
    }
    public void RemoveEventListener(string sEventName, EventCallback eventCallback)
    {
        lock (this)
        {
            if (!registedCallbacks.ContainsKey(sEventName))
            {
                return;
            }
            if (isEnuming)
            {
                Debug.Log("Cannot unregist event this moment!");
                return;
            }
            registedCallbacks[sEventName].Remove(eventCallback);
        }
    }
    private List<EventBase> listEvents = new List<EventBase>();
    private List<EventBase> listPendingEvents = new List<EventBase>();
    public void DispatchEvent<T>(T eventInstance) where T : EventBase
    {
        lock (this)
        {
            if (!registedCallbacks.ContainsKey(eventInstance.eventName))
            {
                return;
            }
            if (isEnuming)
            {
                listPendingEvents.Add(eventInstance);
                Debug.Log("Cannot dispatch event this moment!");
                return;
            }
            foreach (EventBase eb in listPendingEvents)
            {
                listEvents.Add(eb);
            }
            listPendingEvents.Clear();
            listEvents.Add(eventInstance);
        }
    }
    public void DispatchEvent(string eventName, object eventValue)
    {
        lock (this)
        {
            if (!registedCallbacks.ContainsKey(eventName))
            {
                return;
            }
            if (isEnuming)
            {
                listPendingEvents.Add(new EventBase(eventName, eventValue));
                Debug.Log("Cannot dispatch event this moment!");
                return;
            }
            listEvents.Add(new EventBase(eventName, eventValue));
        }
    }

    public void OnTick()
    {
        lock (this)
        {
            //Debug.Log(listEvents.Count);
            if (listEvents.Count == 0)
            {
                foreach (string sEventName in registedCallbacksPending.Keys)
                {
                    foreach (EventCallback ec in registedCallbacksPending[sEventName])
                    {
                        RegistEventListener(sEventName, ec);
                    }
                }
                registedCallbacksPending.Clear();
                testPendingEvents();
                return;
            }
            isEnuming = true;
            foreach (EventBase eb in listEvents)
            {
                for (int i = 0; i < registedCallbacks[eb.eventName].Count; i++)// EventCallback ecb in registedCallbacks[eb.sEventName])
                {
                    EventCallback ecb = registedCallbacks[eb.eventName][i];
                    if (ecb == null)
                    {
                        continue;
                    }
                    ecb(eb);
                }
            }
            listEvents.Clear();
            //Debug.Log("Clear");
        }
        isEnuming = false;
    }

    private void testPendingEvents()
    {
        foreach (EventBase eb in listPendingEvents)
        {
            listEvents.Add(eb);
        }
        listPendingEvents.Clear();
    }
}
