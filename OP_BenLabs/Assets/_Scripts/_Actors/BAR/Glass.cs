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
    #region Inspector

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
        e_sndEmitter.PlaySound(_iceRefillSFXs[Random.Range(0, _iceRefillSFXs.Length)]);
    }

    #endregion
    #region Public 

    public void HitFloor()
    {
        ResetDrink();
        ResetPosition();

        a_logger.Log($"{this} has hit the floor!", TextColor.Lime, a_isDevMode);
    }
    public void Served()
    {
        ResetDrink();
        a_logger.Log($"{this} has been served!", TextColor.Lime, a_isDevMode);
    }
    public void Washed()
    {
        ResetDrink();
        a_logger.Log($"{this} has been washed!", TextColor.Lime, a_isDevMode);
    }
    public void EnableDrink(Cocktail cocktail)
    {
        if (_hasDrink)
        {
            a_logger.Log($"{this} alreaady has an active drink!", a_isDevMode);
            return;
        }

        _hasDrink = true;
        _cocktail = cocktail;
        _checkPanel.SetActive(true);

        switch (_cocktail)
        {
            case Cocktail.Tequila_Sunrise:
                _drinks[0].SetActive(true);
                break;

            case Cocktail.Vodka_Citrus:
                _drinks[1].SetActive(true);
                break;

            case Cocktail.Coconut_Mergarita:
                _drinks[2].SetActive(true);
                break;

            default: break;
        }
        
        e_sndEmitter.PlaySound(_poofSFX);
        GameManager.Instance.Poof(transform);

        a_logger.Log($"{this} has a {_cocktail} active!", TextColor.Yellow, a_isDevMode);
    }

    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        base.InitComponents();
        a_logger.AssertReference(_iceRefillSFXs.Length == 3, this);
    }
    protected override void InitVariables()
    {
        base.InitVariables();

        _hasDrink = false;
        _cocktail = Cocktail.Empty;

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
        _cocktail = Cocktail.Empty;
        _checkPanel.SetActive(false);

        foreach (GameObject drink in _drinks)
            drink.SetActive(false);

        a_logger.Log($"{name} has no more drink!", TextColor.Yellow, a_isDevMode);
    }

    #endregion
}

public enum Cocktail
{
    Wrong = -1,
    Empty = 0,
    Tequila_Sunrise = 1,
    Vodka_Citrus = 2,
    Coconut_Mergarita = 3
}