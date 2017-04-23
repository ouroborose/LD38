using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : BaseActor
{
    public int m_numKeys { get; protected set; }
    public int m_maxHpBonus { get; protected set; }
    public int m_attackBonus { get; protected set; }
    

    protected override void Awake()
    {
        base.Awake();
        Reset();
    }

    public override void Reset()
    {
        base.Reset();
        m_numKeys = 0;
        m_attackBonus = 0;
        m_maxHpBonus = 0;
    }

    public void AddKey()
    {
        m_numKeys++;
    }

    public void RemoveKey()
    {
        m_numKeys--;
    }

    protected override int CalculateAttackDamage()
    {
        return base.CalculateAttackDamage() + m_attackBonus;
    }

    protected override int CalculateMaxHP()
    {
        return base.CalculateMaxHP() + m_maxHpBonus;
    }

    public override void SetTile(BaseTile tile, bool rotateToTile = true, Vector3 localRotation = default(Vector3))
    {
        for(int i = 0; i < tile.m_objs.Count; ++i)
        {
            BaseObject obj = tile.m_objs[i];
            BaseItem item = obj as BaseItem;
            if (item != null)
            {
                // pick up item
                item.Collect(this);
            }

            BaseTrap trap = obj as BaseTrap;
            if (trap != null)
            {
                // hit trap
                trap.Activate(this);
            }
        }

        base.SetTile(tile, rotateToTile, localRotation);
    }
}
