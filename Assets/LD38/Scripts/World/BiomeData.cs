using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BiomeData", menuName = "Data/BiomeData")]
public class BiomeData : ScriptableObject {
    public enum TileType
    {
        Empty = 0,
        Enemy,
        Chest,
        Trap,
    }


    [System.Serializable]
    public class GenerationProbability
    {
        public TileType m_type = TileType.Empty;
        public float m_probabilityWeight = 1.0f;

        public GenerationProbability()
        {
            m_type = TileType.Empty;
            m_probabilityWeight = 1.0f;
        }
    }

    [Header("Layout Generation")]
    public bool m_randomlyGenerateLayout = true;
    public GenerationProbability[] m_layoutGenerationProbabilities = new GenerationProbability[4];

    public BiomeData.TileType GetGeneratedType()
    {
        float sum = 0.0f;
        for(int i = 0; i < m_layoutGenerationProbabilities.Length; ++i)
        {
            sum += m_layoutGenerationProbabilities[i].m_probabilityWeight;
        }

        float rng = Random.value;
        float threshold = 0.0f;
        for (int i = 0; i < m_layoutGenerationProbabilities.Length; ++i)
        {
            threshold += m_layoutGenerationProbabilities[i].m_probabilityWeight / sum;
            if(rng <= threshold)
            {
                return m_layoutGenerationProbabilities[i].m_type;
            }
        }

        return TileType.Empty;
    }
    

    [Header("DefintedTiles")]
    public TileType[] m_tiles = new TileType[5];

    [Header("Prefabs")]
    public GameObject[] m_tileModelPrefabs;
    public GameObject[] m_enemyPrefabs;
    public GameObject[] m_chestPrefabs;
    public GameObject[] m_trapPrefabs;
}
