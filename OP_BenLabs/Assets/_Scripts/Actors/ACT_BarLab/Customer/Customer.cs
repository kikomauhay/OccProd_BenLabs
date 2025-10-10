using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CustomerAppearance), typeof(CustomerActions))]
public class Customer : Actor
{
    #region Properties

    public Cocktail WantedCocktail => _wantedCocktail;
    public float CustomerScore => _customerScore;

    #endregion
    #region Members

    [Header("Customer Stats")]
    [SerializeField] private float _decreaseRate;
    [SerializeField] private float _reactionTimer;

    [Header("Drinks UI")]
    [SerializeField] private GameObject[] _drinkOrdersUI;   
    [SerializeField] private Transform _orderUITransform; 
    [SerializeField] private Cocktail _wantedCocktail;

    #endregion
    #region Private

    private const float PATIENCE_INTERVAL = 5f;
    private const float GRACE_PERIOD = 2f;

    private CustomerActions _actions;
    private CustomerAppearance _appearance;

    private float _customerScore;

    #endregion

    #region Unity

    protected override void Start()
    {
        Debug.Assert(_drinkOrdersUI.Length != 0, "Missing elements in _drinksLength!", gameObject);
        Debug.Assert(_orderUITransform, "Missing reference in _orderUITransform!", gameObject);

        base.Start(); // already contains both init methods

        StartCoroutine(CO_DecreaseRating());
    }
    private void OnDestroy()
    {

    }

    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        _actions = GetComponent<CustomerActions>();
        _appearance = GetComponent<CustomerAppearance>();
    }
    protected override void InitVariables()
    {
        _customerScore = 100f;
        _wantedCocktail = _isDevMode ? Cocktail.TEQUILA_SUNRISE : 
                          (Cocktail)Random.Range(0, System.Enum.GetValues(typeof(Cocktail)).Length);
    }

    #endregion
    #region Enumerators

    private IEnumerator CO_DecreaseRating()
    {
        yield return new WaitForSeconds(GRACE_PERIOD);

        while (_customerScore > 0f)
        {
            yield return new WaitForSeconds(PATIENCE_INTERVAL);
            _customerScore--;

            if (_isDevMode)
                _logger.Log($"Customer Score: {_customerScore}");
        }

        if (_customerScore < 1f)
        {
            _customerScore = 0f;

            if (_isDevMode)
                _logger.Log($"Customer Score: {_customerScore}");

            yield return StartCoroutine(CO_LostPatience());
        }
    }
    private IEnumerator CO_LostPatience()
    {
        if (_isDevMode)
            _logger.Log("Customer waited too much!", ColorType.RED);

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    #endregion
}