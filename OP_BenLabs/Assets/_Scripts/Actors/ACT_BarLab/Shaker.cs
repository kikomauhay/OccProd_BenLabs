using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Shaker : Equipment
{
    #region Properties

    public IReadOnlyList<Ingredient> MixedDrink => _mixedDrink;
    public Cocktail Cocktail => _cocktail;
    public static event System.Action OnBeginPour;
    public static event System.Action OnStopPour;

    #endregion
    #region SerializeField
    
    [SerializeField] private List<Ingredient> _mixedDrink;
    [SerializeField] private Cocktail _cocktail;
    [SerializeField] private Transform _shakerTip;
    [SerializeField] private float _pourThreshold;
    [SerializeField] private GameObject _stream;

    #endregion
    #region Private 

    // dictionary syntax = <key, value>
    private readonly Dictionary<Cocktail, Ingredient[]> _recipes = new()
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

    private void FixedUpdate()
    {
        CheckPourAngle();
    }

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
        if (!_isDevMode) return;

        
    }

    private void CheckPourAngle()
    {
        float angle = Vector3.Angle(_shakerTip.up, Vector3.up);

        if (angle > _pourThreshold)
        {
            if (_isPouring) return;
            
            Pour();
            _isPouring = true;
        }
        else
        {
            _isPouring = false;
            OnStopPour?.Invoke();
        }
    }

    private void Pour()
    {
        Instantiate(_stream, _shakerTip.position, Quaternion.identity, transform);
        OnBeginPour?.Invoke();
        _mixedDrink.Clear();
    }

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
