using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private CloneMeshHandler cloneMeshHandler;

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
        }
        else
        {
            loadedSave = new Save();
        }
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
}
