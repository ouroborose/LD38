using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : MonoBehaviour {
    public BaseEquipment.EquipmentSlotId m_id;
    public GameObject m_equipment;

    public void Equip(BaseEquipment equipment)
    {
        RemoveEquipment();

        m_equipment = Instantiate(equipment.Model.gameObject);
        m_equipment.transform.parent = transform;
        m_equipment.transform.localPosition = Vector3.zero;
        m_equipment.transform.localRotation = Quaternion.identity;
    }

    public void RemoveEquipment()
    {
        if(m_equipment != null)
        {
            Destroy(m_equipment);
        }
    }
}
