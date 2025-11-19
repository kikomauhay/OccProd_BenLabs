using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CustomerActions), typeof(CustomerAppearance))]
public class Customer : Actor
{
    #region Properties
    
    public Cocktail WantedCocktail => _wantedCocktail;
    public float CustomerScore => _customerScore;

    #endregion
    #region SerializeField

    [Header("Customer Stats")]
    [SerializeField] private Cocktail _wantedCocktail;
    [SerializeField] private float _decreaseRate, _reactionTimer;

    [Header("Customer UI")]
    [SerializeField] private Slider _timerSlider;
    [SerializeField] private TextMeshProUGUI _orderTXT;
    [SerializeField] private GameObject[] _drinkOrdersUI;

    #endregion
    #region Private

    private const float PATIENCE_INTERVAL = 2f;
    private const float GRACE_PERIOD = 2f;

    private CustomerActions _actions;
    private CustomerAppearance _appearance;
    private Slider _sliderTimer;
    
    private float _customerScore;

    #endregion

    #region Unity

    protected override void Start()
    {
        base.Start();

        // check Cocktail enum to understand
        _drinkOrdersUI[(int)_wantedCocktail - 1].SetActive(true);

        UI_UpdateTimer();
        UI_UpdateOrderText();
        
        StartCoroutine(CO_DecreaseRating());
    }

    #endregion
    #region Private 

    private void UI_UpdateTimer() => _sliderTimer.value = _customerScore / 100f;
    private void UI_UpdateOrderText() => _orderTXT.text = $"{_wantedCocktail.ToString().Replace("_", " ")}";

    #endregion
    #region Helpers

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (GameObject order in _drinkOrdersUI)
                order.SetActive(false);
            
            _wantedCocktail = (Cocktail)Random.Range(1, System.Enum.GetValues(typeof(Cocktail)).Length - 1);
            _drinkOrdersUI[(int)_wantedCocktail - 1].SetActive(true);
            _logger.Log($"Customer got a {_wantedCocktail}", _isDevMode);
            
            UI_UpdateOrderText();
        }
    }

    protected override void AssertComponents()
    {
        Debug.Assert(_actions, "Missing _actions reference!", this);
        Debug.Assert(_appearance, "Missing _appearance reference!", this);
        Debug.Assert(_timerSlider, "Missing _timerSlider reference!", this);
        
        Debug.Assert(_drinkOrdersUI.Length != 0, "Missing elements in _drinksLength!", this);
    }
    protected override void InitComponents()
    {
        _actions = GetComponent<CustomerActions>();
        _sliderTimer = GetComponentInChildren<Slider>();

        if (!_isDevMode)
            _logger = BarManager.Instance.Logger;
    }
    protected override void InitVariables()
    {
        // only gets from the three possible drinks
        _wantedCocktail = (Cocktail)Random.Range(1, System.Enum.GetValues(typeof(Cocktail)).Length - 1);
     
        name = $"{_wantedCocktail} customer";

        _customerScore = 100f;
        _actions.IsMale = Random.value > 0.5f;        
        _logger.Log($"{this} wants a {_wantedCocktail}", _isDevMode);
    }

    #endregion
    #region Enumerators

    private IEnumerator CO_DecreaseRating()
    {
        IEnumerator CO_LostPatience()
        {
            _logger.Log("Customer lost patience!", TextColor.RED, _isDevMode);
            _actions.WrongReaction();
            yield return new WaitForSeconds(2f);

            Destroy(gameObject);
            BarManager.Instance.Wrong();
        }

        _logger.Log($"{GRACE_PERIOD}s before losing patience!", _isDevMode);
        yield return new WaitForSeconds(GRACE_PERIOD);
        
        _logger.Log($"Initial customer Score: {_customerScore}", _isDevMode);

        while (_customerScore > 0f)
        {
            yield return new WaitForSeconds(PATIENCE_INTERVAL);
            _customerScore--;
            UI_UpdateTimer();

            _logger.Log($"Customer Score: {_customerScore}", _isDevMode);
        }

        if (_customerScore < 1f)
        {
            _customerScore = 0f;
            _logger.Log($"Customer Score: {_customerScore}", _isDevMode);

            UI_UpdateTimer();
            StartCoroutine(CO_LostPatience());
        }
    }

    #endregion
}