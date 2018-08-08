using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VuLib
{
    public class BasePrefabManager :  Singleton<BasePrefabManager>
    {
        [SerializeField] protected PrefabBank[] _prefabBanks;

        protected Dictionary<int, GameObject> _prefabMap = new Dictionary<int, GameObject>();

        protected override void Awake()
        {
            base.Awake();
            for(int i = 0; i < _prefabBanks.Length; ++i)
            {
                RegisterPrefabBank(_prefabBanks[i]);
            }
        }

        public void RegisterPrefabBank(PrefabBank bank)
        {
            for(int i = 0; i < bank._prefabs.Count; ++i)
            {
                RegisterPrefab(bank._prefabs[i]);
            }
        }
        
        public void RegisterPrefab(GameObject prefab)
        {
            BasePrefabIdentifier identifier = prefab.GetComponent<BasePrefabIdentifier>();
            RegisterPrefab(identifier);
        }

        public void RegisterPrefab(BasePrefabIdentifier identifier)
        {
            int id = identifier._id;
            GameObject existingPrefab;
            if (_prefabMap.TryGetValue(id, out existingPrefab))
            {
                Debug.LogWarningFormat("Identifier already exists: {0} - {1} and {2}", id, identifier.gameObject.name, existingPrefab.name);
            }

            _prefabMap[id] = identifier.gameObject;
        }

        public bool TryGetPrefab(int id, out GameObject prefab)
        {
            return _prefabMap.TryGetValue(id, out prefab);
        }

        public GameObject Spawn(int id, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), Transform parent = null)
        {
            GameObject instance = null;
            GameObject prefab;
            if(TryGetPrefab(id, out prefab))
            {
                instance = Instantiate(prefab, position, rotation, parent);
            }
            return instance;
        }

        public T Spawn<T>(int id, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), Transform parent = null) where T:Component
        {
            GameObject instance = Spawn(id, position, rotation, parent);
            if(instance != null)
            {
                return instance.GetComponent<T>();
            }
            return null;
        }
    }

}

