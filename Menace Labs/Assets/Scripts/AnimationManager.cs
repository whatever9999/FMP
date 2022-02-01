using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public enum AnimationType
    {
        IDLE,
        WALKING,
        DANCING,
        DYING,
        BOREDOM,
        REFUSE,
        REACT,
        CLEANING,
        FIXING,
        USE_FRIDGE,
        SIT,
        PLAY_GAMES,
        PHONE_CALL,
        USE_TOILET,
        SHOWER,
        SLEEP,
        USE_OVEN,
        USE_CAMERA,
        PLAY_POOL,
        USE_COMPUTER,
        MOP_PUDDLE,
        TIDY_RUBBISH,
        EAT,
        NUM_ANIMATION_TYPES,
    }

    [SerializeField] private AnimationClip idleAnimation;
    [SerializeField] private AnimationClip walkingAnimation;
    [SerializeField] private AnimationClip dancingAnimation;
    [SerializeField] private AnimationClip dyingAnimation;

    private Animator animator;

    private Dictionary<AnimationType, AnimationClip> animations = new Dictionary<AnimationType, AnimationClip>();

    private void Start()
    {
        animator = GetComponent<Animator>();

        animations.Add(AnimationType.IDLE, idleAnimation);
        animations.Add(AnimationType.WALKING, walkingAnimation);
        animations.Add(AnimationType.DANCING, dancingAnimation);
        animations.Add(AnimationType.DYING, dyingAnimation);
    }

    public void SetAnimation(AnimationType type, bool enable)
    {
        switch (type)
        {
            case AnimationType.IDLE:
                // Set all animation parameters after idle to false so we return to idle
                if (enable)
                {
                    for (AnimationType i = AnimationType.WALKING; i < AnimationType.NUM_ANIMATION_TYPES; i++)
                    {
                        SetAnimation(i, false);
                    }
                }
                break;
            case AnimationType.WALKING:
                animator.SetBool("Walking", enable);
                break;
            case AnimationType.DANCING:
                animator.SetBool("Dancing", enable);
                break;
        }
    }

    public float GetAnimationLength(AnimationType type)
    {
        AnimationClip animation;
        animations.TryGetValue(type, out animation);
        if (animation) return animation.length;
        return 0.0f;
    }
}
