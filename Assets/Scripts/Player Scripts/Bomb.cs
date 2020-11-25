using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private SoundManagement soundManagement;
    public float thrust;
    public float knockTime;
    public float damage;

    private void Start()
    {
        soundManagement = GameObject.Find("Sound Manager").GetComponent<SoundManagement>();
        soundManagement.soundEffectSource[4].clip = Resources.Load<AudioClip>("Audio/SE/Bomb Placed");
        soundManagement.soundEffectSource[4].Play();
    }

    public void ExplosionSound()
    {
        soundManagement.soundEffectSource[4].clip = Resources.Load<AudioClip>("Audio/SE/Bomb Explosion");
        soundManagement.soundEffectSource[4].Play();
    }

    public void Setup(Vector2 velocity, Vector3 direction)
    {
        // Oriente la bombe dans la bonne direction
        transform.rotation = Quaternion.Euler(direction);
    }

    public void Destroy() { Destroy(this.gameObject); }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.isTrigger)
            {
                Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
                bool isInTakeObjectState = false;

                Vector2 difference = (other.transform.position - transform.position);
                difference = difference.normalized * thrust;
                other.GetComponent<Rigidbody2D>().AddForce(difference, ForceMode2D.Impulse);

                if (hit.GetComponent<PlayerMovement>().currentState == PlayerMovement.PlayerState.takeObject)
                {
                    if (!other.GetComponent<Animator>().GetBool("TakingBomb")) { isInTakeObjectState = true; }
                }

                if (other.GetComponent<Animator>().GetBool("TakingBomb"))
                {
                    other.GetComponent<Animator>().SetBool("TakingBomb", false);
                    GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().speed = 6;
                    other.GetComponent<PlayerMovement>().currentState = PlayerMovement.PlayerState.idle;
                }

                if (other.GetComponent<PlayerMovement>().currentState != PlayerMovement.PlayerState.stagger)
                {
                    hit.GetComponent<PlayerMovement>().currentState = PlayerMovement.PlayerState.stagger;
                    other.GetComponent<PlayerMovement>().Knock(knockTime, damage, this.gameObject, isInTakeObjectState);
                }
            }
        }

        if (other.CompareTag("Enemy"))
        {
            if (other.isTrigger)
            {
                Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
                if (hit != null)
                {
                    other.GetComponent<Enemy>().isDamage = true;
                    Vector2 difference = (other.transform.position - transform.position);
                    difference = difference.normalized * thrust;
                    other.GetComponent<Rigidbody2D>().AddForce(difference, ForceMode2D.Impulse);

                    other.GetComponent<Enemy>().currentState = EnemyState.stagger;
                    other.GetComponent<Enemy>().Knock(hit, knockTime, damage, KnockBack.TypeOfMob.Classic);
                }
            }
        }

        if (other.CompareTag("Sign"))
        {
            Debug.Log("Pancarte touché");
        }

        if (other.CompareTag("Pot"))
        {
            Debug.Log("Pot touché");
        }
    }

    IEnumerator EnemyCo(Collider2D other)
    {
        yield return null;
    }
}
