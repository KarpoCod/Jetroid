using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMove : MonoBehaviour
{

    private float dirX;

    [SerializeField]
    public int moveSpeed = 3;
    public int JumpForce = 450;

    private Rigidbody2D rb;

    private float JumpCd = 0f;

    private Vector3 localScale;

    public void Rotate()
    {
        localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    public void Jump()
    {
        rb.AddForce(Vector2.up * JumpForce);
    }

    void Start()
    {
        localScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        dirX = -1f;
    }

    void Update()
    {
        if (JumpCd > 0) { JumpCd -= Time.deltaTime; }
        MoveForward();
    }

    void MoveForward()
    {
        dirX = localScale.x / Math.Abs(localScale.x);
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        if ((dirX > 0 && (localScale.x < 0)) || (dirX < 0 && (localScale.x > 0)))
            localScale.x *= -1;

        transform.localScale = localScale; 
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "bigstump" && JumpCd <= 0)
        {
            Rotate();
        }
    }

}