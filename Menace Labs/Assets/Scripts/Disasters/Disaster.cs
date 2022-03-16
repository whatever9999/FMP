using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disaster : MonoBehaviour
{
    [SerializeField] bool shakeCamera = false;

    private void Awake()
    {
        if (shakeCamera)
        {
            ManagerHandler.instance.camera.ShakeCamera();
        }
    }
}
