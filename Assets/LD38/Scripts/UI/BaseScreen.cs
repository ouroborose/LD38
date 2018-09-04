using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Advertisements;

public class BaseScreen : MonoBehaviour {

    public const float AUTO_ROTATE_SPEED = 30.0f;

    public bool m_autoRotateCamera = true;

    protected bool m_startPressed = false;

    protected Sequence m_lastSequence;
    protected bool m_isHidden = false;

    public virtual void Show(bool instant = false, System.Action onComplete = null)
    {
        m_isHidden = false;
        gameObject.SetActive(true);
        Main.Instance.CameraController.StartRotation();

        if (m_lastSequence != null)
        {
            m_lastSequence.Kill(true);
            m_lastSequence = null;
        }

        if (instant)
        {
            transform.localPosition = Vector3.zero;
            if (onComplete != null)
            {
                onComplete.Invoke();
            }
            return;
        }

        transform.localPosition = Vector3.up * Screen.height * 2.0f;

        m_lastSequence = DOTween.Sequence();
        m_lastSequence.Append(transform.DOLocalMoveY(0.0f, 0.5f).SetEase(Ease.InOutSine));
        m_lastSequence.AppendCallback(()=> {
            if (onComplete != null)
            {
                onComplete.Invoke();
            }
        });
    }

    public void Update()
    {
        if(m_autoRotateCamera)
        {
            Main.Instance.CameraController.RotateCamera(Vector3.right * AUTO_ROTATE_SPEED * Time.deltaTime);
        }
    }

    public virtual void Hide(bool instant = false, System.Action onComplete = null)
    {
        if(m_isHidden)
        {
            return;
        }

        m_isHidden = true;
        if (m_lastSequence != null)
        {
            m_lastSequence.Kill(true);
            m_lastSequence = null;
        }

        if (instant)
        {
            gameObject.SetActive(false);
            transform.localPosition = Vector3.up * Screen.height * 2;

            if (onComplete != null)
            {
                onComplete.Invoke();
            }
            return;
        }

        m_lastSequence = DOTween.Sequence();
        m_lastSequence.Append(transform.DOLocalMoveY(Screen.height * 2, 0.5f).SetEase(Ease.InBack));
        m_lastSequence.AppendCallback(() =>
        {
            gameObject.SetActive(false);
            if (onComplete != null)
            {
                onComplete.Invoke();
            }
        });
    }
}
