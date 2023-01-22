using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float PlayerXScale;
    public float PlayerYScale;
    public float PlayerZ;
    public float Speed = 50;
    public float jetSpeed = 40;
    public float airMultiplier = 0.1f;
    public float standingThreshold = 4f;
    public float boost = 4f;
    public Vector2 maxVelocity = new Vector2(60, 100);

    private Rigidbody2D body2D;
    public GameObject Player;

    private bool standing;

    private Animator animator;

    void Start()
    {
        body2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        PlayerXScale = transform.localScale.x;
        PlayerYScale = transform.localScale.y;
        PlayerZ = transform.localRotation.z;
    }


    void Update()
    {

        float forceY = 0;
        float forceX = 0;
        var absVelX = Mathf.Abs(body2D.velocity.x);
        var absVelY = Mathf.Abs(body2D.velocity.y);
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (absVelY <= standingThreshold)
        {
            standing = true;
        }
        else
        {
            standing = false;
        }

        /*if (absVelY < maxVelocity.y)
        {
            forceY = jetSpeed * Time.deltaTime * verticalInput;
        }*/
        if (horizontalInput != 0)
        {

            animator.SetInteger("AnimState", 1);
   /*         if (absVelX < maxVelocity.x)
            {
                forceX = Speed * Time.deltaTime * horizontalInput;
            }*/
            if (verticalInput < 0)
            {
                forceX = Speed * Time.deltaTime * horizontalInput * boost * 75;

                transform.rotation = Quaternion.Euler(0, 0, -90);
                if (horizontalInput < 0)
                {
                    transform.localScale = new Vector3(1 * PlayerXScale, -1 * PlayerYScale, 1);

                }
                animator.SetInteger("AnimState", 2);
            }
            else if (absVelX < maxVelocity.x)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                var newSpeed = Speed * Time.deltaTime * horizontalInput * 75;
                forceX = standing ? newSpeed : (newSpeed * airMultiplier);
                if (horizontalInput < 0)
                {
                    transform.localScale = new Vector3(-1 * PlayerXScale, 1 * PlayerYScale, 1);
                }
                else if (horizontalInput > 0)
                {
                    transform.localScale = new Vector3(1 * PlayerXScale, 1 * PlayerYScale, 1);
                }
                else
                {
                    transform.localScale = new Vector3(1 * PlayerXScale, -1 * PlayerYScale, 1);
                }
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, PlayerZ);
            animator.SetInteger("AnimState", 0);
            transform.localScale = new Vector3(PlayerXScale, 1 * PlayerYScale, 1);
        }

        if (verticalInput > 0)
        {
            if (absVelY < maxVelocity.y)
            {
                forceY = jetSpeed * Time.deltaTime * verticalInput * 75;
            }
            animator.SetInteger("AnimState", 2);
        }
        else if (absVelY > 0 && !standing && verticalInput > 0)
        {
            animator.SetInteger("AnimState", 3);
        }
        

        body2D.AddForce(new Vector2(forceX, forceY));
    }
}
