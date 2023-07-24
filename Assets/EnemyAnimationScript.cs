using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationScript : MonoBehaviour
{
    [Header("Animations")]
    public AnimationState currentAnimationState = AnimationState.Idle;
    public enum AnimationState { Idle, Walk, Attack };

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!AnimatorPlaying())
        {
            ChangeAnimationState(AnimationState.Idle);
        }
    }

    private bool AnimatorPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length > animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public void ChangeAnimationState(AnimationState newState)
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
}
