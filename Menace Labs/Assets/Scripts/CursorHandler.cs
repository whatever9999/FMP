using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorHandler : MonoBehaviour
{
    [SerializeField] private Texture2D normalCursor;
    [SerializeField] private Texture2D clickCursor;
    [SerializeField] private Texture2D rotateCursor;
    [SerializeField] private Texture2D moveCursor;

    CameraHandler camera;

    private void Start()
    {
        camera = GameObject.FindObjectOfType<CameraHandler>();
    }

    private void Update()
    {
        if (Input.GetKey(camera.GetKey(CameraHandler.KeyTypes.ROTATE)))
        {
            Cursor.SetCursor(rotateCursor, Vector2.zero, CursorMode.Auto);
        }
        else if (Input.GetKey(camera.GetKey(CameraHandler.KeyTypes.MOVE)))
        {
            Cursor.SetCursor(moveCursor, Vector2.zero, CursorMode.Auto);
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            Cursor.SetCursor(clickCursor, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
        }

        
    }
}
