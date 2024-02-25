using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private int Health = 100;
    public float speed = 7;
    public float jumpVelocity = 5f;
    public float jetSpeed = 50f;
    public float boostMultiplier = 3f;
    public float airMultiplier = 0.5f;
    public bool canFly = true;
    private Animator animator;
    private Rigidbody2D body2D;

    void Start()
    {
        body2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void Walk(float Direction, bool standing) 
    {
        if (Direction != 0)
        {
            transform.localScale = new Vector3((Mathf.Abs(Direction) / Direction) * Mathf.Abs(transform.localScale.x), 1 * transform.localScale.y, 1);
        }
        float VelosX = ((standing) ? 1 : airMultiplier) * Direction * speed;
        body2D.velocity = new Vector2((Mathf.Abs(VelosX) > Mathf.Abs(body2D.velocity.x)) ? VelosX : body2D.velocity.x, (standing) ? 0 : body2D.velocity.y);
        if (standing && Direction != 0) { animator.SetInteger("AnimState", 1); }
        else if (standing) { animator.SetInteger("AnimState", 0); }
        else { animator.SetInteger("AnimState", 3); }
    }

    public void Fly(float Direction)
    {
        if (Direction != 0) { transform.localScale = new Vector3((Mathf.Abs(Direction) / Direction) * Mathf.Abs(transform.localScale.x), 1 * transform.localScale.y, 1); }
        float VelosX = airMultiplier * Direction * speed;
        body2D.velocity = new Vector2((Mathf.Abs(VelosX) > Mathf.Abs(body2D.velocity.x)) ? VelosX : body2D.velocity.x, jetSpeed);
        animator.SetInteger("AnimState", 2);
    }


    public void Boost(float Direction,bool standing)
    {
        transform.localScale = new Vector3((Mathf.Abs(Direction) / Direction) * Mathf.Abs(transform.localScale.x), 1 * transform.localScale.y, 1);
        float VelosX = boostMultiplier * Direction * speed;
        body2D.velocity = new Vector2((Mathf.Abs(VelosX) > Mathf.Abs(body2D.velocity.x)) ? VelosX : body2D.velocity.x, (standing) ? 0 : body2D.velocity.y);
        animator.SetInteger("AnimState", 4);
    }
}
