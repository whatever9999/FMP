using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;

    [SerializeField] Slider masterSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Slider ambienceSlider;
    [SerializeField] Slider musicSlider;

    [SerializeField] Toggle objectTooltipToggle;

    // Ensure the UI matches the current settings
    private void Start()
    {
        // Sound
        float masterVolume, sfxVolume, ambienceVolume, musicVolume;

        audioMixer.GetFloat("MasterVolume", out masterVolume);
        audioMixer.GetFloat("SFXVolume", out sfxVolume);
        audioMixer.GetFloat("AmbienceVolume", out ambienceVolume);
        audioMixer.GetFloat("MusicVolume", out musicVolume);

        masterSlider.value = masterVolume;
        SFXSlider.value = sfxVolume;
        ambienceSlider.value = ambienceVolume;
        musicSlider.value = musicVolume;

        // UI
        objectTooltipToggle.SetIsOnWithoutNotify(SaveManager.instance.GetSave().enableObjectTooltips);
    }

    #region Sound
    public void SetMasterVolume(float volume)
    {
        if (volume == masterSlider.minValue)
        {
            audioMixer.SetFloat("MasterVolume", -80);
        }
        else
        {
            audioMixer.SetFloat("MasterVolume", volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        // The min value is about -40 since we want the slider to have a better range - once it reaches this we need to mute it though
        if (volume == SFXSlider.minValue)
        {
            audioMixer.SetFloat("SFXVolume", -80);
        }
        else
        {
            audioMixer.SetFloat("SFXVolume", volume);
        }
    }

    public void SetAmbienceVolume(float volume)
    {
        // The min value is about -40 since we want the slider to have a better range - once it reaches this we need to mute it though
        if (volume == ambienceSlider.minValue)
        {
            audioMixer.SetFloat("AmbienceVolume", -80);
        }
        else
        {
            audioMixer.SetFloat("AmbienceVolume", volume);
        }
    }

    public void SetMusicVolume(float volume)
    {
        // The min value is about -40 since we want the slider to have a better range - once it reaches this we need to mute it though
        if (volume == musicSlider.minValue)
        {
            audioMixer.SetFloat("MusicVolume", -80);
        }
        else
        {
            audioMixer.SetFloat("MusicVolume", volume);
        }
    }

    // If SFX are at 0 don't play the sound boing
    public void PlaySoundBoing(AudioSource slider)
    {
        float sfxVolume;
        audioMixer.GetFloat("SFXVolume", out sfxVolume);
        if (sfxVolume != -80)
        {
            slider.Play();
        }
    }
    #endregion //Sound

    #region UI
    public void SetObjectTooltips()
    {
        SaveManager.instance.EnableObjectTooltips(objectTooltipToggle.isOn);
    }
    #endregion //UI
}
