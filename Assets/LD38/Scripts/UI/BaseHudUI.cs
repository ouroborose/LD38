using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class BaseHudUI : MonoBehaviour {

    public const float SHOW_TIME = 0.25f;
    [SerializeField] protected TextMeshProUGUI m_displayText;
    protected bool m_isVisible = true;

    public virtual void Hide(bool instant = false)
    {
        if (!m_isVisible)
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

    public virtual void Show(bool instant = false)
    {
        if (m_isVisible)
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
