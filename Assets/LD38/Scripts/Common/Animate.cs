using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate : MonoBehaviour {

    public float m_timeBetweenFrames = 1.0f;
    private int m_frame = 0;
    private Transform m_lastFrame;
    private float m_timer = 0.0f;

    protected void Awake()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        m_lastFrame = transform.GetChild(m_frame);
        m_lastFrame.gameObject.SetActive(true);
    }

    protected void Update()
    {
        m_timer += Time.deltaTime;
        if(m_timer > m_timeBetweenFrames)
        {
            m_timer -= m_timeBetweenFrames;
            ShowNextFrame();
        }
    }

    protected void ShowNextFrame()
    {
        m_lastFrame.gameObject.SetActive(false);
        m_frame = (m_frame + 1) % transform.childCount;
        m_lastFrame = transform.GetChild(m_frame);
        m_lastFrame.gameObject.SetActive(true);
    }
}
