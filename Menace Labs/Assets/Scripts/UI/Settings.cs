using UnityEngine;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void ToggleAudio(bool value)
    {
        if (value)
        {
            audioMixer.SetFloat("MasterVolume", 0);
        }
        else
        {
            audioMixer.SetFloat("MasterVolume", -80);
        }
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void SetAmbienceVolume(float volume)
    {
        audioMixer.SetFloat("AmbiencerVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }
}
