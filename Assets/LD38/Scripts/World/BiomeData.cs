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

    public TileType[] m_tiles = new TileType[5];

    public GameObject[] m_tileModelPrefabs;
    public GameObject[] m_enemyPrefabs;
    public GameObject[] m_chestPrefabs;
    public GameObject[] m_trapPrefabs;
}
