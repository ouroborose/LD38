using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager> {
    [SerializeField] private BaseScreen m_titleScreen;
    [SerializeField] private BaseScreen m_notificationScreen;
    [SerializeField] private BaseScreen m_gameOverScreen;
    [SerializeField] private BaseScreen m_menuScreen;
    [SerializeField] private BaseScreen m_fader;

    [SerializeField] private ObjectInfoDisplayUI m_playerInfoUI;
    [SerializeField] private ObjectInfoDisplayUI m_leftObjectInfoUI;
    [SerializeField] private ObjectInfoDisplayUI m_rightObjectInfoUI;
    [SerializeField] private BaseHudUI[] m_hudUI;

    protected override void Awake()
    {
        if(SceneManager.GetActiveScene().name != Strings.MAIN_SCENE_NAME)
        {
            SceneManager.LoadScene(Strings.MAIN_SCENE_NAME);
            return;
        }

        base.Awake();

        EventManager.OnViewRotateStarted.Register(OnViewRotateStarted);
        EventManager.OnViewRotateFinished.Register(OnViewRotateFinished);

        EventManager.OnWorldRotationStarted.Register(OnWorldRoationStarted);
        EventManager.OnWorldRotationFinished.Register(OnWorldRotationFinished);
        
        EventManager.OnSideFlipFinished.Register(OnSideFlipFinished);

        EventManager.OnItemSpawned.Register(OnItemSpawned);

        EventManager.OnGameStart.Register(OnGameStart);
        EventManager.OnGameOver.Register(OnGameOver);

        // setup player UI
        m_playerInfoUI.SetTarget(Main.Instance.Player);
        m_leftObjectInfoUI.SetTarget(null);
        m_rightObjectInfoUI.SetTarget(null);
        HideHUD(true);

        m_gameOverScreen.Hide(true);
        m_menuScreen.Hide(true);
        m_notificationScreen.Hide(true);
        m_fader.Show(true);
    }

    protected void Start()
    {
        if(!Main.Instance.m_autoStart)
        {
            m_fader.Hide(false, ()=>
            {
                m_titleScreen.Show();
            });
        }
    }

    protected override void OnDestroy()
    {
        EventManager.OnViewRotateStarted.Unregister(OnViewRotateStarted);
        EventManager.OnViewRotateFinished.Unregister(OnViewRotateFinished);

        EventManager.OnWorldRotationStarted.Unregister(OnWorldRoationStarted);
        EventManager.OnWorldRotationFinished.Unregister(OnWorldRotationFinished);
        
        EventManager.OnSideFlipFinished.Register(OnSideFlipFinished);

        EventManager.OnItemSpawned.Unregister(OnItemSpawned);

        EventManager.OnGameStart.Unregister(OnGameStart);
        EventManager.OnGameOver.Unregister(OnGameOver);
        base.OnDestroy();
    }

    protected void OnGameStart()
    {
        ShowHUD();
    }

    protected void OnGameOver()
    {
        HideHUD();

        m_gameOverScreen.Show();
    }

    protected void OnViewRotateStarted()
    {
        HideLeftRightInfo();
    }

    protected void OnViewRotateFinished()
    {
        ShowLeftRightInfo();
    }

    protected void OnWorldRoationStarted()
    {
        HideLeftRightInfo();
    }

    protected void OnWorldRotationFinished()
    {
        ShowLeftRightInfo();
    }

    private void OnSideFlipFinished(WorldSide obj)
    {
        ShowLeftRightInfo();
    }


    private void OnItemSpawned(BaseObject obj)
    {
        ShowLeftRightInfo();
    }

    public void ShowMenu()
    {
        m_menuScreen.Show();
        HideHUD();
    }

    public void ShowHUD(bool instant = false)
    {
        for(int i = 0; i < m_hudUI.Length; ++i)
        {
            m_hudUI[i].Show(instant);
        }
    }

    public void HideHUD(bool instant = false)
    {
        for (int i = 0; i < m_hudUI.Length; ++i)
        {
            m_hudUI[i].Hide(instant);
        }
    }

    protected void HideLeftRightInfo()
    {
        m_leftObjectInfoUI.Hide();
        m_rightObjectInfoUI.Hide();
    }

    protected void ShowLeftRightInfo()
    {
        if(Main.Instance.m_currentGameState != Main.GameState.GameStarted)
        {
            return;
        }

        Vector3 back = -Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        back.Normalize();
        Vector3 left = back - Camera.main.transform.right;
        left.Normalize();
        Vector3 right = back + Camera.main.transform.right;
        right.Normalize();

        BaseObject leftObj = null;
        BaseObject rightObj = null;

        for (int i = 0; i < Main.Instance.World.Sides.Length; ++i)
        {
            WorldSide side = Main.Instance.World.Sides[i];
            if (leftObj == null && Vector3.Dot(side.transform.up, left) > 0.5f)
            {
                leftObj = side.m_showingTile.GetFirstObject();
            }

            if (rightObj == null && Vector3.Dot(side.transform.up, right) > 0.5f)
            {
                rightObj = side.m_showingTile.GetFirstObject();
            }
        }

        if(leftObj != null)
        {
            m_leftObjectInfoUI.SetTarget(leftObj);
            m_leftObjectInfoUI.Show();
        }
        else
        {
            m_leftObjectInfoUI.Hide();
        }

        if(rightObj != null)
        {
            m_rightObjectInfoUI.SetTarget(rightObj);
            m_rightObjectInfoUI.Show();
        }
        else
        {
            m_rightObjectInfoUI.Hide();
        }
    }
}
