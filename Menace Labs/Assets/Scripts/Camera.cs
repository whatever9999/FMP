using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private float lookSpeed = 10.0f;
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float zoomSpeed = 8.0f;
    [SerializeField] private float edgeScrollSize = 10.0f;
    [SerializeField] private float edgeScrollSpeed = 1.0f;
    [SerializeField] private Vector3 moveBounds;
    [SerializeField] private float floorClamp = 1.0f;
    [SerializeField] private float rightClickMoveSpeed = 8.0f;

    private static string mouseXString = "Mouse X";
    private static string mouseYString = "Mouse Y";
    private static string verticalString = "Vertical";
    private static string horizontalString = "Horizontal";

    private float rotateX, rotateY, moveVertical, moveHorizontal, moveY;

    private void CheckInput()
    {
        // Reset values
        rotateX = 0.0f;
        rotateY = 0.0f;
        moveVertical = 0.0f;
        moveHorizontal = 0.0f;
        moveY = 0.0f;

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
            moveY = Input.GetAxis(mouseYString) * rightClickMoveSpeed;
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
            moveY += edgeScrollSpeed;
        }
        if (Input.mousePosition.y < edgeScrollSize)
        {
            moveY -= edgeScrollSpeed;
        }

        // Middle mouse scroll also moves vertically
        moveVertical += Input.GetAxis(verticalString) + (Input.mouseScrollDelta.y * zoomSpeed);
        moveHorizontal += Input.GetAxis(horizontalString);
    }

    private void Update()
    {
        CheckInput();

        bool moved = (rotateX != 0.0f) || (rotateY != 0.0f) || (moveVertical != 0.0f) || (moveHorizontal != 0.0f) || (moveY != 0.0f);
        if (moved)
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
            transform.position += transform.up * finalMoveSpeed * moveY;

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
