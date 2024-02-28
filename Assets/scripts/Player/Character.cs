using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] public static int Health = 100;
    public float speed = 7;
    public float jumpSpeed = 5f;
    public float jetSpeed = 5f;
    public float boostMultiplier = 3f;
    public float airMultiplier = 0.5f;
    public bool canFly = true;
    private Animator animator;
    private Rigidbody2D body2D;
    [SerializeField]private JetController JetPack;

    private float VelosX = 0;

    void Awake()
    {
        body2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void Walk(float Direction, bool standing) 
    {
        if (Direction != 0)
        {
            transform.localScale = new Vector3((Mathf.Abs(Direction) / Direction) * Mathf.Abs(transform.localScale.x), 1 * transform.localScale.y, 1);
            VelosX = ((standing) ? 1 : airMultiplier) * Direction * speed;
            if (Mathf.Abs(VelosX) < Mathf.Abs(body2D.velocity.x) && body2D.velocity.x/VelosX > 0) { VelosX = body2D.velocity.x; }
        }
        else if (Mathf.Abs(body2D.velocity.x) < 0.7 * speed)
        {
            VelosX = 0;
        }
        else { VelosX = body2D.velocity.x; }

        body2D.velocity = new Vector2(VelosX, (standing) ? 0 : body2D.velocity.y);
        if (standing && Direction != 0) { animator.SetInteger("AnimState", 1); }
        else if (standing) { animator.SetInteger("AnimState", 0); }
        else { animator.SetInteger("AnimState", 2); }
        JetPack.Fly(false);
    }

    public void Fly(float Direction)
    {
        if (Direction != 0) { transform.localScale = new Vector3((Mathf.Abs(Direction) / Direction) * Mathf.Abs(transform.localScale.x), 1 * transform.localScale.y, 1); }
        float VelosX = airMultiplier * Direction * speed;
        if (Mathf.Abs(VelosX) < Mathf.Abs(body2D.velocity.x) && body2D.velocity.x / VelosX > 0) { VelosX = body2D.velocity.x; }
        body2D.velocity = new Vector2(VelosX, jetSpeed);
        animator.SetInteger("AnimState", 2);
        JetPack.Fly(true);
    }

    public void Jump()
    {
        body2D.velocity = new Vector2(body2D.velocity.x, jumpSpeed);
    }

    public void Boost(float Direction,bool standing)
    {
        transform.localScale = new Vector3((Mathf.Abs(Direction) / Direction) * Mathf.Abs(transform.localScale.x), 1 * transform.localScale.y, 1);
        float VelosX = boostMultiplier * Direction * speed;
        if (Mathf.Abs(VelosX) < Mathf.Abs(body2D.velocity.x) && body2D.velocity.x / VelosX > 0) { VelosX = body2D.velocity.x; }
        body2D.velocity = new Vector2(VelosX, (standing) ? 0 : body2D.velocity.y);
        animator.SetInteger("AnimState", 3);
        JetPack.Fly(true);
    }
}
