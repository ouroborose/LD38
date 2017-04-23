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

    public override void Collect(BasePlayer player)
    {
        player.Equip(this);
        base.Collect(player);
    }

    public override string CreateInfoText()
    {
        return base.CreateInfoText() + string.Format("\n+{0} Atk\n+{1} MaxHp", m_atkBonus, m_hpBonus);
    }
}
