using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;

    [SerializeField] Toggle audioToggle;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Slider ambienceSlider;
    [SerializeField] Slider musicSlider;

    // Ensure the UI matches the current settings
    private void Start()
    {
        float masterVolume, sfxVolume, ambienceVolume, musicVolume;

        audioMixer.GetFloat("MasterVolume", out masterVolume);
        audioMixer.GetFloat("SFXVolume", out sfxVolume);
        audioMixer.GetFloat("AmbienceVolume", out ambienceVolume);
        audioMixer.GetFloat("MusicVolume", out musicVolume);

        audioToggle.isOn = (masterVolume != -80);
        SFXSlider.value = sfxVolume;
        ambienceSlider.value = ambienceVolume;
        musicSlider.value = musicVolume;
    }

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
