﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script gerant le recul du joueur/ennemis
// Ce script permet d'appeller un autre script en fonction de l'interraction du joueur

public class KnockBack : MonoBehaviour
{
    public enum TypeOfMob
    {
        Classic,
        Boss
    }

    // Initialise les variables
    public float thrust;
    public float knockTime;
    public float damage;
    public TypeOfMob typeOfMob;
    private GameObject mobGameObject;

    private void Awake()
    {
        // Indique dans une variable le gameobject ennemis
        mobGameObject = gameObject;
    }

    // Determine ce que le joueur touche en attaquant avec son épée
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si le joueur donne un coup d'épée sur une pancarte
        if (other.gameObject.CompareTag("Sign") && this.gameObject.CompareTag("Player"))
        {
            other.GetComponent<Sign>().Smash();
        }

        // Si le joueur donne un coup d'épée sur un pot
        if (other.CompareTag("Pot") && this.gameObject.CompareTag("Player"))
        {
            other.GetComponent<Pot>().Smash();
        }

        // Si le joueur lance un objet sur un ennemis
        if (other.gameObject.CompareTag("Enemy") && this.gameObject.CompareTag("TakeObject"))
        {
            Rigidbody2D effect = other.GetComponent<Rigidbody2D>();
            if (effect != null)
            {
                Vector2 difference = effect.transform.position - transform.position;
                difference = difference.normalized * thrust;
                effect.AddForce(difference, ForceMode2D.Impulse);

                effect.GetComponent<Enemy>().currentState = EnemyState.stagger;
                other.GetComponent<Enemy>().Knock(effect, knockTime, damage, typeOfMob);
            }
        }

        // Intteraction ennemis/joueur
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Player"))
        {
            if (!this.gameObject.CompareTag("TakeObject"))
            {
                Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
                if (hit != null)
                {
                    Vector2 difference = hit.transform.position - transform.position;
                    difference = difference.normalized * thrust;
                    hit.AddForce(difference, ForceMode2D.Impulse);

                    // Si l'ennemis touche
                    if (other.gameObject.CompareTag("Enemy") && other.isTrigger)
                    {
                        hit.GetComponent<Enemy>().currentState = EnemyState.stagger;
                        other.GetComponent<Enemy>().Knock(hit, knockTime, damage, typeOfMob);
                    }

                    // Si le joueur touche
                    if (other.gameObject.CompareTag("Player"))
                    {
                        bool isInTakeObjectState = false;
                        if (hit.GetComponent<PlayerMovement>().currentState == PlayerMovement.PlayerState.takeObject) { isInTakeObjectState = true; }
                 
                        if (other.GetComponent<PlayerMovement>().currentState != PlayerMovement.PlayerState.stagger)
                        {
                            hit.GetComponent<PlayerMovement>().currentState = PlayerMovement.PlayerState.stagger;
                            other.GetComponent<PlayerMovement>().Knock(knockTime, damage, mobGameObject, isInTakeObjectState);
                        }
                    }
                }
            }
        }
    }
}