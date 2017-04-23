using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private float m_rotationArc = 90.0f;
    [SerializeField] private float m_autoFinishTime = 0.25f;
    [SerializeField] private float m_transitionThreshold = 15.0f;

    private float m_rotation = 0.0f;
    private float m_rotationDelta = 0.0f;
    private float m_bestFinishRotation = 0.0f;
    public bool m_isFinishingRotation { get; private set; }

    protected void Awake()
    {
        m_isFinishingRotation = false;
    }

    public void StartRotation()
    {
        m_isFinishingRotation = false;
        m_rotationDelta = 0.0f;
    }

    public void RotateCamera(Vector3 mouseDelta)
    {
        m_rotationDelta += mouseDelta.x / Screen.width * m_rotationArc;
        m_rotation = m_bestFinishRotation + m_rotationDelta;
    }

    public void FinishRotation()
    {
        m_isFinishingRotation = true;
        if(Mathf.Abs(m_rotationDelta) > m_transitionThreshold)
        {
            m_bestFinishRotation += Mathf.Sign(m_rotationDelta) * 90.0f;
        }
        m_rotationDelta = 0.0f;
    }

    protected void LateUpdate()
    {
        if(m_isFinishingRotation)
        {
            m_rotation = Mathf.Lerp(m_rotation, m_bestFinishRotation, Time.deltaTime * (1.0f/m_autoFinishTime));
            if(Mathf.Abs(m_bestFinishRotation-m_rotation) < 0.1f)
            {
                m_rotation = m_bestFinishRotation;
                m_isFinishingRotation = false;
                EventManager.OnViewRotateFinished.Dispatch();
            }
        }
        transform.eulerAngles = new Vector3(0, m_rotation, 0);
    }
}
