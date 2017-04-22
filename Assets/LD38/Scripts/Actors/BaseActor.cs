using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BaseActor : BaseObject
{
    [SerializeField] private float m_rotationTime = 0.25f;

    [SerializeField] private float m_jumpHeight = 0.5f;
    [SerializeField] private float m_jumpTime = 0.25f;

    public Tweener FaceDir(Vector3 dir, TweenCallback onComplete = null)
    {
        AnimationStarted();
        return transform.DORotate(Quaternion.LookRotation(dir, transform.up).eulerAngles, m_rotationTime).SetEase(Ease.OutBack).OnComplete(()=>
        {
            AnimationEnded();
            if(onComplete != null)
            {
                onComplete();
            }
        });
    }

    public Tweener Jump(TweenCallback onComplete = null)
    {
        AnimationStarted();
        return m_model.DOLocalMoveY(m_jumpHeight, m_jumpTime).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            AnimationEnded();
            if (onComplete != null)
            {
                onComplete();
            }
        });
    }
}
