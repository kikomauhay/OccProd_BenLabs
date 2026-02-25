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
    #region SerializeField

    [Header("Ingredient Mixing")]
    [SerializeField] private Ingredient _ingredient;

    [Header("Pouring")]
    [SerializeField] private GameObject _stream;
    [SerializeField] private Transform _shakerTip;
    [SerializeField] private float _pourThreshold;

    #endregion
    #region Private

    private bool _isPouring;

    #endregion

    #region Actor

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
            yield return new WaitForSeconds(2f);
            BarManager.Instance.OnCustomerSpawn += ResetPosition;
        }

        StartCoroutine(CO_DelayedBinding());
    }
    protected override void OnDisable()
    {
        BarManager.Instance.OnCustomerSpawn -= ResetPosition;
    }
    private void FixedUpdate()
    {
        float angle = Vector3.Angle(_shakerTip.up, Vector3.up);

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

    #endregion
    #region Public   

    public void INT_CheckPourAngle()
    {
        float angle = Vector3.Angle(_shakerTip.up, Vector3.up);

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
        a_logger.Log("pouring", TextColor.Yellow, a_isDevMode);
        Instantiate(_stream, _shakerTip.position,
                    Quaternion.identity, transform);

        OnBeginPourIngredient?.Invoke(_ingredient);
    }

    public void ResetBottle() => ResetPosition();
        
    #endregion
}

public enum Ingredient
{
    Orange_Juice = 0,
    Lime_Juice = 1,
    Coconut_Water = 2,
    Teuila = 3,
    Vodka = 4
}