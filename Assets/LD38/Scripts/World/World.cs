using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class World : MonoBehaviour {
    [SerializeField] private float m_rotationTime = 0.5f;

    [SerializeField] private WorldSide[] m_sides;

    public bool m_isRotating { get; private set; }

    protected void Awake()
    {
        m_isRotating = false;

        for(int i = 0; i < m_sides.Length; ++i)
        {
            m_sides[i].Init(this);
        }
    }

    public void RotateToSide(WorldSide side)
    {
        m_isRotating = true;
        transform.DORotateQuaternion(Quaternion.FromToRotation(Vector3.up, -side.transform.up) * transform.rotation, m_rotationTime).SetEase(Ease.InOutBack).OnComplete(OnRotateComplete);
        Main.Instance.Player.Jump();
    }

    private void OnRotateComplete()
    {
        m_isRotating = false;
    }
}
