using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float standingThreshold = 4f;
    private float jumpCd;
    private Rigidbody2D body2D;
    public static float fuel = 1f;
    private float MaxFuel = 1f;

    [SerializeField] private float Fuel_Waste_Boost = 0.5f;
    [SerializeField] private float Fuel_Waste_Fly = 0.1f;
    [SerializeField] private float Fuel_Recovery = 0.25f;

    private bool canFly = true;

    public bool Jet;

    private bool standing;

    private Character character;

    void Start()
    {
        body2D = GetComponent<Rigidbody2D>();
        character = GetComponent<Character>();
    }

    void Update()
    {
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
            if (verticalInput == 0)
            {
                character.Walk(horizontalInput, standing);
            }
            else if (verticalInput > 0 && canFly && fuel > 0)
            {
                character.Fly(horizontalInput);
                fuel -= Fuel_Waste_Fly * Time.deltaTime;
            }
            else if (verticalInput < 0 && canFly && fuel > 0)
            {
                character.Boost(horizontalInput, standing);
                fuel -= Fuel_Waste_Boost * Time.deltaTime;
            }
            else { character.Walk(0, standing); }
        }
        else if (verticalInput > 0 && canFly && fuel > 0)
        {
            character.Fly(0);
            fuel -= Fuel_Waste_Fly * Time.deltaTime;
        }
        else { character.Walk(0, standing); }
        if (fuel <= 0) { canFly = false; }

        if (verticalInput == 0)
        {
            if (fuel < MaxFuel)
            {
                fuel += Fuel_Recovery * Time.deltaTime;
            }
            if (!canFly && fuel > 0.5 * MaxFuel) { canFly = true; }
        }

    }
}
