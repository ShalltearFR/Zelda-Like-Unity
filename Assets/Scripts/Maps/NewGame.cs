using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGame : MonoBehaviour
{
    public BoolValue isNewGame;
    private CameraMovement cameraMovement;
    private GameObject player;

    // Script de debut de nouvelle partie
    private void Start()
    {
        if (isNewGame.initialValue)
        {
            cameraMovement = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
            player = GameObject.FindWithTag("Player");

            player.GetComponent<Transform>().position = new Vector3(-8, 14, -3);

            StartCoroutine(cameraMovement.InitCamera());




            isNewGame.initialValue = false;
        }
    }
}
