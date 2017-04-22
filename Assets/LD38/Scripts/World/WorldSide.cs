using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WorldSide : BaseObject, IClickable {
    public const float FLIP_POS = 1.0f;
    public const float FLIP_MOVE_TIME = 0.33f;
    public readonly static Vector3 FLIP_TO_BOTTOM_ROTATE_ANGLE = new Vector3(180,0,0);
    public const float FLIP_ROTATE_TIME = 0.5f;

    [SerializeField] private Transform m_pivot;
    [SerializeField] private BaseTile m_topTile;
    [SerializeField] private BaseTile m_bottomTile;

    public BaseTile m_showingTile { get; protected set; }
    public BaseTile m_hiddenTile { get { return (m_showingTile == m_topTile) ? m_bottomTile : m_topTile; } }

    public bool m_isEmpty { get { return m_showingTile == null || m_showingTile.m_isEmpty; } }


    private World m_world;
    private Sequence m_flipSequence;

    public void Init(World world)
    {
        m_world = world;
        m_showingTile = m_topTile;

        m_topTile.SetColor(new Color(0, UnityEngine.Random.Range(0.25f, 1.0f), UnityEngine.Random.Range(0.1f, 0.75f)));
        m_bottomTile.SetColor(new Color(0, UnityEngine.Random.Range(0.25f, 1.0f), UnityEngine.Random.Range(0.1f, 0.75f)));

        m_flipSequence = DOTween.Sequence();
        m_flipSequence.Append(m_pivot.DOLocalMoveY(FLIP_POS, FLIP_MOVE_TIME).SetEase(Ease.InOutBack))
            .Append(m_pivot.DOLocalRotate(FLIP_TO_BOTTOM_ROTATE_ANGLE, FLIP_ROTATE_TIME).SetEase(Ease.OutBack))
            .Append(m_pivot.DOLocalMoveY(0, FLIP_MOVE_TIME).SetEase(Ease.InOutBack))
            .AppendCallback(OnFlipComplete).Rewind();
    }

    public void OnClick()
    {
        if(m_isAnimating)
        {
            return;
        }

        BasePlayer player = Main.Instance.Player;
        if (Vector3.Dot(player.transform.up, transform.up) > 0.9f)
        {
            return;
        }

        player.FaceDir(transform.up, DetermineAction);
    }

    protected void DetermineAction()
    {
        BaseEnemy enemy = m_showingTile.m_obj as BaseEnemy;
        if (enemy != null)
        {
            // attack
            return;
        }

        BaseChest chest = m_showingTile.m_obj as BaseChest;
        if(chest != null)
        {
            return;
        }

        m_world.RotateToSide(this);
    }

    public void Flip()
    {
        if(m_isAnimating)
        {
            return;
        }

        AnimationStarted();
        m_flipSequence.Play();
    }

    protected void OnFlipComplete()
    {
        // stupid hack to get the facing right
        Vector3 tempEuler = m_hiddenTile.transform.localEulerAngles;
        m_hiddenTile.transform.localEulerAngles = m_showingTile.transform.localEulerAngles;
        m_showingTile.transform.localEulerAngles = tempEuler;

        m_showingTile = m_hiddenTile;
        m_hiddenTile.DestroyObj();

        m_flipSequence.Rewind();

        AnimationEnded();
    }

    public bool Contains(BaseObject obj)
    {
        return m_topTile.m_obj == obj || m_bottomTile.m_obj == obj;
    }
}
