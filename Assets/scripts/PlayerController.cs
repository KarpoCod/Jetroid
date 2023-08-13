using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float PlayerXScale;
    public float PlayerYScale;
    public float PlayerZ;
    public float Speed = 50;
    public float jetForce = 40;
    public float jumpForce = 5;
    public float airMultiplier = 0.4f;
    public float standingThreshold = 4f;
    public float boost = 4f;
    public Vector2 maxVelocity = new Vector2(60, 100);
	private float jumpCd;
    private Rigidbody2D body2D;
    public GameObject Player;

	public bool Jet;
	public int AnimSt;

    private bool standing;

    private Animator animator;

    void Start()
    {
		jumpCd = 0;
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
        float VelocityX = 0;
        float VelocityY = 0;
        var absVelX = Mathf.Abs(body2D.velocity.x);
        var absVelY = Mathf.Abs(body2D.velocity.y);
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (absVelY <= standingThreshold && Physics2D.Raycast(transform.position + new Vector3(0.5f, 0, 0), -Vector2.up, 1.05f, LayerMask.GetMask("FG")) || Physics2D.Raycast(transform.position + new Vector3(-0.5f, 0, 0), -Vector2.up, 1.05f, LayerMask.GetMask("FG")))
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
			if (standing)
			{
            	AnimSt = 1;
			}
   /*         if (absVelX < maxVelocity.x)
            {
                forceX = Speed * Time.deltaTime * horizontalInput;
            }*/
            if (verticalInput < 0 && Jet)
            {
                if (absVelX < maxVelocity.x) VelocityX = Speed * horizontalInput * boost;

                if (horizontalInput < 0)
                {
                    transform.localScale = new Vector3(-1 * PlayerXScale, 1 * PlayerYScale, 1);

                }
                else if (horizontalInput > 0)
                {
                    transform.localScale = new Vector3(1 * PlayerXScale, 1 * PlayerYScale, 1);

                }
                AnimSt = 4;
            }
            else if (absVelX < maxVelocity.x)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                var newSpeed = Speed * horizontalInput;
                VelocityX = standing ? newSpeed : (newSpeed * airMultiplier);
                if (horizontalInput < 0)
                {
                    transform.localScale = new Vector3(-1 * PlayerXScale, 1 * PlayerYScale, 1);
                }
                else if (horizontalInput > 0)
                {
                    transform.localScale = new Vector3(1 * PlayerXScale, 1 * PlayerYScale, 1);
                }
                AnimSt = standing ? 1 : 3;
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, PlayerZ);
            AnimSt = 0;
        }

        if (verticalInput > 0 && Jet)
        {
            if (absVelY < maxVelocity.y)
            {
                forceY = jetForce * Time.deltaTime * verticalInput;
            }
            AnimSt = 2;
        }
		else if (verticalInput > 0 && !Jet && standing)
        {
			if (jumpCd <= 0)
			{
				jumpCd = 0.1f;
				VelocityY = jumpForce * 75;
				Debug.Log("Jumping!");
			}
		}
        if (!standing && (verticalInput == 0 || !Jet))
		{
			AnimSt = 3;
		}
		
		if (jumpCd >= 0)
		{
			jumpCd -= Time.deltaTime;
		}

		animator.SetBool("jet", Jet);
		animator.SetInteger("AnimState", AnimSt);

        body2D.velocity = new Vector2((forceX == 0 || Mathf.Abs(body2D.velocity.x) < Mathf.Abs(VelocityX)) ? VelocityX : body2D.velocity.x, (forceY == 0 && standing) ? VelocityY : body2D.velocity.y);
        body2D.AddForce(new Vector2(forceX, forceY));
    }
}
