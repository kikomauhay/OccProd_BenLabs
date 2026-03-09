using System.Collections;
using UnityEngine;

/// <summary> - MOCKTAIL COMBINATIONS -
/// 
/// TROPICAL SPLASH
///     - pineapple juice
///     - orange juice
///     - coconut water
/// 
/// CITRUS SUNRISE
///     - orange juice
///     - lime juice
///     - coconut water
/// 
/// SUNSET COOLER
///     - lime juice
///     - cranberry juice
///     - coconut water
///     
/// </summary>  

/// <summary> - MOCKTAIL METHOD - 
/// 
/// METHOD:
///     1. Add the set ingredients into a shaker
///     2. Shake for 10-15 secs
///     3. Pour into a glass with ice
/// 
/// </summary>  

public class Glass : Equipment
{
    #region Properties

    public bool HasDrink => _hasDrink;
    public Mocktail Mocktail => _mocktail;

    #endregion
    #region Inspector

    [Header("For Testing")]
    [SerializeField] private Renderer _renderer;

    [Header("Drinks"), Tooltip("0 = Tequila, 1 = Vodka, 2 = Coconut")]
    [SerializeField] private GameObject[] _drinks;
    [SerializeField] private GameObject _checkPanel;

    [Header("Drink Stats")]
    [SerializeField] private Mocktail _mocktail;
    [SerializeField] private bool _hasDrink;

    [Header("SFX")]
    [SerializeField] private Sound _poofSFX;
    [SerializeField] private Sound[] _iceRefillSFXs;


    #endregion

    #region Actor

    protected override void Test()
    {
        // if (Input.GetKeyDown(KeyCode.Alpha4)) EnableDrink(0);
        // if (Input.GetKeyDown(KeyCode.Alpha5)) EnableDrink(1);
        // if (Input.GetKeyDown(KeyCode.Alpha6)) EnableDrink(2);

        if (Input.GetKeyDown(KeyCode.Delete)) ResetDrink();
    }
    protected override void InitComponents()
    {
        base.InitComponents();
        
        a_logger.AssertCollection(_drinks, this);
        a_logger.AssertReference(_checkPanel != null, this);

        a_logger.Assert(_mocktail == Mocktail.Empty, "Variable has been set to the wrong value!", this);
        a_logger.Assert(!_hasDrink, "Variable has been set to the wrong value!", this);

        a_logger.AssertReference(_poofSFX != null, this);
        a_logger.AssertCollection(_iceRefillSFXs, this);
    }
    protected override void InitVariables()
    {
        base.InitVariables();

        _mocktail = Mocktail.Empty;
        _hasDrink = false;
        _checkPanel.SetActive(false);
    }
        
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
        e_sndEmtr.PlayRandomSound(_iceRefillSFXs);
    }

    #endregion
    #region Public 

    public void HitFloor()
    {
        ResetDrink();
        ResetPosition();

        a_logger.Log($"{name} has hit the floor!", TextColor.Yellow, a_isDevMode);
    }
    public void Served()
    {
        ResetDrink();
        a_logger.Log($"{name} has been served!", TextColor.Yellow, a_isDevMode);
    }
    public void Washed()
    {
        ResetDrink();
        a_logger.Log($"{name} has been washed!", TextColor.Yellow, a_isDevMode);
    }
    public void EnableDrink(Mocktail cocktail)
    {
        static IEnumerator CO_FillDrink(Material mat, float duration)
        {
            float start = mat.GetFloat("_Fill");
            float target = 0.56f;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time/ duration;

                float value = Mathf.Lerp(start, target, t);
                mat.SetFloat("_Fill",value);

                yield return null;
            }

            mat.SetFloat("_Fill",target);
        }

        if (_hasDrink)
        {
            a_logger.Log($"{name} alreaady has an active drink!", a_isDevMode);
            a_audMgr.PlayWrong();
            return;
        }

        _hasDrink = true;
        _mocktail = cocktail;
        _checkPanel.SetActive(true);
 
        int index = _mocktail switch
        {
            Mocktail.Citrus_Sunrise => 0,
            Mocktail.Tropical_Splash => 1,
            Mocktail.Sunset_Cooler => 2,
            _ => -1
        };

        if (index != -1)
        {
            Renderer rend = _drinks[index].GetComponent<Renderer>();
            StartCoroutine(CO_FillDrink(rend.material, 2f));
        }

        e_sndEmtr.PlaySound(_poofSFX);
        a_gameMgr.Poof(transform);
        a_logger.Log($"{name} has a {_mocktail} inside!", TextColor.Yellow, a_isDevMode);
    }

    #endregion
    #region Private

    private void ResetDrink()
    {
        _hasDrink = false;
        _mocktail = Mocktail.Empty;
        _checkPanel.SetActive(false);

        foreach (GameObject drink in _drinks)
            drink.SetActive(false);

        a_logger.Log($"{name} has no more drink!", TextColor.Yellow, a_isDevMode);
    }

    #endregion
}

public enum Mocktail
{
    Wrong = -1,
    Empty = 0,
    Citrus_Sunrise = 1,
    Tropical_Splash = 2,
    Sunset_Cooler = 3
}