using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundButton : MonoBehaviour
{
    private AudioSource[] audiosource = new AudioSource[2];
    private SoundManagement soundManagement;
    private SaveManager saveManager;

    private void Start()
    {
        soundManagement = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagement>();
        audiosource[0] = soundManagement.soundEffectSource[0];
        audiosource[1] = soundManagement.soundEffectSource[1];
        saveManager = GameObject.Find("Save Manager").GetComponent<SaveManager>();
    }

    private void Cursor()
    {
        // Si le cursor est sur le bouton
        audiosource[0].clip = Resources.Load<AudioClip>("Audio/SE/Cursor");
        audiosource[0].Play();
    }

    private void Select()
    {
        // Si la touche de confirmation est appuyé
        audiosource[1].clip = Resources.Load<AudioClip>("Audio/SE/Message Finish");
        audiosource[1].Play();
    }

    private void CursorSlot()
    {
        // Permet de detecter le type de slot selon la position du curseur
        int slot = 0;
        if (gameObject.name == "Slot Button First") { slot = 1; }
        if (gameObject.name == "Slot Button Second") { slot = 2; }
        if (gameObject.name == "Slot Button Third") { slot = 3; }

        saveManager.cursorSlot = slot;
    }
}
