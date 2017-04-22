using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : Singleton<Main> {

    [SerializeField] private BasePlayer m_player;
    public BasePlayer Player { get { return m_player; } }

    [SerializeField] private World m_world;
    [SerializeField] private CameraController m_camera;

    [Header("Controls")]
    [SerializeField] private float m_mouseDragThreshold = 20;

    private Vector3 m_mouseStartPos;
    private Vector3 m_lastMousePos;
    private bool m_cameraDragStarted = false;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.LoadScene(Strings.MAIN_UI_SCENE_NAME, LoadSceneMode.Additive);
    }

    protected void Update()
    {
        UpdatePlayerControls();
    }
    

    protected void UpdatePlayerControls()
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

        // Testing tile flip
        if(Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
                WorldSide side = hit.collider.GetComponentInParent<WorldSide>();
                if (side != null)
                {
                    side.Flip();
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
        return m_world.m_isRotating | m_player.m_isAnimating;
    }
}
