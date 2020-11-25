using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour
{
    public enum PlayerState
    {
        walk,
        attack,
        interact,
        stagger,
        idle,
        takeObject,
        bloquing,
    }

    public PlayerState currentState;
    public Vector3 playerPosition;
    public float speed;
    private Rigidbody2D playerRigidBody;
    private Vector3 change;
    private Animator animator;
    public FloatValue currentHealth;
    public Signal_Event playerHealthSignal;
    public StringValue playerSide;
    private CameraMovement cameraMovement;
    private SaveManager saveManager;

    public bool isTakingObject = false;
    public GameObject takeObjectInstantiate;
    public Sprite[] takeObjectSprite;
    public string takeObjectString = "";

    public VectorValue startingPosition;
    public Inventory playerInventory;

    public SpriteRenderer receivedItemSprite;

    public UnityEngine.Object[] soundsEffect;
    private AudioSource audioSource;
    private AudioSource audioSourceLinkAttack;

    public Material matFlash;
    private Material matDefault;
    private SpriteRenderer sr;
    private bool breackSpamming = false;

    public GameObject projectile;
    public GameObject Boomerang;
    public GameObject bombPlaced;

    void Start()
    {
        playerPosition = GetComponent<Transform>().position;
        cameraMovement = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
        currentState = PlayerState.walk;
        playerRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        saveManager = GameObject.Find("Save Manager").GetComponent<SaveManager>();

        checkPlayerSide();

        // Sert pour le new game
        // Teleporte le joueur vers le debut de partie
        if (startingPosition.teleporationValue == new Vector2(0, 0))
        {
            transform.position = startingPosition.initialValue;
        }
        else
        {
            // Teleporte le joueur à la valeur demandé
            transform.position = startingPosition.teleporationValue;
            startingPosition.teleporationValue = new Vector2(0, 0);
        }

        // Si le gameobject de teleportation exist, lance la procedure
        if (GameObject.Find("Set Teleport") != null)
        { GameObject.Find("Set Teleport").GetComponent<SetTeleport>().Teleport(); }

        StartCoroutine(cameraMovement.InitCamera());

        audioSource = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagement>().soundEffectSource[2];
        audioSourceLinkAttack = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagement>().soundEffectSource[3];
        matDefault = GetComponent<SpriteRenderer>().material;
        sr = GetComponent<SpriteRenderer>();
    }

    void checkPlayerSide()
    {
        // Initialise la position du début de jeu du joueur
        if (playerSide.RuntimeValue == "") { animator.SetFloat("moveX", 0); animator.SetFloat("moveY", -1); playerSide.RuntimeValue = "Down"; }

        // Oriente le joueur sur la derniere position sauvegardé
        if (playerSide.RuntimeValue == "Up") { animator.SetFloat("moveX", 0); animator.SetFloat("moveY", 1); }
        if (playerSide.RuntimeValue == "Down") { animator.SetFloat("moveX", 0); animator.SetFloat("moveY", -1); }
        if (playerSide.RuntimeValue == "Left") { animator.SetFloat("moveX", -1); animator.SetFloat("moveY", 0); }
        if (playerSide.RuntimeValue == "Right") { animator.SetFloat("moveX", 1); animator.SetFloat("moveY", 0); }
    }

    void Update()
    {
        if (currentState == PlayerState.interact || currentState == PlayerState.bloquing) { return; }

        // Si aucune touche de deplacement n'est appuyé, stop le deplacement du personnage
        change = Vector3.zero;

        // Insère dans le vector "change" les valeur lorsque que l'on appui sur les touches de deplacement
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");

        // Si le joueur n'a pas d'objet en main (pot/pancarte)
        if (currentState != PlayerState.takeObject)
        {
            if (Input.GetButtonDown("Attack") && currentState != PlayerState.attack && currentState != PlayerState.stagger)
            {
                // Si le joueur attaque à l'épée
                StartCoroutine(AttackCo());
            }
            else if (Input.GetButtonDown("RB") && currentState != PlayerState.attack && currentState != PlayerState.stagger)
            {
                // Si le joueur appuie sur RT (actuellement Y)
                StartCoroutine(RBCo());
            }
            else if (Input.GetButtonDown("LT") && currentState != PlayerState.attack && currentState != PlayerState.stagger)
            {
                // Si le joueur appuie sur LT
                StartCoroutine(LTCo());
            }
            else if (currentState == PlayerState.walk || currentState == PlayerState.idle)
            {
                // Si le joueur bouge, change son etat en "walk"
                // S'il ne bouge pas, change en "idle"
                if (change.x != 0 || change.y != 0) { currentState = PlayerState.walk; } else { currentState = PlayerState.idle; }
            }
        }

        // Si le joueur n'est pas en interraction
        if (currentState != PlayerState.interact)
        {
            UpdateAnimationAndMove();
            if (isTakingObject)
            {
                if (Input.GetButtonDown("Interract"))
                {
                    // S'il tient un objet et qu'il appuie sur la touche d'interraction
                    StartCoroutine(LaunchObject());
                }
            }

            if (animator.GetBool("TakingBomb") && Input.GetButtonDown("Interract"))
            {
                StartCoroutine(LaunchObject());
            }
        }
    }

    private IEnumerator LaunchObject()
    {
        // Animation de lancement d'objet
        if (!breackSpamming) { breackSpamming = true; }

        if (breackSpamming)
        {
            if (animator.GetBool("TakingBomb"))
            {
                isTakingObject = true;
                if (isTakingObject)
                {
                    speed = 0;
                    currentState = PlayerState.bloquing;
                    animator.SetBool("LaunchBomb", true);
                    yield return null;
                    animator.SetBool("LaunchBomb", false);
                    animator.SetBool("TakingBomb", false);
                    yield return new WaitForSeconds(0.30f);
                    currentState = PlayerState.idle;
                    if (transform.GetChild(5).transform.GetChild(0).transform != null)
                    {
                        transform.GetChild(5).transform.GetChild(0).transform.SetParent(null);
                    }
                    speed = 6;
                    isTakingObject = false;
                }
            }

            if (takeObjectString == "Sign")
            {
                // Si c'est une pancarte
                currentState = PlayerState.interact;

                GameObject.FindWithTag("TakeObject").GetComponent<SpriteRenderer>().sprite = takeObjectSprite[0];
                GameObject.FindWithTag("TakeObject").GetComponent<SpriteRenderer>().enabled = true;
                animator.SetBool("LaunchPot&Sign", true);
                animator.SetBool("Sign", false);

                bool stopRepeatSound = false;

                if (!stopRepeatSound)
                {
                    stopRepeatSound = true;
                    audioSource.clip = Resources.Load<AudioClip>("Audio/SE/Dropping");
                    audioSource.Play();

                    yield return new WaitForSeconds(0.15f);
                    audioSource.clip = Resources.Load<AudioClip>("Audio/SE/Pot Destroy");
                    audioSource.Play();
                }

                yield return new WaitForSeconds(0.37f);
                animator.SetBool("LaunchPot&Sign", false);
                yield return null;

                isTakingObject = false;
                currentState = PlayerState.idle;
                speed = 6;

                breackSpamming = false;
            }

            if (takeObjectString == "Pot")
            {
                // Si c'est un pot
                currentState = PlayerState.interact;

                GameObject.FindWithTag("TakeObject").GetComponent<SpriteRenderer>().sprite = takeObjectSprite[1];
                GameObject.FindWithTag("TakeObject").GetComponent<SpriteRenderer>().enabled = true;

                animator.SetBool("LaunchPot&Sign", true);
                animator.SetBool("Pot", false);

                bool stopRepeatSound = false;

                if (!stopRepeatSound)
                {
                    stopRepeatSound = true;
                    audioSource.clip = Resources.Load<AudioClip>("Audio/SE/Dropping");
                    audioSource.Play();

                    yield return new WaitForSeconds(0.15f);
                    audioSource.clip = Resources.Load<AudioClip>("Audio/SE/Pot Destroy");
                    audioSource.Play();
                }

                yield return new WaitForSeconds(0.37f);
                animator.SetBool("LaunchPot&Sign", false);
                yield return null;

                isTakingObject = false;
                currentState = PlayerState.idle;
                speed = 6;
                breackSpamming = false;
            }
        }
    }

    private IEnumerator AttackCo()
    {
        if (playerInventory.itemsName.Contains("Sword"))
        {
            // Si le joueur possède l'épée dans son inventaire
            // Effectue l'animation de coup d'épée suivant la direction du joueur
            animator.SetBool("attacking", true);
            currentState = PlayerState.attack;

            audioSourceLinkAttack.clip = soundsEffect[Random.Range(0, 3)] as AudioClip;
            audioSourceLinkAttack.Play();

            yield return null;
            animator.SetBool("attacking", false);
            yield return new WaitForSeconds(0.40f);
            if (currentState != PlayerState.interact && currentState != PlayerState.bloquing) { currentState = PlayerState.walk; }
        }
    }

    private IEnumerator RBCo()
    {
        // Si le touche RB est appuyé, lance pour cette fonction une flèche
        if (saveManager.selectedItem.RuntimeValue == "Bow")
        {
            if (playerInventory.arrow > 0)
            {
                currentState = PlayerState.attack;

                animator.SetBool("FireBow", true);

                yield return null;
                animator.SetBool("FireBow", false);
                yield return new WaitForSeconds(0.40f);
                audioSourceLinkAttack.clip = Resources.Load<AudioClip>("Audio/SE/Arrow");
                audioSourceLinkAttack.Play();

                makeArrow();
                yield return new WaitForSeconds(0.20f);
                if (currentState != PlayerState.interact && currentState != PlayerState.bloquing)
                { currentState = PlayerState.idle; }
                playerInventory.arrow -= 1;
                GameObject.Find("Arrow HUD").GetComponent<ArrowTextManager>().UpdateArrowCount();
            }
        }

        if (saveManager.selectedItem.RuntimeValue == "Boomerang")
        {
            if (GameObject.FindWithTag("Boomerang") == null)
            {
                animator.SetBool("FireBoomerang", true);
                currentState = PlayerState.attack;

                yield return null;
                animator.SetBool("FireBoomerang", false);

                yield return new WaitForSeconds(0.20f);
                makeBoomerang();

                yield return new WaitForSeconds(0.20f);
                if (currentState != PlayerState.interact && currentState != PlayerState.bloquing) { currentState = PlayerState.idle; }
            }
        }

        if (saveManager.selectedItem.RuntimeValue == "Bomb")
        {
            if (playerInventory.bomb > 0)
            {
                // Oriente la bombe suivant la direction du joueur
                Vector2 direction = new Vector2(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
                GameObject bomb = Instantiate(bombPlaced, new Vector3((transform.position.x + direction.x), (transform.position.y + direction.y), -3), Quaternion.identity);

                playerInventory.bomb -= 1;
                GameObject.Find("Bomb HUD").GetComponent<BombTextManager>().UpdateBombCount();
            }

        }
    }

    private void makeArrow()
    {
        // Oriente la flèche suivant la direction du joueur
        Vector2 direction = new Vector2(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        Arrow arrow = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Arrow>();
        arrow.Setup(direction, ChooseArrowDirectionSprite());
    }

    private IEnumerator LTCo()
    {
        /*
        // Si la touche LT est appuyé
        // Effectue pour cette fonction le lancement du boomerang
        if (playerInventory.itemsName.Contains("Sword"))
        {
            if (GameObject.FindWithTag("Boomerang") == null)
            {
                animator.SetBool("FireBoomerang", true);
                currentState = PlayerState.attack;

                yield return null;
                animator.SetBool("FireBoomerang", false);

                yield return new WaitForSeconds(0.20f);
                makeBoomerang();

                yield return new WaitForSeconds(0.20f);
                if (currentState != PlayerState.interact && currentState != PlayerState.bloquing) { currentState = PlayerState.idle; }
            }
        }
        */
        yield return null;
    }

    private void makeBoomerang()
    {
        // Crée le boomerang et l'oriente suivant la direction du joueur
        Vector2 direction = new Vector2(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        Boomerang boomerang = Instantiate(Boomerang, transform.position, Quaternion.identity).GetComponent<Boomerang>();
        boomerang.Setup(direction, ChooseArrowDirectionSprite());
    }

    Vector3 ChooseArrowDirectionSprite()
    {
        // Indique la valeur de la fleche suivant la direction selon laquel le joueur regarde
        float temp = Mathf.Atan2(animator.GetFloat("moveY"), animator.GetFloat("moveX")) * Mathf.Rad2Deg;
        return new Vector3(0, 0, temp);
    }

    public void RaiseItem(TreasureChest.TypeOfItem typeOfItem)
    {
        //        foreach (AnimatorControllerParameter parameter in GetComponent<Animator>().parameters)
        //       {
        //            GetComponent<Animator>().SetBool(parameter.name, false);
        //        }

        // Change l'etat du joueur s'il ouvre un coffre
        //        if (playerInventory.currentItem != null)
        //        {
        if (currentState != PlayerState.interact)
        {
            animator.SetBool(typeOfItem.ToString(), true);
            currentState = PlayerState.interact;
        }
        else
        {
            animator.SetBool(typeOfItem.ToString(), false);
            currentState = PlayerState.idle;
            playerInventory.currentItem = null;
        }
        //        }
    }

    void UpdateAnimationAndMove()
    {
        // Met a jour l'animation du personnage quand il se deplace
        if (change != Vector3.zero)
        {
            if (currentState == PlayerState.walk || currentState == PlayerState.idle)
            {
                MoveCharacter();
                animator.SetFloat("moveX", change.x);
                animator.SetFloat("moveY", change.y);
                animator.SetBool("moving", true);
            }
            if (currentState == PlayerState.takeObject)
            {
                MoveCharacter();
                animator.SetFloat("moveX", change.x);
                animator.SetFloat("moveY", change.y);
                animator.SetBool("WalkingObject", true);
            }
        }
        else
        {
            if (currentState == PlayerState.walk || currentState == PlayerState.idle) { animator.SetBool("moving", false); }
            if (currentState == PlayerState.takeObject) { animator.SetBool("WalkingObject", false); }
        }
    }

    void MoveCharacter()
    {
        // Deplace le personnage
        change.Normalize();
        playerRigidBody.MovePosition(
            transform.position + change * speed * Time.deltaTime);
    }

    public void Knock(float knockTime, float damage, GameObject mobGameObject, bool isInTakeObjectState)
    {
        // Quand le personnage prend un coup
        currentHealth.RuntimeValue -= damage;
        playerHealthSignal.Raise();
        if (currentHealth.RuntimeValue > 0)
        {
            // Si le personnage a encore de la vie
            // Effectue la procedure de degats du joueur
            StartCoroutine(DamageAnimation());
            StartCoroutine(KnockCo(knockTime, mobGameObject, isInTakeObjectState));
        }
        else
        {
            // Si le personnage n'a plus de vie
            // Effectue la procedure de mort
            StartCoroutine(PlayerDeath());
        }
    }

    private IEnumerator PlayerDeath()
    {
        // Procedure de mort du joueur
        GameObject.Find("Manager").GetComponent<PauseMenu>().blockPause = true;
        currentState = PlayerMovement.PlayerState.interact;
        animator.SetBool("moving", false);
        animator.SetBool("WalkingObject", false);
        animator.SetBool("FireBoomerang", false);
        animator.SetBool("FireBow", false);
        animator.SetBool("attacking", false);

        GameObject.Find("Red Background").GetComponent<Animator>().SetBool("death", true);
        playerRigidBody.velocity = Vector2.zero;
        playerRigidBody.mass = 99999;
        animator.SetBool("Death", true);
        yield return new WaitForSeconds(2f);
        StartCoroutine(GameObject.Find("Death HUD").GetComponent<DeathHUD>().Init());
    }

    private IEnumerator KnockCo(float knockTime, GameObject mobGameObject, bool isInTakeObjectState)
    {
        // Fais reculer le joueur quand il prend des degats
        // Joue le son de degats
        if (playerRigidBody != null)
        {
            audioSource.clip = soundsEffect[4] as AudioClip;
            audioSource.Play();
            float speedDump = 0;
            if (mobGameObject.CompareTag("Enemy"))
            {
                speedDump = mobGameObject.GetComponent<Enemy>().moveSpeed;
                mobGameObject.GetComponent<Enemy>().moveSpeed = 0;
            }

            yield return new WaitForSeconds(knockTime);
            playerRigidBody.velocity = Vector2.zero;
            yield return new WaitForSeconds(0.35f);
            playerRigidBody.velocity = Vector2.zero;
            // Si le joueur possède un objet, remet sur l'etat "takeObject"
            // S'il ne porte rien, le remet en idle
            if (isInTakeObjectState)
            { currentState = PlayerState.takeObject; }
            else { currentState = PlayerState.idle; }

            if (mobGameObject != null)
            {
                if (mobGameObject.CompareTag("Enemy")) { mobGameObject.GetComponent<Enemy>().moveSpeed = speedDump; }
            }
        }
    }

    IEnumerator DamageAnimation()
    {
        animator.SetBool("Damage", true);
        yield return null;
        animator.SetBool("Damage", false);

        // Animation de flash du personnage quand il prend des degats
        int i;
        for (i = 0; i < 6; i++)
        {
            sr.material = matFlash;
            yield return new WaitForSeconds(0.015f);
            sr.material = matDefault;
            yield return new WaitForSeconds(0.015f);
        }
    }
}