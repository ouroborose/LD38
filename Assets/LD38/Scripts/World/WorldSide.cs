using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WorldSide : MonoBehaviour, IClickable {
    private const float FLIP_POS = 0.5f;
    private const float FLIP_MOVE_TIME = 0.33f;
    private readonly static Vector3 FLIP_TO_BOTTOM_ROTATE_ANGLE = new Vector3(180,0,0);
    private const float FLIP_ROTATE_TIME = 0.5f;

    [SerializeField] private Transform m_pivot;
    [SerializeField] private Transform m_top;
    [SerializeField] private Transform m_bottom;

    private World m_world;
    private Transform m_showingSide;

    private bool m_isFlipping = false;
    private Sequence m_flipSequence;

    public void Init(World world)
    {
        m_world = world;
        m_showingSide = m_top;

        m_flipSequence = DOTween.Sequence();
        m_flipSequence.Append(m_pivot.DOLocalMoveY(FLIP_POS, FLIP_MOVE_TIME).SetEase(Ease.InOutBack))
            .Append(m_pivot.DOLocalRotate(FLIP_TO_BOTTOM_ROTATE_ANGLE, FLIP_ROTATE_TIME).SetEase(Ease.OutBack))
            .Append(m_pivot.DOLocalMoveY(0, FLIP_MOVE_TIME).SetEase(Ease.InOutBack))
            .AppendCallback(OnFlipToBottomComplete).Rewind();
    }

    public void OnClick()
    {
        if(m_isFlipping)
        {
            return;
        }

        BasePlayer player = Main.Instance.Player;
        if (Vector3.Dot(player.transform.up, transform.up) > 0.9f)
        {
            return;
        }

        if (Vector3.Dot(player.transform.forward, transform.up) > 0.9f)
        {
            // already facing
            DetermineAction();
        }
        else
        {
            player.FaceDir(transform.up, DetermineAction);
        }
    }

    protected void DetermineAction()
    {
        m_world.RotateToSide(this);
    }

    public void Flip()
    {
        if(m_isFlipping)
        {
            return;
        }

        m_isFlipping = true;
        m_flipSequence.Play();
    }

    protected void OnFlipToBottomComplete()
    {
        m_showingSide = (m_showingSide == m_top) ? m_bottom : m_top;
        m_flipSequence.Rewind();
       
        m_isFlipping = false;
    }
}
