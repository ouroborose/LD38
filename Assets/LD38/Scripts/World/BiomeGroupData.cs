using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeGroupData", menuName = "Data/BiomeGroupData")]
public class BiomeGroupData : ScriptableObject {
    public BiomeData[] m_biomes;

    [System.Serializable]
    public class DropTable
    {
        [System.Serializable]
        public class DropTableEntry
        {
            public GameObject m_prefab;
            public float m_probabilityWeight = 1.0f;
        }

        public DropTableEntry[] m_drops;

        public GameObject GetNextDrop()
        {
            float sum = 0.0f;
            for (int i = 0; i < m_drops.Length; ++i)
            {
                sum += m_drops[i].m_probabilityWeight;
            }

            float rng = Random.value;
            float threshold = 0.0f;
            for (int i = 0; i < m_drops.Length; ++i)
            {
                threshold += m_drops[i].m_probabilityWeight / sum;
                if (rng <= threshold)
                {
                    return m_drops[i].m_prefab;
                }
            }

            return null;
        }
    }

    public DropTable m_enemyDrops;
    public DropTable m_chestDrops;
}
