using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugMode : MonoBehaviour
{
    private GameObject DebugGameobject;
    private GameObject GameSaveManagerGameobject;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        { 
            #if UNITY_EDITOR
                DebugGameobject = GameObject.FindWithTag("Debug");
                GameSaveManagerGameobject = GameObject.FindWithTag("SaveManager");

                DebugGameobject.SetActive(true);
            #else
                DebugGameobject.SetActive(false);
            #endif
        }
    }
}
