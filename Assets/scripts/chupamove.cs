using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chupamove : MonoBehaviour
{

    float dirX;

    [SerializeField]
    float moveSpeed = 3f;

    Rigidbody2D rb;

    bool facingRight = false;

    Vector3 localScale;

    // Use this for initialization
    void Start()
    {
        localScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        dirX = -1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < -9f)
            dirX = 1f;
        else if (transform.position.x > 9f)
            dirX = -1f;
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
    }

    void LateUpdate()
    {
        CheckWhereToFace();
    }

    void CheckWhereToFace()
    {
        if (dirX > 0)
            facingRight = true;
        else if (dirX < 0)
            facingRight = false;

        if (((facingRight) && (localScale.x < 0)) || ((!facingRight) && (localScale.x > 0)))
            localScale.x *= -1;

        transform.localScale = localScale;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        switch (col.tag)
        {

            case "bigstump":
                dirX *= -1;
                break;

            case "smallstump":
                rb.AddForce(Vector2.up * 450f);
                break;
        }
    }

}