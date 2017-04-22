using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTile : BaseObject {
    public bool m_isEmpty { get { return m_obj == null; } }

    public BaseObject m_obj { get; protected set; }
    
    public void SetObject(BaseObject obj)
    {
        m_obj = obj;
    }
}
