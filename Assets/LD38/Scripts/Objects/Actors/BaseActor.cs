﻿using System.Collections;
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
        DispatchChangedEvent();
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

        IncrementBusyCounter();
        if(transform.parent != null)
        {
            worldDir = transform.parent.InverseTransformDirection(worldDir);
        }

        transform.DOLocalRotate(Quaternion.LookRotation(worldDir, Vector3.up).eulerAngles, ROTATION_TIME).SetEase(Ease.OutBack).OnComplete(() =>
        {
            DecrementBusyCounter();
            if (onComplete != null)
            {
                onComplete();
            }
        });
    }

    public virtual void Jump(TweenCallback onComplete = null)
    {
        IncrementBusyCounter();
        m_model.localPosition = Vector3.zero;
        m_model.DOLocalMoveY(JUMP_HEIGHT, JUMP_TIME).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            DecrementBusyCounter();
            if (onComplete != null)
            {
                onComplete();
            }
        });
    }

    public virtual void Attack(BaseActor target = null)
    {
        IncrementBusyCounter();
        m_model.localPosition = Vector3.zero;
        m_model.DOLocalMoveZ(ATTACK_MOVE_DIST, ATTACK_TIME).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutBack).OnComplete(DecrementBusyCounter);
        if(target != null)
        {
            target.TakeDamage(CalculateAttackDamage(), this);
        }
    }

    public virtual int CalculateAttackDamage()
    {
        return m_baseAttack;
    }

    public virtual int CalculateMaxHP()
    {
        return m_baseHP;
    }

    public virtual void Heal(int amount)
    {
        m_currentHP += amount;
        int maxHP = CalculateMaxHP();
        if (m_currentHP > maxHP)
        {
            m_currentHP = maxHP;
        }
        DispatchChangedEvent();
    }

    public virtual void TakeDamage(int amount, BaseObject damageSource = null)
    {
        m_currentHP -= amount;
        if(m_currentHP < 0)
        {
            m_currentHP = 0;
        }

        Shake(DAMAGE_SHAKE_DIST, DAMAGE_TIME);
        DispatchChangedEvent();
    }

    public virtual void DispatchChangedEvent()
    {
        EventManager.OnObjectChanged.Dispatch(this);
    }

    public override string CreateInfoText()
    {
        return base.CreateInfoText() + string.Format("\nHp: {0}/{1}\nAtk: {2}", m_currentHP, CalculateMaxHP(), CalculateAttackDamage());
    }
}
