using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventCallBack
{
    private Action m_callBack;
    public void Register(Action callback)
    {
        m_callBack += callback;
    }

    public void Unregister(Action callback)
    {
        m_callBack -= callback;
    }

    public void Dispatch()
    {
        if(m_callBack != null)
        {
            m_callBack();
        }
    }
}


public class EventCallBack<T1>
{
    private Action<T1> m_callBack;
    public void Register(Action<T1> callback)
    {
        m_callBack += callback;
    }

    public void Unregister(Action<T1> callback)
    {
        m_callBack -= callback;
    }

    public void Dispatch(T1 arg1)
    {
        if (m_callBack != null)
        {
            m_callBack(arg1);
        }
    }
}

public class EventCallBack<T1, T2>
{
    private Action<T1, T2> m_callBack;
    public void Register(Action<T1, T2> callback)
    {
        m_callBack += callback;
    }

    public void Unregister(Action<T1, T2> callback)
    {
        m_callBack -= callback;
    }

    public void Dispatch(T1 arg1, T2 arg2)
    {
        if (m_callBack != null)
        {
            m_callBack(arg1, arg2);
        }
    }
}

public class EventCallBack<T1, T2, T3>
{
    private Action<T1, T2, T3> m_callBack;
    public void Register(Action<T1, T2, T3> callback)
    {
        m_callBack += callback;
    }

    public void Unregister(Action<T1, T2, T3> callback)
    {
        m_callBack -= callback;
    }

    public void Dispatch(T1 arg1, T2 arg2, T3 arg3)
    {
        if (m_callBack != null)
        {
            m_callBack(arg1, arg2, arg3);
        }
    }
}

public class EventCallBack<T1,T2,T3,T4>
{
    private Action<T1, T2, T3, T4> m_callBack;
    public void Register(Action<T1, T2, T3, T4> callback)
    {
        m_callBack += callback;
    }

    public void Unregister(Action<T1, T2, T3, T4> callback)
    {
        m_callBack -= callback;
    }

    public void Dispatch(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (m_callBack != null)
        {
            m_callBack(arg1, arg2, arg3, arg4);
        }
    }
}