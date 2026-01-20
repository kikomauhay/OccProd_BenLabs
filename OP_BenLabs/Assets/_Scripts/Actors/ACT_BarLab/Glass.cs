using UnityEngine;

/// <summary> - COCKTAIL COMBINATIONS -
/// 
/// TEQUILA SUNRISE
///     - tequila
///     - orange juice
///     - lime juice
/// 
/// VODKA CIRTUS
///     - vodka
///     - orange juice
///     - lime juice
///     - coconut water
/// 
/// COCONUT MARGARITA
///     - tequila
///     - lime juice
///     - coconut water
///     
/// </summary>  

/// <summary> - COCKTAIL METHOD - 
/// 
/// METHOD:
///     1. Add all ingredients into a shaker with ice
///     2. Shake for 10-15 secs
///     3. Pour into a glass with ice
/// 
/// </summary>  


public class Glass : Equipment
{
    #region Properties

    public bool HasDrink => _hasDrink;
    public Cocktail Cocktail => _cocktail;

    #endregion
    #region SerializeField

    [Header("For Testing")]
    [SerializeField] private Renderer _renderer;

    [Header("Drinks"), Tooltip("0 = Tequila, 1 = Vodka, 2 = Coconut")]
    [SerializeField] private GameObject[] _drinks;
    [SerializeField] private GameObject _checkPanel;
    [SerializeField] private Sound[] _iceRefillSFXs;

    [Header("Drink Stats")]
    [SerializeField] private Cocktail _cocktail;
    [SerializeField] private bool _hasDrink;

    [Header("SFX")]
    [SerializeField] private Sound _poofSFX;

    #endregion

    #region Unity
        

    protected override void OnEnable()
    {
        base.OnEnable();
        LiquidPour.OnGlassHit += EnableDrink;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        LiquidPour.OnGlassHit -= EnableDrink;
    }
    protected override void Start()
    {
        base.Start();
        _soundEmitter.PlaySound(_iceRefillSFXs[Random.Range(0, _iceRefillSFXs.Length)]);
    }

    #endregion
    #region Public 

    public void HitFloor()
    {
        ResetDrink();
        ResetPosition();

        _logger.Log("Hit the floor!", gameObject, _isDevMode);
    }
    public void Served()
    {
        ResetDrink();
        _logger.Log("Drink has been served!", gameObject, _isDevMode);
    }
    public void Washed()
    {
        ResetDrink();
        _logger.Log($"{this} has been washed!", TextColor.GREEN, _isDevMode);
    }
    public void EnableDrink(Cocktail cocktail)
    {
        if (_hasDrink)
        {
            _logger.Log($"{this} alreaady has an active drink!", _isDevMode);
            return;
        }

        _hasDrink = true;
        _cocktail = cocktail;
        _checkPanel.SetActive(true);

        switch (_cocktail)
        {
            case Cocktail.TEQUILA_SUNRISE:
                _drinks[0].SetActive(true);
                break;
            case Cocktail.VODKA_CITRUS:
                _drinks[1].SetActive(true);
                break;
            case Cocktail.COCONUT_MARGARITA:
                _drinks[2].SetActive(true);
                break;
        }
        
        _soundEmitter.PlaySound(_poofSFX);
        GameManager.Instance.Poof(transform);

        _logger.Log($"{this} has a {_cocktail} active!", TextColor.YELLOW, _isDevMode);
    }

    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        base.InitComponents();
        Debug.Assert(_iceRefillSFXs.Length == 3, "Missing _iceRefillSFX elements!", this);
    }
    protected override void InitVariables()
    {
        base.InitVariables();

        name = "Cocktail Glass";
        _hasDrink = false;
        _cocktail = Cocktail.EMPTY;

        _checkPanel.SetActive(false);
    }

    protected override void Test()
    {
        // if (Input.GetKeyDown(KeyCode.Alpha4)) EnableDrink(0);
        // if (Input.GetKeyDown(KeyCode.Alpha5)) EnableDrink(1);
        // if (Input.GetKeyDown(KeyCode.Alpha6)) EnableDrink(2);

        if (Input.GetKeyDown(KeyCode.Delete)) ResetDrink();
    }

    private void ResetDrink()
    {
        _hasDrink = false;
        _cocktail = Cocktail.EMPTY;
        _checkPanel.SetActive(false);

        foreach (GameObject drink in _drinks)
            drink.SetActive(false);

        _logger.Log($"{name} has no more drink!", TextColor.YELLOW, _isDevMode);
    }

    #endregion
}

public enum Cocktail
{
    WRONG = -1,
    EMPTY = 0,
    TEQUILA_SUNRISE = 1,
    VODKA_CITRUS = 2,
    COCONUT_MARGARITA = 3
}