using System;
using System.Collections;
using UnityEngine;

public class Bottle : Equipment, IPourable
{
    #region Properties

    public event Action<Ingredient> OnBeginPourIngredient;
    public Action OnStopPour { get; set; }
    public Ingredient Ingredient => _ingredient;

    #endregion
    #region Inspector

    [Header("Ingredient Mixing")]
    [SerializeField] private Ingredient _ingredient;

    [Header("Pouring Settings")]
    [SerializeField] private GameObject _stream;
    [SerializeField] private Transform _shakerTip;
    [SerializeField] private float _pourThreshold;

    #endregion
    #region Private

    private BarManager _barMgr;
    private bool _isPouring;

    #endregion

    #region Actor

    protected override void AssertReferences()
    {
        base.AssertReferences();

        a_logger.AssertReference(_stream != null, this);
        a_logger.AssertReference(_shakerTip != null, this);
        a_logger.AssertReference(_pourThreshold != 0f, this);
    }
    protected override void InitComponents()
    {
        base.InitComponents();
        _barMgr = BarManager.Instance;
    }
    protected override void InitVariables()
    {
        base.InitVariables();
        name = Ingredient.ToString().Replace("_", " ") + " Bottle";
    }

    #endregion
    #region Unity
    
    protected override void OnEnable()
    {
        IEnumerator CO_DelayedBinding()
        {
            yield return null;
            _barMgr.OnCustomerSpawn += ResetPosition;
        }

        StartCoroutine(CO_DelayedBinding());
    }
    protected override void OnDisable()
    {
        _barMgr.OnCustomerSpawn -= ResetPosition;
    }
    private void FixedUpdate() => INT_CheckPourAngle();

    #endregion
    #region Public   

    public void INT_CheckPourAngle()
    {
        if (Vector3.Angle(_shakerTip.up, Vector3.up) > _pourThreshold)
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
        a_logger.Log("pouring", TextColor.Yellow, a_isDevMode);
        
        Instantiate(_stream, _shakerTip.position, Quaternion.identity, transform);

        OnBeginPourIngredient?.Invoke(_ingredient);
    }
    
    public void ResetBottle() => ResetPosition();
        
    #endregion
}

public enum Ingredient
{
    Orange = 0,
    Lime = 1,
    Coconut = 2,
    Cranberry = 3,
    Pineapple = 4
}