using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using E7.Introloop;

// Script permettant de changer de scène et effectue son animation

public class SceneTransition : MonoBehaviour
{
    // Initialisation des variables
    public VectorValue playerStorage;
    public GameObject fadeInPanel;
    public StringValue playerSideStringValue;
    public StringValue[] worldName = new StringValue[3];
    public string mapName;
    private string worldNameTxt;
    private bool isLoading;
    private SaveManager saveManager;
    
    [Header("")]
    public string sceneToLoad;
    public string playerSide;
    public Vector2 playerPosition;
    public float fadeWait;

    private void Start()
    {
        saveManager = GameObject.Find("Save Manager").GetComponent<SaveManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si le joueur touche le gameobject
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            IntroloopPlayer.Instance.Pause(1f);

            // Sauvegarde l'orientation du joueur pour la prochaine scène
            playerStorage.teleporationValue = playerPosition;

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

            playerSideStringValue.RuntimeValue = playerSide;
            playerSideStringValue.initialValue = playerSide;

            // Bloque les mouvements du joueur
            GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().currentState = PlayerMovement.PlayerState.interact;

            GameObject.FindWithTag("Canvas").SetActive(false);
        }
        if (mapName == null) { mapName = "Map1"; }

        StringValue mapTemp = saveManager.onlyTeleport as StringValue;
        mapTemp.RuntimeValue = mapName;
        GameObject.Find("Save Manager").GetComponent<SaveManager>().objects[8] = mapTemp;

        // Précharge la prochaine scène
        yield return new WaitForSeconds(fadeWait);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }

    public IEnumerator MainMenuTransitionCo(string typeOfTransition, int slot)
    {
        GameObject panel = Instantiate(fadeInPanel, GameObject.Find("MainMenu Foreground").GetComponent<Transform>().position, Quaternion.identity);

        // Précharge la prochaine scène
        yield return new WaitForSeconds(fadeWait);

        // Si le mode de jeu est sur Nouvelle Partie
        if (typeOfTransition == "New Game")
        {
            // Teleporte à la position de début de jeu
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(1);
            while (!asyncOperation.isDone)
            {
                yield return null;
            }
        }

        //Si le mode de jeu est sur Continuer
        if (typeOfTransition == "Continue")
        {
            // Lit le contenu de la variable du monde du joueur contenu dans le slot chargé
            if (slot == 1) { worldNameTxt = worldName[0].RuntimeValue; }
            if (slot == 2) { worldNameTxt = worldName[1].RuntimeValue; }
            if (slot == 3) { worldNameTxt = worldName[2].RuntimeValue; }

            // Teleporte le joueur dans le monde
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(worldNameTxt);
            while (!asyncOperation.isDone)
            {
                yield return null;
            }
         
        }
        yield return null;
    }
}
