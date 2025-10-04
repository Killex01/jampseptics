using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouvementPlayer : MonoBehaviour
{
    private Rigidbody2D rb;
    
    public float MouvementSpeed;
    private Vector2 movementInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == 0 && vertical == 0)
        {
            rb.velocity = new Vector2(0, 0);
            return;
        }
        movementInput = new Vector2(horizontal, vertical);
        rb.velocity = movementInput * MouvementSpeed * Time.fixedDeltaTime;

    }   


}
