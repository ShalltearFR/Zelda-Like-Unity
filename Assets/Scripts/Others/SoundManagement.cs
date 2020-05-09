using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManagement : MonoBehaviour
{
    public AudioSource[] soundEffectSource;
    public AudioSource[] musicSource;

    private MainMenu mainMenu;
    private Slider musicSlider;
    private Slider soundSlider;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            mainMenu = GameObject.Find("Canvas").GetComponent<MainMenu>();
            musicSlider = GameObject.Find("Music Slider").GetComponent<Slider>();
            soundSlider = GameObject.Find("Sound Slider").GetComponent<Slider>();
        }
        StartCoroutine(initMusicVariable());
    }

    IEnumerator initMusicVariable()
    {
        yield return null;
        musicSource[0] = GameObject.FindGameObjectsWithTag("MusicManager")[0].transform.GetChild(0).GetComponent<AudioSource>();
        musicSource[1] = GameObject.FindGameObjectsWithTag("MusicManager")[0].transform.GetChild(1).GetComponent<AudioSource>();
        musicSource[2] = GameObject.FindGameObjectsWithTag("MusicManager")[1].transform.GetChild(0).GetComponent<AudioSource>();
        musicSource[3] = GameObject.FindGameObjectsWithTag("MusicManager")[1].transform.GetChild(1).GetComponent<AudioSource>();
    }

    public void UpdateMusicVolume()
    {
        musicSource[0].volume = musicSlider.value;
        musicSource[1].volume = musicSlider.value;
        musicSource[2].volume = musicSlider.value;
        musicSource[3].volume = musicSlider.value;

        soundEffectSource[1].clip = Resources.Load<AudioClip>("Audio/SE/Cursor");
        soundEffectSource[1].Play();
    }

    public void UpdateSoundVolume()
    {
        soundEffectSource[0].volume = soundSlider.value;
        soundEffectSource[1].volume = soundSlider.value;
        soundEffectSource[2].volume = soundSlider.value;
        soundEffectSource[3].volume = soundSlider.value;
        soundEffectSource[4].volume = soundSlider.value;

        soundEffectSource[1].clip = Resources.Load<AudioClip>("Audio/SE/Cursor");
        soundEffectSource[1].Play();
    }
}
