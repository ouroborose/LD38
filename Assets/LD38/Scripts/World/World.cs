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

    [SerializeField] private AudioClip[] m_flipStartSound;
    [SerializeField] private AudioClip[] m_flipFinishSound;

    public bool m_isBusy { get { return m_busyCount > 0; } }
    protected int m_busyCount = 0;
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

    public void PlayFlipStartSound()
    {
        AudioManager.Instance.PlayOneShot(m_flipStartSound);
    }

    public void PlayFlipFinishSound()
    {
        AudioManager.Instance.PlayOneShot(m_flipFinishSound);
    }

    public void Populate(BiomeData data)
    {
        EventManager.OnLevelPopulationStarted.Dispatch();
        m_busyCount++;
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
        m_busyCount--;
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

    public BaseObject SpawnObject(WorldSide side, GameObject prefab, bool populateTileModel = true)
    {
        if(populateTileModel)
        {
            PopulateTileModel(side);
        }

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
        if(m_isBusy)
        {
            return;
        }

        m_busyCount++;
        EventManager.OnWorldRotationStarted.Dispatch();
        transform.DORotateQuaternion(Quaternion.FromToRotation(Vector3.up, -side.transform.up) * transform.rotation, ROTATE_TO_SIDE_TIME).SetEase(Ease.InOutBack).OnComplete(OnRotateComplete);
        BasePlayer player = Main.Instance.Player;
        player.DetachFromTile();
        player.Jump(() =>
        {
            player.SetTile(side.m_showingTile, false);
            if(BaseEnemy.s_allEnemies.Count <= 0)
            {
                TryToSpawnPortalSpawning();
            }
            EventManager.OnWorldRotationFinished.Dispatch();
        });
    }

    private void OnRotateComplete()
    {
        m_busyCount--;
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

    
    public WorldSide GetTopSide()
    {
        for (int i = 0; i < m_sides.Length; ++i)
        {
            if (Vector3.Dot(m_sides[i].transform.up, Vector3.up) > 0.9f)
            {
                return m_sides[i];
            }
        }
        return null;
    }

    public void TryToSpawnPortalSpawning()
    {
        if (m_portal != null || Main.Instance.Player.m_isBusy)
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

    public WorldSide GetWorldByDirection(Vector3 dir)
    {
        WorldSide best = null;
        float bestDot = float.MinValue;
        for (int i = 0; i < Main.Instance.World.Sides.Length; ++i)
        {
            WorldSide side = Main.Instance.World.Sides[i];
            float dot = Vector3.Dot(side.transform.up, dir);
            if (dot > bestDot)
            {
                best = side;
                bestDot = dot;
            }
        }
        return best;
    }


    public WorldData GenerateWorldData()
    {
        WorldData data = new WorldData();

        WorldSide topSide = GetTopSide();
        for (int i = 0; i < 6; ++i)
        {
            data.m_worldSideDatas[i] = m_sides[i].GenerateWorldSideData();
            if(m_sides[i] == topSide)
            {
                data.m_topSideIndex = i;
            }
        }

        Vector3 rotation = transform.eulerAngles;
        data.m_rotationX = rotation.x;
        data.m_rotationY = rotation.y;
        data.m_rotationZ = rotation.z;
        return data;
    }

    public void LoadFromData(WorldData data, BiomeData biomeData)
    {
        EventManager.OnLevelPopulationStarted.Dispatch();
        m_busyCount++;
        m_currentBiomeData = biomeData;
        StartCoroutine(HandleWorldLoading(data));
    }

    protected IEnumerator HandleWorldLoading(WorldData data)
    {
        Quaternion toRotation = Quaternion.Euler(data.m_rotationX, data.m_rotationY, data.m_rotationZ);
        if(Quaternion.Angle(transform.rotation, toRotation) > 45.0f)
        {
            transform.DORotateQuaternion(toRotation, ROTATE_TO_SIDE_TIME).SetEase(Ease.InOutBack);
            yield return new WaitForSeconds(ROTATE_TO_SIDE_TIME);
        }

        for (int i = 0; i < 6; ++i)
        {
            WorldSide side = m_sides[i];
            WorldSideData sideData = data.m_worldSideDatas[i];

            GameObject prefab;
            if (VuLib.BasePrefabManager.Instance.TryGetPrefab(sideData.m_worldTileId, out prefab))
            {
                // load tile model
                side.m_hiddenTile.SetModel(prefab);
            }
            else
            {
                PopulateTileModel(side);
            }

            if (VuLib.BasePrefabManager.Instance.TryGetPrefab(sideData.m_objectId, out prefab))
            {
                BaseObject obj = SpawnObject(side, prefab, false);
                obj.transform.localRotation = Quaternion.Euler(0.0f, sideData.m_objectRotation, 0.0f);

                BaseActor actor = obj as BaseActor;
                if(actor != null)
                {
                    actor.LoadFromData(sideData.m_actorData);
                }
                else
                {
                    BasePortal portal = obj as BasePortal;
                    if (portal != null)
                    {
                        m_portal = portal;
                    }
                    else
                    {
                        BaseItem item = obj as BaseItem;
                        if (item != null)
                        {
                            item.Model.position = side.m_hiddenTile.transform.TransformPoint(new Vector3(0.0f, 0.5f, 0.0f));
                        }
                    }
                }
            }
            else
            {
                side.Flip();
            }
            yield return new WaitForSeconds(WORLD_POPULATION_STEP_TIME);
        }

        m_busyCount--;
        EventManager.OnLevelPopulationFinished.Dispatch();
    }
}
