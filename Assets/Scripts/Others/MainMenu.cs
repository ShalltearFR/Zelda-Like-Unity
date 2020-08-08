using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    private EventSystem eventSystem;
    private bool fileAreDelete;
    public SaveManager saveManager;
    public SoundManagement soundManager;
    public SceneTransition sceneTransition;

    [Header("Main Menu")]
    public GameObject menu1;
    public Animator pressStart;
    public Animator options;
    public TMP_Dropdown resolutionDropdown;
    public List<int> widthResolution = new List<int>();
    public List<int> heightResolution = new List<int>();

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
    private GameObject textPanel;
    public BoolValue isNewGame;
    private float[] playedTimeFloat = new float[3];
    private int fpsLimite = 60; 

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = fpsLimite;
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        slotMenu = GameObject.Find("Slot Menu");
        isPressStart = true;
        StartCoroutine(CheckSlots());
        textPanel = GameObject.Find("TextPanel");
        sceneTransition = GameObject.Find("Canvas").GetComponent<SceneTransition>();

        StartCoroutine(GetResolutions());
    }

    private IEnumerator GetResolutions()
    {
        // Nettoie la liste
        resolutionDropdown.ClearOptions();
        widthResolution.Clear();
        heightResolution.Clear();

        widthResolution.Add(1);
        heightResolution.Add(2);

        yield return new WaitForSeconds(0.5f);
        // Récupère les resolutions 16/9 et 4/3 de l'ecran
        List<string> resolutionsList = new List<string>();
        resolutionsList.Add("start");

        float[] actualResolutionArray = new float[2];

        Resolution[] resolutions = Screen.resolutions;
        foreach (var res in resolutions)
        {
            float width = res.width;
            float height = res.height;
            double ratio = System.Math.Round(width / height, 5);

            if (ratio == 1.77778) { // 16/9 resolutions
                if (widthResolution[(widthResolution.Count - 1)] != width && heightResolution[(heightResolution.Count - 1)] != height)
                {
                    resolutionsList.Add(width + " x " + height + " (16/9)");
                    widthResolution.Add(Convert.ToInt32(width));
                    heightResolution.Add(Convert.ToInt32(height));
                    if (Convert.ToInt32(width) == Screen.width) { actualResolutionArray[0] = width; }
                }
            }
            if (ratio == 1.33333) { // 4/3 resolutions
                if (widthResolution[(widthResolution.Count - 1)] != width && heightResolution[(heightResolution.Count - 1)] != height)
                {
                    resolutionsList.Add(width + " x " + height + " (4/3)");
                    widthResolution.Add(Convert.ToInt32(width));
                    heightResolution.Add(Convert.ToInt32(height));
                    if (Convert.ToInt32(height) == Screen.height) { actualResolutionArray[1] = height; }
                }
            }
        }

        yield return new WaitForSeconds(1f);
        widthResolution.Remove(1);
        heightResolution.Remove(2);
        resolutionsList.Remove("start");
        resolutionDropdown.AddOptions(resolutionsList);

        string actualRatioName = "";
        float actualWidth = Screen.width;
        float actualHeight = Screen.height;
        double actualRatio = System.Math.Round(actualWidth / actualHeight, 5);

        if (actualRatio == 1.77778) { actualRatioName = " (16/9)"; } // 16/9 resolutions
        if (actualRatio == 1.33333) { actualRatioName = " (4/3)"; } // 4/3 resolutions

        // Recherche la value du dropDown portant la résolution actuel
        int index = resolutionDropdown.options.FindIndex((i) => { return i.text.Equals(Screen.width + " x " + Screen.height + actualRatioName); });
        resolutionDropdown.value = index;
    }

    public void ResolutionSelection()
    {
        Screen.SetResolution(widthResolution[resolutionDropdown.value], heightResolution[resolutionDropdown.value], true);
        Debug.Log("Resolution en " + widthResolution[resolutionDropdown.value] + " x " + heightResolution[resolutionDropdown.value]);
    }

    IEnumerator CheckSlots()
    {
        // Insère les informations des variables "Slot" en jeu
        yield return new WaitForSeconds(0.5f);

        // Variable temporaire permettant de calculer le temps de jeu sur les 3 slots
        FloatValue playedTimeDump = saveManager.slot1Save[6] as FloatValue;
        playedTimeFloat[0] = playedTimeDump.RuntimeValue;

        playedTimeDump = saveManager.slot2Save[6] as FloatValue;
        playedTimeFloat[1] = playedTimeDump.RuntimeValue;

        playedTimeDump = saveManager.slot3Save[6] as FloatValue;
        playedTimeFloat[2] = playedTimeDump.RuntimeValue;

        for (int i = 0; i < 3; i++)
        {
            if (!saveManager.isSlotEmpty[i])
            {
                slotFull[i].alpha = 1;
                slotEmpty[i].alpha = 0;
                slotEmpty[i].interactable = false;
                slotEmpty[i].blocksRaycasts = false;

                int hoursTemp = Convert.ToInt32(playedTimeFloat[i] / 3600);
                int minutesTemp = Convert.ToInt32((playedTimeFloat[i] % 3600) / 60);
                int secondsTemp = Convert.ToInt32((playedTimeFloat[i] % 3600) % 60);

                string hours = "", minutes = "", seconds = "";

                // Conversion Heure, minutes et secondes
                if (hoursTemp < 10)
                { hours = "0" + hoursTemp.ToString(); }
                else { hours = hoursTemp.ToString(); }

                if (minutesTemp < 10)
                { minutes = "0" + minutesTemp.ToString(); }
                else { minutes = minutesTemp.ToString(); }

                if (secondsTemp < 10)
                { seconds = "0" + secondsTemp.ToString(); }
                else { seconds = secondsTemp.ToString(); }

                playedTime[i].text = "Temps " + hours + ":" + minutes + ":" + seconds;

            } else
            {
                slotFull[i].interactable = false;
                slotFull[i].blocksRaycasts = false;
                slotFull[i].alpha = 0;
                slotEmpty[i].alpha = 1;
            }
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

        // Si le jeu est dans le menu principal et que la touche retour est appuyé
        if (isInMainMenu)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                StartCoroutine(BackToPressStart());
                GameObject startSelectedButton = GameObject.Find("NewGame Button");
                eventSystem.SetSelectedGameObject(null);
            }
        }

        // Si le joueur est dans la phase "Press Start" et que la touche "X" est appuyé
        if (isPressStart)
        {
            if (Input.GetButtonDown("X"))
            {
                StartCoroutine(StartPressed());
                isPressStart = false;
            }
        }

        // Si le joueur est dans le "Slot Menu" et que la touche retour est appuyé -> "Main Menu"
        if (isInSlotMenu)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                SlotToMainMenuButton();
            }
        }

        // Si le joueur est dans les options et que la touche retour est appuyé -> "Main Menu"
        if (isInOptions)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                StartCoroutine(OptionsToMainMenu());
            }
        }

        // Si la touche "Y" est selectionné dans le slot menu
        if (saveManager.cursorSlot > 0)
        {
            if (Input.GetButtonDown("Y"))
            {
                StartCoroutine(YButtonProcedure());
            }
        }
    }

    IEnumerator YButtonProcedure()
    {
        // Demande de confirmation pour supprimer la sauvegarde selectionné
        Button yesButton = GameObject.Find("YesButton").GetComponent<Button>();

        // Creation d'evenement au clic du bouton
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => StartCoroutine(OnlyDeleteSave(saveManager.cursorSlot)));

        GameObject.Find("TextPanelMessage").GetComponent<TextMeshProUGUI>().text = "Etes vous sur de vouloir supprimer cette sauvegarde ?";
        slotMenu.GetComponent<Animator>().SetTrigger("Close");
        textPanel.GetComponent<Animator>().SetTrigger("open");
        isInSlotMenu = false;

        GameObject startSelectedButton = GameObject.Find("NoButton");
        EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        eventSystem.SetSelectedGameObject(startSelectedButton);

        yield return null;
    }

    private IEnumerator OnlyDeleteSave(int slot)
    {
        // Proccedure de suppression de sauvegarde et mise à jour des informations des slots
        fileAreDelete = true;

        StartCoroutine(saveManager.DeleteSave((slot - 1), false, true));

        saveManager.GetSlotsInfos();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(CheckSlots());
        
        yield return new WaitForSeconds(0.5f);

        slotMenu.GetComponent<Animator>().SetTrigger("Open");
        textPanel.GetComponent<Animator>().SetTrigger("close");

        yield return new WaitForSeconds(0.05f);

        GameObject startSelectedButton = GameObject.Find("Slot Button First");
        EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        eventSystem.SetSelectedGameObject(startSelectedButton);
    }

    IEnumerator BackToPressStart()
    {
        // Retour sur la phase "Press Start"
        isInMainMenu = false;
        menu1.GetComponent<Animator>().SetTrigger("Close");
        yield return new WaitForSeconds(0.30f);
        pressStart.SetTrigger("idle");
        yield return new WaitForSeconds(0.35f);
        isPressStart = true;
    }

    IEnumerator StartPressed()
    {
        // Animation de la phase "Press Start" puis passage au menu principal
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
        // Affichage du type de slot (Nouvelle partie ou Continuer)
        slotMode = value;
        isInMainMenu = false;
        if (value == "New Game") { slotModeTxt.text = "Nouvelle Partie"; }
        if (value == "Continue") { slotModeTxt.text = "Continuer"; }
        StartCoroutine(SlotModeButton());
    }

    IEnumerator SlotModeButton()
    {
        // Animation du "Slot Menu"
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
        // Animation du Menu d'options
        isInMainMenu = false;
        eventSystem.SetSelectedGameObject(null);

        menu1.GetComponent<Animator>().SetTrigger("Close");
        yield return new WaitForSeconds(0.30f);

        options.GetComponent<Animator>().SetTrigger("Open");
        yield return new WaitForSeconds(0.05f);
        eventSystem.SetSelectedGameObject(GameObject.Find("Resolution Dropdown"));
        isInOptions = true;
    }

    public void OptionsToMainMenuButton()
    {
        StartCoroutine(OptionsToMainMenu());
    }

    IEnumerator OptionsToMainMenu()
    {
        // Animation Options vers le Menu principal
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
        // Bouton pour quitter le jeu
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
        // Animation du Menu Principal vers le "Slot Menu"
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
        StartCoroutine(SlotButtonCo(value));
    }

    private IEnumerator SlotButtonCo(int slot)
    {
        if(slotMode == "Continue")
        {
            // Si le joueur est dans la partie "Continuer" et que le bouton du slot est appuyé
            if (!saveManager.isSlotEmpty[slot])
            {
                slotMenu.GetComponent<Animator>().SetTrigger("Close");
                saveManager.saveSlot.RuntimeValue = (slot + 1);
                saveManager.saveSlot.initialValue = (slot + 1);
                yield return new WaitForSeconds(0.5f);
                soundManager.soundEffectSource[1].clip = Resources.Load<AudioClip>("Audio/SE/Heart piece");
                soundManager.soundEffectSource[1].Play();
                yield return new WaitForSeconds(2f);
                StartCoroutine(sceneTransition.MainMenuTransitionCo("Continue", (slot + 1)));
            } else
            {
                yield return new WaitForSeconds(0.05f);
                soundManager.soundEffectSource[1].clip = Resources.Load<AudioClip>("Audio/SE/error");
                soundManager.soundEffectSource[1].Play();
            }
        }

        if (slotMode == "New Game")
        {
            // Si le joueur est dans la partie "Nouvelle partie" et que le bouton du slot est appuyé
            if (saveManager.isSlotEmpty[(slot)])
            {
                // Si le slot est vide, demarre directement une nouvelle partie
                if (fileAreDelete) { slotMenu.GetComponent<Animator>().SetTrigger("Close"); }
                StartCoroutine(StartNewGameCo(false, slot));
            } else
            {
                // Si le slot n'est pas vide, demande la confirmation pour effacer le slot
                slotMenu.GetComponent<Animator>().SetTrigger("Close");
                textPanel.GetComponent<Animator>().SetTrigger("open");
                isInSlotMenu = false;
                yield return new WaitForSeconds(0.05f);
                
                GameObject startSelectedButton = GameObject.Find("NoButton");
                EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
                eventSystem.SetSelectedGameObject(startSelectedButton);

                
                Button yesButton = GameObject.Find("YesButton").GetComponent<Button>();
                yesButton.onClick.RemoveAllListeners();
                yesButton.onClick.AddListener(() => YesEraseNewGameButton((slot)));
            }
        }
    }

    public void NoButton()
    {
        StartCoroutine(NoButtonCo());
    }

    private IEnumerator NoButtonCo()
    {
        // Retour au slot menu si le bouton Non est confirmé
        textPanel.GetComponent<Animator>().SetTrigger("close");
        slotMenu.GetComponent<Animator>().SetTrigger("Open");
        isInSlotMenu = true;

        yield return new WaitForSeconds(0.3f);

        GameObject startSelectedButton = GameObject.Find("Slot Button First");
        EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        eventSystem.SetSelectedGameObject(startSelectedButton);
    }

    private void YesEraseNewGameButton(int slot)
    {
        // Procedure d'ecrasement de fichier
        StartCoroutine(StartNewGameCo(true, slot));
        StartCoroutine(saveManager.DeleteSave(slot, false, false));
    }

    private IEnumerator StartNewGameCo(bool isStringText, int slot)
    {
        // Procedure de nouvelle partie
        yield return new WaitForSeconds(0.05f);
        Debug.Log("Processus de nouvelle partie");
        soundManager.soundEffectSource[1].clip = Resources.Load<AudioClip>("Audio/SE/Heart piece");
        soundManager.soundEffectSource[1].Play();
        StartCoroutine(saveManager.DeleteSave(slot, true, false));
        if (isStringText) { textPanel.GetComponent<Animator>().SetTrigger("close"); }
        if (isInSlotMenu) { slotMenu.GetComponent<Animator>().SetTrigger("Close"); }
        isNewGame.initialValue = true;
        saveManager.saveSlot.RuntimeValue = (slot + 1);
        saveManager.saveSlot.initialValue = (slot + 1);
        yield return new WaitForSeconds(2f);
        StartCoroutine(sceneTransition.MainMenuTransitionCo("New Game", (slot +1)));
    }
}
