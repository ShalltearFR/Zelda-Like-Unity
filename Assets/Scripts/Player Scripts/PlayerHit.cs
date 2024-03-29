﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si le joueur tape un type d'objet avec son épée
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
