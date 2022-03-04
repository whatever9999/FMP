using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private Transform jumpToClonePos;

    [SerializeField] private float lookSpeed = 10.0f;
    [SerializeField] private float zoomSpeed = 8.0f;
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float rotateSpeed = 0.5f;

    [SerializeField] private float edgeScrollSize = 10.0f;
    
    [SerializeField] private float jumpToCloneSpeed = 10.0f;
    [SerializeField] private float jumpToCloneRotateSpeed = 2.0f;

    [SerializeField] private Vector3 moveClamp;
    [SerializeField] private float floorClamp = 1.0f;

    private static string mouseXString = "Mouse X";
    private static string mouseYString = "Mouse Y";

    private float rotateX, rotateY, zoom, moveHorizontal, moveVertical;

    private bool jumpingToClone = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void CheckInput()
    {
        // Reset values
        rotateX = 0.0f;
        rotateY = 0.0f;
        zoom = 0.0f;
        moveHorizontal = 0.0f;
        moveVertical = 0.0f;

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
            moveVertical += moveSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveVertical -= moveSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveHorizontal -= moveSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveHorizontal += moveSpeed;
        }

        // Middle mouse scroll zooms
        zoom += (Input.mouseScrollDelta.y * zoomSpeed);

        // Q and E rotate the camera
        if (Input.GetKey(KeyCode.Q))
        {
            rotateX -= rotateSpeed;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotateX += rotateSpeed;
        }

        // Jump to the clone if spacebar is pressed
        if (Input.GetKey(KeyCode.Space))
        {
            jumpingToClone = true;
        }
    }

    private void Update()
    {
        // Don't move the camera if the pause menu is open
        if (!ManagerHandler.instance.UIM.PauseMenuOpen())
        {
            CheckInput();

            bool moved = (rotateX != 0.0f) || (rotateY != 0.0f) || (zoom != 0.0f) || (moveHorizontal != 0.0f) || (moveVertical != 0.0f);
            if (jumpingToClone)
            {
                jumpingToClone = false;

                // Rotation
                float step = jumpToCloneRotateSpeed * Time.unscaledDeltaTime;
                Vector3 targetDir = ManagerHandler.instance.clone.transform.position - transform.position;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
                Quaternion targetRotation = Quaternion.LookRotation(newDir);
                if (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
                {
                    jumpingToClone = true;
                    transform.rotation = targetRotation;
                }

                // Position
                if (Vector3.Distance(transform.position, jumpToClonePos.position) >= 5f)
                {
                    jumpingToClone = true;

                    step = jumpToCloneSpeed * Time.fixedDeltaTime;
                    Vector3 newPosition = Vector3.Lerp(transform.position, jumpToClonePos.position, step);
                    transform.position = newPosition;
                }
            }
            else if (moved)
            {
                // ROTATE
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
