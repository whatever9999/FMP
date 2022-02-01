using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/GoalData")]
public class GoalData : ScriptableObject
{
    public GoalManager.GoalType goalType;
    public string goalName;
    public string goal;
    [TextArea(15, 20)]
    public string description;
    [TextArea(15, 20)]
    public string story;
    public Sprite goalIcon;
}
