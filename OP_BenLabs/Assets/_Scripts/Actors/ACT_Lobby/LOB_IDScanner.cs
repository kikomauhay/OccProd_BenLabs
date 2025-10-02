using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOB_IDScanner : Actor
{

    [SerializeField] private Renderer _renderer;

    protected override void OnEnable()
    {
        base.OnEnable();
        //add ID grab event
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        //remove ID grab event
    }

    protected override void Start()
    {
        base.Start();
        _renderer = GetComponent<Renderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.GetComponent<Equipment>()) return;

        if(collision.gameObject.GetComponent<ID>())
        {
            _renderer.material.color = Color.green;
        }
        else
        {
            _renderer.material.color = Color.red;
        }
    }

}
