using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    public float speed;
    public Rigidbody2D myRigidBody;
    private SoundManagement soundManagement;
    private int i;
    private Transform player;
    private bool autoDestroy;
    private Transform attachedItem;
    private string typeOfItem;

    private void Awake()
    {
        soundManagement = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagement>();
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    public void Setup(Vector2 velocity, Vector3 direction)
    {
        // Oriente la fleche dans la bonne direction et la fait avancer
        myRigidBody.velocity = velocity.normalized * speed;
        transform.rotation = Quaternion.Euler(direction);
        InvokeRepeating("BoomerangRotation", 0.0f, 0.02f);
        InvokeRepeating("BoomerangReturn", 0.75f, 0.01f);
        InvokeRepeating("enableAutoDestroy", 0.75f, 0f);
        StartCoroutine(SoundEffect());
    }

    IEnumerator SoundEffect()
    {
        int i = 0;
        for (i = 0; i < 5; i++)
        {
            soundManagement.soundEffectSource[5].clip = Resources.Load<AudioClip>("Audio/SE/Boomerang");
            soundManagement.soundEffectSource[5].Play();

            yield return new WaitForSeconds(0.15f);
        }
        yield return null;
    }

    private void enableAutoDestroy()
    {
        // Active la destruction du boomerang s'il touche le joueur
        autoDestroy = true;
    }

    private void BoomerangRotation()
    {
        // Effectue la rotation du boomerang
        transform.rotation = Quaternion.Euler(0,0,i);
        i -= 15;
    }

    private void BoomerangReturn()
    {
        // Retour du boomerang vers le joueur
        myRigidBody.velocity = new Vector2(0,0);
        Vector3 temp = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        myRigidBody.MovePosition(temp);
    }


    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Sign") || other.gameObject.CompareTag("Pot") || other.gameObject.CompareTag("Chest"))
        {
            if (!other.isTrigger)
            {
                // S'il touche un objet ou un mur, retour au joueur
                myRigidBody.velocity = new Vector2(0, 0);
                InvokeRepeating("BoomerangReturn", 0.0f, 0.01f);
                autoDestroy = true;
            }
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            if (!other.GetComponent<Enemy>().isEnemyFreeze)
            {
                // S'il touche un ennemis, effectue le freeze
                myRigidBody.velocity = new Vector2(0, 0);
                InvokeRepeating("BoomerangReturn", 0.0f, 0.01f);
                autoDestroy = true;

                other.GetComponent<Enemy>().BoomerangEffect();
            }

        }

        if (other.gameObject.CompareTag("Player"))
        {
            if (autoDestroy)
            {
                if (attachedItem != null)
                {
                    // Si le boomerang touche le joueur
                    attachedItem.transform.SetParent(player.gameObject.transform);
                    attachedItem.transform.position = player.position;
                    //InvokeRepeating("waitForDestroy", 0.5f, 0.00f);

                    // Si le boomerang à avec lui un objet de type rubis
                    if (typeOfItem == "Rubis")
                    { attachedItem.GetComponent<Rubis>().RubisOperation(); }

                    // Si le boomerang à avec lui un objet de type coeur
                    else if (typeOfItem == "Heart")
                    { attachedItem.GetComponent<Heart>().HeartProcedure(); }

                    // Détruit le boomerang
                    Destroy(this.gameObject);
                } else { Destroy(this.gameObject); }
            }
        }

        if (other.gameObject.CompareTag("Rubis") || other.gameObject.CompareTag("Heart"))
        {
            if (attachedItem == null)
            {
                // Si le boomerang récupère un objet de type rubis
                if (other.gameObject.CompareTag("Rubis"))
                { typeOfItem = "Rubis"; }

                // Si le boomerang récupère un objet de type coeur
                if (other.gameObject.CompareTag("Heart"))
                { typeOfItem = "Heart"; }

                // Attache le gameobject rubis/coeur au boomerang et procedure de retour du joueur
                myRigidBody.velocity = new Vector2(0, 0);
                InvokeRepeating("BoomerangReturn", 0.0f, 0.01f);
                autoDestroy = true;
                other.gameObject.transform.SetParent(this.gameObject.transform);
                attachedItem = other.transform;
            }
        }
    }

    private void waitForDestroy()
    {
        Destroy(this.gameObject);
    }

    private void Update()
    {
        if (attachedItem != null)
        {
            // Evite la rotation du gameobjet de type coeur/rubis quand il est attaché au gameobject
            attachedItem.rotation = Quaternion.Euler(0, 0, 0);
            attachedItem.localPosition = new Vector3(0, 0, 0);
        }
    }
}
