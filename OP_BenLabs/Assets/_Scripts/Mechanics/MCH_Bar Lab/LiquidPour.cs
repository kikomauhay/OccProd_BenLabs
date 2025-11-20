using System.Collections;
using UnityEngine;

public class LiquidPour : MonoBehaviour
{
    #region Properties

    public static event System.Action<Cocktail> OnGlassHit;
    public static event System.Action<Ingredient> OnShakerHit;
    public static event System.Action ShakerEmptied;

    #endregion
    #region SerializeField

    [SerializeField] private ParticleSystem _splashParticle;
    [SerializeField] private LayerMask _layerMask;

    #endregion
    #region Private

    private LineRenderer _lineRenderer;
    private Vector3 _targetPosition;
    private Bottle _bottle;

    #endregion

    #region Unity

    private void OnEnable()
    {
        _bottle = this.GetComponentInParent<Bottle>();

        Shaker.OnBeginPourCocktail += BeginPourCocktail;
        Shaker.OnStopPour += EndPour;

        _bottle.OnBeginPourIngredient += BeginPourIngredient;
        _bottle.OnStopPour += EndPour;
    }
    private void OnDisable()
    {
        Shaker.OnBeginPourCocktail -= BeginPourCocktail;
        Shaker.OnStopPour -= EndPour;

        _bottle.OnBeginPourIngredient -= BeginPourIngredient;
        _bottle.OnStopPour -= EndPour;
    }
    private void Awake() => _lineRenderer = GetComponent<LineRenderer>();
    private void Start()
    {
        _targetPosition = Vector3.zero;

        MoveToPosition(0, transform.position);
        MoveToPosition(1, transform.position);
    }

    #endregion
    #region Helpers

    private void BeginPourCocktail(Cocktail cocktail)
    {
        _layerMask = LayerMask.GetMask("Glass");

        Vector3 FindEndPoint()
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.down);

            Physics.Raycast(ray, out hit, 2.0F);
            Vector3 endPoint = hit.collider ? hit.point : ray.GetPoint(2.0F);

            if (Physics.Raycast(hit.point, endPoint, _layerMask))
            {
                Debug.LogWarning("Glass has been hit, invoke event");
                OnGlassHit?.Invoke(cocktail);
            }

            return endPoint;
        }
        IEnumerator CO_BeginPour()
        {
            while (gameObject.activeSelf)
            {
                _targetPosition = FindEndPoint();
                MoveToPosition(0, transform.position);
                AnimateToPosition(1, _targetPosition);

                yield return null;

                ShakerEmptied?.Invoke();
            }
        }
        IEnumerator CO_UpdateParticle()
        {
            while (gameObject.activeSelf)
            {
                _splashParticle.gameObject.transform.position = _targetPosition;

                bool isHitting = HasReachedPosition(1, _targetPosition);
                _splashParticle.gameObject.SetActive(isHitting);

                yield return null;
            }
        }

        StartCoroutine(CO_UpdateParticle());
        StartCoroutine(CO_BeginPour());
    }
    private void BeginPourIngredient(Ingredient ingredient)
    {
        _layerMask = LayerMask.GetMask("Shaker");

        Vector3 FindEndPoint()
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.down);

            Physics.Raycast(ray, out hit, 2.0F);
            Vector3 endPoint = hit.collider ? hit.point : ray.GetPoint(2.0F);

            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit secondHit, 2.0f, _layerMask))
            {
                Debug.Log("Ingredient Added");
                OnShakerHit?.Invoke(ingredient);
            }

            return endPoint;
        }
        IEnumerator CO_BeginPour()
        {
            while (gameObject.activeSelf)
            {
                _targetPosition = FindEndPoint();
                MoveToPosition(0, transform.position);
                AnimateToPosition(1, _targetPosition);

                yield return null;
            }
        }
        IEnumerator CO_UpdateParticle()
        {
            while (gameObject.activeSelf)
            {
                _splashParticle.gameObject.transform.position = _targetPosition;

                bool isHitting = HasReachedPosition(1, _targetPosition);
                _splashParticle.gameObject.SetActive(isHitting);

                yield return null;
            }
        }

        StartCoroutine(CO_UpdateParticle());
        StartCoroutine(CO_BeginPour());
}
    private void EndPour()
    {
        IEnumerator CO_EndPour()
        {
            while (!HasReachedPosition(0, _targetPosition))
            {
                AnimateToPosition(0, _targetPosition);
                AnimateToPosition(1, _targetPosition);

                yield return null;
            }
            Destroy(gameObject);
        }

        StartCoroutine(CO_EndPour());
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
}
