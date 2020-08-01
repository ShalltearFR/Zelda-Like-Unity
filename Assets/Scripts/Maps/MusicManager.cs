using UnityEngine;
using E7.Introloop;

public class MusicManager : MonoBehaviour
{
    public IntroloopAudio intro;
    public IntroloopAudio[] music;
    public IntroloopAudio[] danger;
    public bool isMultipleFileMusic;
    public int random;
    public string scenename;

    private void Start()
    {
        // Joue l'intro suivant le type de monde
        if (scenename == "Overworld") {
            isMultipleFileMusic = true;
            IntroloopPlayer.Instance.Play(intro);
        } else if (scenename == "Dungeon"){ IntroloopPlayer.Instance.Play(intro);
        } else if (scenename == "Main Menu") { IntroloopPlayer.Instance.Play(intro); }
    }

    public void ChangeMusic()
    {
        // Joue un bout de musique dangereux aléatoirement
        if (scenename == "Overworld")
        {
            random = Random.Range(0, 2);
            IntroloopPlayer.Instance.Play(danger[random], 0.25f);
        }
    }

    public void baseMusic()
    {
        // Joue un bout de musique calme aléatoirement
        if (scenename == "Overworld")
        {
            random = Random.Range(0, 4);
            IntroloopPlayer.Instance.Play(music[random], 0.25f);
        }
    }

    private void Update()
    {
        // Une fois l'intro terminé, passe sur les fichiers multiple de musiques
        if (isMultipleFileMusic)
        {
            if ((IntroloopPlayer.Instance.GetPlayheadTime()) >= intro.ClipLength)
            {
                isMultipleFileMusic = false;
                baseMusic();
            }
        }
    }
}
