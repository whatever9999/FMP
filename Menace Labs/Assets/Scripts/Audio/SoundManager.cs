using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum SoundName
    {
        ARCADE_MACHINE,
        BORED,
        BROKEN_ITEM,
        BUTTON_CLICK,
        CAMERA,
        CHANGE_VOLUME,
        COUGHING,
        DEATH,
        EARTHQUAKE,
        EATING,
        ELECTROCUTION,
        FAILURE,
        FIRE,
        FLARE,
        FLOOD,
        FRIDGE,
        JUKEBOX,
        OVEN,
        PHONE,
        PUDDLE,
        REACTING,
        REFUSING,
        RIOT,
        RUBBISH,
        SHOWER,
        SLEEP,
        SNOOKER,
        SOFA,
        STEP,
        SUCCESS,
        THROWING_UP,
        TYPING,
    }

    [SerializeField] private SoundEffect[] soundEffects;

    private Dictionary<SoundName, AudioClip> soundDictionary = new Dictionary<SoundName, AudioClip>();

    private void Start()
    {
        for (int i = 0; i < soundEffects.Length; i++)
        {
            if (soundDictionary.ContainsKey(soundEffects[i].name))
            {
                Debug.LogError("Trying to add multiple sound effects of type " + soundEffects[i].name);
            }
            else
            {
                soundDictionary.Add(soundEffects[i].name, soundEffects[i].clip);
            }
        }
    }
}

[System.Serializable]
public struct SoundEffect
{
    public SoundManager.SoundName name;
    public AudioClip clip;
}
