using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CafeteriaChairs : MonoBehaviour
{
    private void Start() => 
        transform.localRotation = Quaternion.Euler(0f, Random.Range(-5f, 5f), 0f);
}
