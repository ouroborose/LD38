using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;

public class ObjectInfoDisplayUI : MonoBehaviour {
    public const float SHOW_TIME = 0.25f;

    [SerializeField] private BaseObject m_target;
    [SerializeField] private TextMeshProUGUI m_displayText;
    protected bool m_isVisible = true;

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

    public void Hide(bool instant = false)
    {
        if(!m_isVisible)
        {
            return;
        }

        m_isVisible = false;
        if (instant)
        {
            m_displayText.alpha = 0.0f;
            return;
        }
        DOTween.To(() => m_displayText.alpha, (x) => m_displayText.alpha = x, 0.0f, SHOW_TIME);
    }

    public void Show(bool instant = false)
    {
        if(m_isVisible)
        {
            return;
        }

        m_isVisible = true;
        if (instant)
        {
            m_displayText.alpha = 1.0f;
            return;
        }
        DOTween.To(() => m_displayText.alpha, (x) => m_displayText.alpha = x, 1.0f, SHOW_TIME);
    }
}
