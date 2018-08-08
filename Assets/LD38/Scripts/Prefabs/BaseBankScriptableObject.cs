using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace VuLib
{
    public class BaseBankScriptableObject : ScriptableObject
    {
        public string _dataPath;

#if UNITY_EDITOR
        public static void RepopulateBanks<T>(string pathToBanks) where T : BaseBankScriptableObject
        {
            string path = Path.Combine(Application.dataPath, pathToBanks);
            int dataPathLength = Application.dataPath.Length - 6;
            string[] files = Directory.GetFiles(path, searchPattern: "*", searchOption: SearchOption.AllDirectories);
            for (int i = 0, n = files.Length; i < n; ++i)
            {
                T bank = AssetDatabase.LoadAssetAtPath<T>(files[i].Substring(dataPathLength));
                if (bank != null)
                {
                    bank.Populate();
                }
            }
        }

        public virtual string GetRootDataPath()
        {
            return string.Empty;
        }

        [ContextMenu("Clear")]
        public virtual void ClearBank()
        {

        }

        [ContextMenu("Populate")]
        public virtual void Populate()
        {
            ClearBank();
            string path = Path.Combine(Application.dataPath, Path.Combine(GetRootDataPath(), _dataPath));
            int dataPathLength = Application.dataPath.Length - 6;
            string[] files = Directory.GetFiles(path, searchPattern: "*", searchOption: SearchOption.AllDirectories);
            //Debug.Log(files.Length);
            for (int i = 0, n = files.Length; i < n; ++i)
            {
                if(!files[i].EndsWith(".meta"))
                {
                    OnDataFound(files[i].Substring(dataPathLength));
                }
            }

            EditorUtility.SetDirty(this);
        }

        protected virtual void OnDataFound(string pathToAsset)
        {

        }
#endif
    }

}
