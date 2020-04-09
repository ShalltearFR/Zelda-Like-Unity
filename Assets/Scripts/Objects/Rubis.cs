using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rubis : Powerup
{
    public Inventory playerInventory;
    private AudioSource audioSource;
    public int NumberOfRubis;
    private AudioSource audioSource2;

    private void Start()
    {
        powerupSignal.Raise();
        audioSource = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagement>().audioSource[0];
        audioSource2 = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagement>().audioSource[4];
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
            {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            audioSource.clip = Resources.Load<AudioClip>("Audio/SE/Get Rubis");
            audioSource.Play();

            if (playerInventory.rubisTemp == 0)
            { playerInventory.rubisTemp = playerInventory.rubis + NumberOfRubis; }
            else
            { playerInventory.rubisTemp = playerInventory.rubisTemp + NumberOfRubis; }

            StartCoroutine(rubisCountAnimation());
        }
    }

    private IEnumerator rubisCountAnimation()
    {
        int i = playerInventory.rubis;

        for (i = playerInventory.rubis; playerInventory.rubis < playerInventory.rubisTemp; i++)
        {
            if (playerInventory.rubis <= 998)
            {
                audioSource2.clip = Resources.Load<AudioClip>("Audio/SE/Get Rubis");
                audioSource2.Play();

                playerInventory.rubis++;
                powerupSignal.Raise();
                yield return new WaitForSeconds(0.06f);
            }
        }
        Destroy(this.gameObject);
    }
}
