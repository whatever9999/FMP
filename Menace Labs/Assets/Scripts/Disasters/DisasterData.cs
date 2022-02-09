using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/DisasterData")]
public class DisasterData : ScriptableObject
{
    [Header("Disaster Data")]
    public string title;
    [TextArea(15, 20)]
    public string description;
    public DisasterEffect disasterEffect;

    [Header("Visualisation")]
    public GameObject representationPrefab;
    public SoundManager.SoundName soundEffect;
}
