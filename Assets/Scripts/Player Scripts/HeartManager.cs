using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HeartManager : MonoBehaviour
{
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite halfFullHeart;
    public Sprite emptyHeart;
    public FloatValue heartContainers;
    public FloatValue playerCurrentHealth;
    private SaveManager saveManager;
    private SoundManagement soundManagement;
    private bool isChangeMusic;

    // Start is called before the first frame update
    void Start()
    {
        saveManager = GameObject.FindWithTag("SaveManager").GetComponent<SaveManager>();

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            DefaultHeart();
            soundManagement = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagement>();
            InvokeRepeating("HealthDangerSound", 2.0f, 2f);
        }
        StartCoroutine(ShowHeart());
    }

    void HealthDangerSound()
    {
        float heartPercent = (playerCurrentHealth.RuntimeValue / (heartContainers.RuntimeValue * 2)) * 100;
        if (heartPercent <= 30)
        {
            // Change la musique quand le joueur à une vie faible et joue le son de danger de vie
            ChangeMusic();
            soundManagement.soundEffectSource[5].clip = Resources.Load<AudioClip>("Audio/SE/Low HP");
            soundManagement.soundEffectSource[5].Play();
        } else if (heartPercent > 30 && isChangeMusic)
        {
            GameObject.FindWithTag("SoundManager").GetComponent<MusicManager>().baseMusic();
            isChangeMusic = false;
        }
    }

    void ChangeMusic()
    {
        if (!isChangeMusic)
        {
            isChangeMusic = true;
            GameObject.FindWithTag("SoundManager").GetComponent<MusicManager>().ChangeMusic();
        }
    }

    IEnumerator ShowHeart()
    {
        yield return new WaitForSeconds(0.05f);
        UpdateHearts();
    }

    private void DefaultHeart()
    {
        // Si le fichier de sauvegarde n'existe pas, initialise à 3 coeurs (n'est normalement plus necessaire vu que le fichier est crée dans tous les cas)
        if (!File.Exists(saveManager.dataPath + "/1.dat"))
        {
            heartContainers.initialValue = 3;
            heartContainers.RuntimeValue = 3;
            playerCurrentHealth.initialValue = 6;
            playerCurrentHealth.RuntimeValue = 6;
        }
    }
    
    public void InitHearts()
    {
        // Initialise dans l'HUD les coeurs
        for (int i = 0; i < heartContainers.RuntimeValue; i ++)
        {
            hearts[i].gameObject.SetActive(true);
            hearts[i].sprite = fullHeart;
        }
    }
    

    public void UpdateHearts()
    {
        InitHearts();

        // Fait apparaitre les coeurs dans l'HUD
        float tempHealth = playerCurrentHealth.RuntimeValue / 2;
        for (int i = 0; i < heartContainers.RuntimeValue; i++)
        {
            if (i <= tempHealth - 1)
            {
                //Full Heart
                hearts[i].sprite = fullHeart;
            }
            else if (i >= tempHealth)
            {
                //empty heart
                hearts[i].sprite = emptyHeart;
            }
            else
            {
                //half full heart
                hearts[i].sprite = halfFullHeart;
            }
        }
    }
}
