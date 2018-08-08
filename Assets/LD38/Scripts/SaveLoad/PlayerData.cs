using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData : ActorData {
    public int m_numKeys;
    public List<int> m_equipmentIds = new List<int>();
}
