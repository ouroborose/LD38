using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Advertisements;

public class BaseScreen : MonoBehaviour {

    public const float AUTO_ROTATE_SPEED = 30.0f;

    [SerializeField] protected bool m_skippable = true;
    [SerializeField] protected RectTransform m_titleImage;
    [SerializeField] protected TextMeshProUGUI m_playLabel;
    [SerializeField] protected TextMeshProUGUI m_extraText;
    [SerializeField] protected bool m_showAds = false;

    protected bool m_startPressed = false;

    protected Sequence m_lastSequence;

    public virtual void Show(bool instant = false)
    {
        gameObject.SetActive(true);


        m_startPressed = false;

        Main.Instance.CameraController.StartRotation();
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
            .Append(DOTween.To(() => m_titleImage.anchorMin, (x) => m_titleImage.anchorMin = x, Vector2.zero, 1.5f).SetEase(Ease.InOutSine))
            .Append(DOTween.To(() => m_playLabel.alpha, (x) => m_playLabel.alpha = x, 1.0f, 1.0f));

        if (m_extraText != null)
        {
            m_lastSequence.Append(DOTween.To(() => m_extraText.alpha, (x) => m_extraText.alpha = x, 1.0f, 1.0f));
        }
    }

    public void Update()
    {
        if(Input.anyKey)
        {
            OnStartPressed();
        }
        Main.Instance.CameraController.RotateCamera(Vector3.right * AUTO_ROTATE_SPEED * Time.deltaTime);
    }

    public virtual void Hide(bool instant = false)
    {
        if (m_lastSequence != null)
        {
            m_lastSequence.Kill(true);
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

            if(m_startPressed)
            {
                Main.Instance.CameraController.FinishRotation();
                Main.Instance.StartGame();
            }
            
            return;
        }

        m_lastSequence = DOTween.Sequence();

        if (m_extraText != null)
        {
            m_lastSequence.Append(DOTween.To(() => m_extraText.alpha, (x) => m_extraText.alpha = x, 0.0f, 1.0f));
        }

        m_lastSequence
            .Append(DOTween.To(() => m_playLabel.alpha, (x) => m_playLabel.alpha = x, 0.0f, 0.5f))
            .Append(DOTween.To(() => m_titleImage.anchorMin, (x) => m_titleImage.anchorMin = x, Vector2.up, 0.5f).SetEase(Ease.InBack))
            .AppendCallback(()=>
            {
                gameObject.SetActive(false);

                if (m_startPressed)
                {
                    if(m_showAds)
                    {
                        ShowAd();
                    }
                    else
                    {
                        StartGame();
                    }
                }
            });
    }

    protected void ShowAd()
    {
        if (Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
        else
        {
            StartGame();
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        StartGame();
    }

    protected void StartGame()
    {
        Main.Instance.CameraController.ResetRotation();
        Main.Instance.StartGame();
    }

    public void OnStartPressed()
    {
        if (m_startPressed)
        {
            return;
        }

        if(!m_skippable && m_playLabel.alpha < 0.5f)
        {
            return;
        }

        AudioManager.Instance.PlayConfirm();
        m_startPressed = true;
        Hide();
    }
}
