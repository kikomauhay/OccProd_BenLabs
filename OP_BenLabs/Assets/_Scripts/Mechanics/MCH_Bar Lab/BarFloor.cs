using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarFloor : Floor
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<Shaker>() != null)
        {
            Shaker s = collision.gameObject.GetComponent<Shaker>();
            s.Washed();
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("ShakerCap"))
        {
            Destroy(collision.gameObject);
            BarManager.Instance.CapDespawned();
            BarManager.Instance.SpawnCap();
        }
        else if(collision.gameObject.GetComponent<Bottle>() != null)
        {
            Bottle b = collision.gameObject.GetComponent<Bottle>();
            b.ResetBottle();
        }
    }
}
