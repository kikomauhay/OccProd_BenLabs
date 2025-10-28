using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Shaker : Equipment, IPourable
{
    #region Properties

    public IReadOnlyList<Ingredient> MixedDrink => _mixedDrink;
    public Cocktail Cocktail => _cocktail;
    public static event System.Action<Cocktail> OnBeginPour;
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
    private bool _isPouring;

    #endregion

    #region Unity

    protected override void OnEnable()
    {
        base.OnEnable();
        LiquidPour.ShakerEmptied += ResetShaker;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        LiquidPour.ShakerEmptied -= ResetShaker;
    }
    private void FixedUpdate() => INT_CheckPourAngle();
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bottle>())
        {
            _mixedDrink.Add(other.GetComponent<Bottle>().Ingredient);

            if (_isDevMode)
                _logger.Log($"Added a drink to {this}!", TextColor.GREEN);
        }
    }

    #endregion
    #region Public

    public void INT_CheckPourAngle()
    {
        float angle = Vector3.Angle(_shakerTip.up, Vector3.up);

        if (_cocktail == Cocktail.EMPTY) return;

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
        Instantiate(_stream, _shakerTip.position, Quaternion.identity, transform);
        OnBeginPour?.Invoke(_cocktail);
        _mixedDrink.Clear();
    }
    
    public void Washed()
    {
        StopAllCoroutines();
    
        _isPouring = false;
        _mixedDrink.Clear();
        _cocktail = Cocktail.EMPTY;

        if (_isDevMode)
            _logger.Log($"{this} has no more drink!", TextColor.YELLOW);
    }
        
    #endregion
    #region Helpers

    protected override void InitVariables()
    {
        base.InitVariables();

        _mixedDrink = new List<Ingredient>();
        _cocktail = Cocktail.EMPTY;
        _isPouring = false;
    }

    protected override void Test()
    {

    }

    private void ResetShaker() => _cocktail = Cocktail.EMPTY;

    #endregion
    #region Enumerators

    private IEnumerator CO_ShakeDrink() // gets called when the GO is picked up
    {
        void CompareIngredients()
        {
            foreach (var recipe in _recipes)
            {
                // ensures that it has no extra/missing ingredients
                bool isMatching = recipe.Value.All(i => _mixedDrink.Contains(i)) && 
                                  _mixedDrink.Count == recipe.Value.Length;  
                
                if (isMatching)
                {
                    _cocktail = recipe.Key;

                    if (_isDevMode)
                        _logger.Log($"Created {recipe.Key}!", TextColor.GREEN);

                    return;
                }
            }

            _cocktail = Cocktail.WRONG;

            if (_isDevMode)
                _logger.Log("Created dubious drink!", TextColor.RED);
        }

        // time for the player to earn bonus points
        yield return new WaitForSeconds(Random.Range(10f, 15f)); 
        CompareIngredients();
    }
        
    #endregion
}
