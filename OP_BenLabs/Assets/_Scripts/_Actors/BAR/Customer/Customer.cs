using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CustomerActions), typeof(CustomerAppearance))]
public class Customer : Actor
{
    #region Properties
    
    public Mocktail WantedCocktail => _wantedCocktail;
    public float CustomerScore => _customerScore;

    #endregion
    #region SerializeField

    [Header("Customer Stats")]
    [SerializeField] private Mocktail _wantedCocktail;
    [SerializeField] private float _decreaseRate, _reactionTimer, _rotOffset;

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

        _drinkOrdersUI[(int)_wantedCocktail - 1].SetActive(true); // check Cocktail enum to understand
        //_appearance.SetEmotion(Emotion.NEUTRAL);
        //_appearance.SetupCustomerBody(_actions.IsMale);
        
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
            
            _wantedCocktail = (Mocktail)Random.Range(1, System.Enum.GetValues(typeof(Mocktail)).Length - 1);
            _drinkOrdersUI[(int)_wantedCocktail - 1].SetActive(true);
            a_logger.Log($"Customer got a {_wantedCocktail}", a_isDevMode);
            
            UI_UpdateOrderText();
        }
    }

    protected override void AssertReferences()
    {
        a_logger.AssertReference(_actions != null, this);
        a_logger.AssertReference(_appearance != null, this);
        a_logger.AssertReference(_timerSlider != null, this);
        
        a_logger.AssertCollection(_drinkOrdersUI, this);
    }
    protected override void InitComponents()
    {
        _actions = GetComponent<CustomerActions>();
        _appearance = GetComponent<CustomerAppearance>();
        _sliderTimer = GetComponentInChildren<Slider>();
    }
    protected override void InitVariables()
    {
        // only gets from the three possible drinks
        _wantedCocktail = (Mocktail)Random.Range(1, System.Enum.GetValues(typeof(Mocktail)).Length - 1);
     
        name = $"{_wantedCocktail} customer";

        _customerScore = 100f;
        _actions.IsMale = Random.value > 0.5f;

        transform.rotation = Quaternion.Euler(transform.rotation.x,
                                              transform.rotation.y + _rotOffset,
                                              transform.rotation.z);

        a_logger.Log($"{this} wants a {_wantedCocktail}", a_isDevMode);
    }

    #endregion
    #region Enumerators

    private IEnumerator CO_DecreaseRating()
    {
        IEnumerator CO_LostPatience()
        {
            a_logger.Log("Customer lost patience!", TextColor.Red, a_isDevMode);
            _actions.DoWrongReaction();
            yield return new WaitForSeconds(2f);

            Destroy(gameObject);
            BarManager.Instance.Wrong();
        }

        a_logger.Log($"{GRACE_PERIOD}s before losing patience!", a_isDevMode);
        yield return new WaitForSeconds(GRACE_PERIOD);
        
        a_logger.Log($"Initial customer Score: {_customerScore}", a_isDevMode);

        while (_customerScore > 0f)
        {
            yield return new WaitForSeconds(PATIENCE_INTERVAL);
            _customerScore--;
            UI_UpdateTimer();

            a_logger.Log($"Customer Score: {_customerScore}", a_isDevMode);
        }

        if (_customerScore < 1f)
        {
            _customerScore = 0f;
            a_logger.Log($"Customer Score: {_customerScore}", a_isDevMode);

            UI_UpdateTimer();
            StartCoroutine(CO_LostPatience());
        }
    }

    #endregion
}