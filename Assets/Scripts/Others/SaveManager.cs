using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Environment = System.Environment;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public string dataPath;
    public FloatValue saveSlot;

    [Header("Slot 1")]
    public bool isSlot1Empty;
    public ScriptableObject Slot1HeartContainers;
    public ScriptableObject Slot1Health;
    public FloatValue Slot1PlayedTime;

    [Header("Slot 2")]
    public bool isSlot2Empty = false;
    public ScriptableObject Slot2HeartContainers;
    public ScriptableObject Slot2Health;
    public FloatValue Slot2PlayedTime;

    [Header("Slot 3")]
    public bool isSlot3Empty = false;
    public ScriptableObject Slot3HeartContainers;
    public ScriptableObject Slot3Health;
    public FloatValue Slot3PlayedTime;

    public List<ScriptableObject> objects = new List<ScriptableObject>();

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
    }

    public void GetSlotsInfos()
    {
        BinaryFormatter binary = new BinaryFormatter();

        // Slot 1
        if (File.Exists(dataPath + "Save 1/1.data"))
        {
            FileStream file = File.Open(dataPath + "Save 1/1.data", FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), Slot1HeartContainers);
            file.Close();
        } else { isSlot1Empty = true;}

        if (File.Exists(dataPath + "Save 1/3.data"))
        {
            FileStream file = File.Open(dataPath + "Save 1/3.data", FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), Slot1Health);
            file.Close();
        }

        if (File.Exists(dataPath + "Save 1/6.data"))
        {
            FileStream file = File.Open(dataPath + "Save 1/6.data", FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), Slot1PlayedTime);
            file.Close();
        }

        // Slot 2
        if (File.Exists(dataPath + "Save 2/1.data"))
        {
            FileStream file = File.Open(dataPath + "Save 2/1.data", FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), Slot2HeartContainers);
            file.Close();
        } else { isSlot2Empty = true; }

        if (File.Exists(dataPath + "Save 2/3.data"))
        {
            FileStream file = File.Open(dataPath + "Save 2/3.data", FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), Slot2Health);
            file.Close();
        }

        if (File.Exists(dataPath + "Save 2/6.data"))
        {
            FileStream file = File.Open(dataPath + "Save 2/6.data", FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), Slot2PlayedTime);
            file.Close();
        }

        // Slot 3
        if (File.Exists(dataPath + "Save 3/1.data"))
        {
            FileStream file = File.Open(dataPath + "Save 3/1.data", FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), Slot3HeartContainers);
            file.Close();
        } else { isSlot3Empty = true; }

        if (File.Exists(dataPath + "Save 3/3.data"))
        {
            FileStream file = File.Open(dataPath + "Save 3/3.data", FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), Slot3Health);
            file.Close();
        }

        if (File.Exists(dataPath + "Save 3/6.data"))
        {
            FileStream file = File.Open(dataPath + "Save 3/6.data", FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), Slot3PlayedTime);
            file.Close();
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            LoadScriptibles();
        } else
        {
            GetSlotsInfos();
        }
    }

    public void ResetScriptibles()
    {
        Debug.Log("Suppression des sauvegardes");

        for (int i = 0; i < objects.Count; i++)
        {
            if (File.Exists(dataPath + string.Format("/{0}.data", i)))
            {
                File.Delete(dataPath + string.Format("/{0}.data", i));
            }
        }
        StartCoroutine(Shutdown());
    }

    private IEnumerator Shutdown()
    {
        yield return new WaitForSeconds(1f);
        UnityEditor.EditorApplication.isPlaying = false;
    }

    public void SaveButton()
    {
        SaveScriptibles();
        
    }

    public void SaveScriptibles()
    {
        Debug.Log("Sauvegarde du jeu : " + dataPath);
        if (!Directory.Exists(dataPath))
        {
            //if it doesn't, create it
            Directory.CreateDirectory(dataPath);
            Debug.Log("Creation du dossier de sauvegarde du jeu");
        }

        for (int i = 0; i < objects.Count; i++)
        {
            FileStream file = File.Create(dataPath + string.Format("/{0}.data", i));
            BinaryFormatter binary = new BinaryFormatter();
            var json = JsonUtility.ToJson(objects[i]);
            binary.Serialize(file, json);
            file.Close();
        }

       // Debug.Log("Sauvegarde du jeu : " + dataPath);
        
    }

    public void LoadScriptibles()
    {
        Debug.Log("Chargement du jeu : " + dataPath);
        for (int i = 0; i < objects.Count; i++)
        {
            if (File.Exists(dataPath + string.Format("/{0}.data", i)))
            {
                FileStream file = File.Open(dataPath + string.Format("/{0}.data", i), FileMode.Open);
                BinaryFormatter binary = new BinaryFormatter();
                JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), objects[i]);
                file.Close();
            }
        }
    }
}
