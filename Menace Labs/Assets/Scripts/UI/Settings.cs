using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

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

    [SerializeField] TextMeshProUGUI upKeyText;
    [SerializeField] TextMeshProUGUI downKeyText;
    [SerializeField] TextMeshProUGUI leftKeyText;
    [SerializeField] TextMeshProUGUI rightKeyText;
    [SerializeField] TextMeshProUGUI rotateLeftKeyText;
    [SerializeField] TextMeshProUGUI rotateRightKeyText;
    [SerializeField] TextMeshProUGUI jumpToCloneKeyText;
    [SerializeField] TextMeshProUGUI speedCameraKeyText;
    [SerializeField] TextMeshProUGUI toggleUIKeyText;
    [SerializeField] TextMeshProUGUI pauseMenuKeyText;
    [SerializeField] TextMeshProUGUI playSpeedKeyText;
    [SerializeField] TextMeshProUGUI doubleSpeedKeyText;
    [SerializeField] TextMeshProUGUI tripleSpeedKeyText;
    [SerializeField] TextMeshProUGUI pauseSpeedKeyText;

    [SerializeField] Toggle objectTooltipToggle;

    // Ensure the UI matches the current settings
    private void Start()
    {
        UpdateSoundUI();
        UpdateCameraUI();
        UpdateUIUI();
        UpdateControlsUI();
    }

    private CameraHandler.KeyTypes settingControl = CameraHandler.KeyTypes.NUM_KEYS;
    private Image settingControlButton;
    private void Update()
    {
        if (settingControl != CameraHandler.KeyTypes.NUM_KEYS)
        {
            foreach (KeyCode code in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(code) && CodeIsValid(code))
                {
                    SaveManager.instance.SetControl(settingControl, code);
                    settingControl = CameraHandler.KeyTypes.NUM_KEYS;
                    settingControlButton.color = Color.white;
                    UpdateControlsUI();
                }
            }
        }
    }
    private bool CodeIsValid(KeyCode code)
    {
        // If this code is already applied then it isn't valid
        if (SaveManager.instance.GetSave().upKey == code) return false;
        if (SaveManager.instance.GetSave().downKey == code) return false;
        if (SaveManager.instance.GetSave().leftKey == code) return false;
        if (SaveManager.instance.GetSave().rightKey == code) return false;
        if (SaveManager.instance.GetSave().rotateLeftKey == code) return false;
        if (SaveManager.instance.GetSave().rotateRightKey == code) return false;
        if (SaveManager.instance.GetSave().jumpToCloneKey == code) return false;
        if (SaveManager.instance.GetSave().speedCameraKey == code) return false;
        if (SaveManager.instance.GetSave().toggleUIKey == code) return false;
        if (SaveManager.instance.GetSave().pauseMenuKey == code) return false;
        if (SaveManager.instance.GetSave().playSpeedShortcutKey == code) return false;
        if (SaveManager.instance.GetSave().doubleSpeedShortcutKey == code) return false;
        if (SaveManager.instance.GetSave().tripleSpeedShortcutKey == code) return false;
        if (SaveManager.instance.GetSave().pauseSpeedShortcutKey == code) return false;

        // If the code is an already used mouse button then it isn't valid
        if (KeyCode.Mouse0 == code) return false;
        if (KeyCode.Mouse1 == code) return false;
        if (KeyCode.Mouse2 == code) return false;

        return true;
    }

    private void OnDisable()
    {
        DisableControlPanel();
    }
    public void DisableControlPanel()
    {
        settingControl = CameraHandler.KeyTypes.NUM_KEYS;
        if (settingControlButton) settingControlButton.color = Color.white;
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

    #region Controls
    public void SetControlKey(int keyType)
    {
        settingControl = (CameraHandler.KeyTypes)keyType;
    }
    public void SetControlButton(Image button)
    {
        settingControlButton = button;
        settingControlButton.color = Color.grey;
    }
    #endregion // Controls

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
    public void ResetControls()
    {
        SaveManager.instance.Reset(SaveManager.SettingsTypes.CONTROLS);
        UpdateControlsUI();
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
    public void UpdateControlsUI()
    {
        upKeyText.text = SaveManager.instance.GetSave().upKey.ToString();
        downKeyText.text = SaveManager.instance.GetSave().downKey.ToString();
        leftKeyText.text = SaveManager.instance.GetSave().leftKey.ToString();
        rightKeyText.text = SaveManager.instance.GetSave().rightKey.ToString();
        rotateLeftKeyText.text = SaveManager.instance.GetSave().rotateLeftKey.ToString();
        rotateRightKeyText.text = SaveManager.instance.GetSave().rotateRightKey.ToString();
        jumpToCloneKeyText.text = SaveManager.instance.GetSave().jumpToCloneKey.ToString();
        speedCameraKeyText.text = SaveManager.instance.GetSave().speedCameraKey.ToString();
        toggleUIKeyText.text = SaveManager.instance.GetSave().toggleUIKey.ToString();
        pauseMenuKeyText.text = SaveManager.instance.GetSave().pauseMenuKey.ToString();
        playSpeedKeyText.text = SaveManager.instance.GetSave().playSpeedShortcutKey.ToString();
        doubleSpeedKeyText.text = SaveManager.instance.GetSave().doubleSpeedShortcutKey.ToString();
        tripleSpeedKeyText.text = SaveManager.instance.GetSave().tripleSpeedShortcutKey.ToString();
        pauseSpeedKeyText.text = SaveManager.instance.GetSave().pauseSpeedShortcutKey.ToString();
    }
    #endregion // Reset
}
