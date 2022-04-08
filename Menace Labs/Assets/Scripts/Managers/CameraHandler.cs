using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private Transform jumpToClonePos;
    [SerializeField] private Transform rotateAroundPos;

    [SerializeField] private Vector3 moveClamp;
    [SerializeField] private float floorClamp = 1.0f;

    [SerializeField] private float shiftSpeedUp = 2.0f;

    [SerializeField] private float shakeDuration = 1.0f;
    [SerializeField] private float shakeAmount = 0.7f;
    [SerializeField] private float decreaseFactor = 1.0f;
    private Vector3 startShakePos;
    private bool shakingCamera = false;

    private static string mouseXString = "Mouse X";
    private static string mouseYString = "Mouse Y";

    private float rotateX, rotateY, zoom, moveHorizontal, moveVertical;

    private bool jumpingToClone = false;
    public void JumpToClone() { jumpingToClone = true; }

    #region Controls
    public enum KeyTypes
    {
        ROTATE,
        MOVE,
        UP,
        DOWN,
        LEFT,
        RIGHT,
        ROTATE_LEFT,
        ROTATE_RIGHT,
        JUMP_TO_CLONE,
        SPEED_CAMERA,
        TOGGLE_UI,
        PAUSE_MENU,
        PLAY_SPEED,
        DOUBLE_SPEED,
        TRIPLE_SPEED,
        PAUSE_SPEED,
        NUM_KEYS,
    }

    public KeyCode GetKey(KeyTypes key)
    {
        switch (key)
        {
            case KeyTypes.ROTATE:
                return rotateKey;
            case KeyTypes.MOVE:
                return moveKey;
            case KeyTypes.UP:
                return SaveManager.instance.GetSave().upKey;
            case KeyTypes.DOWN:
                return SaveManager.instance.GetSave().downKey;
            case KeyTypes.LEFT:
                return SaveManager.instance.GetSave().leftKey;
            case KeyTypes.RIGHT:
                return SaveManager.instance.GetSave().rightKey;
            case KeyTypes.ROTATE_LEFT:
                return SaveManager.instance.GetSave().rotateLeftKey;
            case KeyTypes.ROTATE_RIGHT:
                return SaveManager.instance.GetSave().rotateRightKey;
            case KeyTypes.JUMP_TO_CLONE:
                return SaveManager.instance.GetSave().jumpToCloneKey;
            case KeyTypes.SPEED_CAMERA:
                return SaveManager.instance.GetSave().speedCameraKey;
            case KeyTypes.TOGGLE_UI:
                return SaveManager.instance.GetSave().toggleUIKey;
            case KeyTypes.PAUSE_MENU:
                return SaveManager.instance.GetSave().pauseMenuKey;
            case KeyTypes.PLAY_SPEED:
                return SaveManager.instance.GetSave().playSpeedShortcutKey;
            case KeyTypes.DOUBLE_SPEED:
                return SaveManager.instance.GetSave().doubleSpeedShortcutKey;
            case KeyTypes.TRIPLE_SPEED:
                return SaveManager.instance.GetSave().tripleSpeedShortcutKey;
            case KeyTypes.PAUSE_SPEED:
                return SaveManager.instance.GetSave().pauseSpeedShortcutKey;
        }
        Debug.LogError("Didn't find key of type: " + key);
        return KeyCode.None;
    }

    // Mouse Keys
    KeyCode rotateKey = KeyCode.Mouse2;
    KeyCode moveKey = KeyCode.Mouse1;

    #endregion // Controls


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ShakeCamera()
    {
        startShakePos = transform.position;
        shakingCamera = true;
    }

    private void CheckInput()
    {
        // Reset values
        rotateX = 0.0f;
        rotateY = 0.0f;
        zoom = 0.0f;
        moveHorizontal = 0.0f;
        moveVertical = 0.0f;

        // Get modifiable values
        float mouseRotateSpeed = SaveManager.instance.GetSave().mouseRotateSpeed;
        float keyboardRotateSpeed = SaveManager.instance.GetSave().keyboardRotateSpeed;
        float zoomSpeed = SaveManager.instance.GetSave().zoomSpeed;
        float moveSpeed = SaveManager.instance.GetSave().moveSpeed;
        float edgeScrollSize = SaveManager.instance.GetSave().edgeScrollSize;

        float shiftMultiplier = Input.GetKey(GetKey(KeyTypes.SPEED_CAMERA)) ? shiftSpeedUp : 1.0f;

        // Rotate camera on middle mouse
        if (Input.GetKey(rotateKey))
        {
            rotateX = Input.GetAxis(mouseXString) * mouseRotateSpeed;
            rotateY = Input.GetAxis(mouseYString) * mouseRotateSpeed;


        }
        // Move camera on right click
        if (Input.GetKey(moveKey))
        {
            moveHorizontal = Input.GetAxis(mouseXString) * moveSpeed;
            moveVertical = Input.GetAxis(mouseYString) * moveSpeed;
        }

        // Edge Scrolling
        if (Input.mousePosition.x > Screen.width - edgeScrollSize)
        {
            moveHorizontal += moveSpeed;
        }
        if (Input.mousePosition.x < edgeScrollSize)
        {
            moveHorizontal -= moveSpeed;
        }
        if (Input.mousePosition.y > Screen.height - edgeScrollSize)
        {
            moveVertical += moveSpeed;
        }
        if (Input.mousePosition.y < edgeScrollSize)
        {
            moveVertical -= moveSpeed;
        }

        // WSAD
        if (Input.GetKey(GetKey(KeyTypes.UP)))
        {
            moveVertical += moveSpeed * shiftMultiplier;
        }
        if (Input.GetKey(GetKey(KeyTypes.DOWN)))
        {
            moveVertical -= moveSpeed * shiftMultiplier;
        }
        if (Input.GetKey(GetKey(KeyTypes.LEFT)))
        {
            moveHorizontal -= moveSpeed * shiftMultiplier;
        }
        if (Input.GetKey(GetKey(KeyTypes.RIGHT)))
        {
            moveHorizontal += moveSpeed * shiftMultiplier;
        }

        // Middle mouse scroll zooms
        zoom += (Input.mouseScrollDelta.y * zoomSpeed * shiftMultiplier);

        // Q and E rotate the camera
        if (Input.GetKey(GetKey(KeyTypes.ROTATE_LEFT)))
        {
            rotateX -= keyboardRotateSpeed * shiftMultiplier;
        }
        if (Input.GetKey(GetKey(KeyTypes.ROTATE_RIGHT)))
        {
            rotateX += keyboardRotateSpeed * shiftMultiplier;
        }

        // Jump to the clone if spacebar is pressed (will be cancelled if other keys are pressed)
        if (Input.GetKeyDown(GetKey(KeyTypes.JUMP_TO_CLONE)))
        {
            jumpingToClone = !jumpingToClone;
        }
    }

    private void Update()
    {
        // Don't move the camera if the pause menu is open
        if (!ManagerHandler.instance.UIM.PauseMenuOpen())
        {
            CheckInput();

            // Get modifiable values
            float jumpToCloneMoveSpeed = SaveManager.instance.GetSave().jumpToCloneMoveSpeed;
            float jumpToCloneRotateSpeed = SaveManager.instance.GetSave().jumpToCloneRotateSpeed;

            bool moved = (rotateX != 0.0f) || (rotateY != 0.0f) || (zoom != 0.0f) || (moveHorizontal != 0.0f) || (moveVertical != 0.0f);
            // Stop following the clone if we get movement input
            if (moved) jumpingToClone = false;

            // For disasters such as earthquake
            if (shakingCamera)
            {
                if (shakeDuration > 0)
                {
                    transform.localPosition = startShakePos + Random.insideUnitSphere * shakeAmount;

                    shakeDuration -= Time.deltaTime * decreaseFactor;
                }
                else
                {
                    shakeDuration = 0f;
                    transform.localPosition = startShakePos;
                    shakingCamera = false;
                }
            }
            else if (jumpingToClone)
            {
                // Rotation
                float step = jumpToCloneRotateSpeed * Time.unscaledDeltaTime;
                Vector3 targetDir = ManagerHandler.instance.clone.transform.position - transform.position;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
                Quaternion targetRotation = Quaternion.LookRotation(newDir);
                if (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
                {
                    transform.rotation = targetRotation;
                }

                // Position
                if (Vector3.Distance(transform.position, jumpToClonePos.position) >= 5f)
                {
                    step = jumpToCloneMoveSpeed * Time.fixedDeltaTime;
                    Vector3 newPosition = Vector3.MoveTowards(transform.position, jumpToClonePos.position, step);
                    transform.position = newPosition;
                }
            }
            else if (moved)
            {
                // ROTATE
                if (SaveManager.instance.GetSave().pivotAroundCentre)
                {
                    transform.RotateAround(rotateAroundPos.position, Vector3.up, rotateX);

                    float rotationX = transform.localEulerAngles.x;
                    // Clamp
                    float newRotationX = (rotationX - rotateY);
                    if (rotationX <= 90.0f && newRotationX >= 0.0f)
                    {
                        newRotationX = Mathf.Clamp(newRotationX, 0.0f, 90.0f);
                    }
                    if (rotationX >= 270.0f)
                    {
                        newRotationX = Mathf.Clamp(newRotationX, 270.0f, 360.0f);
                    }
                    transform.localRotation = Quaternion.Euler(newRotationX, transform.localEulerAngles.y, transform.localEulerAngles.z);
                }
                else
                {
                    float rotationX = transform.localEulerAngles.x;
                    float newRotationY = transform.localEulerAngles.y + rotateX;

                    // Clamp
                    float newRotationX = (rotationX - rotateY);
                    if (rotationX <= 90.0f && newRotationX >= 0.0f)
                    {
                        newRotationX = Mathf.Clamp(newRotationX, 0.0f, 90.0f);
                    }
                    if (rotationX >= 270.0f)
                    {
                        newRotationX = Mathf.Clamp(newRotationX, 270.0f, 360.0f);
                    }

                    transform.localRotation = Quaternion.Euler(newRotationX, newRotationY, transform.localEulerAngles.z);
                }

                // ZOOM
                transform.position += transform.forward * Time.unscaledDeltaTime * zoom;

                // MOVE
                transform.position += transform.right * Time.unscaledDeltaTime * moveHorizontal;
                // Don't move the camera in the y axis
                Vector3 upNoY = transform.up;
                upNoY.y = 0;
                transform.position += upNoY * Time.unscaledDeltaTime * moveVertical;

                Vector3 clampedPosition = transform.position;
                if (clampedPosition.x > moveClamp.x) clampedPosition.x = moveClamp.x;
                if (clampedPosition.x < -moveClamp.x) clampedPosition.x = -moveClamp.x;
                if (clampedPosition.y > moveClamp.y) clampedPosition.y = moveClamp.y;
                if (clampedPosition.y < floorClamp) clampedPosition.y = floorClamp;
                if (clampedPosition.z > moveClamp.z) clampedPosition.z = moveClamp.z;
                if (clampedPosition.z < -moveClamp.z) clampedPosition.z = -moveClamp.z;

                transform.position = clampedPosition;
            }
        }
    }
}
