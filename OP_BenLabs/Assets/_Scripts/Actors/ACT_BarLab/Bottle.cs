using UnityEngine;

public class Bottle : Equipment
{
    #region Members

    public Ingredient Ingredient => _ingredient;
    [SerializeField] private Ingredient _ingredient;
        
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