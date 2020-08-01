using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sign : Interactable
{
    public GameObject dialogBox;
    public Text dialogText;
    public string dialog;

    public GameObject takeObject;

    private Animator anim;
    public Object[] soundsEffect;
    private AudioSource audioSource;
    public BoxCollider2D[] boxCollider;
    public BoxCollider2D playerBoxCollider;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagement>().soundEffectSource[0]; ;
    }

    void Update()
    {
        if (Input.GetButtonDown("Interract") && playerInRange && boxCollider[1].IsTouching(playerBoxCollider)) // Fais lire le message de la pancarte
        {
            if (dialogBox.activeInHierarchy)
            {
                dialogBox.SetActive(false);
                GameObject.FindWithTag("ContextClue").GetComponent<SpriteRenderer>().enabled = true;
            } else
            {
                dialogBox.SetActive(true);
                GameObject.FindWithTag("ContextClue").GetComponent<SpriteRenderer>().enabled = false;
                dialogText.text = dialog;
            }
        }

        if (Input.GetButtonDown("Interract") && playerInRange && boxCollider[2].IsTouching(playerBoxCollider)) // Fais porter la pancarte
        {
            StartCoroutine(TakeObjectCo());
        }
    }

    IEnumerator TakeObjectCo()
    {
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().currentState = PlayerMovement.PlayerState.interact;
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().speed = 3;
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().takeObjectString = "Sign";
        enable = false;
        playerInRange = false;
        boxCollider[0].isTrigger = true;
        boxCollider[0].enabled = false;
        boxCollider[1].enabled = false;
        boxCollider[2].enabled = false;
        GameObject.FindWithTag("Player").GetComponent<Animator>().SetBool("Lifting", true);
        yield return new WaitForSeconds(0.7f);

        this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;

        GameObject.FindWithTag("Player").GetComponent<Animator>().SetBool("Lifting", false);
        
        GameObject.FindWithTag("Player").GetComponent<Animator>().SetBool("Sign", true);
        gameObject.GetComponent<Animator>().SetBool("SignTake", true);

        audioSource.clip = Resources.Load<AudioClip>("Audio/SE/Lift");
        audioSource.Play();

        yield return new WaitForSeconds(0.5f);
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().currentState = PlayerMovement.PlayerState.takeObject;
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().isTakingObject = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            contextOff.Raise();
            playerInRange = false;
            dialogBox.SetActive(false);
        }
    }

    public void Smash()
    {
        this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        anim.SetBool("smash", true);

        StartCoroutine(breakCo());
    }

    IEnumerator breakCo()
    {
        if (enable) // Evite le double son de la pancarte qui se casse
        {
            audioSource.clip = soundsEffect[0] as AudioClip;
            audioSource.Play();
        }

        yield return new WaitForSeconds(0.35f);
        enable = false;
        boxCollider[0].enabled = false;
        boxCollider[1].enabled = false;
        boxCollider[2].enabled = false;
        this.gameObject.GetComponent<Sign>().enabled = false;
    }
}
