using UnityEngine;

public class Disaster : MonoBehaviour
{
    [SerializeField] bool shakeCamera = false;
    [SerializeField] float lifetime = 10.0f;

    private float timer;

    private void Awake()
    {
        if (shakeCamera)
        {
            ManagerHandler.instance.camera.ShakeCamera();
        }
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (timer > lifetime)
        {
            Destroy(gameObject);
        }
    }
}
