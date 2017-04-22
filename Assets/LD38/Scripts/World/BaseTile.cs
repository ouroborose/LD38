using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTile : BaseObject {
    public bool m_isEmpty { get { return m_obj == null; } }

    public BaseObject m_obj { get; protected set; }
    public WorldSide m_side { get; protected set; }

    public void Init(WorldSide side)
    {
        m_side = side;
    }
    
    public void SetObject(BaseObject obj)
    {
        m_obj = obj;
    }

    public void DestroyObj()
    {
        if(m_obj != null)
        {
            Destroy(m_obj.gameObject);
        }

        m_obj = null;
    }
}
