using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private int m_biomeGroupLevelSeparation = 10;
    [SerializeField] private BiomeGroupData[] m_biomeGroups;


    public int m_currentLevel { get; protected set; }

    [Header("Controls")]
    [SerializeField] private float m_mouseDragThreshold = 20;

    private Vector3 m_mouseStartPos;
    private Vector3 m_lastMousePos;
    private bool m_cameraDragStarted = false;
    
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
            m_currentGameState = GameState.GameStarted;
            m_world.Populate(m_startingBiomeData);
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

    public void GameOver()
    {
        m_currentGameState = GameState.GameOver;
        EventManager.OnGameOver.Dispatch();
    }

    protected void Update()
    {
#if UNITY_EDITOR
        UpdateDebugControls();
#endif
        UpdatePlayerControls();
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

    protected void UpdatePlayerControls()
    {

        if(m_currentGameState != GameState.GameStarted)
        {
            return;
        }

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
