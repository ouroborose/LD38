﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Main : Singleton<Main>
{
    public bool m_autoStart = false;

    [SerializeField] private CameraController m_camera;
    public CameraController CameraController { get { return m_camera; } }

    [SerializeField] private BasePlayer m_player;
    public BasePlayer Player { get { return m_player; } }

    [SerializeField] private World m_world;
    public World World { get { return m_world; } }

    public GameObject m_keyPrefab;

    [SerializeField] private BiomeData m_startingBiomeData;
    public int m_biomeGroupLevelSeparation = 10;
    [SerializeField] private BiomeGroupData[] m_biomeGroups;

    public float m_enemyHpScalingFactor = 1.15f;
    public float m_enemyAtkScalingFactor = 1.075f;

    public float m_playerHpScalingFactor = 1.125f;
    public float m_playerAtkScalingFactor = 1.05f;

    public int m_currentLevel { get; protected set; }

    [Header("Controls")]
    [SerializeField] private float m_mouseDragThreshold = 20;

    private Vector3 m_mouseStartPos;
    private Vector3 m_lastMousePos;
    private bool m_cameraDragStarted = false;
    private float m_playTime = 0.0f;

    public enum GameState
    {
        Title,
        GameStarted,
        GameOver,
    }

    public GameState m_currentGameState = GameState.Title;

    protected override void Awake()
    {
        base.Awake();

        Input.simulateMouseWithTouches = true;
        SceneManager.LoadScene(Strings.MAIN_UI_SCENE_NAME, LoadSceneMode.Additive);
    }

    protected void Start()
    {
        if(m_autoStart)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        Analytics.CustomEvent("GameStart");
        m_playTime = 0.0f;
        m_currentLevel = 0;
        m_player.m_displayName = string.Format("World {0}", m_currentLevel);
        EventManager.OnObjectChanged.Dispatch(m_player);
        WorldSide topSide = m_world.GetTopSide();

        m_player.SetTile(topSide.m_hiddenTile);
        m_player.gameObject.SetActive(true);
        m_player.Reset();
        topSide.m_hiddenTile.SetModel(m_startingBiomeData.m_tileModelPrefabs[UnityEngine.Random.Range(0, m_startingBiomeData.m_tileModelPrefabs.Length)]);
        topSide.Flip();

        DOVirtual.DelayedCall(World.WORLD_POPULATION_STEP_TIME, () =>
        {
            m_world.Populate(m_startingBiomeData);
            m_currentGameState = GameState.GameStarted;
        });

        EventManager.OnGameStart.Dispatch();
    }

    public BiomeGroupData GetCurrentBiomeGroup()
    {
        return m_biomeGroups[Mathf.Min(m_currentLevel/m_biomeGroupLevelSeparation, m_biomeGroups.Length-1)];
    }

    public void AdvanceToNextStage()
    {
        m_currentLevel++;
        m_player.m_displayName = string.Format("World {0}", m_currentLevel);
        EventManager.OnObjectChanged.Dispatch(m_player);
        BiomeGroupData currentGroup = GetCurrentBiomeGroup();
        m_world.Populate(currentGroup.m_biomes[UnityEngine.Random.Range(0, currentGroup. m_biomes.Length)]);
    }

    public int GetProgressionScaledValue(int value, float scaler, float levelOffset = 0, int level = -1)
    {
        return Mathf.CeilToInt(value * Main.Instance.GetProgressionScaler(scaler, levelOffset, level)) - value;
    }

    public float GetProgressionScaler(float baseScaler, float levelOffset = 0, int level = -1)
    {
        if(level < 0)
        {
            level = m_currentLevel;
        }
        return Mathf.Pow(baseScaler, Mathf.Max(0.0f, level - levelOffset));
    }

    public void GameOver()
    {
        m_currentGameState = GameState.GameOver;
        Analytics.CustomEvent("GameOver", new Dictionary<string, object>()
        {
            { "PlayTime", m_playTime },
            { "MaxHp", m_player.CalculateMaxHP() },
            { "Atk", m_player.CalculateAttackDamage() },
            { "Keys", m_player.m_numKeys },
            { "World", m_currentLevel }
        });
        EventManager.OnGameOver.Dispatch();
    }

    protected void Update()
    {
#if UNITY_EDITOR
        UpdateDebugControls();
#endif
        if (m_currentGameState != GameState.GameStarted)
        {
            return;
        }

        m_playTime += Time.deltaTime;

        UpdatePlayerKeyboardControls();
        UpdatePlayerMouseControls();
    }
    
    protected void UpdateDebugControls()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            AdvanceToNextStage();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            m_player.TakeDamage(m_player.CalculateMaxHP());
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            Vector3 dir = new Vector3(Random.Range(-0.75f,0.75f), 1.0f, 0);
            dir.Normalize();
            PopTextManager.Instance.Show("Test", m_player.transform.position + Vector3.up, dir * 5, Color.white);
        }
        
        if (Input.GetKeyDown(KeyCode.F4))
        {
            m_player.Heal(m_player.CalculateMaxHP());
            m_player.AddKey();
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
                WorldSide side = hit.collider.GetComponentInParent<WorldSide>();
                if (side != null && !side.Contains(m_player))
                {
                    side.Flip();
                }
            }
        }
    }

    protected void SelectFrontLeft()
    {
        Vector3 dir = -Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        dir.Normalize();
        dir -= Camera.main.transform.right;
        SelectWorld(dir);
    }

    protected void SelectFrontRight()
    {
        Vector3 dir = -Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        dir.Normalize();
        dir += Camera.main.transform.right;
        SelectWorld(dir);
    }

    protected void SelectBackLeft()
    {
        Vector3 dir = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        dir.Normalize();
        dir -= Camera.main.transform.right;
        SelectWorld(dir);
    }

    protected void SelectBackRight()
    {
        Vector3 dir = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        dir.Normalize();
        dir += Camera.main.transform.right;
        SelectWorld(dir);
    }

    protected void SelectWorld(Vector3 dir)
    {
        m_world.GetWorldByDirection(dir).OnClick();
    }

    protected void UpdatePlayerKeyboardControls()
    {
        if(Input.GetKeyDown(KeyCode.Comma) || Input.GetKeyDown(KeyCode.Z))
        {
            m_camera.RotateClockwise();
        }
        else if (Input.GetKeyDown(KeyCode.Period) || Input.GetKeyDown(KeyCode.X))
        {
            m_camera.RotateCounterClockwise();
        }

        if (PlayerInputIsBlocked())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            SelectFrontLeft();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            SelectFrontRight();
        }
    }

    protected void UpdatePlayerMouseControls()
    {           
        if(Input.GetMouseButtonDown(0))
        {
            m_mouseStartPos = Input.mousePosition;
            m_lastMousePos = m_mouseStartPos;
            m_cameraDragStarted = false;
        }
        else if(Input.GetMouseButton(0))
        {
            if(m_cameraDragStarted)
            {
                // do drag
                Vector3 mouseDelta = Input.mousePosition - m_lastMousePos;
                m_camera.RotateCamera(mouseDelta);
            }
            else if (Mathf.Abs(m_mouseStartPos.x - Input.mousePosition.x) > m_mouseDragThreshold)
            {
                m_cameraDragStarted = true;
                m_camera.StartRotation();
            }

        }
        else if(Input.GetMouseButtonUp(0))
        {
            if(m_cameraDragStarted)
            {
                // finish rotation
                m_camera.FinishRotation();
                m_cameraDragStarted = false;
            }
            else
            {
                if (PlayerInputIsBlocked())
                {
                    return;
                }

                // Handle click
                RaycastHit hit;
                if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
                {
                    IClickable clickable = hit.collider.GetComponentInParent<IClickable>();
                    if (clickable != null)
                    {
                        clickable.OnClick();
                    }
                }
            }
        }
        else if (m_cameraDragStarted)
        {
            // finish rotation
            m_camera.FinishRotation();
            m_cameraDragStarted = false;
        }
    }

    protected void LateUpdate()
    {
        m_lastMousePos = Input.mousePosition;
    }

    protected bool PlayerInputIsBlocked()
    {
        return m_player.m_isBusy || m_world.m_isBusy || m_player.m_currentHP <= 0 || m_currentGameState == GameState.Title || m_currentGameState == GameState.GameOver;
    }
}
