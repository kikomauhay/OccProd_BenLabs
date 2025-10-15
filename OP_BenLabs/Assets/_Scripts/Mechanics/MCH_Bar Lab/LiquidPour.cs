using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidPour : MonoBehaviour
{
    #region SerializedField

    [SerializeField] private ParticleSystem _splashParticle;

    #endregion

    #region Private

    private LineRenderer _lineRenderer;
    private Vector3 _targetPosition = Vector3.zero;

    #endregion

    #region Unity

    private void OnEnable()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        Shaker.EventPour += BeginPour;
        Shaker.EventStopPour += EndPour;
    }

    private void OnDisable()
    {
        Shaker.EventPour -= BeginPour;
        Shaker.EventStopPour -= EndPour;
    }

    private void Start()
    {
        MoveToPosition(0, transform.position);
        MoveToPosition(1, transform.position);
    }

    #endregion
    #region Helpers

    private void BeginPour()
    {
        StartCoroutine(CO_UpdateParticle());
        StartCoroutine(CO_BeginPour());
    }

    private void EndPour()
    {
        StartCoroutine(CO_EndPour());
    }

    private Vector3 FindEndPoint()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);

        Physics.Raycast(ray, out hit, 2.0F);
        Vector3 endPoint = hit.collider ? hit.point : ray.GetPoint(2.0F);

        return endPoint;
    }

    private void MoveToPosition(int index, Vector3 targetPosition)
    {
        _lineRenderer.SetPosition(index, targetPosition);
    }

    private void AnimateToPosition(int index, Vector3 targetPosition)
    {
        Vector3 currentPoint = _lineRenderer.GetPosition(index);
        Vector3 newPosition = Vector3.MoveTowards(currentPoint, targetPosition, Time.deltaTime * 1.75f);
        _lineRenderer.SetPosition(index, newPosition);
    }

    private bool HasReachedPosition(int index, Vector3 targetPosition)
    {
        Vector3 currentPosition = _lineRenderer.GetPosition(index);
        return currentPosition == targetPosition;
    }

    #endregion
    #region Enumerators

    private IEnumerator CO_BeginPour()
    {
        while(gameObject.activeSelf)
        {
            _targetPosition = FindEndPoint();
            MoveToPosition(0,transform.position);
            AnimateToPosition(1,_targetPosition);

            yield return null;
        }
    }

    private IEnumerator CO_EndPour()
    {
        while(!HasReachedPosition(0,_targetPosition))
        {
            AnimateToPosition(0,_targetPosition);
            AnimateToPosition(1, _targetPosition);

            yield return null;
        }
        Destroy(gameObject);
    }

    private IEnumerator CO_UpdateParticle()
    {
        while(gameObject.activeSelf)
        {
            _splashParticle.gameObject.transform.position = _targetPosition;

            bool isHitting = HasReachedPosition(1,_targetPosition);
            _splashParticle.gameObject.SetActive(isHitting);

            yield return null;
        }

    }

    #endregion
}
