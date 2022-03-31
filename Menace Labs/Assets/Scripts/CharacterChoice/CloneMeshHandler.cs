using UnityEngine;

public class CloneMeshHandler : MonoBehaviour
{
    [SerializeField] private Mesh[] meshes;
    private int currentMesh = 0;
    private SkinnedMeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    public int GetChosenClone() 
    { 
        return currentMesh;
    }
    public void SetChosenClone(int meshID)
    {
        currentMesh = meshID;
        meshRenderer.sharedMesh = meshes[currentMesh];
    }

    public void ChangeCharacter()
    {
        if (currentMesh == meshes.Length - 1) currentMesh = -1;
        meshRenderer.sharedMesh = meshes[++currentMesh];

        SaveManager.instance.SaveGame();
    }
}
