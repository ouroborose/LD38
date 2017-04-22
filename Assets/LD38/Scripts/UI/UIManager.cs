using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
    protected void Awake()
    {
        if(SceneManager.GetActiveScene().name != Strings.MAIN_SCENE_NAME)
        {
            SceneManager.LoadScene(Strings.MAIN_SCENE_NAME);
        }
    }
}
