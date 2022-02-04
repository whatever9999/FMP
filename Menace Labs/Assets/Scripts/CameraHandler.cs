using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private float lookSpeed = 10.0f;
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float zoomSpeed = 8.0f;
    [SerializeField] private float edgeScrollSize = 10.0f;
    [SerializeField] private float edgeScrollSpeed = 1.0f;
    [SerializeField] private Vector3 moveBounds;
    [SerializeField] private float floorClamp = 1.0f;
    [SerializeField] private float rightClickMoveSpeed = 1.0f;
    [SerializeField] private float jumpToCloneSpeed = 10.0f;
    [SerializeField] private float jumpToCloneRotateSpeed = 2.0f;
    [SerializeField] private float jumpToCloneYLimit = 3.0f;

    private static string mouseXString = "Mouse X";
    private static string mouseYString = "Mouse Y";
    private static string verticalString = "Vertical";
    private static string horizontalString = "Horizontal";

    private float rotateX, rotateY, moveVertical, moveHorizontal, moveNoY;

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
        moveVertical = 0.0f;
        moveHorizontal = 0.0f;
        moveNoY = 0.0f;

        // Rotate camera on middle mouse
        if (Input.GetMouseButton(2))
        {
            rotateX = Input.GetAxis(mouseXString) * lookSpeed;
            rotateY = Input.GetAxis(mouseYString) * lookSpeed;
        }
        // Move camera on right click
        if (Input.GetMouseButton(1))
        {
            moveHorizontal = Input.GetAxis(mouseXString) * rightClickMoveSpeed;
            moveNoY = Input.GetAxis(mouseYString) * rightClickMoveSpeed;
        }

        // Edge Scrolling
        if (Input.mousePosition.x > Screen.width - edgeScrollSize)
        {
            moveHorizontal += edgeScrollSpeed;
        }
        if (Input.mousePosition.x < edgeScrollSize)
        {
            moveHorizontal -= edgeScrollSpeed;
        }
        if (Input.mousePosition.y > Screen.height - edgeScrollSize)
        {
            moveNoY += edgeScrollSpeed;
        }
        if (Input.mousePosition.y < edgeScrollSize)
        {
            moveNoY -= edgeScrollSpeed;
        }

        moveNoY += Input.GetAxis(verticalString);
        // Middle mouse scroll moves vertically
        moveVertical += (Input.mouseScrollDelta.y * zoomSpeed);
        moveHorizontal += Input.GetAxis(horizontalString);

        // Jump to the clone if spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpingToClone = true;
        }
    }

    private void Update()
    {
        CheckInput();

        bool moved = (rotateX != 0.0f) || (rotateY != 0.0f) || (moveVertical != 0.0f) || (moveHorizontal != 0.0f) || (moveNoY != 0.0f);
        if (jumpingToClone)
        {
            jumpingToClone = false;

            // Rotation
            float step = jumpToCloneRotateSpeed * Time.deltaTime;
            Vector3 targetDir = ManagerHandler.instance.clone.transform.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
            Quaternion targetRotation = Quaternion.LookRotation(newDir);
            if (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
            {
                jumpingToClone = true;
                transform.rotation = targetRotation;
            }

            // Position
            if (Vector3.Distance(transform.position, ManagerHandler.instance.clone.transform.position) >= 5f)
            {
                jumpingToClone = true;

                step = jumpToCloneSpeed * Time.deltaTime;
                // Ensure the camera doesn't move too far in the y axis
                Vector3 newPosition = Vector3.MoveTowards(transform.position, ManagerHandler.instance.clone.transform.position, step);
                if (newPosition.y < jumpToCloneYLimit) newPosition.y = jumpToCloneYLimit;
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

            // MOVE
            float finalMoveSpeed = moveSpeed * Time.deltaTime;
            transform.position += transform.forward * finalMoveSpeed * moveVertical;
            transform.position += transform.right * finalMoveSpeed * moveHorizontal;
            // Ensure that edge scroll and WS don't move the camera in the y axis
            Vector3 edgeScroll = transform.up;
            edgeScroll.y = 0;
            transform.position += edgeScroll * finalMoveSpeed * moveNoY;

            Vector3 clampedPosition = transform.position;
            if (clampedPosition.x > moveBounds.x) clampedPosition.x = moveBounds.x;
            if (clampedPosition.x < -moveBounds.x) clampedPosition.x = -moveBounds.x;
            if (clampedPosition.y > moveBounds.y) clampedPosition.y = moveBounds.y;
            if (clampedPosition.y < floorClamp) clampedPosition.y = floorClamp;
            if (clampedPosition.z > moveBounds.z) clampedPosition.z = moveBounds.z;
            if (clampedPosition.z < -moveBounds.z) clampedPosition.z = -moveBounds.z;

            transform.position = clampedPosition;
        }
    }
}
