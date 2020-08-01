using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeathHUD : MonoBehaviour
{
    public bool isDeath = false;
    private GameObject DeathGameobject;
    private Animator animator;
    private SoundManagement soundManagement;

    // Start is called before the first frame update

    private void Start()
    {
        DeathGameobject = GameObject.Find("Death HUD");
        animator = gameObject.GetComponent<Animator>();
        soundManagement = GameObject.Find("Sound Manager").GetComponent<SoundManagement>();
    }

    public IEnumerator Init()
    {
        // Initialise l'HUD de mort
        isDeath = true;
        animator.SetTrigger("on");

        soundManagement.soundEffectSource[5].volume = 0;


        GameObject startSelectedButton = GameObject.Find("Save&Continue");
        EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        yield return new WaitForSeconds(0.05f);
        eventSystem.SetSelectedGameObject(startSelectedButton);

        
    }

    private void Update()
    {
        if (isDeath)
        {



        }
    }
}
