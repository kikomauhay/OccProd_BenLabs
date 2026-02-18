using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
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
    [SerializeField] private GameObject _checkPanel;

    #endregion
    #region Private 

    private readonly Dictionary<Cocktail, Ingredient[]> _recipes = new() // dictionary syntax = <key, value>
    {
        { Cocktail.Tequila_Sunrise, new[] { Ingredient.TEQUILA,
                                            Ingredient.ORANGE_JUICE,
                                            Ingredient.LIME_JUICE } },

        { Cocktail.Vodka_Citrus, new[] { Ingredient.VODKA,
                                         Ingredient.ORANGE_JUICE,
                                         Ingredient.LIME_JUICE,
                                         Ingredient.COCONUT_WATER } },

        { Cocktail.Coconut_Mergarita, new[] { Ingredient.TEQUILA,
                                              Ingredient.LIME_JUICE,
                                              Ingredient.COCONUT_WATER } },
    };
    private bool _isPouring, _isLocked, _isShaking, _isGrabbed;
    private XRBaseInteractor _interactor;

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

        if(e_rb.velocity.magnitude > 0.5f)
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

        if (_cocktail == Cocktail.Empty && !_isGrabbed) return;

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
        if (_cocktail == Cocktail.Empty) return;

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
        a_logger.Log($"{this} object is shaking", TextColor.Lime, a_isDevMode);
    }
    
    public void Washed()
    {
        ResetPosition();
        StopAllCoroutines();

        if (_isLocked)
            BarManager.Instance.DeductCapCount();
    
        _isShaking = false;
        _isPouring = false;
        _isLocked = false;

        _mixedDrink.Clear();
        _cocktail = Cocktail.Empty;
        e_rb.velocity = Vector3.zero;

        _checkPanel.SetActive(false);
        _shakerCap.gameObject.SetActive(false);
        BarManager.Instance.SpawnCap();

        a_logger.Log($"{this} has no more drink!", TextColor.Yellow, a_isDevMode);
    }

    public void SubscribeController(SelectEnterEventArgs args) => _interactor = args.interactorObject as XRBaseInteractor;
    public void RemoveController(SelectExitEventArgs args) => _interactor = null;
    public void IsReleased() => _isGrabbed = false;
        
    #endregion
    #region Helpers

    protected override void InitVariables()
    {
        base.InitVariables();

        _mixedDrink = new List<Ingredient>();
        _cocktail = Cocktail.Empty;
        _isPouring = false;
        _isLocked = false;

        _checkPanel.SetActive(false);
    }

    private void AddIngredient(Ingredient ingredient)
    {
        if (_isLocked) return;

        if (!_mixedDrink.Contains(ingredient))
        {
            _mixedDrink.Add(ingredient);
            a_logger.Log($"Added {ingredient} to shaker!", TextColor.Cyan, a_isDevMode);
        }
        else
        {
            a_logger.Log($"{ingredient} is already in the shaker.", TextColor.Yellow, a_isDevMode);
        }
    }

    private void SendHaptics(float amp, float dur)
    {
        if (_interactor is XRBaseControllerInteractor controllerInteractor)
        {
            controllerInteractor.SendHapticImpulse(amp, dur);
        }
    }

    private void ResetShaker() => StartCoroutine(CO_DrainDrink());

    #endregion
    #region Enumerators

    private IEnumerator CO_DrainDrink()
    {
        yield return new WaitForSeconds(5F);

        ShakerUnlocked?.Invoke();
        OnStopPour?.Invoke();
        _checkPanel.SetActive(false);
        _shakerCap.gameObject.SetActive(false);
        _cocktail = Cocktail.Empty;
    }

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
                    SendHaptics(0.4f, 0.4f);
                    _cocktail = recipe.Key;
                    a_logger.Log($"Created {recipe.Key}!", TextColor.Lime, a_isDevMode);
                    Debug.Log($"Created {recipe.Key}!");
                    return;
                }
            }

            SendHaptics(0.8f, 0.5f);
            _cocktail = Cocktail.Wrong;
            a_logger.Log("Created dubious drink!", TextColor.Red, a_isDevMode);
            Debug.Log("Created dubious drink!");
        }

        // time for the player to earn bonus points
        yield return new WaitForSeconds(Random.Range(10f, 15f));
        CompareIngredients();
        _checkPanel.SetActive(true);
    }

    #endregion
}
