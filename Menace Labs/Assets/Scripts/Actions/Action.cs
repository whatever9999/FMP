using UnityEngine;

public abstract class Action : MonoBehaviour
{
    [SerializeField] protected bool cancellable = true;
    [SerializeField] protected ActionManager.ActionType actionType = ActionManager.ActionType.NUM_ACTION_TYPES;
    [SerializeField] protected AnimationManager.AnimationType animationType = AnimationManager.AnimationType.NUM_ANIMATION_TYPES;
    public void SetCancellable(bool setTo) { cancellable = setTo; }
    public ActionManager.ActionType GetActionType() { return actionType; }
    public void SetActionType(ActionManager.ActionType newType) { actionType = newType; }
    public AnimationManager.AnimationType GetAnimationType() { return animationType; }
    public void SetAnimationType(AnimationManager.AnimationType newType) { animationType = newType; }

    protected bool completed;
    protected bool started;
    public bool HasStarted() { return started; }
    public bool HasCompleted() { return completed; }

    public abstract bool StartAction();
    public abstract bool ContinueAction();
    public abstract void EndAction();
    public abstract void CancelAction();

    public void OnClick()
    {
        if (cancellable)
        {
            ActionManager.instance.CancelAction(gameObject);
        }
    }
}