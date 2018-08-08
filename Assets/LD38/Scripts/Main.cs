using System.Collections;
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
    public int m_currentBiomeIndex { get; protected set; }

    [Header("Controls")]
    [SerializeField] private float m_mouseDragThreshold = 20;
    [SerializeField] private float m_touchDragThreshold = 1;

    private Vector3 m_mouseStartPos;
    private Vector3 m_lastMousePos;
    private bool m_cameraDragStarted = false;
    private float m_playTime = 0.0f;

    [SerializeField] private LocalSpaceTrailRenderer m_dragTrail;
    [SerializeField] private GameObject m_clickFeedbackPrefab;

    private bool m_hasAutoSavedSinceLastInput = true;

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
        if (m_autoStart)
        {
            StartGame();
        }
    }

    protected override void OnDestroy()
    {
        if (m_currentGameState == GameState.GameStarted)
        {
            Analytics.CustomEvent("GameClosed", new Dictionary<string, object>()
            {
                { "PlayTime", m_playTime },
                { "MaxHp", m_player.CalculateMaxHP() },
                { "Atk", m_player.CalculateAttackDamage() },
                { "Keys", m_player.m_numKeys },
                { "World", m_currentLevel }
            });
        }

        base.OnDestroy();
    }

    public void StartGame()
    {
        Analytics.CustomEvent("GameStart");
        ClearData();
        m_playTime = 0.0f;
        m_currentLevel = 0;

        WorldSide topSide = m_world.GetTopSide();
        m_player.SetTile(topSide.m_hiddenTile);
        m_player.gameObject.SetActive(true);
        m_player.Reset();
        m_player.m_displayName = string.Format("World {0}", m_currentLevel);
        EventManager.OnObjectChanged.Dispatch(m_player);

        topSide.m_hiddenTile.SetModel(m_startingBiomeData.m_tileModelPrefabs[UnityEngine.Random.Range(0, m_startingBiomeData.m_tileModelPrefabs.Length)]);
        topSide.Flip();

        DOVirtual.DelayedCall(World.WORLD_POPULATION_STEP_TIME, () =>
        {
            m_world.Populate(m_startingBiomeData);
            m_currentGameState = GameState.GameStarted;
        });

        EventManager.OnGameStart.Dispatch();
    }

    public void RevivePlayer()
    {
        Analytics.CustomEvent("GameContinued");
        WorldSide topSide = m_world.GetTopSide();
        m_player.SetTile(topSide.m_hiddenTile);
        m_player.gameObject.SetActive(true);
        m_player.Heal(m_player.CalculateMaxHP(), false);

        topSide.Flip();

        m_currentGameState = GameState.GameStarted;
        EventManager.OnGameStart.Dispatch();
    }

    public BiomeGroupData GetCurrentBiomeGroup()
    {
        return m_biomeGroups[Mathf.Min(m_currentLevel / m_biomeGroupLevelSeparation, m_biomeGroups.Length - 1)];
    }

    public void AdvanceToNextStage()
    {
        m_currentLevel++;
        m_player.m_displayName = string.Format("World {0}", m_currentLevel);
        EventManager.OnObjectChanged.Dispatch(m_player);
        
        BiomeGroupData currentGroup = GetCurrentBiomeGroup();
        m_currentBiomeIndex = UnityEngine.Random.Range(0, currentGroup.m_biomes.Length);
        m_world.Populate(currentGroup.m_biomes[m_currentBiomeIndex]);
    }

    public int GetProgressionScaledValue(int value, float scaler, float levelOffset = 0, int level = -1)
    {
        return Mathf.CeilToInt(value * Main.Instance.GetProgressionScaler(scaler, levelOffset, level)) - value;
    }

    public float GetProgressionScaler(float baseScaler, float levelOffset = 0, int level = -1)
    {
        if (level < 0)
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

#if UNITY_IOS && !UNITY_EDITOR
        UpdatePlayerTouchControls();
#else
        UpdatePlayerKeyboardControls();
        UpdatePlayerMouseControls();
#endif

        HandleAutoSave();
    }

    protected void HandleAutoSave()
    {
        // wait for nothing to be busy
        if(PlayerInputIsBlocked() || m_hasAutoSavedSinceLastInput)
        {
            return;
        }

        for(int i = 0; i < BaseObject.s_allObjects.Count; ++i)
        {
            if(BaseObject.s_allObjects[i].m_isBusy)
            {
                return;
            }
        }

        m_hasAutoSavedSinceLastInput = true;
        SaveData();
    }


    protected void UpdateDebugControls()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            AdvanceToNextStage();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            m_player.TakeDamage(m_player.CalculateMaxHP());
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            Vector3 dir = new Vector3(Random.Range(-0.75f, 0.75f), 1.0f, 0);
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
        if (Input.GetKeyDown(KeyCode.Comma) || Input.GetKeyDown(KeyCode.Z))
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
        if (Input.GetMouseButtonDown(0))
        {
            m_mouseStartPos = Input.mousePosition;
            m_lastMousePos = m_mouseStartPos;
            m_cameraDragStarted = false;
        }
        else if (Input.GetMouseButton(0))
        {
            if (m_cameraDragStarted)
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
        else if (Input.GetMouseButtonUp(0))
        {
            if (m_cameraDragStarted)
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
                HandleClick(Input.mousePosition);
            }
        }
    }

    protected void UpdatePlayerTouchControls()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            Vector3 touchPos = touch.position;
            if (touch.phase == TouchPhase.Began)
            {
                m_mouseStartPos = touchPos;
                m_lastMousePos = m_mouseStartPos;
                m_cameraDragStarted = false;
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                if (m_cameraDragStarted)
                {
                    // do drag
                    Vector3 mouseDelta = touch.deltaPosition;
                    //mouseDelta.x *= -1.0f;
                    m_camera.RotateCamera(mouseDelta);

                }
                else if (Mathf.Abs(m_mouseStartPos.x - touchPos.x) > m_touchDragThreshold)
                {
                    m_cameraDragStarted = true;
                    m_camera.StartRotation();
                }
                
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                if (m_cameraDragStarted)
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
                    HandleClick(touchPos);
                }

            }
        }
    }

    protected void HandleClick(Vector3 clickPos)
    {
        RaycastHit hit;
        Ray ray = m_camera.m_cam.ScreenPointToRay(clickPos);
        Vector3 feedbackPos = ray.GetPoint(m_camera.m_cam.transform.localPosition.magnitude);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            IClickable clickable = hit.collider.GetComponentInParent<IClickable>();
            if (clickable != null)
            {
                clickable.OnClick();
                m_hasAutoSavedSinceLastInput = false;
            }
            feedbackPos = hit.point;
        }

        DoClickFeedback(feedbackPos, -m_camera.m_cam.transform.forward);
    }

    protected void LateUpdate()
    {

#if UNITY_IOS && !UNITY_EDITOR
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            m_lastMousePos = touch.position;
        }
