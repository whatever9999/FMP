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
}
