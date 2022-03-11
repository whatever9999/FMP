using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/DisasterEffects/BreakObject")]
public class BreakObjectEffect : DisasterEffect
{
    public int number;

    public override void TriggerEffect()
    {
        base.TriggerEffect();

        int brokenObjects = 0;
        ConstantObject[] objects = FindObjectsOfType<ConstantObject>();
        while (brokenObjects < number)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].GetBreakType() == ConstantObject.BreakType.WORKING)
                {
                    // Flip a coin to see if this object breaks
                    int rand = Random.Range(0, 2);
                    if (rand == 0)
                    {
                        objects[i].SetToBreakableAlternate();
                        brokenObjects++;
                        // If we reach the required number of broken objects we're done
                        if (brokenObjects >= number) break;
                    }
                }
            }
        }
    }
}