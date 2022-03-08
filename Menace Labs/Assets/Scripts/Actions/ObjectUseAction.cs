using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectUseAction : Action
{
    public enum ObjectType
    {
        CONSTANT,
        TIMED,
        MAX_NEED,
        NUM_OBJECT_TYPES,
    }

    [SerializeField] private Image actionImage;
    [SerializeField] private TextMeshProUGUI tooltipText;

    private ObjectType objectType;

    private ConstantObject usedObject;
    public void SetObject(ConstantObject setTo)
    {
        actionImage.sprite = setTo.GetActionIcon();
        tooltipText.text = setTo.GetTooltip();
        usedObject = setTo;

        // Identify what type of object this is
        if (setTo is TimedObject) objectType = ObjectType.TIMED;
        else if (setTo is MaxNeedObject) objectType = ObjectType.MAX_NEED;
        else objectType = ObjectType.CONSTANT;
    }
    public ConstantObject GetUsedObject() { return usedObject; }

    public override bool StartAction()
    {
        actionStatus = Action_Status.STARTED;
        return usedObject.StartUsing();
    }
    public override bool ContinueAction()
    {
        if (objectType != ObjectType.CONSTANT && usedObject.IsFinished()) actionStatus = Action_Status.COMPLETED;
        return actionStatus == Action_Status.COMPLETED ? true : usedObject.Use();
    }
    public override void EndAction()
    {
        base.EndAction();
        usedObject.FinishUsing();
    }
    public override void CancelAction()
    {
        base.CancelAction();
        usedObject.CancelUsing();
    }
}
