using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using TMPro;

public class GameOverScreen : BaseScreen
{
    public const string GAME_OVER_TEXT = "World\n{0}\n\nGame\nOver";

    [SerializeField] private TextMeshProUGUI m_gameOverText;

    public override void Show(bool instant = false)
    {
        m_gameOverText.text = string.Format(GAME_OVER_TEXT, Main.Instance.m_currentLevel);
        base.Show(instant);
    }

    public void OnRestartPressed()
    {
        AudioManager.Instance.PlayConfirm();
        Hide(false, RestartGame);
    }

    protected void RestartGame()
    {
        Main.Instance.CameraController.ResetRotation();
        Main.Instance.StartGame();
    }

    public void OnContinuePressed()
    {
        AudioManager.Instance.PlayConfirm();
        Hide(false, ShowAd);
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
            ContinueGame();
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        ContinueGame();
    }

    protected void ContinueGame()
    {
        Main.Instance.CameraController.ResetRotation();
        Main.Instance.RevivePlayer();
    }
}
