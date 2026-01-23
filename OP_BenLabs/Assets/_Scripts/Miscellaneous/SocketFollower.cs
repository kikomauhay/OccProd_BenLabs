using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketFollower : MonoBehaviour
{
    [SerializeField] private Transform _mainCamera;

    [SerializeField] private float _heightOffset = 0.35f; // Distance below the camera
    [SerializeField] private float _forwardOffset = 0.03f; // Distance in front of the chest

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = _mainCamera.position;

        Vector3 cameraForward = _mainCamera.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 targetPosition = cameraPos + (Vector3.down * _heightOffset) + (cameraForward * _forwardOffset);

        transform.position = targetPosition;

        transform.rotation = Quaternion.Euler(0, _mainCamera.eulerAngles.y, 0);
    }

    
}
