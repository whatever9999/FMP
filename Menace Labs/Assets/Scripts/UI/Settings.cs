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

    [SerializeField] Slider mouseRotateSpeedSlider;
    [SerializeField] Slider keyboardRotateSpeedSlider;
    [SerializeField] Slider zoomSpeedSlider;
    [SerializeField] Slider moveSpeedSlider;
    [SerializeField] Slider edgeScrollSizeSlider;
    [SerializeField] Slider jumpToCloneMoveSpeedSlider;
    [SerializeField] Slider jumpToCloneRotateSpeedSlider;

    [SerializeField] Toggle objectTooltipToggle;

    // Ensure the UI matches the current settings
    private void Start()
    {
        UpdateSoundUI();
        UpdateCameraUI();
        UpdateCameraUI();
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

    #region Camera
    public void SetMouseRotateSpeed(float value)
    {
        SaveManager.instance.SetCameraValue(SaveManager.CameraValueTypes.MOUSE_ROTATE_SPEED, value);
    }
    public void SetKeyboardRotateSpeed(float value)
    {
        SaveManager.instance.SetCameraValue(SaveManager.CameraValueTypes.KEYBOARD_ROTATE_SPEED, value);
    }
    public void SetZoomSpeed(float value)
    {
        SaveManager.instance.SetCameraValue(SaveManager.CameraValueTypes.ZOOM_SPEED, value);
    }
    public void SetMoveSpeed(float value)
    {
        SaveManager.instance.SetCameraValue(SaveManager.CameraValueTypes.MOVE_SPEED, value);
    }
    public void SetEdgeScrollSize(float value)
    {
        SaveManager.instance.SetCameraValue(SaveManager.CameraValueTypes.EDGE_SCROLL_SIZE, value);
    }
    public void SetJTCMoveSpeed(float value)
    {
        SaveManager.instance.SetCameraValue(SaveManager.CameraValueTypes.JUMP_TO_CLONE_MOVE_SPEED, value);
    }
    public void SetJTCRotateSpeed(float value)
    {
        SaveManager.instance.SetCameraValue(SaveManager.CameraValueTypes.JUMP_TO_CLONE_ROTATE_SPEED, value);
    }
    #endregion // Camera

    #region Reset
    public void ResetCamera()
    {
        SaveManager.instance.Reset(SaveManager.SettingsTypes.CAMERA);
        UpdateCameraUI();
    }
    public void ResetUI()
    {
        SaveManager.instance.Reset(SaveManager.SettingsTypes.UI);
        UpdateUIUI();
    }
    public void ResetSound()
    {
        SaveManager.instance.Reset(SaveManager.SettingsTypes.SOUND);
        UpdateSoundUI();
    }

    public void UpdateSoundUI()
    {
        masterSlider.value = SaveManager.instance.GetSave().masterVolume;
        SFXSlider.value = SaveManager.instance.GetSave().SFXVolume;
        ambienceSlider.value = SaveManager.instance.GetSave().ambienceVolume;
        musicSlider.value = SaveManager.instance.GetSave().musicVolume;
    }
    public void UpdateUIUI()
    {
        objectTooltipToggle.SetIsOnWithoutNotify(SaveManager.instance.GetSave().enableObjectTooltips);
    }
    public void UpdateCameraUI()
    {
        mouseRotateSpeedSlider.value = SaveManager.instance.GetSave().mouseRotateSpeed;
        keyboardRotateSpeedSlider.value = SaveManager.instance.GetSave().keyboardRotateSpeed;
        zoomSpeedSlider.value = SaveManager.instance.GetSave().zoomSpeed;
        moveSpeedSlider.value = SaveManager.instance.GetSave().moveSpeed;
        edgeScrollSizeSlider.value = SaveManager.instance.GetSave().edgeScrollSize;
        jumpToCloneMoveSpeedSlider.value = SaveManager.instance.GetSave().jumpToCloneMoveSpeed;
        jumpToCloneRotateSpeedSlider.value = SaveManager.instance.GetSave().jumpToCloneRotateSpeed;
    }
    #endregion // Reset
}
