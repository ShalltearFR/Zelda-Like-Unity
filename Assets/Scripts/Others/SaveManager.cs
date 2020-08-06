using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;
using UnityEngine.UI;


// objects[0] (asSword) n'est pas utilisé

public class SaveManager : MonoBehaviour
{
    public string dataPath;
    public FloatValue saveSlot;

    [Header("Menu Principal")]
    public bool[] isSlotEmpty = new bool[3];
    private Transform playerPos;
    public int cursorSlot;
    private int currentSlot = -1;

    [Header("Slot 1")]
    public List<ScriptableObject> slot1Save = new List<ScriptableObject>();

    [Header("Slot 2")]
    public List<ScriptableObject> slot2Save = new List<ScriptableObject>();

    [Header("Slot 3")]
    public List<ScriptableObject> slot3Save = new List<ScriptableObject>();

    [Header("Slot Temporaire")]
    public List<ScriptableObject> objects = new List<ScriptableObject>();
    private GameObject activeMap;
    public StringValue onlyTeleport;
    public StringValue selectedItem;
    public Sprite[] itemDescriptionImage;

    private void Awake()
    {
        dataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
        if (saveSlot.RuntimeValue == 0) { saveSlot.RuntimeValue = 1; }

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            dataPath += "/my games/ZeldaLike/Save " + saveSlot.RuntimeValue;
        } else
        {
            dataPath += "/my games/ZeldaLike/";
        }

