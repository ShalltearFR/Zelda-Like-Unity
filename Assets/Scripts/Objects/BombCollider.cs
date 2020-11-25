using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombCollider : MonoBehaviour
{
    private Rigidbody2D bombRigidBody;

    void Start()
    {
        bombRigidBody = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            StartCoroutine(StopAnimation());
        }
    }

    IEnumerator StopAnimation()
    {
        Debug.Log("touché mur");
        bombRigidBody.isKinematic = true;
        bombRigidBody.useFullKinematicContacts = true;
        bombRigidBody.velocity = Vector2.zero;
        bombRigidBody.mass = 999999999;
        bombRigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>().enabled = false;
        yield return new WaitForSeconds(0.15f);
        bombRigidBody.velocity = Vector2.zero;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>().enabled = true;
    }
}
