[System.Serializable]
public class Save
{
    public int chosenClone = 0;
    public bool enableObjectTooltips = true;

    public float masterVolume = 0;
    public float SFXVolume = 0;
    public float ambienceVolume = 0;
    public float musicVolume = 0;

    public float mouseRotateSpeed = 5.0f;
    public float keyboardRotateSpeed = 0.5f;
    public float zoomSpeed = 8.0f;
    public float moveSpeed = 5.0f;
    public float edgeScrollSize = 10.0f;
    public float jumpToCloneMoveSpeed = 10.0f;
    public float jumpToCloneRotateSpeed = 2.0f;

    public void ResetClone()
    {
        chosenClone = 0;
    }
    public void ResetUI()
    {
        enableObjectTooltips = true;
    }
    public void ResetSound()
    {
        masterVolume = 0;
        SFXVolume = 0;
        ambienceVolume = 0;
        musicVolume = 0;
    }
    public void ResetCamera()
    {
        mouseRotateSpeed = 5.0f;
        keyboardRotateSpeed = 0.5f;
        zoomSpeed = 8.0f;
        moveSpeed = 5.0f;
        edgeScrollSize = 10.0f;
        jumpToCloneMoveSpeed = 10.0f;
        jumpToCloneRotateSpeed = 2.0f;
    }
}
