using System.Collections;
using System;
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
    public bool isEnemyFreeze = false;
    public bool stopFreezing = false;

    private float[] enemyStats = new float[3];
    private int i;
    private int i2;

    private void Awake()
    {
        // Attribut les valeurs de certaines variables au demarrage
        health = maxHealth.initialValue;
        audioSource = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagement>().soundEffectSource[1]; ;
        animator = GetComponent<Animator>();
        matFlash = Resources.Load("Graphics/Effects/Flash Effect", typeof(Material)) as Material;
        matDefault = gameObject.GetComponent<SpriteRenderer>().material;
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    public void BoomerangEffect()
    {
        StartCoroutine(BoomerangEffectCo());
    }


    private IEnumerator BoomerangEffectCo()
    {
        // Freeze l'ennemis si le boomerang le touche
        stopFreezing = false;
        i = 0;
        i2 = 0;

        if (!stopFreezing)
        {
            stopFreezing = false;
            audioSource.clip = Resources.Load<AudioClip>("Audio/SE/Enemy Hurt");
            audioSource.Play();

            isEnemyFreeze = true;

            enemyStats[0] = moveSpeed;
            enemyStats[1] = baseAttack;
            enemyStats[2] = gameObject.GetComponent<Rigidbody2D>().mass;
            moveSpeed = 0;
            baseAttack = 0;
            gameObject.GetComponent<Rigidbody2D>().mass = 99999;

            gameObject.GetComponent<KnockBack>().isEnable = false;
            
            StartCoroutine(DamageAnimation());
            yield return new WaitForSeconds(2f);

            StartCoroutine(DefreezeAnimation());

            yield return new WaitForSeconds(3f);
            DeFreeze();
        }
    }

    public void DeFreeze()
    {
        // Defreeze l'ennemis
        if (enemyStats[0] != 0)
        {
            StopCoroutine(BoomerangEffectCo());
            StopCoroutine(DefreezeAnimation());

            moveSpeed = enemyStats[0];
            baseAttack = System.Convert.ToInt32(enemyStats[1]);
            gameObject.GetComponent<Rigidbody2D>().mass = enemyStats[2];

            gameObject.GetComponent<KnockBack>().isEnable = true;

            i = 99;
            i2 = 99;

            isEnemyFreeze = false;
            stopFreezing = false;

            enemyStats[0] = 0;
            enemyStats[1] = 0;
            enemyStats[2] = 0;
        }
    }

    IEnumerator DefreezeAnimation()
    {
        // Animation qui fait trembler l'ennemis avant de le defreeze
        if (!stopFreezing)
        {
            Vector3 initPosition = gameObject.transform.position;

            // Pragma evite un affichage de "bug" dans la console alors qu'il n'y a pas de problème
            #pragma warning disable CS1717 // Assignation effectuée à la même variable
            for (i = i; i < 15; i++)
            #pragma warning restore CS1717 // Assignation effectuée à la même variable
            {
                gameObject.transform.position = new Vector3(initPosition.x - 0.03f, initPosition.y, initPosition.z);
                yield return new WaitForSeconds(0.02f);
                gameObject.transform.position = new Vector3(initPosition.x + 0.03f, initPosition.y, initPosition.z);
                yield return new WaitForSeconds(0.02f);
            }

            yield return new WaitForSeconds(0.25f);

            for (i = i2; i < 30; i++)
            {
                gameObject.transform.position = new Vector3(initPosition.x - 0.05f, initPosition.y, initPosition.z);
                yield return new WaitForSeconds(0.02f);
                gameObject.transform.position = new Vector3(initPosition.x + 0.05f, initPosition.y, initPosition.z);
                yield return new WaitForSeconds(0.02f);
            }
        }
        yield return null;
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
        yield return null;
    }

    private void MakeLoot()
    {
        // Active la loot table du mob quand il meure
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
        // Animation de mort du mob et desactivation
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
            stopFreezing = true;
            DeFreeze();

            yield return new WaitForSeconds(knockTime);
            myRigidbody.velocity = Vector2.zero;
            yield return new WaitForSeconds(0.5f);
            myRigidbody.GetComponent<Enemy>().currentState = EnemyState.idle;
            myRigidbody.velocity = Vector2.zero;
        }
    }
}
