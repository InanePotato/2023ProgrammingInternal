using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationScript : MonoBehaviour
{
    // Declare Class scope variables
    [Header("Animations")]
    public AnimationState currentAnimationState = AnimationState.Idle;
    public enum AnimationState { Idle, Walk, Attack, Damaged };

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        // Get and set a reference to the objects animator component
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // IF there is not a animation currently playing (using AnimatorPlaying method)
        if (!AnimatorPlaying())
        {
            // Play the idle animation
            ChangeAnimationState(AnimationState.Idle);
        }
    }

    /// <summary>
    /// Handles finding out whether the animation is playing or not
    /// </summary>
    /// <returns></returns>
    private bool AnimatorPlaying()
    {
        // Return whether the length of the animation is longer than the time it has been playing for
        return animator.GetCurrentAnimatorStateInfo(0).length > animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    /// <summary>
    /// Handles changes to animations
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeAnimationState(AnimationState newState)
    {
        // IF the new state is the same as the current state
        if (newState == currentAnimationState)
        {
            // Don’t continue (return)
            return;
        }

        // Play the new states animation
        animator.Play(newState.ToString());

        // Set the current state to the new state
        currentAnimationState = newState;
    }
}
