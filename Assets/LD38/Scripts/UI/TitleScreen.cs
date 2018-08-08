using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleScreen : BaseScreen {
    public GameObject m_playButton;
    public GameObject m_continueButton;
    public TextMeshProUGUI m_continueWorldText;

    public GameObject m_newGameButton;
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

    public void OnStartGamePressed()
    {
        AudioManager.Instance.PlayConfirm();
        Hide(false, StartGame);
    }

    protected void StartGame()
    {
        Main.Instance.CameraController.ResetRotation();
        Main.Instance.StartGame();
    }

    public void OnLoadGamePressed()
    {
        AudioManager.Instance.PlayConfirm();
        Hide(false, LoadGame);
    }
    
    protected void LoadGame()
    {
        Main.Instance.CameraController.ResetRotation();
        Main.Instance.LoadGame(m_saveData);
    }
}
