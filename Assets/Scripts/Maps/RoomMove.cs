using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script effectuant une animation de caméra entre changement de map

public class RoomMove : MonoBehaviour
{
    public Vector3 playerChange;
    private CameraMovement cam;
    public GameObject futurMap;

    public enum Orientation { Left, Right, Top, Bottom }

    public Orientation orientation;

    void Start()
    {
        cam = Camera.main.GetComponent<CameraMovement>();

        // Deplace le personnage en fonction de l'orientation choisi dans l'inspector
        if (orientation.ToString() == "Left") { playerChange.x = -2f; }
        if (orientation.ToString() == "Right") { playerChange.x = 2f; }

        if (orientation.ToString() == "Top") { playerChange.y = 2f; }
        if (orientation.ToString() == "Bottom") { playerChange.y = -2f; }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si le joueur touche le gameobject, effectue une animation de deplacement de map
        // Met à jour les valeurs de quadrillage de la nouvelle map
        if (other.CompareTag("Player"))
        {
            cam.RefreshCamLimit(futurMap);
            other.transform.position += playerChange;
        }
    }
}
