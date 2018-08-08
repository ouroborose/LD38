using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : BaseScreen {
    public GameObject m_playButton;
    public GameObject m_continueButton;
    public GameObject m_newGameButton;

    protected void Start()
    {
        bool hasSave = Main.Instance.HasSave();
        m_playButton.SetActive(!hasSave);
        m_continueButton.SetActive(hasSave);
        m_newGameButton.SetActive(hasSave);
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
        Main.Instance.LoadData();
    }
}
