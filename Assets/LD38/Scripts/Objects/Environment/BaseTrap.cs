﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTrap : BaseObject {
    [SerializeField] protected int m_baseDamage = 1;

    public void Activate(BaseActor actor)
    {
        actor.TakeDamage(m_baseDamage, this);
    }

    public override string CreateInfoText()
    {
        return base.CreateInfoText() + string.Format("\nDmg: {0}", m_baseDamage);
    }
}
