using System;
using UnityEngine;

public class Bottle : Equipment, IPourable
{
    #region Properties

    public Ingredient Ingredient => _ingredient;
    // has no OnBeginPour event since it has a param idk how to work around
    public static Action OnStopPour { get; set; }

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

    #region Unity

    private void FixedUpdate() => INT_CheckPourAngle();

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
        Instantiate(_stream, _shakerTip.position,
                    Quaternion.identity, transform);
    }
        
    #endregion
}

public enum Ingredient
{
    ORANGE_JUICE = 0,
    LIME_JUICE = 1,
    COCONUT_WATER = 2,
    TEQUILA = 3,
    VODKA = 4
}