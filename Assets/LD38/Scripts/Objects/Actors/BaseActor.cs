using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BaseActor : BaseObject
{
    protected const float ATTACK_MOVE_DIST = 0.5f;
    protected const float ATTACK_TIME = 0.2f;

    protected const float DAMAGE_SHAKE_DIST = 0.33f;
    protected const float DAMAGE_TIME = 0.5f;

    protected const float ROTATION_TIME = 0.25f;

    protected const float JUMP_HEIGHT = 0.5f;
    protected const float JUMP_TIME = 0.25f;

    [SerializeField] protected int m_baseHP = 1;
    public int m_currentHP { get; protected set; }

    [SerializeField] protected int m_baseAttack = 1;

    protected override void Awake()
    {
        base.Awake();
        Reset();
    }

    public virtual void Reset()
    {
        m_currentHP = m_baseHP;
    }

    public virtual void FaceDir(Vector3 worldDir, TweenCallback onComplete = null)
    {
        if (Vector3.Dot(transform.forward, worldDir) > 0.9f)
        {
            // already facing dir
            if (onComplete != null)
            {
                onComplete();
            }
            return;
        }

        AnimationStarted();
        if(transform.parent != null)
        {
            worldDir = transform.parent.InverseTransformDirection(worldDir);
        }

        transform.DOLocalRotate(Quaternion.LookRotation(worldDir, Vector3.up).eulerAngles, ROTATION_TIME).SetEase(Ease.OutBack).OnComplete(() =>
        {
            AnimationEnded();
            if (onComplete != null)
            {
                onComplete();
            }
        });
    }

    public virtual void Jump(TweenCallback onComplete = null)
    {
        AnimationStarted();
        m_model.DOLocalMoveY(JUMP_HEIGHT, JUMP_TIME).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            AnimationEnded();
            if (onComplete != null)
            {
                onComplete();
            }
        });
    }

    public virtual void Attack(BaseActor target)
    {
        m_model.DOLocalMoveZ(ATTACK_MOVE_DIST, ATTACK_TIME).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutBack);
        if(target != null)
        {
            target.TakeDamage(CalculateAttackDamage(), this);
        }
    }

    protected int CalculateAttackDamage()
    {
        return m_baseAttack;
    }

    public virtual void TakeDamage(int amount, BaseObject damageSource)
    {
        m_currentHP -= amount;
        if(m_currentHP < 0)
        {
            m_currentHP = 0;
        }

        Vector3 shakeDir = UnityEngine.Random.onUnitSphere;
        shakeDir.y = 0.0f;
        shakeDir.Normalize();
        m_model.DOShakePosition(DAMAGE_TIME, shakeDir * DAMAGE_SHAKE_DIST, 20);
    }
}
