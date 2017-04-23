﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePotion : BaseItem {
    [SerializeField] private int m_healAmount = 10;

    public override void Collect(BasePlayer player)
    {
        player.Heal(m_healAmount);

        base.Collect(player);
    }
}
