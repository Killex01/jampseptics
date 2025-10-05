using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        sprite = transform.Find("sprite");
        if (sprite == null)
            Debug.LogError("No child called 'sprite' found under player!");
        anim = GetComponent<Animator>();
        spriteStartPos = sprite.localPosition;
    }

    private void Update()
    {
        HandleMovement();
        HandleJumpInput();
        UpdateAnimations();
    }

    private void HandleMovement()
    {
        if (isDashing)
        {
            return;
        }

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        // Update last move direction only when there’s actual input
        if (input.sqrMagnitude > 0.01f)
            lastMoveDir = input.normalized;

        rb.velocity = input.normalized * movementSpeed;

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
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

        // Temporarily ignore collisions with jumpable platforms
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

        // Re-enable collisions
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMaskToLayer(jumpableLayer), false);

        isJumping = false;
    }

    // Converts LayerMask to layer index (works if only one layer is selected)
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

}
