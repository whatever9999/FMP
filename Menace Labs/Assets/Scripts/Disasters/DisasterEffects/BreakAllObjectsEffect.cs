using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/DisasterEffects/BreakAllObjects")]
public class BreakAllObjectsEffect : DisasterEffect
{
    public override void TriggerEffect()
    {
        ConstantObject[] objects = FindObjectsOfType<ConstantObject>();
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].GetBreakType() == ConstantObject.BreakType.WORKING)
            {
                objects[i].SetToBreakableAlternate();
            }
        }
    }
}