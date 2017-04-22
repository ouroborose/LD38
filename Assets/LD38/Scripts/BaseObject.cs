using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour {
    [SerializeField] protected Transform m_model;

    public bool m_isAnimating { get; protected set; }

    protected Renderer[] m_renderers;
    protected BaseTile m_tile;
    
    protected virtual void Awake()
    {
        m_isAnimating = false;
        m_renderers = GetComponentsInChildren<Renderer>();
    }

    protected virtual void AnimationStarted()
    {
        m_isAnimating = true;
    }

    protected virtual void AnimationEnded()
    {
        m_isAnimating = false;
    }

    public virtual void SetColor(Color c)
    {
        for(int i = 0; i < m_renderers.Length; ++i)
        {
            m_renderers[i].material.color = c;
        }
    }

    public void DetachFromTile()
    {
        if(m_tile != null)
        {
            m_tile.SetObject(null);
        }

        transform.parent = null;
    }

    public void SetTile(BaseTile tile, bool rotateToTile = true, Vector3 localRotation = default(Vector3))
    {
        m_tile = tile;
        m_tile.SetObject(tile);
        transform.parent = m_tile.transform;
        transform.localPosition = Vector3.zero;
        if(rotateToTile)
        {
            transform.localEulerAngles = localRotation;
        }
    }
}
