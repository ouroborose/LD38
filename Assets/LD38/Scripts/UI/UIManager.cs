using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager> {
    [SerializeField] private ObjectInfoDisplayUI m_playerInfoUI;
    [SerializeField] private ObjectInfoDisplayUI m_leftObjectInfoUI;
    [SerializeField] private ObjectInfoDisplayUI m_rightObjectInfoUI;

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
        EventManager.OnSideFlipStarted.Register(OnSideFlipStarted);
        EventManager.OnSideFlipFinished.Register(OnSideFlipFinished);
        EventManager.OnItemSpawned.Register(OnItemSpawned);

        EventManager.OnLevelPopulationStarted.Register(OnLevelPopulationStarted);
        EventManager.OnLevelPopulationFinished.Register(OnLevelPopulationFinished);

        // setup player UI
        m_playerInfoUI.SetTarget(Main.Instance.Player);
        m_leftObjectInfoUI.SetTarget(null);
        m_leftObjectInfoUI.Hide(true);
        m_rightObjectInfoUI.SetTarget(null);
        m_rightObjectInfoUI.Hide(true);
    }

    protected override void OnDestroy()
    {
        EventManager.OnViewRotateStarted.Unregister(OnViewRotateStarted);
        EventManager.OnViewRotateFinished.Unregister(OnViewRotateFinished);
        EventManager.OnWorldRotationStarted.Unregister(OnWorldRoationStarted);
        EventManager.OnWorldRotationFinished.Unregister(OnWorldRotationFinished);
        EventManager.OnSideFlipStarted.Register(OnSideFlipStarted);
        EventManager.OnSideFlipFinished.Register(OnSideFlipFinished);
        EventManager.OnItemSpawned.Unregister(OnItemSpawned);

        EventManager.OnLevelPopulationStarted.Unregister(OnLevelPopulationStarted);
        EventManager.OnLevelPopulationFinished.Unregister(OnLevelPopulationFinished);
        base.OnDestroy();
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

    private void OnSideFlipStarted(WorldSide obj)
    {

    }

    private void OnSideFlipFinished(WorldSide obj)
    {
        ShowLeftRightInfo();
    }


    private void OnItemSpawned(BaseObject obj)
    {
        ShowLeftRightInfo();
    }

    protected void OnLevelPopulationStarted()
    {
    }

    protected void OnLevelPopulationFinished()
    {
    }

    protected void HideLeftRightInfo()
    {
        m_leftObjectInfoUI.Hide();
        m_rightObjectInfoUI.Hide();
    }

    protected void ShowLeftRightInfo()
    {
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
