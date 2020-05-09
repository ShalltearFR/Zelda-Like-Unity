using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public bool isTakingObject = false;
    public GameObject takeObjectInstantiate;
    public Sprite[] takeObjectSprite;
    public string takeObjectString = "";

    public VectorValue startingPosition;
    public Inventory playerInventory;

    public SpriteRenderer receivedItemSprite;

    public Object[] soundsEffect;
    private AudioSource audioSource;
    private AudioSource audioSourceLinkAttack;

    public Material matFlash;
    private Material matDefault;
    private SpriteRenderer sr;
    private bool breackSpamming = false;

    public GameObject projectile;

    void Start()
    {
        currentState = PlayerState.walk;
        playerRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();


        checkPlayerSide();
     //   animator.SetFloat("moveX", 0);
    //    animator.SetFloat("moveY", -1);

        transform.position = startingPosition.initialValue;

        audioSource = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagement>().soundEffectSource[2];
        audioSourceLinkAttack = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagement>().soundEffectSource[3];
        matDefault = GetComponent<SpriteRenderer>().material;
        sr = GetComponent<SpriteRenderer>();
    }

    void checkPlayerSide()
    {
        // Initialise la position du début de jeu du joueur
        if (playerSide.initialValue == "") { animator.SetFloat("moveX", 0); animator.SetFloat("moveY", -1); playerSide.initialValue = "Down"; }

        // Oriente le joueur sur la derniere position sauvegardé
        if (playerSide.initialValue == "Up") { animator.SetFloat("moveX", 0); animator.SetFloat("moveY", 1); }
        if (playerSide.initialValue == "Down") { animator.SetFloat("moveX", 0); animator.SetFloat("moveY", -1); }
        if (playerSide.initialValue == "Left") { animator.SetFloat("moveX", -1); animator.SetFloat("moveY", 0); }
        if (playerSide.initialValue == "Right") { animator.SetFloat("moveX", 1); animator.SetFloat("moveY", 0); }
    }

    void FixedUpdate()
    {
        playerPosition = GetComponent<Transform>().position;
        //is the player in an interraction
        if (currentState == PlayerState.interact || currentState == PlayerState.bloquing)
        {
            return;
        }

        change = Vector3.zero;

        
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");



            if (currentState != PlayerState.takeObject)
        {
            if (Input.GetButtonDown("Attack") && currentState != PlayerState.attack && currentState != PlayerState.stagger)
            {
                StartCoroutine(AttackCo());
            } else if (Input.GetButtonDown("RT") && currentState != PlayerState.attack && currentState != PlayerState.stagger)
            {
                StartCoroutine(RTCo());
            } else if (currentState == PlayerState.walk || currentState == PlayerState.idle)
            {
                if (change.x != 0 || change.y != 0) { currentState = PlayerState.walk; } else { currentState = PlayerState.idle; }
            }
        }

        if (currentState != PlayerState.interact)
        {
            UpdateAnimationAndMove();

            if (isTakingObject)
            {
                if (Input.GetButtonDown("Interract"))
                {
                    StartCoroutine(LaunchObject());
                }
            }
        }
    }

    private IEnumerator LaunchObject()
    {
        if (!breackSpamming) { breackSpamming = true; }

        if (breackSpamming)
        {
            if (takeObjectString == "Sign")
            {
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


                //   animator.SetBool("LaunchPot&Sign", false);
                yield return new WaitForSeconds(0.37f);
                
                animator.SetBool("LaunchPot&Sign", false);

                yield return null;

                //   animator.SetBool("LaunchPot&Sign", false);
                isTakingObject = false;
                currentState = PlayerState.idle;
                speed = 6;

                breackSpamming = false;

                //    GameObject objectLaunch = Instantiate(takeObjectInstantiate, new Vector3(playerPosition.x, playerPosition.y + 0.4374f, 0), Quaternion.identity) as GameObject;
                //    objectLaunch.GetComponent<Transform>().SetParent(GameObject.FindWithTag("Player").GetComponent<Transform>());
            }

            if (takeObjectString == "Pot")
            {
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

                //   animator.SetBool("LaunchPot&Sign", false);
                yield return new WaitForSeconds(0.37f);

                animator.SetBool("LaunchPot&Sign", false);

                yield return null;

                //   animator.SetBool("LaunchPot&Sign", false);
                isTakingObject = false;
                currentState = PlayerState.idle;
                speed = 6;

                breackSpamming = false;

                //    GameObject objectLaunch = Instantiate(takeObjectInstantiate, new Vector3(playerPosition.x, playerPosition.y + 0.4374f, 0), Quaternion.identity) as GameObject;
                //    objectLaunch.GetComponent<Transform>().SetParent(GameObject.FindWithTag("Player").GetComponent<Transform>());
            }
        }
    }

    private IEnumerator AttackCo()
    {
        if (playerInventory.itemsName.Contains("Sword"))
        {
            animator.SetBool("attacking", true);
            currentState = PlayerState.attack;

            audioSourceLinkAttack.clip = soundsEffect[Random.Range(0, 3)] as AudioClip;
            audioSourceLinkAttack.Play();

            yield return null;
            animator.SetBool("attacking", false);
            yield return new WaitForSeconds(0.40f);
            if (currentState != PlayerState.interact && currentState != PlayerState.bloquing)
            {
                currentState = PlayerState.walk;
            }
        }
    }

    private IEnumerator RTCo()
    {
        if (playerInventory.itemsName.Contains("Sword"))
        {
            //animator.SetBool("attacking", true);
            currentState = PlayerState.attack;

            animator.SetBool("FireBow", true);
            
            //audioSourceLinkAttack.clip = soundsEffect[Random.Range(0, 3)] as AudioClip;
            //audioSourceLinkAttack.Play();

            yield return null;
            animator.SetBool("FireBow", false);
            yield return new WaitForSeconds(0.40f);
            audioSourceLinkAttack.clip = Resources.Load<AudioClip>("Audio/SE/Arrow");
            audioSourceLinkAttack.Play();

            makeArrow();
            //animator.SetBool("attacking", false);
            yield return new WaitForSeconds(0.20f);
            if (currentState != PlayerState.interact && currentState != PlayerState.bloquing)
            {
                currentState = PlayerState.idle;
            }
        }
    }

    private void makeArrow()
    {
        Vector2 direction = new Vector2(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        Arrow arrow = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Arrow>();
        arrow.Setup(direction, ChooseArrowDirectionSprite());
    }

    Vector3 ChooseArrowDirectionSprite()
    {
        float temp = Mathf.Atan2(animator.GetFloat("moveY"), animator.GetFloat("moveX")) * Mathf.Rad2Deg;
        return new Vector3(0, 0, temp);
    }

    public void RaiseItem(TreasureChest.TypeOfItem typeOfItem)
    {
        if (playerInventory.currentItem != null)
        {
            if (currentState != PlayerState.interact)
            {
                animator.SetBool(typeOfItem.ToString(), true);
                currentState = PlayerState.interact;
                //     receivedItemSprite.sprite = playerInventory.currentItem.itemSprite;
            }
            else
            {
                animator.SetBool(typeOfItem.ToString(), false);
                currentState = PlayerState.idle;
                playerInventory.currentItem = null;
                //   receivedItemSprite.sprite = null;

            }
        }

    }

    void UpdateAnimationAndMove()
    {
        if (change != Vector3.zero)
        {
            if (currentState == PlayerState.walk || currentState == PlayerState.idle) {
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
        } else
        {
            if (currentState == PlayerState.walk || currentState == PlayerState.idle) { animator.SetBool("moving", false); }
            if (currentState == PlayerState.takeObject) { animator.SetBool("WalkingObject", false); }
        }
    }

    void MoveCharacter()
    {
        change.Normalize();
        playerRigidBody.MovePosition(
            transform.position + change * speed * Time.deltaTime);
    }

    public void Knock(float knockTime, float damage, GameObject mobGameObject, bool isInTakeObjectState)
    {
        currentHealth.RuntimeValue -= damage;
        playerHealthSignal.Raise();
        if (currentHealth.RuntimeValue > 0)
        {
            StartCoroutine(DamageAnimation());
            StartCoroutine(KnockCo(knockTime, mobGameObject, isInTakeObjectState));
        } else
        {
            // Player Death
            this.gameObject.SetActive(false);
        }
    }
    private IEnumerator KnockCo(float knockTime, GameObject mobGameObject, bool isInTakeObjectState)
    {
        if (playerRigidBody != null)
        {
            audioSource.clip = soundsEffect[4] as AudioClip;
            audioSource.Play();

            float speedDump = mobGameObject.GetComponent<Enemy>().moveSpeed;
            mobGameObject.GetComponent<Enemy>().moveSpeed = 0;
            yield return new WaitForSeconds(knockTime);
            playerRigidBody.velocity = Vector2.zero;
            yield return new WaitForSeconds(0.25f);
            if (isInTakeObjectState)
            {
                currentState = PlayerState.takeObject;
            } else
            {
                currentState = PlayerState.idle;
            }
            
            mobGameObject.GetComponent<Enemy>().moveSpeed = speedDump;
            playerRigidBody.velocity = Vector2.zero;
        }
    }

    IEnumerator DamageAnimation()
    {
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
