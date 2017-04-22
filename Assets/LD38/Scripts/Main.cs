using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {

    [SerializeField] private BasePlayer m_player;
    [SerializeField] private World m_world;

    protected void Awake()
    {
        SceneManager.LoadScene(Strings.MAIN_UI_SCENE_NAME, LoadSceneMode.Additive);
    }

    protected void Update()
    {
        
    }
    
}
