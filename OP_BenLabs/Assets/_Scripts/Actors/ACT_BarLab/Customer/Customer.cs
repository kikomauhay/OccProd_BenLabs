using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CustomerActions))]
public class Customer : Actor
{
    #region Properties

    public Cocktail WantedCocktail => _wantedCocktail;
    public float CustomerScore => _customerScore;

    #endregion
    #region Members

    [Header("Customer Stats")]
    [SerializeField] private Cocktail _wantedCocktail;
    [SerializeField] private float _decreaseRate;
    [SerializeField] private float _reactionTimer;

    [Header("Drinks UI")]
    [SerializeField] private GameObject[] _drinkOrdersUI;   
    [SerializeField] private Transform _orderUITransform;   

    #endregion
    #region Private

    private const float PATIENCE_INTERVAL = 2f;
    private const float GRACE_PERIOD = 2f;

    private CustomerActions _actions;
    private float _customerScore;

    #endregion

    #region Unity

    protected override void Start()
    {
        // Debug.Assert(_drinkOrdersUI.Length != 0, "Missing elements in _drinksLength!", gameObject);
        // Debug.Assert(_orderUITransform, "Missing reference in _orderUITransform!", gameObject);

        base.Start();
                
        StartCoroutine(CO_DecreaseRating());
    }

    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        _logger = BarManager.Instance.Logger;
        _actions = GetComponent<CustomerActions>();
    }
    protected override void InitVariables()
    {
        Cocktail SetRandomCocktail() // only gets from the 3 possible drinks
        {            
            int randomFromEnum = Random.Range(1, System.Enum.GetValues(typeof(Cocktail)).Length - 1);
            return (Cocktail)randomFromEnum;
        }

        name = "Customer";
        _wantedCocktail = _isDevMode ? Cocktail.TEQUILA_SUNRISE : SetRandomCocktail();
        _customerScore = 100f;

        // _drinkOrdersUI[(int)_wantedCocktail].SetActive(true);

        _logger.Log($"{this} wants a {_wantedCocktail}", _isDevMode);
    }

    #endregion
    #region Enumerators

    private IEnumerator CO_DecreaseRating()
    {
        IEnumerator CO_LostPatience()
        {
            _logger.Log("Customer lost patience!", TextColor.RED, _isDevMode);
            yield return new WaitForSeconds(2f);

            Destroy(gameObject);
            BarManager.Instance.Wrong();
        }

        _logger.Log($"{GRACE_PERIOD}s before losing patience!", _isDevMode);
        yield return new WaitForSeconds(GRACE_PERIOD);
        
        _logger.Log($"Customer Score: {_customerScore}", _isDevMode);

        while (_customerScore > 0f)
        {
            yield return new WaitForSeconds(PATIENCE_INTERVAL);
            _customerScore--;
            _logger.Log($"Customer Score: {_customerScore}", _isDevMode);
        }

        if (_customerScore < 1f)
        {
            _customerScore = 0f;
            _logger.Log($"Customer Score: {_customerScore}", _isDevMode);
            StartCoroutine(CO_LostPatience());
        }
    }

    #endregion
}