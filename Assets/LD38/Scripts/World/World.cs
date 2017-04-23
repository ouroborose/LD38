using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class World : MonoBehaviour {
    public const float WORLD_POPULATION_STEP_TIME = 0.5f;

    [SerializeField] private float m_rotationTime = 0.5f;

    [SerializeField] private WorldSide[] m_sides = new WorldSide[6];
    public WorldSide[] Sides { get { return m_sides; } }

    public BiomeData[] m_biomes;

    public BiomeData m_currentBiomeData { get; protected set; }

    public bool m_isBusy { get; private set; }
    public bool m_anySideBusy
    {
        get
        {
            for(int i = 0; i < m_sides.Length; ++i)
            {
                if(m_sides[i].m_isBusy)
                {
                    return true;
                }
            }
            return false;
        }
    }

    protected void Awake()
    {
        m_isBusy = false;

        for(int i = 0; i < m_sides.Length; ++i)
        {
            m_sides[i].Init(this);
        }
    }

    public void FlipAllEmptyTiles()
    {
        for(int i = 0; i < m_sides.Length; ++i)
        {
            if(m_sides[i].m_isEmpty)
            {
                m_sides[i].Flip();
            }
        }
    }

    public void Populate(BiomeData data)
    {
        m_isBusy = true;
        StartCoroutine(HandleWorldPopulation(data));
    }

    protected IEnumerator HandleWorldPopulation(BiomeData data)
    {
        m_currentBiomeData = data;
        int tileIndex = 0;
        for (int i = 0; i < m_sides.Length; ++i)
        {
            WorldSide side = m_sides[i];
            if (side.Contains(Main.Instance.Player))
            {
                continue;
            }
            
            switch (m_currentBiomeData.m_tiles[tileIndex])
            {
                case BiomeData.TileType.Enemy:
                    SpawnObject(side, m_currentBiomeData.m_enemyPrefabs);
                    break;
                case BiomeData.TileType.Chest:
                    SpawnObject(side, m_currentBiomeData.m_chestPrefabs);
                    break;
                case BiomeData.TileType.Trap:
                    SpawnObject(side, m_currentBiomeData.m_trapPrefabs);
                    break;
                case BiomeData.TileType.Empty:
                    SpawnObject(side);
                    break;
            }

            yield return new WaitForSeconds(WORLD_POPULATION_STEP_TIME);
            tileIndex++;
        }
        m_isBusy = false;
    }

    public void SpawnObject(WorldSide side, GameObject[] prefabs = null)
    {
        if (m_currentBiomeData.m_tileModelPrefabs.Length > 0)
        {
            side.m_hiddenTile.SetModel(m_currentBiomeData.m_tileModelPrefabs[UnityEngine.Random.Range(0, m_currentBiomeData.m_tileModelPrefabs.Length)]);
        }

        if(prefabs != null)
        {
            GameObject prefab = prefabs[UnityEngine.Random.Range(0, prefabs.Length)];
            if(prefab != null)
            {
                GameObject obj = Instantiate(prefab) as GameObject;
                BaseObject objScript = obj.GetComponent<BaseObject>();
                objScript.SetTile(side.m_hiddenTile, true, new Vector3(0, 90 * UnityEngine.Random.Range(0, 4), 0));
            }
        }

        side.Flip();
    }

    public void RotateToSide(WorldSide side)
    {
        m_isBusy = true;
        transform.DORotateQuaternion(Quaternion.FromToRotation(Vector3.up, -side.transform.up) * transform.rotation, m_rotationTime).SetEase(Ease.InOutBack).OnComplete(OnRotateComplete);
        BasePlayer player = Main.Instance.Player;
        player.DetachFromTile();
        player.Jump(() => player.SetTile(side.m_showingTile, false));
    }

    private void OnRotateComplete()
    {
        m_isBusy = false;
    }
}
