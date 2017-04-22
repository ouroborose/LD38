using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class World : MonoBehaviour {
    [SerializeField] private float m_rotationTime = 0.5f;

    [SerializeField] private WorldSide[] m_sides;
    public WorldSide[] Sides { get { return m_sides; } }

    public bool m_isRotating { get; private set; }
    public bool m_anySideFlipping
    {
        get
        {
            for(int i = 0; i < m_sides.Length; ++i)
            {
                if(m_sides[i].m_isAnimating)
                {
                    return true;
                }
            }
            return false;
        }
    }

    protected void Awake()
    {
        m_isRotating = false;

        for(int i = 0; i < m_sides.Length; ++i)
        {
            m_sides[i].Init(this);
        }
    }

    public void FlipAllEmptyTiles()
    {
        for(int i = 0; i < m_sides.Length; ++i)
        {
            if(m_sides[i].m_isEmpty)
            {
                m_sides[i].Flip();
            }
        }
    }

    public void RotateToSide(WorldSide side)
    {
        m_isRotating = true;
        transform.DORotateQuaternion(Quaternion.FromToRotation(Vector3.up, -side.transform.up) * transform.rotation, m_rotationTime).SetEase(Ease.InOutBack).OnComplete(OnRotateComplete);
        BasePlayer player = Main.Instance.Player;
        player.DetachFromTile();
        player.Jump(() => player.SetTile(side.m_showingTile, false));
    }

    private void OnRotateComplete()
    {
        m_isRotating = false;
    }
}
