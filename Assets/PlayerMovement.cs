        using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Game Manager")]
    private GameManagerScript gameManagerScript;

    private PlayerStats playerStatsScript;

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

        playerStatsScript = gameManagerScript.player.GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManagerScript.gamePaused && !gameManagerScript.gameOver)
        {
            inputHorizontal = Input.GetAxisRaw("Horizontal");
            inputVertical = Input.GetAxisRaw("Vertical");
        }
    }

    private void FixedUpdate()
    {
        // IF paused or over
        if (gameManagerScript.gamePaused || gameManagerScript.gameOver)
        {
            // don't continue / return
            return;
        }

        // IF inputs are not 0
        if (inputHorizontal != 0 || inputVertical != 0)
        {
            // IF both are not 0
            if (inputHorizontal != 0 && inputVertical != 0)
            {
                // times by speed limiter
                inputHorizontal *= speedLimiter;
                inputVertical *= speedLimiter;
            }

            // add velocity in input directions
            rb.velocity = new Vector2 (inputHorizontal * (walkSpeed + playerStatsScript.speedMultiplier), inputVertical * (walkSpeed + playerStatsScript.speedMultiplier));
            // change to walking animation
            ChangeAnimationState(AnimationState.Walk);

            // IF moving left then flip the sprite
            if (inputHorizontal < 0)
            {
                childSpriteRenderer.flipX = true;
            }
            else if (inputHorizontal > 0)
            {
                // ELSE IF moving right then don't flip the sprite
                childSpriteRenderer.flipX = false;
            }
        }
        else
        {
            // ELSE set velocity to 0 in both directions
            rb.velocity = new Vector2(0, 0);
            // set animation to idle
            ChangeAnimationState(AnimationState.Idle);
        }
    }

    /// <summary>
    /// Handles changing of animations
    /// </summary>
    /// <param name="newState"></param>
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

    /// <summary>
    /// Handles sprite changes
    /// </summary>
    public void setPlayerSprite()
    {
        // get references to sprite renderer and animatior
        animator = gameManagerScript.playerSprite.GetComponent<Animator>();
        childSpriteRenderer = gameManagerScript.playerSprite.GetComponent<SpriteRenderer>();

        // set animation ststus
        currentAnimationState = AnimationState.Idle;
        // play the animation
        animator.Play(currentAnimationState.ToString());
    }
}
