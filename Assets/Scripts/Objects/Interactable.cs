using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script detectant si le joueur se situe dans le rayon d'interraction de l'objet
// Affiche une info bulle sur le joueur pour indiquer une interraction

public class Interactable : MonoBehaviour
{
    // Initialise les variables
    public bool enable = true;
    public Signal_Event contextOn;
    public Signal_Event contextOff;
    public bool playerInRange;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si le joueur est dans le rayon de detection de l'objet
        if (enable && GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().currentState != PlayerMovement.PlayerState.takeObject)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                contextOn.Raise();
                playerInRange = true;

            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Si le joueur hors du rayon de detection de l'objet
        if (enable)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                contextOff.Raise();
                playerInRange = false;
            }
        }
    }
}
