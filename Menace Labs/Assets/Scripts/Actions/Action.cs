using UnityEngine;
using UnityEngine.UI;

public abstract class Action : MonoBehaviour
{
    public enum Action_Status
    {
        QUEUED,
        STARTED,
        COMPLETED,
        ENDING,
        CANCELLING,
        FINALISED,
    }

    [SerializeField] protected bool cancellable = true;
    [SerializeField] protected ActionManager.ActionType actionType = ActionManager.ActionType.NUM_ACTION_TYPES;
    [SerializeField] protected AnimationManager.AnimationType animationType = AnimationManager.AnimationType.NUM_ANIMATION_TYPES;
    [SerializeField] protected SoundManager.SoundName soundEffect = SoundManager.SoundName.NUM_SOUND_NAMES;
    [SerializeField] protected bool loopSound = false;

    // UI that appears when the action is cancelling
    private GameObject finalisingCross;
    private Image background;

    public void SetCancellable(bool setTo) { cancellable = setTo; }
    public ActionManager.ActionType GetActionType() { return actionType; }
    public void SetActionType(ActionManager.ActionType newType) { actionType = newType; }
    public AnimationManager.AnimationType GetAnimationType() { return animationType; }
    public void SetAnimationType(AnimationManager.AnimationType newType) { animationType = newType; }

    protected Action_Status actionStatus = Action_Status.QUEUED;
    public bool HasStarted() { return actionStatus >= Action_Status.STARTED; }
    public bool HasCompleted() { return actionStatus >= Action_Status.COMPLETED; }
    public bool IsEnding() { return actionStatus == Action_Status.ENDING || actionStatus == Action_Status.FINALISED; }
    public bool IsCancelling() { return actionStatus == Action_Status.CANCELLING || actionStatus == Action_Status.FINALISED; }
    public bool HasFinalised() { return actionStatus >= Action_Status.FINALISED; }

    protected virtual void Awake()
    {
        finalisingCross = transform.Find("FinalisingCross").gameObject;
        background = GetComponent<Image>();
    }

    private void Update()
    {
        if (actionStatus == Action_Status.ENDING || actionStatus == Action_Status.CANCELLING)
        {
            // If we're back to the idle animation then the action has finished cancelling/ended
            if (ManagerHandler.instance.AnimationM.IsIdle()) actionStatus = Action_Status.FINALISED;
        }
    }

    public abstract bool StartAction();
    public abstract bool ContinueAction();
    public virtual void EndAction()
    {
        actionStatus = Action_Status.ENDING;
    }
    public virtual void CancelAction()
    {
        finalisingCross.SetActive(true);
        actionStatus = Action_Status.CANCELLING;
    }

    public void SetColour(Color color)
    {
        background.color = color;
    }

    public void OnClick()
    {
        if (cancellable)
        {
            ManagerHandler.instance.ActionM.CancelAction(gameObject);
        }
    }
}