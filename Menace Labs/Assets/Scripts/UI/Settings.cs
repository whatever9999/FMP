using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] Slider masterSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Slider ambienceSlider;
    [SerializeField] Slider musicSlider;

    [SerializeField] Toggle objectTooltipToggle;

    // Ensure the UI matches the current settings
    private void Start()
    {
        // Sound
        masterSlider.value = SaveManager.instance.GetSave().masterVolume;
        SFXSlider.value = SaveManager.instance.GetSave().SFXVolume;
        ambienceSlider.value = SaveManager.instance.GetSave().ambienceVolume;
        musicSlider.value = SaveManager.instance.GetSave().musicVolume;

        // UI
        objectTooltipToggle.SetIsOnWithoutNotify(SaveManager.instance.GetSave().enableObjectTooltips);
    }

    #region Sound
    public void SetMasterVolume(float volume)
    {
        if (volume == masterSlider.minValue)
        {
            volume = -80;
        }
        audioMixer.SetFloat("MasterVolume", volume);
        SaveManager.instance.SetSoundVolume(SaveManager.SoundTypes.MASTER, volume);
    }

    public void SetSFXVolume(float volume)
    {
        // The min value is about -40 since we want the slider to have a better range - once it reaches this we need to mute it though
        if (volume == SFXSlider.minValue)
        {
            volume = -80;
        }
        audioMixer.SetFloat("SFXVolume", volume);
        SaveManager.instance.SetSoundVolume(SaveManager.SoundTypes.SFX, volume);
    }

    public void SetAmbienceVolume(float volume)
    {
        // The min value is about -40 since we want the slider to have a better range - once it reaches this we need to mute it though
        if (volume == ambienceSlider.minValue)
        {
            volume = -80;
        }
        audioMixer.SetFloat("AmbienceVolume", volume);
        SaveManager.instance.SetSoundVolume(SaveManager.SoundTypes.AMBIENCE, volume);
    }

    public void SetMusicVolume(float volume)
    {
        // The min value is about -40 since we want the slider to have a better range - once it reaches this we need to mute it though
        if (volume == musicSlider.minValue)
        {
            volume = -80;
        }
        audioMixer.SetFloat("MusicVolume", volume);
        SaveManager.instance.SetSoundVolume(SaveManager.SoundTypes.MUSIC, volume);
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
