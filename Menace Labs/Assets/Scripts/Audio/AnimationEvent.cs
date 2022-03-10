using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/AnimationEvent")]
public class AnimationEvent : ScriptableObject
{
    public string objectName;
    public SoundManager.SoundName soundName;
}
