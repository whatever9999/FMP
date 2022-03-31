using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private CloneMeshHandler cloneMeshHandler;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        cloneMeshHandler = GameObject.FindObjectOfType<CloneMeshHandler>();

        LoadGame();
    }

    public void SaveGame()
    {
        // CREATE DATA
        Save save = new Save();
        save.chosenClone = cloneMeshHandler.GetChosenClone();

        // SAVE FILE
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();
    }
    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            cloneMeshHandler.SetChosenClone(save.chosenClone);
        }
    }
}
