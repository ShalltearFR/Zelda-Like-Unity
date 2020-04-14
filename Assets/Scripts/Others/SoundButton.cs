using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundButton : MonoBehaviour
{
    private AudioSource audiosource;
    private SoundManagement soundManagement;

    private void Start()
    {
        soundManagement = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagement>();
        audiosource = soundManagement.audioSource[0];
    }

    private void Cursor()
    {
        audiosource.clip = Resources.Load<AudioClip>("Audio/SE/Cursor");
        audiosource.Play();
    }

    private void Select()
    {
        audiosource.clip = Resources.Load<AudioClip>("Audio/SE/Message Finish");
        audiosource.Play();
    }
}
