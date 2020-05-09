using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script gerant l'interraction avec un pot

public class Pot : Interactable
{
    // Initialise les variables
    private Animator anim;
    public Object[] soundsEffect;
    private AudioSource audioSource;
    public BoxCollider2D[] boxCollider;
    public BoxCollider2D playerBoxCollider;
    public LootTable thisLoot;

    void Start()
    {
        // Attribut des valeurs sur certaines variables
        anim = GetComponent<Animator>();
        audioSource = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagement>().soundEffectSource[0];
    }

    public void Smash()
    {
        // Si le joueur donne un coup d'épée
        anim.SetBool("smash", true);
        
        audioSource.clip = soundsEffect[0] as AudioClip;
        audioSource.Play();

        StartCoroutine(breakCo());
    }

    IEnumerator breakCo()
    {
        yield return new WaitForSeconds(0.35f);
        MakeLoot();
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interract")) // Fais porter la pancarte
        {
            if (boxCollider[1].IsTouching(playerBoxCollider) || boxCollider[2].IsTouching(playerBoxCollider))
            {
                StartCoroutine(TakeObjectCo());
            }
        }
    }

    private void MakeLoot()
    {
        if (thisLoot != null)
        {
            Powerup current = thisLoot.LootPowerup();
            if (current != null)
            {
                Instantiate(current.gameObject, transform.position, Quaternion.identity);
            }
        }
    }

    IEnumerator TakeObjectCo()
    {
        // Supprime le gameobject de pancarte et effectue une animation où le joueur soulève le pot
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().currentState = PlayerMovement.PlayerState.interact;
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().speed = 3;
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().takeObjectString = "Pot";
        enable = false;
        playerInRange = false;
        contextOff.Raise();
        boxCollider[0].isTrigger = true;
        boxCollider[0].enabled = false;
        boxCollider[1].enabled = false;
        GameObject.FindWithTag("Player").GetComponent<Animator>().SetBool("Lifting", true);
        yield return new WaitForSeconds(0.7f);

        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;

        GameObject.FindWithTag("Player").GetComponent<Animator>().SetBool("Lifting", false);

        GameObject.FindWithTag("Player").GetComponent<Animator>().SetBool("Pot", true);
        GameObject.FindWithTag("SoundManager").GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Audio/SE/Lift");
        GameObject.FindWithTag("SoundManager").GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(0.5f);
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().currentState = PlayerMovement.PlayerState.takeObject;
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().isTakingObject = true;

        MakeLoot();
        this.gameObject.SetActive(false);


    }
}
