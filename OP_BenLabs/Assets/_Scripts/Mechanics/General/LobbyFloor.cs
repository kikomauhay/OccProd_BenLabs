using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyFloor : Floor
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<ID>())
        {
            collision.gameObject.GetComponent<ID>().ResetID();
        }
    }
}
