using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

public class MainMenu : MonoBehaviour
{
    private EventSystem eventSystem;
    public SaveManager saveManager;
    public SoundManagement soundManager;

    [Header("Main Menu")]
    public GameObject menu1;
    public Animator pressStart;
    public Animator options;

    [Header("Verification du menu actif")]
    public bool isPressStart;
    public bool isInMainMenu;
    public bool isInSlotMenu;
    public bool isInOptions;

    [Header("Slot Menu")]
    private GameObject slotMenu;
    public CanvasGroup[] slotFull;
    public CanvasGroup[] slotEmpty;
    public TextMeshProUGUI[] playedTime;

    [Header("Others")]
    public string slotMode;
    public TextMeshProUGUI slotModeTxt;


    private int fpsLimite = 60; 

    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = fpsLimite;
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        slotMenu = GameObject.Find("Slot Menu");
        isPressStart = true;
        StartCoroutine(CheckSlots());
    }

    IEnumerator CheckSlots()
    {
        yield return new WaitForSeconds(0.5f);
        // Check Slot 1
        if (!saveManager.isSlot1Empty)
        {
            slotFull[0].alpha = 1;
            slotEmpty[0].alpha = 0;
            slotEmpty[0].interactable = false;
            slotEmpty[0].blocksRaycasts = false;

            int hoursTemp = Convert.ToInt32(saveManager.Slot1PlayedTime.RuntimeValue / 3600);
            int minutesTemp = Convert.ToInt32((saveManager.Slot1PlayedTime.RuntimeValue % 3600) / 60);
            int secondsTemp = Convert.ToInt32((saveManager.Slot1PlayedTime.RuntimeValue % 3600) % 60);

            string hours = "", minutes = "", seconds = "";

            if (hoursTemp < 10)
            { hours = "0" + hoursTemp.ToString(); }
            else { hours = hoursTemp.ToString(); }

            if (minutesTemp < 10)
            { minutes = "0" + minutesTemp.ToString(); }
            else { minutes = minutesTemp.ToString(); }

            if (secondsTemp < 10)
            { seconds = "0" + secondsTemp.ToString(); }
            else { seconds = secondsTemp.ToString(); }

            playedTime[0].text = "Temps " + hours + ":" + minutes + ":" + seconds;

        } else
        {
            slotFull[0].interactable = false;
            slotFull[0].blocksRaycasts = false;
            slotFull[0].alpha = 0;
            slotEmpty[0].alpha = 1;
        }

        // Check Slot 2
        if (!saveManager.isSlot2Empty)
        {
            slotFull[1].alpha = 1;
            slotEmpty[1].alpha = 0;
            slotEmpty[1].interactable = false;
            slotEmpty[1].blocksRaycasts = false;
            
            int hoursTemp = Convert.ToInt32(saveManager.Slot2PlayedTime.RuntimeValue / 3600);
            int minutesTemp = Convert.ToInt32((saveManager.Slot2PlayedTime.RuntimeValue % 3600) / 60);
            int secondsTemp = Convert.ToInt32((saveManager.Slot2PlayedTime.RuntimeValue % 3600) % 60);

            string hours = "", minutes = "", seconds = "";

            if (hoursTemp < 10)
            { hours = "0" + hoursTemp.ToString(); } else { hours = hoursTemp.ToString(); }

            if (minutesTemp < 10)
            { minutes = "0" + minutesTemp.ToString(); } else { minutes = minutesTemp.ToString(); }

            if (secondsTemp < 10)
            { seconds = "0" + secondsTemp.ToString(); } else { seconds = secondsTemp.ToString(); }

            playedTime[1].text = "Temps "+ hours + ":" + minutes + ":" + seconds;
        }
        else
        {
            slotFull[1].interactable = false;
            slotFull[1].blocksRaycasts = false;
            slotFull[1].alpha = 0;
            slotEmpty[1].alpha = 1;
        }

        // Check Slot 3
        if (!saveManager.isSlot3Empty)
        {
            slotFull[2].alpha = 1;
            slotEmpty[2].alpha = 0;
            slotEmpty[2].interactable = false;
            slotEmpty[2].blocksRaycasts = false;

            int hoursTemp = Convert.ToInt32(saveManager.Slot3PlayedTime.RuntimeValue / 3600);
            int minutesTemp = Convert.ToInt32((saveManager.Slot3PlayedTime.RuntimeValue % 3600) / 60);
            int secondsTemp = Convert.ToInt32((saveManager.Slot3PlayedTime.RuntimeValue % 3600) % 60);

            string hours = "", minutes = "", seconds = "";

            if (hoursTemp < 10)
            { hours = "0" + hoursTemp.ToString(); }
            else { hours = hoursTemp.ToString(); }

            if (minutesTemp < 10)
            { minutes = "0" + minutesTemp.ToString(); }
            else { minutes = minutesTemp.ToString(); }

            if (secondsTemp < 10)
            { seconds = "0" + secondsTemp.ToString(); }
            else { seconds = secondsTemp.ToString(); }

            playedTime[2].text = "Temps " + hours + ":" + minutes + ":" + seconds;
        }
        else
        {
            slotFull[2].interactable = false;
            slotFull[2].blocksRaycasts = false;
            slotFull[2].alpha = 0;
            slotEmpty[2].alpha = 1;
        }
    }

    void CursorNewGame()
    {
        GameObject startSelectedButton = GameObject.Find("NewGame Button");
        eventSystem.SetSelectedGameObject(startSelectedButton);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = fpsLimite;

        if (isInMainMenu)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                StartCoroutine(BackToPressStart());
                GameObject startSelectedButton = GameObject.Find("NewGame Button");
                eventSystem.SetSelectedGameObject(null);
            }
        }

        if (isPressStart)
        {
            if (Input.GetButtonDown("X"))
            {
                StartCoroutine(StartPressed());
                isPressStart = false;
            }
        }

        if (isInSlotMenu)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                SlotToMainMenuButton();
            }
        }

        if (isInOptions)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                StartCoroutine(OptionsToMainMenu());
            }
        }
    }

    IEnumerator BackToPressStart()
    {
        isInMainMenu = false;
        menu1.GetComponent<Animator>().SetTrigger("Close");
        yield return new WaitForSeconds(0.30f);
        pressStart.SetTrigger("idle");
        yield return new WaitForSeconds(0.35f);
        isPressStart = true;
    }

    IEnumerator StartPressed()
    {
        pressStart.SetTrigger("pressed");

        soundManager.soundEffectSource[0].clip = Resources.Load<AudioClip>("Audio/SE/Heart piece");
        soundManager.soundEffectSource[0].Play();

        yield return new WaitForSeconds(1f);
        menu1.GetComponent<Animator>().SetTrigger("Open");
        CursorNewGame();
        yield return new WaitForSeconds(0.35f);
        isInMainMenu = true;
    }

    public void SlotModeButton(string value)
    {
        slotMode = value;
        isInMainMenu = false;
        if (value == "New Game") { slotModeTxt.text = "Nouvelle Partie"; }
        if (value == "Continue") { slotModeTxt.text = "Continuer"; }
        StartCoroutine(SlotModeButton());
    }

    IEnumerator SlotModeButton()
    {
        eventSystem.SetSelectedGameObject(null);
        
        menu1.GetComponent<Animator>().SetTrigger("Close");
        yield return new WaitForSeconds(0.30f);

        slotMenu.GetComponent<Animator>().SetTrigger("Open");
        yield return new WaitForSeconds(0.05f);
        eventSystem.SetSelectedGameObject(GameObject.Find("Slot Button First"));
        yield return new WaitForSeconds(0.35f);
        isInSlotMenu = true;

    }

    public void OptionsButton()
    {
        StartCoroutine(OptionsButtonCo());
    }

    IEnumerator OptionsButtonCo()
    {
        isInMainMenu = false;
        eventSystem.SetSelectedGameObject(null);

        menu1.GetComponent<Animator>().SetTrigger("Close");
        yield return new WaitForSeconds(0.30f);

        options.GetComponent<Animator>().SetTrigger("Open");
        yield return new WaitForSeconds(0.05f);
        eventSystem.SetSelectedGameObject(GameObject.Find("Options"));
        yield return new WaitForSeconds(0.35f);
        eventSystem.SetSelectedGameObject(GameObject.Find("Music Slider"));
        isInOptions = true;
    }

    public void OptionsToMainMenuButton()
    {
        StartCoroutine(OptionsToMainMenu());
    }

    IEnumerator OptionsToMainMenu()
    {
        isInOptions = false;
        eventSystem.SetSelectedGameObject(null);

        options.SetTrigger("Close");
        yield return new WaitForSeconds(0.30f);

        menu1.GetComponent<Animator>().SetTrigger("Open");
        yield return new WaitForSeconds(0.35f);
        CursorNewGame();
        isInMainMenu = true;
    }

    public void ExitButton()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    public void SlotToMainMenuButton()
    {
        isInSlotMenu = false;
        StartCoroutine(SlotToMainMenuButtonCo());
    }

    IEnumerator SlotToMainMenuButtonCo()
    {
        eventSystem.SetSelectedGameObject(null);
        slotMenu.GetComponent<Animator>().SetTrigger("Close");
        yield return new WaitForSeconds(0.35f);

        menu1.GetComponent<Animator>().SetTrigger("Open");
        CursorNewGame();
        yield return new WaitForSeconds(0.35f);
        isInMainMenu = true;
    }

    public void SlotButton(int value)
    {
        if(slotMode == "Continue")
        {
            Debug.Log("continue");
        }

        if (slotMode == "New Game")
        {
            Debug.Log("new game");
        }
    }
}
