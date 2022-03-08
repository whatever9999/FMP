using UnityEngine;

public class TimedObject : ConstantObject
{
    [Header("Timer")]
    [Tooltip("The time to use this object will be determined by how long it takes for its animations to run through")]
    [SerializeField] private bool useAnimationSetForTimeToUse = true;
    [Tooltip("If this is 0 the animation set time will be used")]
    [SerializeField] private float timeToUse = 0.0f;

    private new void Start()
    {
        base.Start();

        if (useAnimationSetForTimeToUse || timeToUse == 0.0f)
        {
            timeToUse = ManagerHandler.instance.AnimationM.GetAnimationLength(animationType);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (beingUsed)
        {
            int timeBeingUsed = ManagerHandler.instance.TimeM.TimeSince(startedUsingTime);

            if (timeBeingUsed > timeToUse)
            {
                finished = true;
            }
        }
    }
}
