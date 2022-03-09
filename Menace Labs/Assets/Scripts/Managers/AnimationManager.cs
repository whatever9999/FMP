using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public enum AnimationType
    {
        IDLE,
        WALKING,
        DANCING,
        BORED,
        CLEAN_PUDDLE,
        CLEAN_OBJECT,
        USE_FRIDGE,
        DIE,
        EATING,
        FIXING,
        USE_OVEN,
        REFUSE,
        PLAY_GAMES,
        REACT,
        SHOWER,
        SLEEP,
        TYPE,
        PLAY_DARTS,
        TIDY_RUBBISH,
        USE_CAMERA,
        USE_PHONE,
        SIT,
        FIRE,
        PUT_OUT_FIRE,
        ELECTROCUTION,
        PASS_OUT,
        NUM_ANIMATION_TYPES,
    }

    [SerializeField] private AnimationClip[] idleAnimationSet;
    [SerializeField] private AnimationClip[] walkingAnimationSet;
    [SerializeField] private AnimationClip[] dancingAnimationSet;
    [SerializeField] private AnimationClip[] boredAnimationSet;
    [SerializeField] private AnimationClip[] cleanPuddleAnimationSet;
    [SerializeField] private AnimationClip[] cleanObjectAnimationSet;
    [SerializeField] private AnimationClip[] useFridgeAnimationSet;
    [SerializeField] private AnimationClip[] dieAnimationSet;
    [SerializeField] private AnimationClip[] eatingAnimationSet;
    [SerializeField] private AnimationClip[] fixingAnimationSet;
    [SerializeField] private AnimationClip[] useOvenAnimationSet;
    [SerializeField] private AnimationClip[] refuseAnimationSet;
    [SerializeField] private AnimationClip[] playGamesAnimationSet;
    [SerializeField] private AnimationClip[] reactAnimationSet;
    [SerializeField] private AnimationClip[] showerAnimationSet;
    [SerializeField] private AnimationClip[] sleepAnimationSet;
    [SerializeField] private AnimationClip[] typeAnimationSet;
    [SerializeField] private AnimationClip[] playDartsAnimationSet;
    [SerializeField] private AnimationClip[] tidyRubbishAnimationSet;
    [SerializeField] private AnimationClip[] useCameraAnimationSet;
    [SerializeField] private AnimationClip[] usePhoneAnimationSet;
    [SerializeField] private AnimationClip[] sitAnimationSet;
    [SerializeField] private AnimationClip[] onFireanimationSet;
    [SerializeField] private AnimationClip[] putOutFireAnimationSet;
    [SerializeField] private AnimationClip[] electrocutionAnimationSet;
    [SerializeField] private AnimationClip[] passOutAnimationSet;

    private Animator animator;

    private Dictionary<AnimationType, AnimationClip[]> animations = new Dictionary<AnimationType, AnimationClip[]>();

    private void Awake()
    {
        animator = GetComponent<Animator>();

        animations.Add(AnimationType.IDLE, idleAnimationSet);
        animations.Add(AnimationType.WALKING, walkingAnimationSet);
        animations.Add(AnimationType.DANCING, dancingAnimationSet);
        animations.Add(AnimationType.BORED, boredAnimationSet);
        animations.Add(AnimationType.CLEAN_PUDDLE, cleanPuddleAnimationSet);
        animations.Add(AnimationType.CLEAN_OBJECT, cleanObjectAnimationSet);
        animations.Add(AnimationType.USE_FRIDGE, useFridgeAnimationSet);
        animations.Add(AnimationType.DIE, dieAnimationSet);
        animations.Add(AnimationType.EATING, eatingAnimationSet);
        animations.Add(AnimationType.FIXING, fixingAnimationSet);
        animations.Add(AnimationType.USE_OVEN, useOvenAnimationSet);
        animations.Add(AnimationType.REFUSE, refuseAnimationSet);
        animations.Add(AnimationType.PLAY_GAMES, playGamesAnimationSet);
        animations.Add(AnimationType.REACT, reactAnimationSet);
        animations.Add(AnimationType.SHOWER, showerAnimationSet);
        animations.Add(AnimationType.SLEEP, sleepAnimationSet);
        animations.Add(AnimationType.TYPE, typeAnimationSet);
        animations.Add(AnimationType.PLAY_DARTS, playDartsAnimationSet);
        animations.Add(AnimationType.TIDY_RUBBISH, tidyRubbishAnimationSet);
        animations.Add(AnimationType.USE_CAMERA, useCameraAnimationSet);
        animations.Add(AnimationType.USE_PHONE, usePhoneAnimationSet);
        animations.Add(AnimationType.SIT, sitAnimationSet);
        animations.Add(AnimationType.FIRE, onFireanimationSet);
        animations.Add(AnimationType.PUT_OUT_FIRE, putOutFireAnimationSet);
        animations.Add(AnimationType.ELECTROCUTION, electrocutionAnimationSet);
        animations.Add(AnimationType.PASS_OUT, passOutAnimationSet);
    }

    public bool IsIdle()
    {
        return animator.GetCurrentAnimatorClipInfo(0)[0].clip == idleAnimationSet[0] || animator.GetCurrentAnimatorClipInfo(0)[0].clip == onFireanimationSet[0];
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
            case AnimationType.BORED:
                animator.SetBool("Bored", enable);
                break;
            case AnimationType.CLEAN_PUDDLE:
                animator.SetBool("Clean_Puddle", enable);
                break;
            case AnimationType.CLEAN_OBJECT:
                animator.SetBool("Clean_Object", enable);
                break;
            case AnimationType.USE_FRIDGE:
                animator.SetBool("Use_Fridge", enable);
                break;
            case AnimationType.DIE:
                if (enable) animator.SetTrigger("Die");
                break;
            case AnimationType.EATING:
                animator.SetBool("Eating", enable);
                break;
            case AnimationType.FIXING:
                animator.SetBool("Fixing", enable);
                break;
            case AnimationType.USE_OVEN:
                animator.SetBool("Use_Oven", enable);
                break;
            case AnimationType.REFUSE:
                animator.SetBool("Refuse", enable);
                break;
            case AnimationType.PLAY_GAMES:
                animator.SetBool("Play_Games", enable);
                break;
            case AnimationType.REACT:
                animator.SetBool("React", enable);
                break;
            case AnimationType.SHOWER:
                animator.SetBool("Shower", enable);
                break;
            case AnimationType.SLEEP:
                animator.SetBool("Sleep", enable);
                break;
            case AnimationType.TYPE:
                animator.SetBool("Type", enable);
                break;
            case AnimationType.PLAY_DARTS:
                animator.SetBool("Play_Darts", enable);
                break;
            case AnimationType.TIDY_RUBBISH:
                animator.SetBool("Tidy_Rubbish", enable);
                break;
            case AnimationType.USE_CAMERA:
                animator.SetBool("Use_Camera", enable);
                break;
            case AnimationType.USE_PHONE:
                animator.SetBool("Use_Phone", enable);
                break;
            case AnimationType.SIT:
                animator.SetBool("Sit", enable);
                break;
            case AnimationType.FIRE:
                animator.SetBool("Fire", enable);
                break;
            case AnimationType.PUT_OUT_FIRE:
                animator.SetBool("Put_Out_Fire", enable);
                break;
            case AnimationType.ELECTROCUTION:
                if (enable) animator.SetTrigger("Electrocution");
                break;
            case AnimationType.PASS_OUT:
                animator.SetBool("Pass_Out", enable);
                break;
        }
    }

    public float GetAnimationLength(AnimationType type)
    {
        AnimationClip[] animation_set;
        animations.TryGetValue(type, out animation_set);

        float animation_length = 0.0f;
        for (int i = 0; i < animation_set.Length; i++)
        {
            animation_length += animation_set[i].length;
        }
        return animation_length;
    }
}
