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
        // Si le joueur n'est pas dans le menu principal
        if (SceneManager.GetActiveScene().name != "MainMenu")
        { 
            // Change le gameobject si le jeu est en mode build ou editor
            #if UNITY_EDITOR
                if (GameObject.FindWithTag("Debug") != null)
                {
                    DebugGameobject = GameObject.FindWithTag("Debug");
                    DebugGameobject.SetActive(true);
                }
                GameSaveManagerGameobject = GameObject.FindWithTag("SaveManager");

                
            #else
                DebugGameobject.SetActive(false);
            #endif
        }
    }
}
