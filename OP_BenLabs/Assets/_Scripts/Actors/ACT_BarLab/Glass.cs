using System;
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

    public event System.Action<float> CocktailServed;

    public bool HasDrink => _hasDrink;
    public float Score => _score;
    public Cocktail Cocktail => _cocktail;

    #endregion
    #region SerializeField

    [Header("For Testing")]
    [SerializeField] private Renderer _renderer;

    [Header("Drinks"), Tooltip("0 = Tequila, 1 = Vodka, 2 = Coconut")]
    [SerializeField] private GameObject[] _drinks;

    [Header("Drink Stats")]
    [SerializeField] private bool _hasDrink;
    [SerializeField] private float _score;
    [SerializeField] private Cocktail _cocktail;
        
    #endregion
    #region Private

    #endregion

    #region Unity
        
    protected override void Start()
    {
        // Debug.Assert(_drinks.Length == DRINK_COUNT, "Missing elements in _drinks!", gameObject);
        base.Start();
    }

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
        // give the score to GameMgr + STARTING_SCORE before resetting the drink

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
        _drinks[(int)cocktail].SetActive(true);

        _logger.Log($"{this} has a {_cocktail} active!", TextColor.YELLOW, _isDevMode);
    }
        
    #endregion
    #region Helpers
    
    protected override void InitVariables()
    {
        base.InitVariables();

        name = "Cocktail Glass";
        _hasDrink = false;
        _score = 0f;
        _cocktail = Cocktail.EMPTY;
    }

    protected override void Test()
    {
/*        if (Input.GetKeyDown(KeyCode.Alpha4)) EnableDrink(0);
        if (Input.GetKeyDown(KeyCode.Alpha5)) EnableDrink(1);
        if (Input.GetKeyDown(KeyCode.Alpha6)) EnableDrink(2);

        if (Input.GetKeyDown(KeyCode.Delete)) ResetDrink();*/
    }

    private void ResetDrink()
    {
        _hasDrink = false;
        _score = 0f;
        _cocktail = Cocktail.EMPTY;

        foreach (GameObject drink in _drinks)
            drink.SetActive(false);

        _logger.Log($"{this} has no more drink!", TextColor.YELLOW, _isDevMode);
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