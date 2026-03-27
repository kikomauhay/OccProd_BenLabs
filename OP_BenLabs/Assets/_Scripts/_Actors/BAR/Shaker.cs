using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

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
    [SerializeField] private Renderer _liquidRenderer;
    
    [Header("Pouring Logic")]
    [SerializeField] private GameObject _stream;
    [SerializeField] private Transform _shakerTip;
    [SerializeField] private float _pourThreshold;

    [Header("Toggle Objects")]
    [SerializeField] private GameObject _shakerCap;
    [SerializeField] private GameObject _checkPanel;
    [SerializeField] private GameObject _radialPanel;

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
        _radialPanel.SetActive(false);
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
        _liquidRenderer.material.SetFloat("_Fill", 0.54F);
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
        _liquidRenderer.material.SetFloat("_Fill", 0.52F);

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
            
            switch (_mixedDrink.Count)
            {
                case 1:
                    StartCoroutine(CO_FillDrink(0.56F, 0.33F, 2F));
                    break;
                case 2:
                    StartCoroutine(CO_FillDrink(0.58F, 0.66F, 2F));
                    break;
                case 3:
                    StartCoroutine(CO_FillDrink(0.6F, 1F, 2F));
                    break;
            }

        }
        else a_logger.Log($"{ingredient} is already in the shaker.", TextColor.Yellow, a_isDevMode);
    }

    private void ResetShaker() => StartCoroutine(CO_DrainDrink());
        
    #endregion
    
    #region Enumerators

    private IEnumerator CO_FillDrink(float targetFill, float sliderFill, float duration)
    {
        float startLiquid = _liquidRenderer.material.GetFloat("_Fill");
        float startSlider = _radialPanel.gameObject.GetComponentInChildren<Slider>().value;
        float time = 0f;

        _radialPanel.SetActive(true);

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            float value = Mathf.Lerp(startLiquid, targetFill, t);
            float sliderValue = Mathf.Lerp(startSlider, sliderFill, t);
            _liquidRenderer.material.SetFloat("_Fill", value);
            _radialPanel.gameObject.GetComponentInChildren<Slider>().value = sliderValue;

            yield return null;
        }

        _radialPanel.gameObject.GetComponentInChildren<Slider>().value = sliderFill;
        _liquidRenderer.material.SetFloat("_Fill", targetFill);
        _radialPanel.SetActive(false);

        if (targetFill >= 0.6F)
            _radialPanel.gameObject.GetComponentInChildren<Slider>().value = 0.52F;
    }

    private IEnumerator CO_DrainDrink()
    {

        _liquidRenderer.material.SetFloat("_Fill", 0.52F);
        yield return _drainTime;

        ShakerUnlocked?.Invoke();
        OnStopPour?.Invoke();

        _checkPanel.SetActive(false);
        _shakerCap.SetActive(false);

        _mocktailContent = Mocktail.Empty;

    }
    private IEnumerator CO_ShakeDrink() // gets called when the GO is picked up
    {
        float randomValue = Random.Range(10f, 15f);

        void StartShaking()
        {
            float time = 0f;
            _radialPanel.SetActive(true);

            while (time < randomValue)
            {
                time += Time.deltaTime;
                float t = time/randomValue;

                float sliderValue = Mathf.Lerp(0f,1f, t);
                _radialPanel.gameObject.GetComponentInChildren<Slider>().value = sliderValue;
            }
            _radialPanel.gameObject.GetComponentInChildren<Slider>().value = 1F;
        }

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

        StartShaking();

        // random countdown for the player to earn bonus points
        yield return new WaitForSeconds(randomValue);
        
        CompareIngredients();
        _checkPanel.SetActive(false);
    }

    #endregion
}
