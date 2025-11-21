using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Shaker : Equipment, IPourable
{
    #region Properties

    public IReadOnlyList<Ingredient> MixedDrink => _mixedDrink;
    public Cocktail Cocktail => _cocktail;
    public static event System.Action ShakerLocked;
    public static event System.Action ShakerUnlocked;
    public static event System.Action<Cocktail> OnBeginPourCocktail;
    public static event System.Action OnStopPour;

    #endregion
    #region SerializeField
    
    [Header("Ingredient Mixing")]
    [SerializeField] private List<Ingredient> _mixedDrink;
    [SerializeField] private Cocktail _cocktail;
    
    [Header("Pouring Logic")]
    [SerializeField] private GameObject _stream;
    [SerializeField] private Transform _shakerTip;
    [SerializeField] private float _pourThreshold;

    [Header("Toggle Objects")]
    [SerializeField] private GameObject _shakerCap;

    #endregion
    #region Private 

    private readonly Dictionary<Cocktail, Ingredient[]> _recipes = new() // dictionary syntax = <key, value>
    {
        { Cocktail.TEQUILA_SUNRISE, new[] { Ingredient.TEQUILA,
                                            Ingredient.ORANGE_JUICE,
                                            Ingredient.LIME_JUICE } },

        { Cocktail.VODKA_CITRUS, new[] { Ingredient.VODKA,
                                         Ingredient.ORANGE_JUICE,
                                         Ingredient.LIME_JUICE,
                                         Ingredient.COCONUT_WATER } },

        { Cocktail.COCONUT_MARGARITA, new[] { Ingredient.TEQUILA,
                                              Ingredient.LIME_JUICE,
                                              Ingredient.COCONUT_WATER } },
    };
    private bool _isPouring, _isLocked, _isShaking, _isGrabbed;

    #endregion

    #region Unity

    protected override void OnEnable()
    {
        base.OnEnable();
        LiquidPour.OnShakerHit += AddIngredient;
        LiquidPour.ShakerEmptied += ResetShaker;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        LiquidPour.OnShakerHit -= AddIngredient;
        LiquidPour.ShakerEmptied -= ResetShaker;
    }
    private void FixedUpdate()
    {
        if (!_isLocked) return;

        if(_rb.velocity.magnitude > 0.5F)
        {
            MixingCocktail();
        }

        INT_CheckPourAngle();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("ShakerCap"))
        {
            _isLocked = true;
            _shakerCap.gameObject.SetActive(true);
            ShakerLocked?.Invoke();
            Destroy(other.gameObject);
        }
    }

    #endregion
    #region Public

    public void INT_CheckPourAngle()
    {
        float angle = Vector3.Angle(_shakerTip.up, Vector3.up);

        if (_cocktail == Cocktail.EMPTY && !_isGrabbed) return;

        if (angle > _pourThreshold)
        {
            if (_isPouring) return;

            INT_Pour();
            _isPouring = true;
        }
        else
        {
            _isPouring = false;
            OnStopPour?.Invoke();
        }
    }
    public void INT_Pour()
    {
        if (_cocktail == Cocktail.EMPTY) return;

        Instantiate(_stream, _shakerTip.position, 
                    Quaternion.identity, transform);
        OnBeginPourCocktail?.Invoke(_cocktail);
        _mixedDrink.Clear();
    }

    public void MixingCocktail()
    {
        if(_isShaking) return;

        _isGrabbed = true;
        StartCoroutine(CO_ShakeDrink());
        _isShaking = true;
        _logger.Log($"{this} object is shaking", TextColor.GREEN, _isDevMode);
    }
    
    public void Washed()
    {
        StopAllCoroutines();
    
        _isShaking = false;
        _isPouring = false;
        _isLocked = false;
        _mixedDrink.Clear();
        _cocktail = Cocktail.EMPTY;
        _shakerCap.gameObject.SetActive(false);
        ResetPosition();
        BarManager.Instance.SpawnCap();

        _logger.Log($"{this} has no more drink!", TextColor.YELLOW, _isDevMode);
    }

    public void IsReleased() => _isGrabbed = false;
        
    #endregion
    #region Helpers

    protected override void InitVariables()
    {
        base.InitVariables();

        _mixedDrink = new List<Ingredient>();
        _cocktail = Cocktail.EMPTY;
        _isPouring = false;
        _isLocked = false;
    }

    protected override void Test()
    {

    }

    private void AddIngredient(Ingredient ingredient)
    {
        if(_isLocked) return;

        if (!_mixedDrink.Contains(ingredient))
        {
            _mixedDrink.Add(ingredient);
            _logger.Log($"Added {ingredient} to shaker!", TextColor.CYAN, _isDevMode);
        }
        else
        {
            _logger.Log($"{ingredient} is already in the shaker.", TextColor.YELLOW, _isDevMode);
        }
    }

    private void ResetShaker() => StartCoroutine(CO_DrainDrink());

    #endregion
    #region Enumerators

    private IEnumerator CO_DrainDrink()
    {
        yield return new WaitForSeconds(5F);

        ShakerUnlocked?.Invoke();
        _shakerCap.gameObject.SetActive(false);
        _cocktail = Cocktail.EMPTY;
    }

    private IEnumerator CO_ShakeDrink() // gets called when the GO is picked up
    {
        Debug.Log("CO_ShakeDrink Starting: Line 196");
        void CompareIngredients()
        {
            Debug.Log("Compare Ingredients Starting: Line 199");
            foreach (var recipe in _recipes)
            {
                // ensures that it has no extra/missing ingredients
                bool isMatching = recipe.Value.All(i => _mixedDrink.Contains(i)) &&
                                  _mixedDrink.Count == recipe.Value.Length;

                if (isMatching)
                {
                    _cocktail = recipe.Key;
                    _logger.Log($"Created {recipe.Key}!", TextColor.GREEN, _isDevMode);
                    Debug.Log($"Created {recipe.Key}!");
                    return;
                }
            }

            _cocktail = Cocktail.WRONG;
            _logger.Log("Created dubious drink!", TextColor.RED, _isDevMode);
            Debug.Log("Created dubious drink!");
        }

        // time for the player to earn bonus points
        yield return new WaitForSeconds(Random.Range(10F, 15F));
        CompareIngredients();
    }

    #endregion
}
