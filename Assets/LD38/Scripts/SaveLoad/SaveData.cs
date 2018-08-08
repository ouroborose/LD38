using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData {
    public int m_currentLevel;
    public int m_currentBiomeIndex;
    public WorldData m_worldData;
    public PlayerData m_playerData;
    
    public string GenerateSaveString()
    {
        return JsonUtility.ToJson(this);
    }
}
