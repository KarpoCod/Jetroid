using System;
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
    public static float fuel = 1f;

    [SerializeField] private float Fuel_Waste_Boost = 0.18f;
    [SerializeField] private float Fuel_Waste_Fly = 0.03f;
    [SerializeField] private float Fuel_Recovery = 0.02f;

    [SerializeField] private float FlyCD_const = 3f;
    private float FlyCD;
    private bool canFly = true;

    public bool Jet;
    public int AnimSt;

    private bool standing;

    private Animator animator;

    void Start()
    {
        jumpCd = 0;
        FlyCD = FlyCD_const;
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

        if (absVelY <= standingThreshold && Physics2D.Raycast(transform.position + new Vector3(0.5f, 0, 0), -Vector2.up, 1.05f, LayerMask.GetMask("FG"))
            || Physics2D.Raycast(transform.position + new Vector3(-0.5f, 0, 0), -Vector2.up, 1.05f, LayerMask.GetMask("FG"))) // проверка если чел завтыкал на земле
        {
            standing = true;
        }
        else
        {
            standing = false;
        }

        if (horizontalInput != 0)
        {
            if (standing)
            {
                AnimSt = 1;
            }
            if (verticalInput < 0 && Jet && fuel > 0 && canFly) // токийское наваливание боком
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

                fuel -= Time.deltaTime * Fuel_Waste_Boost;
            }
            else if (absVelX < maxVelocity.x) // можем добавить скорости если она не достигла максимума
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
        else // не можем
        {
            transform.rotation = Quaternion.Euler(0, 0, PlayerZ);
            AnimSt = 0;
        }

        if (verticalInput > 0 && Jet && fuel > 0 && canFly) // взлет с баконура
        {
            if (absVelY < maxVelocity.y)
            {
                forceY = jetForce * Time.deltaTime * verticalInput;
            }
            AnimSt = 2;

            fuel -= Time.deltaTime * Fuel_Waste_Fly;
        }
        else if (verticalInput > 0 && !Jet && standing) // прыг скок
        {
            if (jumpCd <= 0)
            {
                jumpCd = 0.1f;
                VelocityY = jumpForce * 75;
            }
        }
        if (!standing && (verticalInput == 0 || !Jet)) // аооаоаооооо падаю
        {
            AnimSt = 3;
        }

        if (jumpCd >= 0) // кд на прыг скок
        {
            jumpCd -= Time.deltaTime;
        }

        animator.SetBool("jet", Jet);
        animator.SetInteger("AnimState", AnimSt);

        body2D.velocity = new Vector2((Mathf.Abs(body2D.velocity.x) < Mathf.Abs(VelocityX)) ? VelocityX : body2D.velocity.x, (forceY == 0 && standing) ? VelocityY : body2D.velocity.y);
        body2D.AddForce(new Vector2(forceX, forceY));

        if (fuel < 0) fuel = 0; // обнуление топлива если оно ушло в минус

        if (fuel > 1) fuel = 1; // откат на 1 топлива если оно больше 1

        if (standing && canFly) fuel += Fuel_Recovery * Time.deltaTime; // восстановление топлива

        if (fuel == 0)
        {
            canFly = false;
            FlyCD -= Time.deltaTime;
        }
        if (FlyCD <= 0)
        {
            canFly = true;
            FlyCD = FlyCD_const;
        }
    }
}
