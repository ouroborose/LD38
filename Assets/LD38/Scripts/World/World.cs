using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class World : MonoBehaviour {
    public const float WORLD_POPULATION_STEP_TIME = 0.5f;
    private const float ROTATE_TO_SIDE_TIME = 0.5f;

    [SerializeField] private GameObject m_portalPrefab;
    [SerializeField] private WorldSide[] m_sides = new WorldSide[6];
    public WorldSide[] Sides { get { return m_sides; } }

    public BiomeData m_currentBiomeData { get; protected set; }
    private BasePortal m_portal;

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
        EventManager.OnLevelPopulationStarted.Dispatch();
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

            BiomeData.TileType type = m_currentBiomeData.m_tiles[tileIndex];
            if(m_currentBiomeData.m_randomlyGenerateLayout)
            {
                type = m_currentBiomeData.GetGeneratedType();
            }

            switch (type)
            {
                case BiomeData.TileType.Enemy:
                    SpawnRandomPrefab(side, m_currentBiomeData.m_enemyPrefabs);
                    break;
                case BiomeData.TileType.Chest:
                    SpawnRandomPrefab(side, m_currentBiomeData.m_chestPrefabs);
                    break;
                case BiomeData.TileType.Trap:
                    SpawnRandomPrefab(side, m_currentBiomeData.m_trapPrefabs);
                    break;
                case BiomeData.TileType.Empty:
                    PopulateTileModel(side);
                    side.Flip();
                    break;
            }

            yield return new WaitForSeconds(WORLD_POPULATION_STEP_TIME);
            tileIndex++;
        }

        m_isBusy = false;
        EventManager.OnLevelPopulationFinished.Dispatch();
    }

    public BaseObject SpawnRandomPrefab(WorldSide side, GameObject[] prefabs = null)
    {
        if(prefabs != null)
        {
            return SpawnObject(side, prefabs[UnityEngine.Random.Range(0, prefabs.Length)]);
        }

        Debug.LogError("No prefabs provided!");
        return null;
    }

    public BaseObject SpawnObject(WorldSide side, GameObject prefab)
    {
        PopulateTileModel(side);

        BaseObject objScript = null;
        if (prefab != null)
        {
            GameObject obj = Instantiate(prefab) as GameObject;
            objScript = obj.GetComponent<BaseObject>();
            objScript.SetTile(side.m_hiddenTile, true, new Vector3(0, 90 * UnityEngine.Random.Range(0, 4), 0));
        }

        side.Flip();

        return objScript;
    }

    public void PopulateTileModel(WorldSide side)
    {
        if (m_currentBiomeData.m_tileModelPrefabs.Length > 0)
        {
            side.m_hiddenTile.SetModel(m_currentBiomeData.m_tileModelPrefabs[UnityEngine.Random.Range(0, m_currentBiomeData.m_tileModelPrefabs.Length)]);
        }
    }

    public void RotateToSide(WorldSide side)
    {
        m_isBusy = true;
        transform.DORotateQuaternion(Quaternion.FromToRotation(Vector3.up, -side.transform.up) * transform.rotation, ROTATE_TO_SIDE_TIME).SetEase(Ease.InOutBack).OnComplete(OnRotateComplete);
        BasePlayer player = Main.Instance.Player;
        player.DetachFromTile();
        player.Jump(() =>
        {
            player.SetTile(side.m_showingTile, false);
            HandlePortalSpawning();
            
        });
    }

    private void OnRotateComplete()
    {
        m_isBusy = false;
    }

    public WorldSide FindEmptySide()
    {
        for(int i =0; i < m_sides.Length; ++i)
        {
            if(m_sides[i].m_isEmpty)
            {
                return m_sides[i];
            }
        }
        return null;
    }

    private void HandlePortalSpawning()
    {
        if (m_portal != null || BaseEnemy.s_allEnemies.Count > 0)
        {
            return;
        }

        WorldSide side = FindEmptySide();
        if(side == null)
        {
            return;
        }

        m_portal = SpawnObject(side, m_portalPrefab) as BasePortal;
    }
}
