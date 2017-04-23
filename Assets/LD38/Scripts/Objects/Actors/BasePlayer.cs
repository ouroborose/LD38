using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : BaseActor
{
    public int m_numKeys { get; protected set; }
    public int m_maxHpBonus { get; protected set; }
    public int m_atkBonus { get; protected set; }
    

    protected override void Awake()
    {
        base.Awake();
        Reset();
    }

    public override void Reset()
    {
        m_numKeys = 0;
        m_atkBonus = 0;
        m_maxHpBonus = 0;
        base.Reset();
    }

    [ContextMenu("Give Key")]
    public void AddKey()
    {
        m_numKeys++;
        DispatchChangedEvent();
    }

    public void RemoveKey()
    {
        m_numKeys--;
        DispatchChangedEvent();
    }

    public void Equip(BaseEquipment equipment)
    {
        m_atkBonus += equipment.m_atkBonus;
        m_maxHpBonus += equipment.m_hpBonus;

        DispatchChangedEvent();
    }

    public override int CalculateAttackDamage()
    {
        return base.CalculateAttackDamage() + m_atkBonus;
    }

    public override int CalculateMaxHP()
    {
        return base.CalculateMaxHP() + m_maxHpBonus;
    }

    public override void SetTile(BaseTile tile, bool rotateToTile = true, Vector3 localRotation = default(Vector3))
    {
        base.SetTile(tile, rotateToTile, localRotation);

        for (int i = 0; i < tile.m_objs.Count; ++i)
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
    }

    public override string CreateInfoText()
    {
        return base.CreateInfoText() + string.Format("\nKeys: {0}", m_numKeys);
    }
}
