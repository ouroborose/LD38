using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEquipment : BaseItem {
    public enum EquipmentSlotId
    {
        None = 0,
        RightHand,
        LeftHand,
    }

    public EquipmentSlotId m_equipmentSlot = EquipmentSlotId.None;

    public int m_atkBonus = 0;
    public int m_hpBonus = 0;

    public int m_levelOffset;

    protected override void Awake()
    {
        base.Awake();

        m_atkBonus += Main.Instance.GetProgressionScaledValue(m_atkBonus, Main.Instance.m_playerAtkScalingFactor, m_levelOffset);
        m_hpBonus += Main.Instance.GetProgressionScaledValue(m_hpBonus, Main.Instance.m_playerHpScalingFactor, m_levelOffset);
    }

    [ContextMenu("Print attack values")]
    public void PrintAtkValues()
    {
        int sum = 0;
        for(int i = 0; i < 100; ++i)
        {
            int value = m_atkBonus + Main.Instance.GetProgressionScaledValue(m_atkBonus, Main.Instance.m_playerAtkScalingFactor, m_levelOffset, i);
            sum += value;
            Debug.LogFormat("{0}: {1}  {2}", i, value, sum);
        }
    }

    [ContextMenu("Print max hp values")]
    public void PrintMaxHpValues()
    {
        int sum = 0;
        for (int i = 0; i < 100; ++i)
        {
            int value = m_hpBonus + Main.Instance.GetProgressionScaledValue(m_hpBonus, Main.Instance.m_playerHpScalingFactor, m_levelOffset, i);
            sum += value;
            Debug.LogFormat("{0}: {1}  {2}", i, value, sum);
        }
    }

    public override void Collect(BasePlayer player)
    {
        player.Equip(this);
        base.Collect(player);
    }

    public override string CreateInfoText()
    {
        if (m_atkBonus > 0 && m_hpBonus > 0)
        {
            return base.CreateInfoText() + string.Format("\n+{0} Atk\n+{1} MaxHp", m_atkBonus, m_hpBonus);
        }
        else if(m_atkBonus > 0)
        {
            return base.CreateInfoText() + string.Format("\n+{0} Atk", m_atkBonus);
        }
        else if(m_hpBonus > 0)
        {
            return base.CreateInfoText() + string.Format("\n+{0} MaxHp", m_hpBonus);
        }

        return base.CreateInfoText();
    }
}
