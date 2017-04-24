using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BasePlayer : BaseActor
{
    public int m_numKeys { get; protected set; }

    [SerializeField] private EquipmentSlot[] m_equipmentSlots;

    private Dictionary<BaseEquipment.EquipmentSlotId, EquipmentSlot> m_equipmentSlotMapping = new Dictionary<BaseEquipment.EquipmentSlotId, EquipmentSlot>();

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < m_equipmentSlots.Length; ++i)
        {
            m_equipmentSlotMapping.Add(m_equipmentSlots[i].m_id, m_equipmentSlots[i]);
        }

        Reset();
    }

    public override void Reset()
    {
        m_numKeys = 0;
        m_atkBonus = 0;
        m_maxHpBonus = 0;

        for (int i = 0; i < m_equipmentSlots.Length; ++i)
        {
            m_equipmentSlots[i].RemoveEquipment();
        }

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
        if(equipment.m_atkBonus > 0)
        {
            ShowNeutralPopText(string.Format("+{0} atk", equipment.m_atkBonus));
        }
        if (equipment.m_hpBonus > 0)
        {
            ShowNeutralPopText(string.Format("+{0} max hp", equipment.m_hpBonus));
        }
        m_atkBonus += equipment.m_atkBonus;
        m_maxHpBonus += equipment.m_hpBonus;
        m_currentHP += equipment.m_hpBonus;

        EquipmentSlot slot;
        if(m_equipmentSlotMapping.TryGetValue(equipment.m_equipmentSlot, out slot))
        {
            slot.Equip(equipment);
        }

        DispatchChangedEvent();
    }

    public override void TakeDamage(int amount, BaseObject damageSource = null)
    {
        base.TakeDamage(amount, damageSource);
        if (m_currentHP <= 0)
        {
            DOVirtual.DelayedCall(DAMAGE_TIME + POST_DAMAGE_FEEDBACK_DELAY, () =>
            {
                gameObject.SetActive(false);
                DetachFromTile();

                DOVirtual.DelayedCall(DAMAGE_TIME + POST_DAMAGE_FEEDBACK_DELAY, Main.Instance.GameOver);
            });
        }
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
