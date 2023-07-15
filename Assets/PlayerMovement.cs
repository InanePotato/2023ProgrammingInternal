using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Game Manager")]
    private GameManagerScript gameManagerScript;

    [Header("Movement")]
    public float walkSpeed = 1.2f;
    public float speedLimiter = 0.7f;

    float inputHorizontal;
    float inputVertical;
    Rigidbody2D rb;
    SpriteRenderer childSpriteRenderer;


    [Header("Animations")]
    public AnimationState currentAnimationState;
    public enum AnimationState { Idle, Walk, Damaged };

    Animator animator;
    

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameManagerScript.Instance;

        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManagerScript.gamePaused)
        {
            inputHorizontal = Input.GetAxisRaw("Horizontal");
            inputVertical = Input.GetAxisRaw("Vertical");
        }
    }

    private void FixedUpdate()
    {
        //rb.MovePosition(rb.position + new Vector2(inputHorizontal, inputVertical) * walkSpeed * Time.fixedDeltaTime);
        if (inputHorizontal != 0 || inputVertical != 0)
        {
            if (inputHorizontal != 0 && inputVertical != 0)
            {
                inputHorizontal *= speedLimiter;
                inputVertical *= speedLimiter;
            }

            rb.velocity = new Vector2 (inputHorizontal * walkSpeed, inputVertical * walkSpeed);
            ChangeAnimationState(AnimationState.Walk);

            if (inputHorizontal < 0)
            {
                childSpriteRenderer.flipX = true;
            }
            else if (inputHorizontal > 0)
            {
                childSpriteRenderer.flipX = false;
            }
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
            ChangeAnimationState(AnimationState.Idle);
        }
    }

    private void ChangeAnimationState(AnimationState newState)
    {
        //stop from changing if the same
        if (newState == currentAnimationState)
        {
            return;
        }

        // changes animation
        animator.Play(newState.ToString());

        // sets new animation state
        currentAnimationState = newState;
    }

    public void setPlayerSprite()
    {
        animator = gameManagerScript.playerSprite.GetComponent<Animator>();
        childSpriteRenderer = gameManagerScript.playerSprite.GetComponent<SpriteRenderer>();

        currentAnimationState = AnimationState.Idle;
        animator.Play(currentAnimationState.ToString());
    }
}
