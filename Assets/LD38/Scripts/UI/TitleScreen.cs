using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleScreen : BaseScreen {
    public GameObject m_playButton;
    public GameObject m_continueButton;
    public TextMeshProUGUI m_continueWorldText;

    public GameObject m_newGameButton;

    public NotificationScreen m_notificationScreen;

    protected SaveData m_saveData;

    protected void Start()
    {
        bool hasSave = Main.Instance.HasSave();
        m_playButton.SetActive(!hasSave);

        m_continueButton.SetActive(hasSave);
        m_newGameButton.SetActive(hasSave);

        if(hasSave)
        {
            m_saveData = Main.Instance.GetSaveData();
            if (m_saveData != null)
            {
                m_continueWorldText.text = string.Format("( World {0} )", m_saveData.m_currentLevel);
            }
        }
    }

    protected void StartGame()
    {
        Main.Instance.CameraController.ResetRotation();
        Main.Instance.StartGame();
    }

    public void OnStartGamePressed()
    {
        AudioManager.Instance.PlayConfirm();
        Hide(false, StartGame);
    }

    public void OnLoadGamePressed()
    {
        if (!m_notificationScreen.m_isHidden)
        {
            return;
        }

        AudioManager.Instance.PlayConfirm();
        Hide(false, LoadGame);
    }

    public void OnNewGamePressed()
    {
        if(!m_notificationScreen.m_isHidden)
        {
            return;
        }

        Hide();
        m_notificationScreen.m_autoRotateCamera = true;
        m_notificationScreen.ShowNotification("Starting a new game will delete your current save.\n\nStart new game?", "Yes", StartGame, "No", ()=> { Show(); });
    }
    
    protected void LoadGame()
    {
        Main.Instance.CameraController.ResetRotation();
        Main.Instance.LoadGame(m_saveData);
    }
}
