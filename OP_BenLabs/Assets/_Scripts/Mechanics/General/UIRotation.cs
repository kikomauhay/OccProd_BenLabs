using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRotation : MonoBehaviour
{
    private void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward, Camera.main.transform.up);
    }
}