#else
        m_lastMousePos = Input.mousePosition;
#endif
        UpdateDragFeedBack();
    }

    protected void DoClickFeedback(Vector3 pos, Vector3 normal)
    {
        // spawn click vfx at click position
        GameObject clickVfx = Instantiate(m_clickFeedbackPrefab, pos,Quaternion.LookRotation(normal));
        clickVfx.transform.parent = m_camera.m_cam.transform;
        Destroy(clickVfx, clickVfx.GetComponent<ParticleSystem>().main.duration);
    }
    
    protected void UpdateDragFeedBack()
    {
        if(m_cameraDragStarted)
        {
            m_dragTrail.transform.position = m_camera.m_cam.ScreenPointToRay(m_lastMousePos).GetPoint(2.0f);
        }

        m_dragTrail.m_useSegmentLifeTime = !m_cameraDragStarted;
    }
    

    protected bool PlayerInputIsBlocked()
    {
        return m_player.m_isBusy || m_world.m_isBusy || m_player.m_currentHP <= 0 || m_currentGameState == GameState.Title || m_currentGameState == GameState.GameOver;
    }

    protected const string SAVE_KEY = "__SAVE__";

    public SaveData GenerateSaveData()
    {
        SaveData data = new SaveData();
        data.m_currentLevel = m_currentLevel;
        data.m_currentBiomeIndex = m_currentBiomeIndex;
        data.m_worldData = m_world.GenerateWorldData();
        data.m_playerData = m_player.GeneratePlayerData();
        return data;
    }

    [ContextMenu("SaveData")]
    public void SaveData()
    {
        SaveData data = GenerateSaveData();
        string saveDataString = data.GenerateSaveString();
        //Debug.Log("GameSaved");
        PlayerPrefs.SetString(SAVE_KEY, saveDataString);
    }

    [ContextMenu("ClearData")]
    public void ClearData()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
    }

    public bool HasSave()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    [ContextMenu("LoadData")]
    public void LoadData()
    {
        LoadGame(GetSaveData());
    }

    public SaveData GetSaveData()
    {
        if (HasSave())
        {
            string saveDataString = PlayerPrefs.GetString(SAVE_KEY, "");
            if (!string.IsNullOrEmpty(saveDataString))
            {
                //Debug.Log(saveDataString);
                return JsonUtility.FromJson<SaveData>(saveDataString);
            }
        }
        return null;
    }

    public void LoadGame(SaveData data)
    {
        if(data == null)
        {
            return;
        }

        Analytics.CustomEvent("GameLoaded");
        m_camera.ResetRotation();

        // restore level
        m_currentLevel = data.m_currentLevel;

        // restore player
        m_player.SetTile(m_world.Sides[data.m_worldData.m_topSideIndex].m_hiddenTile);
        m_player.gameObject.SetActive(true);
        m_player.Reset();
        m_player.LoadFromPlayerData(data.m_playerData);
        m_player.m_displayName = string.Format("World {0}", m_currentLevel);
        EventManager.OnObjectChanged.Dispatch(m_player);

        // restore world
        BiomeData worldBiomeData = m_startingBiomeData;
        if(m_currentLevel > 0)
        {
            BiomeGroupData biomeGroup = GetCurrentBiomeGroup();
            worldBiomeData = biomeGroup.m_biomes[data.m_currentBiomeIndex];
        }
        m_world.LoadFromData(data.m_worldData, worldBiomeData);
        
        // start game
        m_currentGameState = GameState.GameStarted;
        EventManager.OnGameStart.Dispatch();
    }
}
