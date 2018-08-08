using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VuLib
{
    public class BasePrefabIdentifier : MonoBehaviour
    {
        public const int INVALID_PREFAB_ID = 0;

        public int _id;

#if UNITY_EDITOR
        [ContextMenu("GenerateId")]
        public void GenerateId()
        {
            int id = Animator.StringToHash(gameObject.name);
            if(id != _id)
            {
                _id = id;
                EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}

