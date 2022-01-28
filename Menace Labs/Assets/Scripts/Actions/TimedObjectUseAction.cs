public class TimedObjectUseAction : Action
{
    private TimedObject usedObject;
    public void SetObject(TimedObject setTo) { usedObject = setTo; }

    public override bool StartAction()
    {
        started = true;
        return usedObject.StartUsing();
    }
    public override bool ContinueAction()
    {
        completed = usedObject.IsFinished();
        return usedObject.Use();
    }
    public override void EndAction()
    {
        usedObject.FinishUsing();
    }
    public override void CancelAction()
    {
        usedObject.CancelUsing();
    }
}
