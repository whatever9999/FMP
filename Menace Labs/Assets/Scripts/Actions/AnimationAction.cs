using UnityEngine;

public class AnimationAction : Action
{
    private AnimationManager animationManager;
    private float timeForAction;
    private float actionTimer;

    private void Start()
    {
        animationManager = FindObjectOfType<AnimationManager>();
        timeForAction = animationManager.GetAnimationLength(animationType);
    }

    public override bool StartAction()
    {
        actionStatus = Action_Status.STARTED;

        if (soundEffect != SoundManager.SoundName.NUM_SOUND_NAMES)
        {
            ManagerHandler.instance.clone.PlaySound(soundEffect, loopSound);
        }

        return true;
    }
    public override bool ContinueAction()
    {
        actionTimer += Time.deltaTime;
        if (actionTimer > timeForAction)
        {
            actionStatus = Action_Status.COMPLETED;
        }
        return true;
    }
    public override void EndAction()
    {
        base.EndAction();
        if (loopSound) ManagerHandler.instance.clone.SetSoundLooping(false);
    }
    public override void CancelAction()
    {
        base.CancelAction();
        if (loopSound) ManagerHandler.instance.clone.SetSoundLooping(false);
    }
}
