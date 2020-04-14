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

    // Start is called before the first frame update
    void Start()
    {
        saveManager = GameObject.FindWithTag("SaveManager").GetComponent<SaveManager>();

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            DefaultHeart();
        }
        StartCoroutine(ShowHeart());
    }

    IEnumerator ShowHeart()
    {
        yield return new WaitForSeconds(0.05f);
        UpdateHearts();
    }

    private void DefaultHeart()
    {
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
        for (int i = 0; i < heartContainers.RuntimeValue; i ++)
        {
            hearts[i].gameObject.SetActive(true);
            hearts[i].sprite = fullHeart;
        }
    }

    public void UpdateHearts()
    {
        InitHearts();
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
