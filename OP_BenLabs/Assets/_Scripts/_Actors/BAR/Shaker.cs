using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Shaker : Equipment, IPourable
{
    #region Properties

    public IReadOnlyList<Ingredient> MixedDrink => _mixedDrink;
    public Mocktail Cocktail => _mocktailContent;
    public static event System.Action ShakerLocked;
    public static event System.Action ShakerUnlocked;
    public static event System.Action<Mocktail> OnBeginPourCocktail;
    public static event System.Action OnStopPour;

    #endregion
    #region Inspector
    
    [Header("Ingredient Mixing")]
    [SerializeField] private List<Ingredient> _mixedDrink;
    [SerializeField] private Mocktail _mocktailContent;
    
    [Header("Pouring Logic")]
    [SerializeField] private GameObject _stream;
    [SerializeField] private Transform _shakerTip;
    [SerializeField] private float _pourThreshold;

    [Header("Toggle Objects")]
    [SerializeField] private GameObject _shakerCap;
    [SerializeField] private GameObject _checkPanel;

    #endregion
    #region Private 

    private BarManager _barMgr;

    private readonly Dictionary<Mocktail, Ingredient[]> _recipes = new(3) // dictionary syntax = <key, value>
    {
        { Mocktail.Tropical_Splash, new[] { Ingredient.Orange,
                                            Ingredient.Coconut,
                                            Ingredient.Pineapple } },

        { Mocktail.Citrus_Sunrise, new[] { Ingredient.Orange,
                                           Ingredient.Lime,
                                           Ingredient.Coconut } },

        { Mocktail.Sunset_Cooler, new[] { Ingredient.Cranberry,
                                          Ingredient.Lime,
                                          Ingredient.Coconut } }
    };
    private XRBaseInteractor _interactor;
    private WaitForSeconds _drainTime;
    private bool _isPouring, _isLocked;
    private bool _isShaking, _isGrabbed;

    #endregion

    #region Actor

    protected override void AssertReferences()
    {
        base.AssertReferences();
        
        a_logger.AssertReference(_mixedDrink.Count == 0, this);
        a_logger.Assert(_mocktailContent == Mocktail.Empty, $"{name} is using the wrong Mocktail type!", this);

        a_logger.AssertReference(_stream != null, this);
        a_logger.AssertReference(_shakerTip != null, this);
        a_logger.AssertReference(_pourThreshold != 0f, this);

        a_logger.AssertReference(_shakerCap != null, this);
        a_logger.AssertReference(_checkPanel != null, this);
    }
    protected override void InitVariables()
    {
        base.InitVariables();

        _barMgr = BarManager.Instance;

        _drainTime = new WaitForSeconds(5f);

        _isPouring = false;
        _isLocked = false;
        _isShaking = false;
        _isGrabbed = false;

        _checkPanel.SetActive(false);
    }

    #endregion
    #region Unity

    protected override void OnEnable()
    {
        LiquidPour.OnShakerHit += AddIngredient;
        LiquidPour.ShakerEmptied += ResetShaker;       
    }
    protected override void OnDisable()
    {
        LiquidPour.OnShakerHit -= AddIngredient;
        LiquidPour.ShakerEmptied -= ResetShaker;
    }
    private void FixedUpdate()
    {
        if (!_isLocked) return;

        if (e_rb.velocity.magnitude > 0.5f)
            MixMocktail();

        INT_CheckPourAngle();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("ShakerCap"))
        {
            _isLocked = true;
            _shakerCap.SetActive(true);
            ShakerLocked?.Invoke();

            a_logger.Log($"{other.name} has been destroyed!", a_isDevMode);
            Destroy(other.gameObject);
        }
    }

    #endregion
    #region Public

    public void INT_CheckPourAngle()
    {
        float angle = Vector3.Angle(_shakerTip.up, Vector3.up);

        if (_mocktailContent == Mocktail.Empty && !_isGrabbed) return;

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
        if (_mocktailContent == Mocktail.Empty) return;

        Instantiate(_stream, _shakerTip.position, 
                    Quaternion.identity, transform);
        OnBeginPourCocktail?.Invoke(_mocktailContent);
        _mixedDrink.Clear();
    }

    public void MixMocktail()
    {
        if (_isShaking) return;

        _isGrabbed = true;
        StartCoroutine(CO_ShakeDrink());

        _isShaking = true;
        a_logger.Log($"{name} is currently shaking", TextColor.Yellow, a_isDevMode);
    }
    public void WashShaker()
    {
        ResetPosition();
        StopAllCoroutines();

        if (_isLocked)
            _barMgr.DeductCapCount();
    
        _isShaking = false;
        _isPouring = false;
        _isLocked = false;

        _mixedDrink.Clear();
        _mocktailContent = Mocktail.Empty;
        e_rb.velocity = Vector3.zero;

        _checkPanel.SetActive(false);
        _shakerCap.gameObject.SetActive(false);
        _barMgr.SpawnCap();

        a_logger.Log($"{name} has been washed!", TextColor.Yellow, a_isDevMode);
    }

    public void SubscribeController(SelectEnterEventArgs args) => _interactor = args.interactorObject as XRBaseInteractor;
    public void RemoveController(SelectExitEventArgs args) => _interactor = null;
    public void IsReleased() => _isGrabbed = false;
        
    #endregion
    #region Private

    private void AddIngredient(Ingredient ingredient)
    {
        if (_isLocked) return;

        if (!_mixedDrink.Contains(ingredient))
        {
            _mixedDrink.Add(ingredient);
            a_logger.Log($"Added {ingredient} to shaker!", TextColor.Cyan, a_isDevMode);
        }
        else a_logger.Log($"{ingredient} is already in the shaker.", TextColor.Yellow, a_isDevMode);
    }
    private void ResetShaker() => StartCoroutine(CO_DrainDrink());
        
    #endregion
    
    #region Enumerators

    private IEnumerator CO_DrainDrink()
    {
        yield return _drainTime;

        ShakerUnlocked?.Invoke();
        OnStopPour?.Invoke();

        _checkPanel.SetActive(false);
        _shakerCap.SetActive(false);

        _mocktailContent = Mocktail.Empty;
    }
    private IEnumerator CO_ShakeDrink() // gets called when the GO is picked up
    {
        void SendHaptics(float amp, float dur)
        {
            if (_interactor is XRBaseControllerInteractor controllerInteractor)
                controllerInteractor.SendHapticImpulse(amp, dur);       
        }
        void CompareIngredients()
        {
            foreach (var recipe in _recipes)
            {
                // ensures that it has no extra/missing ingredients
                // also prevents the player from adding too much of 1 ingredient
                bool isMatching = recipe.Value.All(i => _mixedDrink.Contains(i)) &&
                                  _mixedDrink.Count == recipe.Value.Length;

                if (isMatching)
                {
                    SendHaptics(0.4f, 0.4f);
                    _mocktailContent = recipe.Key;
                    a_logger.Log($"Created a {recipe.Key}!", TextColor.Lime, a_isDevMode);
                    return;
                }
            }

            SendHaptics(0.8f, 0.5f);
            _mocktailContent = Mocktail.Wrong;
            a_logger.Log("Created dubious drink!", TextColor.Red, a_isDevMode);
        }

        // random countdown for the player to earn bonus points
        yield return new WaitForSeconds(Random.Range(10f, 15f));
        
        CompareIngredients();
        _checkPanel.SetActive(true);
    }

    #endregion
}
