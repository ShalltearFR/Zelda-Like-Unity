using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTeleport : MonoBehaviour
{
    public bool enable;
    public Vector3 position;
    private GameObject player;
    private GameObject mainCamera;


    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        mainCamera = GameObject.Find("Main Camera");

        //InvokeRepeating("Teleport", 0.065f, 0.0f);
    }

    public void Teleport()
    {
        // Sert de debug et à forcer le joueur à se teleporter sur une position
        player.transform.position = new Vector3(position.x, position.y, position.z);
        mainCamera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -4);
    }
}
