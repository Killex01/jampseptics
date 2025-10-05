using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private Transform sprite;

    public float movementSpeed = 5f;

    public float jumpHeight = 0.5f;       // visual height
    public float jumpDuration = 0.5f;     // total up & down time
    public float maxScale = 1.2f;         // bigger when "closer" to camera
    public float minScale = 1.0f;         // base scale
    private bool isJumping = false;
    private Vector3 spriteStartPos;

    public LayerMask jumpableLayer;
    private Collider2D playerCollider;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 800f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;
    private Vector2 lastMoveDir = Vector2.right;
    private Vector2 input;
    // key interaction
    public bool hasKey = false;
    public bool nearKey = false;
    [SerializeField] private GameObject key;

    //Door interactions
    public bool lockedDoor = true;
    public bool nearDoor = false;
    [SerializeField] private GameObject[] door = new GameObject[2];
    [SerializeField] private GameObject doorLock;
    [SerializeField] private GameObject doorblock;



    //for storing spawn position for ghost collision reset
    private Vector3 spawnPosition;

    private enum PlayerState
    {
        Alive,
        Dead
    }

    private static PlayerState currentState = PlayerState.Alive;
    void Start()
    {
        door[0].name = "closedDoor";
        door[1].name = "openDoor";
        door[0].SetActive(true);
        door[1].SetActive(false);
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>(); 
        sprite = transform.Find("sprite");
        if (sprite == null)
            Debug.LogError("No child called 'sprite' found under player!");
        anim = GetComponent<Animator>();
        spriteStartPos = sprite.localPosition;

        //stores the spawn position at level start
        spawnPosition = transform.position;
    }

    //reset player to spawn position
    public void ResetToSpawn()
    {
        rb.velocity = Vector2.zero;
        transform.position = spawnPosition;
    }

    private void Update()
    {
        HandleMovement();
        HandleJumpInput();
        UpdateAnimations();
        HandleStateSwitch();
        PickUpKey();
        UnlockDoor();
    }

    private void HandleStateSwitch()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (currentState == PlayerState.Alive)
            {
                SceneManager.LoadScene("deadState");
                currentState = PlayerState.Dead;

            }
            else if (currentState == PlayerState.Dead)
            {
                SceneManager.LoadScene("aliveState");
                currentState = PlayerState.Alive;

            }
        }
    }

    private void HandleMovement()
    {
        if (isDashing)
        {
            return;
        }

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (input.sqrMagnitude > 0.01f)
            lastMoveDir = input.normalized;

        rb.velocity = input.normalized * movementSpeed;

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && currentState != PlayerState.Alive)
        {
            StartCoroutine(Dash());
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            StartCoroutine(JumpEffect());
            if (anim != null)
                anim.SetTrigger("Jump");
        }
    }

    private void UpdateAnimations()
    {
        if (anim != null)
        {
            anim.SetFloat("MoveX", input.x);
            anim.SetFloat("MoveY", input.y);
            anim.SetFloat("Speed", rb.velocity.magnitude);
        }
    }

    private IEnumerator JumpEffect()
    {
        isJumping = true;
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMaskToLayer(jumpableLayer), true);

        float half = jumpDuration / 2f;

        // Going up
        for (float t = 0; t < half; t += Time.deltaTime)
        {
            float normalized = t / half;
            float height = Mathf.Sin(normalized * Mathf.PI / 2) * jumpHeight;
            float scale = Mathf.Lerp(minScale, maxScale, normalized);

            sprite.localPosition = spriteStartPos + Vector3.up * height;
            sprite.localScale = Vector3.one * scale;
            yield return null;
        }

        // Going down
        for (float t = 0; t < half; t += Time.deltaTime)
        {
            float normalized = t / half;
            float height = Mathf.Cos(normalized * Mathf.PI / 2) * jumpHeight;
            float scale = Mathf.Lerp(maxScale, minScale, normalized);

            sprite.localPosition = spriteStartPos + Vector3.up * height;
            sprite.localScale = Vector3.one * scale;
            yield return null;
        }

        sprite.localPosition = spriteStartPos;
        sprite.localScale = Vector3.one * minScale;

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMaskToLayer(jumpableLayer), false);

        isJumping = false;
    }

    private int LayerMaskToLayer(LayerMask mask)
    {
        int layer = 0;
        int maskVal = mask.value;
        while (maskVal > 1)
        {
            maskVal = maskVal >> 1;
            layer++;
        }
        return layer;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        rb.velocity = lastMoveDir * dashingPower;

        yield return new WaitForSeconds(dashingTime);

        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);

        canDash = true;
    }

    private void PickUpKey()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Press E to pick up key
        {
            if (nearKey == true && !hasKey) // Check if player is near the key and doesn't already have it
            {
                hasKey = true;
                key.SetActive(false); // Hide the key object in the scene
                Debug.Log("Key picked up!");
            }
            else if (!nearKey)
            {
                Debug.Log("No key nearby to pick up.");
            }
            else if (hasKey)
            {
                Debug.Log("You already have the key.");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Key")
        {
            nearKey = true;
            Debug.Log("near the key!");
        }
        if (other.gameObject.tag == "Lock")
        {
            nearDoor = true;
            Debug.Log("near the door!");
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Key")
        {
            nearKey = false;
            Debug.Log("no key!");
        }
        if (other.gameObject.tag == "Lock")
        {
            nearDoor = false;
            Debug.Log("left the lock.");
        }
    }

    private void UnlockDoor()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Press E to pick up key
        {
            if (nearDoor == true ) // Check if player is near the key and doesn't already have it
            {
               if (hasKey && lockedDoor)
                {
                    lockedDoor = false;
                    door[0].SetActive(false);
                    door[1].SetActive(true);
                    doorLock.SetActive(false);
                    doorblock.SetActive(false);
                    Debug.Log("Door unlocked!");
                }
                else if (!hasKey && lockedDoor)
                {
                    Debug.Log("You need a key to unlock the door.");
                }
            }
        }
    }
}

