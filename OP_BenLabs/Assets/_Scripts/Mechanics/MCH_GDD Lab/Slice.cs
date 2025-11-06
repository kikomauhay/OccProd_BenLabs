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
        if (hasHit)
        {
            GameObject targetObject = hit.transform.gameObject;
            SliceTarget(targetObject);
        }
    }

    private void SliceTarget(GameObject target)
    {
        Vector3 velocity = _velocityEstimator.GetVelocityEstimate();
        Vector3 planeNormal = Vector3.Cross(_endSlicePoint.position -_startSlicePoint.position, velocity);
        planeNormal.Normalize();
        SlicedHull hull = target.Slice(_endSlicePoint.position, planeNormal);

        if (hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(target, _crossSectionMaterial);
            SetupSlicedComponent(upperHull);

            GameObject lowerHull = hull.CreateLowerHull(target, _crossSectionMaterial);
            SetupSlicedComponent(lowerHull);

            Destroy(target);
        }
    }

    private void SetupSlicedComponent(GameObject slicedObject)
    {
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        rb.AddExplosionForce(_cutForce, slicedObject.transform.position, 1);
    }
}
