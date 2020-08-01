using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed;
    public Rigidbody2D myRigidBody;
    private SoundManagement soundManagement;

    private void Start()
    {
        soundManagement = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagement>();
    }
    public void Setup(Vector2 velocity, Vector3 direction)
    {
        // Oriente la fleche dans la bonne direction et la fait avancer
        myRigidBody.velocity = velocity.normalized * speed;
        transform.rotation = Quaternion.Euler(direction);
        StartCoroutine(AutoDestroy());
    }

    IEnumerator AutoDestroy()
    {
        // Destruction du gameobject de fleche au bout de 5 secondes
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Si elle touche un ennemis, détruit le gameobject de fleche
            Destroy(this.gameObject);
        } else if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Sign") || other.gameObject.CompareTag("Pot") || other.gameObject.CompareTag("Chest"))
        {
            if (!other.isTrigger)
            {
                // Stop la vitesse de la fleche et joue le le son de la fleche qui touche le mur
                myRigidBody.velocity = new Vector2(0, 0);
                gameObject.GetComponent<KnockBack>().isEnable = false;

                soundManagement.soundEffectSource[4].clip = Resources.Load<AudioClip>("Audio/SE/Arrow Hit Wall");
                soundManagement.soundEffectSource[4].Play();
            }
            
        }

    }
}
