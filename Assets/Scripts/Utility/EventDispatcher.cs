using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventDictionaryBase<TId, TEvent> where TEvent : UnityEventBase, new ()
{
    private Dictionary<TId, TEvent> m_dictionary;

    public EventDictionaryBase ()
    {
        m_dictionary = new Dictionary<TId, TEvent> ();
    }

    ~EventDictionaryBase ()
    {
        foreach (var unityEvent in m_dictionary.Values)
        {
            unityEvent.RemoveAllListeners ();
        }

        m_dictionary.Clear ();
    }

    public TEvent GetEvent (TId id)
    {
        if (m_dictionary.ContainsKey (id) == false)
        {
            m_dictionary.Add (id, new TEvent ());
        }

        return m_dictionary[id];
    }
}

public class EventDictionary<TId> : EventDictionaryBase<TId, UnityEvent>
{

}

public class UnityEventOneParam<T0> : UnityEvent<T0>
{

}

public class EventDictionary<TId, TParam0> : EventDictionaryBase<TId, UnityEventOneParam<TParam0>>
{

}

public class UnityEventTwoParam<T0, T1> : UnityEvent<T0, T1>
{

}

public class EventDictionary<TId, TParam0, TParam1> : EventDictionaryBase<TId, UnityEventTwoParam<TParam0, TParam1>>
{

}

public class UnityEventThreeParam<T0, T1, T2> : UnityEvent<T0, T1, T2>
{

}

public class EventDictionary<TId, TParam0, TParam1, TParam2> : EventDictionaryBase<TId, UnityEventThreeParam<TParam0, TParam1, TParam2>>
{

}

public class UnityEventFourParam<T0, T1, T2, T3> : UnityEvent<T0, T1, T2, T3>
{

}

public class EventDictionary<TId, TParam0, TParam1, TParam2, TParam3> : EventDictionaryBase<TId, UnityEventFourParam<TParam0, TParam1, TParam2, TParam3>>
{

}