        #if UNITY_EDITOR
        #else
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        #endif
    }

    public void GetSlotsInfos()
    {
        // Recupère les informations des différentes sauvegardes
        BinaryFormatter binary = new BinaryFormatter();

        for (int i = 0; i < objects.Count; i++)
        {
            if (File.Exists(dataPath + "Save 1/0.data"))
            {
                FileStream file = File.Open(dataPath + "Save 1/" + i + ".data", FileMode.Open);
                JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), slot1Save[i]);
                file.Close();
            } else { isSlotEmpty[0] = true; }

            if (File.Exists(dataPath + "Save 2/0.data"))
            {
                FileStream file = File.Open(dataPath + "Save 2/" + i + ".data", FileMode.Open);
                JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), slot2Save[i]);
                file.Close();
            } else { isSlotEmpty[1] = true; }

            if (File.Exists(dataPath + "Save 3/0.data"))
            {
                FileStream file = File.Open(dataPath + "Save 3/" + i + ".data", FileMode.Open);
                JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), slot3Save[i]);
                file.Close();
            } else { isSlotEmpty[2] = true; }
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            StartCoroutine(LoadScriptiblesCo(false));
            playerPos = GameObject.FindWithTag("Player").GetComponent<Transform>();
        } else
        {
            GetSlotsInfos();
        }
    }

    public void ResetScriptibles()
    {
        // Supprime toutes les sauvegardes
        for (int i = 0; i < objects.Count; i++)
        {
            if (File.Exists(dataPath + string.Format("/{0}.data", i)))
            {
                File.Delete(dataPath + string.Format("/{0}.data", i));
            }
        }
        StartCoroutine(Shutdown());
    }

    public IEnumerator DeleteSave(int value, bool isEmptySlot, bool isOnlyDelete)
    {
        // Ré-initialise les variables pour demarrer une nouvelle partie
#if UNITY_EDITOR
        FileUtil.DeleteFileOrDirectory(dataPath + "Save " + (value + 1));
#else
        Directory.Delete(dataPath + "Save " + (value + 1), true);
#endif

        currentSlot = (value + 1);

        VectorValue position = objects[2] as VectorValue;
        position.teleporationValue = new Vector3(0, 0);
        position.initialValue = new Vector3(0, 0);
        position.defaultValue = new Vector3(0, 0);
        objects[2] = position;

        FloatValue playedTime = objects[6] as FloatValue;
        playedTime.initialValue = 0;
        playedTime.RuntimeValue = 0;
        objects[6] = playedTime;

        BoolValue asSword = objects[0] as BoolValue;
        asSword.initialValue = false;
        objects[0] = asSword;

        FloatValue heartContainers = objects[1] as FloatValue;
        heartContainers.initialValue = 3;
        heartContainers.RuntimeValue = 3;
        objects[1] = heartContainers;

        FloatValue health = objects[3] as FloatValue;
        health.initialValue = 6;
        health.RuntimeValue = 6;
        objects[3] = health;

        StringValue worldName = objects[7] as StringValue;
        worldName.initialValue = "Overworld";
        worldName.RuntimeValue = "Overworld";
        objects[7] = worldName;

        Inventory playerInventory = objects[4] as Inventory;
        playerInventory.items.Clear();
        playerInventory.itemsName.Clear();
        playerInventory.currentItem = null;
        playerInventory.numberofKeys = 0;
        playerInventory.rubis = 0;
        playerInventory.rubisTemp = 0;
        objects[4] = playerInventory;

        if (isEmptySlot) { dataPath = dataPath + "Save " + (value + 1); }
        yield return new WaitForSeconds(0.5f);

        if (!isOnlyDelete) { SaveScriptibles(); }
    }

    private IEnumerator Shutdown()
    {
        // Ferme le jeu sous sa version editor
        yield return new WaitForSeconds(1f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void SaveButton()
    {
        StartCoroutine(SaveButtonCo());
        currentSlot = Convert.ToInt32(saveSlot.initialValue);
    }

    IEnumerator SaveButtonCo()
    {
        // Prepare la sauvegarde du jeu
        StringValue saveWorldName = objects[7] as StringValue;
        saveWorldName.RuntimeValue = SceneManager.GetActiveScene().name;
        objects[7] = saveWorldName;

        VectorValue position = objects[2] as VectorValue;
        position.teleporationValue = new Vector3(playerPos.position.x, playerPos.position.y, -3);
        objects[2] = position;

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            StringValue Map = objects[8] as StringValue;
            Map.RuntimeValue = GameObject.Find("Main Camera").GetComponent<CameraMovement>().activeMap.name;
            objects[8] = Map;
        }

        yield return null;
        SaveScriptibles();
    }

    public void SaveScriptibles()
    {
        // Sauvegarde le jeu dans le slot correspondant
        if (!Directory.Exists(dataPath))
        {
            // Creation du dossier de sauvegarde s'il n'existe pas
            Directory.CreateDirectory(dataPath);
            Debug.Log("Creation du dossier de sauvegarde du jeu");
        }

        for (int i = 0; i < objects.Count; i++)
        {
            // Sauvegarde les variables dans des fichiers
            FileStream file = File.Create(dataPath + string.Format("/{0}.data", i));
            BinaryFormatter binary = new BinaryFormatter();
            var json = JsonUtility.ToJson(objects[i]);
            binary.Serialize(file, json);
            file.Close();
        }

        StartCoroutine(LoadScriptiblesCo(true));
    }

    public IEnumerator LoadScriptiblesCo(bool isLoadSlot)
    {
        if (onlyTeleport.RuntimeValue == "")
        {
            for (int i = 0; i < objects.Count; i++)
            {
                // Lit le contenu des fichiers et les insèrent dans les variables correspondante
                if (File.Exists(dataPath + string.Format("/{0}.data", i)))
                {
                    FileStream file = File.Open(dataPath + string.Format("/{0}.data", i), FileMode.Open);
                    BinaryFormatter binary = new BinaryFormatter();
                    if (isLoadSlot)
                    {
                        // Charge les fichiers dans les variables "Slot"
                        if (currentSlot == 1) { JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), slot1Save[i]); }
                        if (currentSlot == 2) { JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), slot2Save[i]); }
                        if (currentSlot == 3) { JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), slot3Save[i]); }
                    }
                    else
                    {
                        // Charge les fichiers dans les variables "Temporaire"
                        JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), objects[i]);
                    }
                    file.Close();
                }
            }
        } else { onlyTeleport.RuntimeValue = ""; }
        yield return null;
    }

    private void OnApplicationQuit()
    {
        selectedItem.RuntimeValue = "";
        selectedItem.initialValue = "";
        onlyTeleport.RuntimeValue = "";
        onlyTeleport.initialValue = "";

        Inventory inventory = objects[4] as Inventory;
        inventory.items.Clear();
        inventory.itemsName.Clear();
        inventory.numberofKeys = 0;
        inventory.rubis = 0;
        inventory.rubisTemp = 0;
        objects[4] = inventory;

        BoolArrayValue tresureChest = objects[9] as BoolArrayValue;
        int i;
        for (i = 0; i < tresureChest.initialValue.Length; i++)
        {
            tresureChest.initialValue[i] = false;
        }
    }
}
