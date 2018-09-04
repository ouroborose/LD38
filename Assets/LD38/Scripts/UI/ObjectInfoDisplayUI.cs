using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInfoDisplayUI : BaseHudUI
{
    [SerializeField] private BaseObject m_target;

    protected void Awake()
    {
        EventManager.OnObjectChanged.Register(OnObjectChanged);
        EventManager.OnObjectDestroyed.Register(OnObjectDestroyed);
    }

    protected void OnDestroy()
    {
        EventManager.OnObjectChanged.Unregister(OnObjectChanged);
        EventManager.OnObjectDestroyed.Unregister(OnObjectDestroyed);
    }

    public override void Show(bool instant = false)
    {
        if(m_target == null)
        {
            return;
        }
        base.Show(instant);
    }

    public void SetTarget(BaseObject target)
    {
        m_target = target;
        UpdateDisplay();
    }

    protected void OnObjectDestroyed(BaseObject obj)
    {
        if(obj == m_target)
        {
            Hide();
        }
    }

    protected void OnObjectChanged(BaseObject obj)
    {
        if(obj == m_target)
        {
            UpdateDisplay();
        }
    }

    public void UpdateDisplay()
    {
        if(m_target != null)
        {
            m_displayText.text = m_target.CreateInfoText();
        }
        else
        {
            m_displayText.text = string.Empty;
        }
    }
}
