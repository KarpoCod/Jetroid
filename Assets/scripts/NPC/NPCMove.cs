using System;
using UnityEngine;

public class NPCMove : MonoBehaviour
{

    private float dirX;

    [SerializeField] private JumpCheck jumpCheck;
    public int moveSpeed = 3;
    public int JumpForce = 450;

    private float JumpCd = 0.1f;

    private Rigidbody2D rb;

    public bool standing = true;

    private Vector3 localScale;

    public void Rotate()
    {
        JumpCd = 0.2f;
        localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
        Debug.Log("rotated");
    }

    public void Jump()
    {
        rb.AddForce(Vector2.up * JumpForce);
        JumpCd = 0.2f;
        Debug.Log("jumped");
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
        if (Physics2D.Raycast(transform.position, -Vector2.up, 1.05f, LayerMask.GetMask("FG"))) { standing = true; }
        else { standing = false; }
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
        if (jumpCheck.posible && standing && JumpCd <= 0)
        {
            Jump();
        }
        else if((!jumpCheck.posible) && standing)
        {
            Rotate();
        }
    }

}