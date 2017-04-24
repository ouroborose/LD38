using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class BaseScreen : MonoBehaviour {

    [SerializeField] private RectTransform m_titleImage;
    [SerializeField] private TextMeshProUGUI m_playLabel;
    [SerializeField] private TextMeshProUGUI m_extraText;

    protected bool m_startPressed = false;

    protected Sequence m_lastSequence;

    public virtual void Show(bool instant = false)
    {
        gameObject.SetActive(true);
        m_startPressed = false;
        if (instant)
        {
            m_playLabel.alpha = 1.0f;
            m_titleImage.anchorMin = Vector2.zero;
            if(m_extraText != null)
            {
                m_extraText.alpha = 1.0f;
            }
            return;
        }

        m_playLabel.alpha = 0;
        m_titleImage.anchorMin = Vector2.up;
        if (m_extraText != null)
        {
            m_extraText.alpha = 0.0f;
        }

        m_lastSequence = DOTween.Sequence()
            .Append(DOTween.To(() => m_titleImage.anchorMin, (x) => m_titleImage.anchorMin = x, Vector2.zero, 1.5f).SetEase(Ease.OutSine))
            .Append(DOTween.To(() => m_playLabel.alpha, (x) => m_playLabel.alpha = x, 1.0f, 1.0f));

        if (m_extraText != null)
        {
            m_lastSequence.Append(DOTween.To(() => m_extraText.alpha, (x) => m_extraText.alpha = x, 1.0f, 1.0f));
        }
    }

    public virtual void Hide(bool instant = false)
    {
        if (m_lastSequence != null)
        {
            m_lastSequence.Kill();
            m_lastSequence = null;
        }

        if(instant)
        {
            gameObject.SetActive(false);
            m_playLabel.alpha = 0.0f;
            m_titleImage.anchorMin = Vector2.up;
            if (m_extraText != null)
            {
                m_extraText.alpha = 0.0f;
            }
            return;
        }

        m_lastSequence = DOTween.Sequence().Append(DOTween.To(() => m_playLabel.alpha, (x) => m_playLabel.alpha = x, 0.0f, 0.5f));

        if (m_extraText != null)
        {
            m_lastSequence.Append(DOTween.To(() => m_extraText.alpha, (x) => m_extraText.alpha = x, 0.0f, 1.0f));
        }

        m_lastSequence.Append(DOTween.To(() => m_titleImage.anchorMin, (x) => m_titleImage.anchorMin = x, Vector2.up, 0.5f).SetEase(Ease.InBack))
            .AppendCallback(()=>
            {
                gameObject.SetActive(false);
                Main.Instance.StartGame();
            });

        if (m_extraText != null)
        {
            m_lastSequence.Append(DOTween.To(() => m_extraText.alpha, (x) => m_extraText.alpha = x, 1.0f, 1.0f));
        }
    }

    public void OnStartPressed()
    {
        if (m_startPressed)
        {
            return;
        }

        m_startPressed = true;
        Hide();
    }
}
