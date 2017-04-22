using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BaseActor : BaseObject
{
    [SerializeField] private float m_rotationTime = 0.25f;

    [SerializeField] private float m_jumpHeight = 0.5f;
    [SerializeField] private float m_jumpTime = 0.25f;

    public int m_baseHP;
    public int m_currentHP;

    public int m_baseAttack;

    public virtual void Reset()
    {
        m_currentHP = m_baseHP;
    }

    public void FaceDir(Vector3 dir, TweenCallback onComplete = null)
    {
        if(Vector3.Dot(transform.forward, dir) > 0.9f)
        {
            // already facing dir
            if (onComplete != null)
            {
                onComplete();
            }
            return;
        }

        AnimationStarted();
        transform.DORotate(Quaternion.LookRotation(dir, transform.up).eulerAngles, m_rotationTime).SetEase(Ease.OutBack).OnComplete(()=>
        {
            AnimationEnded();
            if(onComplete != null)
            {
                onComplete();
            }
        });
    }

    public void Jump(TweenCallback onComplete = null)
    {
        AnimationStarted();
        m_model.DOLocalMoveY(m_jumpHeight, m_jumpTime).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            AnimationEnded();
            if (onComplete != null)
            {
                onComplete();
            }
        });
    }
}
