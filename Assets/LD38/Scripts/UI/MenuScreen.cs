using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScreen : BaseScreen {
    [SerializeField] protected BaseScreen m_Fader;

    protected bool m_isQuitting = false;

    public override void Show(bool instant = false, System.Action onComplete = null)
    {
        EventManager.OnMenuOpen.Dispatch();
        base.Show(instant, onComplete);
    }

    public override void Hide(bool instant = false, Action onComplete = null)
    {
        onComplete += EventManager.OnMenuClose.Dispatch;
        base.Hide(instant, onComplete);
    }

    public void OnQuitPressed()
    {
        if(m_isQuitting)
        {
            return;
        }

        m_isQuitting = true;
        m_Fader.Show(false, () =>
        {
            // restart game
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        });
    }

    public void OnResumePressed()
    {
        if (m_isQuitting)
        {
            return;
        }

        Hide();
    }
}
