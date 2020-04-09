using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script global pour l'IA ennemis (lié au script Knockback)
public enum EnemyState
{
    idle,
    walk,
    attack,
    stagger,
    death,
}

public class Enemy : MonoBehaviour
{
    // Initialisation des variables
    public EnemyState currentState;
    public FloatValue maxHealth;
    public float health;
    public string enemyName;
    public int baseAttack;
    public float moveSpeed;
    private AudioSource audioSource;
    private Animator animator;
    public Material matDefault;
    public Material matFlash;
    public SpriteRenderer sr;
    public bool isActuallyRandomMoving;
    public LootTable thisLoot;

    private void Awake()
    {
        // Attribut les valeurs de certaines variables au demarrage
        health = maxHealth.initialValue;
        audioSource = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagement>().audioSource[1]; ;
        animator = GetComponent<Animator>();
        matFlash = Resources.Load("Graphics/Effects/Flash Effect", typeof(Material)) as Material;
        matDefault = gameObject.GetComponent<SpriteRenderer>().material;
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float damage, KnockBack.TypeOfMob typeOfMob)
    {
        health -= damage;

        // Si mob est encore en vie
        if (health > 0)
        {
            // Si le mob est un ennemis standart, joue le son de degat de mob standart
            if (typeOfMob == KnockBack.TypeOfMob.Classic)
            {
                audioSource.clip = Resources.Load<AudioClip>("Audio/SE/Enemy Hurt");
                audioSource.Play();
            }

            // Si le mob est un boss, joue le son de degats de boss
            if (typeOfMob == KnockBack.TypeOfMob.Boss)
            {

            }

            // Lance la coroutine animation degats
            StartCoroutine(DamageAnimation());
        }
        // Si mob meurt
        if (health <= 0)
        {
            // Lance la coroutine animation de mort puis suppréssion du gameobject
            StartCoroutine(deleteEnemy(typeOfMob));
        }
    }

    IEnumerator DamageAnimation()
    {
        // Repète une animation clignotant blanc 10 fois
        // Uniquement quand il prend des dommages mais ne meurt pas
        int i;
        for (i = 0; i < 10; i++)
        {
            sr.material = matFlash;
            yield return new WaitForSeconds(0.015f);
            sr.material = matDefault;
            yield return new WaitForSeconds(0.015f);
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

    IEnumerator deleteEnemy(KnockBack.TypeOfMob typeOfMob)
    {
        // baseAttack et moveSpeed mis à 0 pour rendre le mob inoffensif car il est mort
        baseAttack = 0;
        moveSpeed = 0;
        currentState = EnemyState.death;
        // Effectue l'animation de mort
        animator.SetBool("death", true);

        // Si le mob est un ennemis standart, joue le son de mort de mob standart
        if (typeOfMob == KnockBack.TypeOfMob.Classic)
        {
            audioSource.clip = Resources.Load<AudioClip>("Audio/SE/Enemy Killed");
            audioSource.Play();

            yield return new WaitForSeconds(0.5f);
            MakeLoot();
            this.gameObject.SetActive(false);

        }

        // Si le mob est un boss, joue le son de mort de boss
        if (typeOfMob == KnockBack.TypeOfMob.Boss)
        {

        }
    }

    // Quand l'épée touche le mob
    public void Knock(Rigidbody2D myRigidbody, float knockTime, float damage, KnockBack.TypeOfMob typeOfMob)
    {
        // Coroutine qui effectue un knockback (recule de l'ennemis)
        StartCoroutine(KnockCo(myRigidbody, knockTime));

        // Fonction permettant de retirer des pv au mob
        TakeDamage(damage, typeOfMob);
    }

    private IEnumerator KnockCo(Rigidbody2D myRigidbody, float knockTime)
    {
        // Effectue le recul du gameobject
        if (myRigidbody != null)
        {
            yield return new WaitForSeconds(knockTime);
            myRigidbody.velocity = Vector2.zero;
            yield return new WaitForSeconds(0.5f);
            myRigidbody.GetComponent<Enemy>().currentState = EnemyState.idle;
            myRigidbody.velocity = Vector2.zero;
        }
    }
}
