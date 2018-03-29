using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalSpaceTrailRenderer : MonoBehaviour {
    public float m_segmentDist = 0.01f;
    public float m_segmentLifeTime = 0.1f;
    public int m_maxLength = 25;

    [SerializeField] protected LineRenderer m_lineRenderer;
    protected Queue<Vector3> m_positions = new Queue<Vector3>();

    protected float m_segmentLifeTimer = 0.0f;
    protected Vector3 m_lastLocalPos;

    protected void FixedUpdate()
    {
        bool dirty = false;

        Vector3 localPos = transform.localPosition;
        if (Vector3.Distance(localPos, m_lastLocalPos) > m_segmentDist)
        {
            // add new segment
            if (m_positions.Count >= m_maxLength)
            {
                m_positions.Dequeue();
            }
            m_positions.Enqueue(localPos);
            m_lastLocalPos = localPos;
            dirty = true;
        }

        if (m_positions.Count > 0)
        {
            m_segmentLifeTimer += Time.deltaTime;
            if (m_segmentLifeTimer >= m_segmentLifeTime)
            {
                // remove segment
                m_positions.Dequeue();
                m_segmentLifeTimer = 0.0f;
                dirty = true;
            }
        }

        if (dirty)
        {
            m_lineRenderer.positionCount = m_positions.Count;
            m_lineRenderer.SetPositions(m_positions.ToArray());
        }
    }
}
