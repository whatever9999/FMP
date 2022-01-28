public class ConstantObjectUseAction : Action
{
    private ConstantObject usedObject;
    public void SetObject(ConstantObject setTo) { usedObject = setTo; }

    public override bool StartAction()
    {
        started = true;
        return usedObject.StartUsing();
    }
    public override bool ContinueAction()
    {
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
