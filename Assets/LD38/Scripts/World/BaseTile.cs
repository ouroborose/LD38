using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTile : BaseObject {
    public bool m_isEmpty { get { return m_objs.Count <= 0; } }
    
    public List<BaseObject> m_objs { get; protected set; }
    public WorldSide m_side { get; protected set; }

    public void Init(WorldSide side)
    {
        m_side = side;
        m_objs = new List<BaseObject>();
    }

    public void SetModel(GameObject modelPrefab)
    {
        if(m_model != null)
        {
            Destroy(m_model.gameObject);
        }

        GameObject modelObj = Instantiate(modelPrefab);
        m_model = modelObj.transform;
        m_model.parent = transform;
        m_model.localPosition = Vector3.zero;
        m_model.localRotation = Quaternion.identity;
    }

    public BaseObject GetFirstObject()
    {
        if(m_objs.Count > 0)
        {
            return m_objs[0];
        }
        return null;
    }
    
    public bool Contains(BaseObject obj)
    {
        for(int i = 0; i < m_objs.Count; ++i)
        {
            if(m_objs[i] == obj)
            {
                return true;
            }
        }

        return false;
    }

    public void AddObject(BaseObject obj)
    {
        m_objs.Add(obj);
    }

    public void RemoveObject(BaseObject obj)
    {
        m_objs.Remove(obj);
    }

    public void DestroyAllObjects()
    {
        for(int i = 0; i < m_objs.Count; ++i)
        {
            Destroy(m_objs[i].gameObject);
        }
        m_objs.Clear();
    }
}
