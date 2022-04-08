using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Audio;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private AudioMixer audioMixer;

    private Save loadedSave;
    public Save GetSave() { return loadedSave; }

    private void Awake()
    {
        instance = this;

        LoadGame();
    }

    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, loadedSave);
        file.Close();
    }
    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            loadedSave = (Save)bf.Deserialize(file);
            file.Close();

            // Sound
            audioMixer.SetFloat("MasterVolume", loadedSave.masterVolume);
            audioMixer.SetFloat("SFXVolume", loadedSave.SFXVolume);
            audioMixer.SetFloat("AmbienceVolume", loadedSave.ambienceVolume);
            audioMixer.SetFloat("MusicVolume", loadedSave.musicVolume);
        }
        else
        {
            loadedSave = new Save();
        }
    }

    public enum SoundTypes
    {
        MASTER,
        SFX,
        AMBIENCE,
        MUSIC,
    }

    public void SetChosenClone(int chosenClone)
    {
        loadedSave.chosenClone = chosenClone;
        SaveGame();
    }
    public void EnableObjectTooltips(bool enable)
    {
        loadedSave.enableObjectTooltips = enable;
        SaveGame();
    }
    public void EnablePivotAroundCentre(bool enable)
    {
        loadedSave.pivotAroundCentre = enable;
        SaveGame();
    }
    public void SetSoundVolume(SoundTypes soundType, float volume)
    {
        switch (soundType)
        {
            case SoundTypes.MASTER:
                loadedSave.masterVolume = volume;
                break;
            case SoundTypes.SFX:
                loadedSave.SFXVolume = volume;
                break;
            case SoundTypes.AMBIENCE:
                loadedSave.ambienceVolume = volume;
                break;
            case SoundTypes.MUSIC:
                loadedSave.musicVolume = volume;
                break;
        }
        SaveGame();
    }

    public enum CameraValueTypes
    {
        MOUSE_ROTATE_SPEED,
        KEYBOARD_ROTATE_SPEED,
        ZOOM_SPEED,
        MOVE_SPEED,
        EDGE_SCROLL_SIZE,
        JUMP_TO_CLONE_MOVE_SPEED,
        JUMP_TO_CLONE_ROTATE_SPEED,
    }
    public void SetCameraValue(CameraValueTypes type, float value)
    {
        switch (type)
        {
            case CameraValueTypes.MOUSE_ROTATE_SPEED:
                loadedSave.mouseRotateSpeed = value;
                break;
            case CameraValueTypes.KEYBOARD_ROTATE_SPEED:
                loadedSave.keyboardRotateSpeed = value;
                break;
            case CameraValueTypes.ZOOM_SPEED:
                loadedSave.zoomSpeed = value;
                break;
            case CameraValueTypes.MOVE_SPEED:
                loadedSave.moveSpeed = value;
                break;
            case CameraValueTypes.EDGE_SCROLL_SIZE:
                loadedSave.edgeScrollSize = value;
                break;
            case CameraValueTypes.JUMP_TO_CLONE_MOVE_SPEED:
                loadedSave.jumpToCloneMoveSpeed = value;
                break;
            case CameraValueTypes.JUMP_TO_CLONE_ROTATE_SPEED:
                loadedSave.jumpToCloneRotateSpeed = value;
                break;
        }
        SaveGame();
    }

    public enum SettingsTypes
    {
        UI,
        SOUND,
        CAMERA,
        CONTROLS,
        GAMEPLAY,
    }
    public void Reset(SettingsTypes settingsType)
    {
        switch (settingsType)
        {
            case SettingsTypes.UI:
                loadedSave.ResetUI();
                break;
            case SettingsTypes.SOUND:
                loadedSave.ResetSound();
                break;
            case SettingsTypes.CAMERA:
                loadedSave.ResetCamera();
                break;
            case SettingsTypes.CONTROLS:
                loadedSave.ResetControls();
                break;
            case SettingsTypes.GAMEPLAY:
                loadedSave.ResetGameplay();
                break;
        }
        SaveGame();
    }

    public void SetControl(CameraHandler.KeyTypes keyType, KeyCode keyCode)
    {
        switch (keyType)
        {
            case CameraHandler.KeyTypes.UP:
                loadedSave.upKey = keyCode;
                break;
            case CameraHandler.KeyTypes.DOWN:
                loadedSave.downKey = keyCode;
                break;
            case CameraHandler.KeyTypes.LEFT:
                loadedSave.leftKey = keyCode;
                break;
            case CameraHandler.KeyTypes.RIGHT:
                loadedSave.rightKey = keyCode;
                break;
            case CameraHandler.KeyTypes.ROTATE_LEFT:
                loadedSave.rotateLeftKey = keyCode;
                break;
            case CameraHandler.KeyTypes.ROTATE_RIGHT:
                loadedSave.rotateRightKey = keyCode;
                break;
            case CameraHandler.KeyTypes.JUMP_TO_CLONE:
                loadedSave.jumpToCloneKey = keyCode;
                break;
            case CameraHandler.KeyTypes.SPEED_CAMERA:
                loadedSave.speedCameraKey = keyCode;
                break;
            case CameraHandler.KeyTypes.TOGGLE_UI:
                loadedSave.toggleUIKey = keyCode;
                break;
            case CameraHandler.KeyTypes.PAUSE_MENU:
                loadedSave.pauseMenuKey = keyCode;
                break;
            case CameraHandler.KeyTypes.PLAY_SPEED:
                loadedSave.playSpeedShortcutKey = keyCode;
                break;
            case CameraHandler.KeyTypes.DOUBLE_SPEED:
                loadedSave.doubleSpeedShortcutKey = keyCode;
                break;
            case CameraHandler.KeyTypes.TRIPLE_SPEED:
                loadedSave.tripleSpeedShortcutKey = keyCode;
                break;
            case CameraHandler.KeyTypes.PAUSE_SPEED:
                loadedSave.pauseSpeedShortcutKey = keyCode;
                break;
        }
        SaveGame();
    }
    public KeyCode GetControl(CameraHandler.KeyTypes keyType)
    {
        switch (keyType)
        {
            case CameraHandler.KeyTypes.UP:
                return loadedSave.upKey;
            case CameraHandler.KeyTypes.DOWN:
                return loadedSave.downKey;
            case CameraHandler.KeyTypes.LEFT:
                return loadedSave.leftKey;
            case CameraHandler.KeyTypes.RIGHT:
                return loadedSave.rightKey;
            case CameraHandler.KeyTypes.ROTATE_LEFT:
                return loadedSave.rotateLeftKey;
            case CameraHandler.KeyTypes.ROTATE_RIGHT:
                return loadedSave.rotateRightKey;
            case CameraHandler.KeyTypes.JUMP_TO_CLONE:
                return loadedSave.jumpToCloneKey;
            case CameraHandler.KeyTypes.SPEED_CAMERA:
                return loadedSave.speedCameraKey;
            case CameraHandler.KeyTypes.TOGGLE_UI:
                return loadedSave.toggleUIKey;
            case CameraHandler.KeyTypes.PAUSE_MENU:
                return loadedSave.pauseMenuKey;
            case CameraHandler.KeyTypes.PLAY_SPEED:
                return loadedSave.playSpeedShortcutKey;
            case CameraHandler.KeyTypes.DOUBLE_SPEED:
                return loadedSave.doubleSpeedShortcutKey;
            case CameraHandler.KeyTypes.TRIPLE_SPEED:
                return loadedSave.tripleSpeedShortcutKey;
            case CameraHandler.KeyTypes.PAUSE_SPEED:
                return loadedSave.pauseSpeedShortcutKey;
        }
        return KeyCode.None;
    }

    public void SetDifficulty(Director.DifficultyLevel level)
    {
        loadedSave.difficulty = level;
        SaveGame();
    }

    public enum CloneValues
    {
        ID,
        FREED_CLONES,
        KILLED_CLONES,
    }
    public void IncrementCloneValue(CloneValues value)
    {
        switch (value)
        {
            case CloneValues.ID:
                loadedSave.cloneID++;
                break;
            case CloneValues.FREED_CLONES:
                loadedSave.clonesFreed++;
                break;
            case CloneValues.KILLED_CLONES:
                loadedSave.clonesKilled++;
                break;
        }
        SaveGame();
    }
}
