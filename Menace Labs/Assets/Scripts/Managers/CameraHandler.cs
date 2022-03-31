using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private Transform jumpToClonePos;
    [SerializeField] private Transform rotateAroundPos;

    [SerializeField] private float lookSpeed = 10.0f;
    [SerializeField] private float zoomSpeed = 8.0f;
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float rotateSpeed = 0.5f;

    [SerializeField] private float edgeScrollSize = 10.0f;
    
    [SerializeField] private float jumpToCloneSpeed = 10.0f;
    [SerializeField] private float jumpToCloneRotateSpeed = 2.0f;

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

    private bool rotateAround = false;


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

        float shiftMultiplier = Input.GetKey(KeyCode.LeftShift) ? shiftSpeedUp : 1.0f;

        // Rotate camera on middle mouse
        if (Input.GetMouseButton(2))
        {
            rotateX = Input.GetAxis(mouseXString) * lookSpeed;
            rotateY = Input.GetAxis(mouseYString) * lookSpeed;
        }
        // Don't move cursor until middle click is released
        if (Input.GetMouseButtonDown(2))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            Cursor.lockState = CursorLockMode.Confined;
        }

        // Move camera on right click
        if (Input.GetMouseButton(1))
        {
            moveHorizontal = Input.GetAxis(mouseXString) * moveSpeed;
            moveVertical = Input.GetAxis(mouseYString) * moveSpeed;
        }
        // Don't move cursor until right click is released
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.Confined;
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
        if (Input.GetKey(KeyCode.W))
        {
            moveVertical += moveSpeed * shiftMultiplier;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveVertical -= moveSpeed * shiftMultiplier;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveHorizontal -= moveSpeed * shiftMultiplier;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveHorizontal += moveSpeed * shiftMultiplier;
        }

        // Middle mouse scroll zooms
        zoom += (Input.mouseScrollDelta.y * zoomSpeed * shiftMultiplier);

        // Q and E rotate the camera
        if (Input.GetKey(KeyCode.Q))
        {
            rotateX -= rotateSpeed * shiftMultiplier;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotateX += rotateSpeed * shiftMultiplier;
        }

        // Jump to the clone if spacebar is pressed (will be cancelled if other keys are pressed)
        if (Input.GetKeyDown(KeyCode.Space))
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
                    step = jumpToCloneSpeed * Time.fixedDeltaTime;
                    Vector3 newPosition = Vector3.MoveTowards(transform.position, jumpToClonePos.position, step);
                    transform.position = newPosition;
                }
            }
            else if (moved)
            {
                // ROTATE
                if (rotateAround)
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
