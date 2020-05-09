using UnityEngine;
using E7.Introloop;

public class MusicManager : MonoBehaviour
{
    public IntroloopAudio music;

    private void Start()
    {
        IntroloopPlayer.Instance.Play(music);
    }
}
