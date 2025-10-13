using System.Collections;
using System.Collections.Generic;
using EzySlice;
using UnityEngine;

public class Slice : MonoBehaviour
{
    [SerializeField] private Transform _startSlicePoint;
    [SerializeField] private Transform _endSlicePoint;
    [SerializeField] private Material _crossSectionMaterial;
    [SerializeField] private LayerMask _sliceableLayer;
    [SerializeField] private VelocityEstimator _velocityEstimator;

    private float _cutForce = 2000F;

    private void FixedUpdate()
    {
        bool hasHit = Physics.Linecast(_startSlicePoint.position, _endSlicePoint.position, out RaycastHit hit, _sliceableLayer);
        if(hasHit)
        {
            GameObject _targetObject = hit.transform.gameObject;
            SliceTarget(_targetObject);
        }
    }

    private void SliceTarget(GameObject _target)
    {
        Vector3 _velocity = _velocityEstimator.GetVelocityEstimate();
        Vector3 _planeNormal = Vector3.Cross(_endSlicePoint.position -_startSlicePoint.position, _velocity);
        _planeNormal.Normalize();
        SlicedHull hull = _target.Slice(_endSlicePoint.position, _planeNormal);

        if (hull != null)
        {
            GameObject _upperHull = hull.CreateUpperHull(_target, _crossSectionMaterial);
            SetupSlicedComponent(_upperHull);

            GameObject _lowerHull = hull.CreateLowerHull(_target, _crossSectionMaterial);
            SetupSlicedComponent(_lowerHull);

            Destroy(_target);
        }
    }

    private void SetupSlicedComponent(GameObject _slicedObject)
    {
        Rigidbody rb = _slicedObject.AddComponent<Rigidbody>();
        MeshCollider collider = _slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        rb.AddExplosionForce(_cutForce, _slicedObject.transform.position, 1);
    }
}
