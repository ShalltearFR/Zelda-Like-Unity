using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pot"))
        {
            other.GetComponent<Pot>().Smash();
        }

        if (other.CompareTag("Sign"))
        {
            other.GetComponent<Sign>().Smash();
        }
    }
}
