using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BaseObject : MonoBehaviour {

    public static readonly List<BaseObject> s_allObjects = new List<BaseObject>();

    public string m_displayName;
    [SerializeField] protected Transform m_model;
    public Transform Model { get { return m_model; } }

    public bool m_isBusy { get { return m_busyCounter > 0 
                || (m_tile != null && (m_tile.m_isBusy || m_tile == m_tile.m_side.m_hiddenTile))
                || transform.parent == null; } }

    private int m_busyCounter = 0;
    protected Renderer[] m_renderers;
    public BaseTile m_tile { get; protected set; }

    public VuLib.BasePrefabIdentifier m_prefabIdentifier { get; protected set; }

    protected virtual void Awake()
    {
        s_allObjects.Add(this);
        m_busyCounter = 0;
        m_renderers = GetComponentsInChildren<Renderer>();
        m_prefabIdentifier = GetComponent<VuLib.BasePrefabIdentifier>();
    }

    protected virtual void OnDestroy()
    {
        s_allObjects.Remove(this);
        EventManager.OnObjectDestroyed.Dispatch(this);
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

    public void Shake(float magnitude, float duration)
    {
        Vector3 shakeDir = UnityEngine.Random.onUnitSphere;
        
        shakeDir.y = 0.0f;
        shakeDir.Normalize();
        IncrementBusyCounter();
        m_model.localPosition = Vector3.zero;
        m_model.DOShakePosition(duration, shakeDir * magnitude, 20).OnComplete(DecrementBusyCounter);
    }

    public virtual string CreateInfoText()
    {
        return m_displayName;
    }
    
    public virtual void DispatchChangedEvent()
    {
        EventManager.OnObjectChanged.Dispatch(this);
    }

    public void ShowPositivePopText(string text)
    {
        ShowPopText(text, Color.green, Vector3.up * 4.5f, 0.75f, 4.0f);
    }

    public void ShowNeutralPopText(string text)
    {
        ShowPopText(text, Color.white, Vector3.up * 4.5f, 0.75f, 4.0f);
    }

    public void ShowNegativePopText(string text)
    {
        Vector3 dir = new Vector3(Random.Range(-0.5f, 0.5f), 1.0f, 0);
        dir.Normalize();
        ShowPopText(text, Color.red, dir * 4);
    }

    public void ShowPopText(string text, Color color, Vector3 dir, float offset = 0.75f, float lifeTime = 2.0f)
    {
        PopTextManager.Instance.Show(text, transform.position + transform.up * offset, dir, color, lifeTime);
    }
}
