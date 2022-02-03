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
        started = true;

        if (soundEffect != SoundManager.SoundName.NUM_SOUND_NAMES)
        {
            if (loopSound) ManagerHandler.instance.clone.SetSoundLooping(true);
            ManagerHandler.instance.clone.PlaySound(soundEffect);
        }

        return true;
    }
    public override bool ContinueAction()
    {
        actionTimer += Time.deltaTime;
        if (actionTimer > timeForAction)
        {
            completed = true;
        }
        return true;
    }
    public override void EndAction()
    {
        if(loopSound) ManagerHandler.instance.clone.SetSoundLooping(false);

        switch (actionType)
        {
            case ActionManager.ActionType.DIE:
                ManagerHandler.instance.PopupM.ShowDeath();
                break;
        }
    }
    public override void CancelAction()
    {
        if (loopSound) ManagerHandler.instance.clone.SetSoundLooping(false);
    }
}
