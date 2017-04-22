using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour {
    [SerializeField] protected Transform m_model;

    public bool m_isAnimating { get; protected set; }

    protected virtual void Awake()
    {
        m_isAnimating = false;
    }

    protected virtual void AnimationStarted()
    {
        m_isAnimating = true;
    }

    protected virtual void AnimationEnded()
    {
        m_isAnimating = false;
    }
}
