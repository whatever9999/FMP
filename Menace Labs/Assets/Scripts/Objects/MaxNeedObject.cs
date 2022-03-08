using UnityEngine;

public class MaxNeedObject : ConstantObject
{
    [Header("Needs to Max")]
    [Tooltip("The clone will use this object until these needs are maxed out")]
    [SerializeField] private NeedsManager.NeedType[] needs;

    protected override void Update()
    {
        base.Update();

        if (beingUsed)
        {
            bool needsMaxed = true;

            for (int i = 0; i < needs.Length; i++)
            {
                if (ManagerHandler.instance.NeedsM.GetNeedValue(needs[i]) < (float)NeedsManager.NeedLevel.MAX)
                {
                    needsMaxed = false;
                }
            }

            if (needsMaxed) finished = true;
        }
    }
}
