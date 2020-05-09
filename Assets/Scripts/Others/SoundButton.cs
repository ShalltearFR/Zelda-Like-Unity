using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundButton : MonoBehaviour
{
    private AudioSource[] audiosource = new AudioSource[2];
    private SoundManagement soundManagement;
    private void Start()
    {
        soundManagement = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagement>();
        audiosource[0] = soundManagement.soundEffectSource[0];
        audiosource[1] = soundManagement.soundEffectSource[1];
    }

    private void Cursor()
    {
        audiosource[0].clip = Resources.Load<AudioClip>("Audio/SE/Cursor");
        audiosource[0].Play();
    }

    private void Select()
    {
        audiosource[1].clip = Resources.Load<AudioClip>("Audio/SE/Message Finish");
        audiosource[1].Play();
    }
}
