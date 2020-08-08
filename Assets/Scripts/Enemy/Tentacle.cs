using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script de l'IA "Tentacle", lié au script ennemis

public class Tentacle : Enemy
{
    // Initialise les variables
    private Rigidbody2D myRigidBody;
    public Transform target;
    public float chaseRadius;
    public float attackRadius;
    public Transform homePosition;
    public Animator anim;
    private PlayerMovement playerMovement;
    private PauseMenu pauseMenu;

    private Vector3 randomlyMovingAnimation;
    private int randomNumberX;
    private int randomNumberY;
    private int randomPositifOrNegatifX;
    private int randomPositifOrNegatifY;

    private float chaseRadiusDump;

    private void Start()
    {
        // Attribut les valeurs de certaines variable au demarrage
        currentState = EnemyState.idle;
        myRigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player").transform;
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        pauseMenu = GameObject.Find("Manager").GetComponent<PauseMenu>();
    }

    void FixedUpdate()
    {
        // Si le joueur est dans le rayon de recherche du mob, suit le joueur
        if (playerMovement.currentState != PlayerMovement.PlayerState.interact && !pauseMenu.isPause)
        {
            if (!isEnemyFreeze)
            {
                CheckDistance();
            }
        }

        // Si le joueur est hors du rayon de recherche, effectue des mouvements aléatoire
        if (isActuallyRandomMoving)
        {
            if (!isEnemyFreeze)
            {
                RandomlyMovement();
            }
        }
    }

    private void RandomlyMovement()
    {
        if (!isDamage)
        {
            // Fonction permettant de deplacer aléatoirement le mob
            if (randomPositifOrNegatifX == 1 && randomPositifOrNegatifY == 1)
            {
                randomlyMovingAnimation = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x + randomNumberX, transform.position.y + randomNumberY, 0), 0.01f);
            }

            if (randomPositifOrNegatifX == 0 && randomPositifOrNegatifY == 1)
            {
                randomlyMovingAnimation = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x - randomNumberX, transform.position.y + randomNumberY, 0), 0.01f);
            }

            if (randomPositifOrNegatifX == 1 && randomPositifOrNegatifY == 0)
            {
                randomlyMovingAnimation = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x + randomNumberX, transform.position.y - randomNumberY, 0), 0.01f);
            }

            if (randomPositifOrNegatifX == 0 && randomPositifOrNegatifY == 0)
            {
                randomlyMovingAnimation = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x - randomNumberX, transform.position.y - randomNumberY, 0), 0.01f);
            }

            myRigidBody.MovePosition(randomlyMovingAnimation);
        }
    }

    private void CheckDistance()
    {
        // Fonction permettant de verifier si le joueur est dans le rayon de recherche du mob
        // Si le joueur est dans le rayon de recherche
        if (Vector3.Distance(target.position,
                            transform.position) <= chaseRadius
            && Vector3.Distance(target.position, transform.position) > attackRadius)
        {
            // Poursuite du mob vers le joueur
            if (currentState == EnemyState.idle || currentState == EnemyState.walk
                && currentState != EnemyState.stagger) { 
                Vector3 temp = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
                myRigidBody.MovePosition(temp);
                ChangeState(EnemyState.walk);
            }
        }
        else
        {
            // Si il est hors du rayon de recherche
            if (!isActuallyRandomMoving)
            {
                StartCoroutine(RandomlyMovingPathFinding());
            }
        }
    }


    private IEnumerator RandomlyMovingPathFinding()
    {
        // Effectue un path finding aléatoire de deplacement tous les X temps
        isActuallyRandomMoving = true;
        float movingTime = Random.Range(0.5f,1f);

        randomNumberX = Random.Range(0, 2);
        randomNumberY = Random.Range(0, 2);

        randomPositifOrNegatifX = Random.Range(0, 2); // Positif = 1, Negatif = 0
        randomPositifOrNegatifY = Random.Range(0, 2); // Positif = 1, Negatif = 0

        yield return new WaitForSeconds(movingTime);
        isActuallyRandomMoving = false;
    }


    private void ChangeState(EnemyState newState)
    {
        // Change l'etat du mob
        if (currentState != newState) { currentState = newState; }
    }
}
