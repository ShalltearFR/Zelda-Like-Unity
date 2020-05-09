using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Script permettant de changer de scène et effectue son animation

public class SceneTransition : MonoBehaviour
{
    // Initialisation des variables
    public VectorValue playerStorage;
    public GameObject fadeInPanel;
    public GameObject fadeOutPanel;
    public StringValue playerSideStringValue;
    private bool isLoading;
    
    [Header("")]
    public string sceneToLoad;
    public string playerSide;
    public Vector2 playerPosition;
    public float fadeWait;

    private void Start()
    {
        // Animation de fondu de sortie
        if (fadeOutPanel != null)
        {
            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                GameObject panel = Instantiate(fadeOutPanel, playerStorage.initialValue, Quaternion.identity) as GameObject;
                Destroy(panel, 1);
            } else
            {
                // panel = Instantiate(fadeOutPanel, new Vector3(0,0,0), Quaternion.identity) as GameObject;
            }

            
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si le joueur touche le gameobject
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            // Sauvegarde l'orientation du joueur pour la prochaine scène
            playerStorage.initialValue = playerPosition;

            // Si la procèdure de chargement n'a pas été effectué (evite de faire une infinité de fois l'animation de transition)
            if (!isLoading)
            {
                StartCoroutine(FadeCo());
                isLoading = true;
            } 
        }
    }

    public IEnumerator FadeCo()
    {
        // Effectue une animation de fondu entrant
        if (fadeInPanel != null)
        {
            GameObject panel = Instantiate(fadeInPanel, GameObject.FindWithTag("Player").GetComponent<Transform>().position, Quaternion.identity);

            playerSideStringValue.initialValue = playerSide;

            // Bloque les mouvements du joueur
            GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().currentState = PlayerMovement.PlayerState.interact;

            GameObject.FindWithTag("Canvas").SetActive(false);
        }

        // Précharge la prochaine scène
        yield return new WaitForSeconds(fadeWait);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        
    }
}
