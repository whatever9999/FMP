using UnityEngine;

[System.Serializable]
public class Save
{
    // Clone
    public int chosenClone = 0;

    // UI
    public bool enableObjectTooltips = true;

    // Sound
    public float masterVolume = 0;
    public float SFXVolume = 0;
    public float ambienceVolume = 0;
    public float musicVolume = 0;

    // Camera
    public float mouseRotateSpeed = 5.0f;
    public float keyboardRotateSpeed = 0.5f;
    public float zoomSpeed = 8.0f;
    public float moveSpeed = 5.0f;
    public float edgeScrollSize = 10.0f;
    public float jumpToCloneMoveSpeed = 10.0f;
    public float jumpToCloneRotateSpeed = 2.0f;

    // Controls
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode rotateLeftKey = KeyCode.Q;
    public KeyCode rotateRightKey = KeyCode.E;
    public KeyCode jumpToCloneKey = KeyCode.Space;
    public KeyCode speedCameraKey = KeyCode.LeftShift;
    public KeyCode toggleUIKey = KeyCode.Tab;
    public KeyCode pauseMenuKey = KeyCode.Escape;
    public KeyCode playSpeedShortcutKey = KeyCode.Alpha1;
    public KeyCode doubleSpeedShortcutKey = KeyCode.Alpha2;
    public KeyCode tripleSpeedShortcutKey = KeyCode.Alpha3;
    public KeyCode pauseSpeedShortcutKey = KeyCode.P;

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
    public void ResetControls()
    {
        upKey = KeyCode.W;
        downKey = KeyCode.S;
        leftKey = KeyCode.A;
        rightKey = KeyCode.D;
        rotateLeftKey = KeyCode.Q;
        rotateRightKey = KeyCode.E;
        jumpToCloneKey = KeyCode.Space;
        speedCameraKey = KeyCode.LeftShift;
        toggleUIKey = KeyCode.Tab;
        pauseMenuKey = KeyCode.Escape;
        playSpeedShortcutKey = KeyCode.Alpha1;
        doubleSpeedShortcutKey = KeyCode.Alpha2;
        tripleSpeedShortcutKey = KeyCode.Alpha3;
        pauseSpeedShortcutKey = KeyCode.P;
    }
}
