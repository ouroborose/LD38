using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour {
    [SerializeField] protected Transform m_model;

    public bool m_isBusy { get { return m_busyCounter > 0; } }

    private int m_busyCounter = 0;
    protected Renderer[] m_renderers;
    protected BaseTile m_tile;
    
    protected virtual void Awake()
    {
        m_busyCounter = 0;
        m_renderers = GetComponentsInChildren<Renderer>();
    }

    protected virtual void OnDestroy()
    {
        if(m_tile != null)
        {
            DetachFromTile();
        }
    }

    protected void IncrementBusyCounter()
    {
        m_busyCounter++;
    }

    protected void DecrementBusyCounter()
    {
        m_busyCounter--;
    }

    public virtual void SetColor(Color c)
    {
        for(int i = 0; i < m_renderers.Length; ++i)
        {
            m_renderers[i].material.color = c;
        }
    }

    public virtual void DetachFromTile()
    {
        if(m_tile != null)
        {
            m_tile.RemoveObject(this);
        }

        transform.parent = null;
    }

    public virtual void SetTile(BaseTile tile, bool rotateToTile = true, Vector3 localRotation = default(Vector3))
    {
        m_tile = tile;
        m_tile.AddObject(this);
        transform.parent = m_tile.transform;
        transform.localPosition = Vector3.zero;
        if(rotateToTile)
        {
            transform.localEulerAngles = localRotation;
        }
    }
}
