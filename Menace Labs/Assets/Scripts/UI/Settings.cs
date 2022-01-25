using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void ToggleAudio(Toggle toggle)
    {
        if (toggle.isOn)
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
        audioMixer.SetFloat("AmbienceVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }
}
