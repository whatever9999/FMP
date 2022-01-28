using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public enum AnimationType
    {
        IDLE,
        WALKING,
        DANCING,
        NUM_ANIMATION_TYPES,
    }

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
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
}
